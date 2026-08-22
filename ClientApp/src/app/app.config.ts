import { ApplicationConfig, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';
import { msalInstance } from './auth/msal.instance';

/**
 * Completes the redirect flow after loginRedirect() returns (processes the response hash and
 * marks the resulting account active) — required for authGuard/authInterceptor to see it. No-op
 * when there's no MSAL instance (authMode === 'dev').
 */
async function initializeMsal(): Promise<void> {
  if (!msalInstance) {
    return;
  }

  await msalInstance.initialize();

  try {
    const response = await msalInstance.handleRedirectPromise();
    // Temporary diagnostic — remove once the post-login redirect is confirmed working.
    console.log('[msal] handleRedirectPromise result:', response, 'activeAccount before:', msalInstance.getActiveAccount(), 'allAccounts:', msalInstance.getAllAccounts());
    if (response?.account) {
      msalInstance.setActiveAccount(response.account);
    }
  } catch (err) {
    console.error('[msal] handleRedirectPromise threw:', err);
  }
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideRouter(routes),
    provideAppInitializer(() => initializeMsal()),
  ]
};
