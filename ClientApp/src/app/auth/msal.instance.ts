import { PublicClientApplication } from '@azure/msal-browser';
import { environment } from '../../environments/environment';

/**
 * Only constructs a real MSAL instance in 'azuread' mode. In 'dev' mode (no Azure AD app
 * registration yet) this stays null and nothing in the app should call into MSAL.
 */
export const msalInstance: PublicClientApplication | null =
  environment.authMode === 'azuread'
    ? new PublicClientApplication({
        auth: {
          clientId: environment.azureAd.clientId,
          authority: `https://login.microsoftonline.com/${environment.azureAd.tenantId}`,
          redirectUri: environment.azureAd.redirectUri,
        },
        cache: { cacheLocation: 'localStorage' },
      })
    : null;
