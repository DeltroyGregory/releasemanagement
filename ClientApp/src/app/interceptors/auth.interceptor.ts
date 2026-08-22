import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { environment } from '../../environments/environment';
import { msalInstance } from '../auth/msal.instance';

const API_SCOPES = [`api://${environment.azureAd.clientId}/access_as_user`];

/**
 * Dev mode: backend's DevAuthHandler authenticates every request as the seeded dev admin, so no
 * token is needed — pass requests through unchanged. Azuread mode: acquire a token silently for
 * the active MSAL account and attach it as Bearer, falling back to a redirect if that fails.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return next(req);
  }

  const instance = msalInstance;
  const account = instance.getActiveAccount() ?? instance.getAllAccounts()[0];
  if (!account) {
    // Temporary diagnostic — remove once the post-login redirect is confirmed working.
    console.log('[authInterceptor] no account for request:', req.url);
    return next(req);
  }

  return from(instance.acquireTokenSilent({ scopes: API_SCOPES, account })).pipe(
    catchError((error) => {
      // Temporary diagnostic — remove once the post-login redirect is confirmed working.
      console.error('[authInterceptor] acquireTokenSilent failed for', req.url, error);
      if (error instanceof InteractionRequiredAuthError) {
        instance.acquireTokenRedirect({ scopes: API_SCOPES, account });
      }
      return throwError(() => error);
    }),
    switchMap((result) => {
      const authReq = req.clone({ setHeaders: { Authorization: `Bearer ${result.accessToken}` } });
      return next(authReq);
    }),
  );
};
