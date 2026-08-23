import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter, Router } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor';
import { msalInstance } from './auth/msal.instance';

/**
 * Completes the redirect flow after loginRedirect() returns (processes the response hash and
 * marks the resulting account active) — required for authGuard/authInterceptor to see it.
 * navigateToLoginRequestUrl: false below stops MSAL from sending the browser straight back to
 * /login on its own (its default behavior — /login is always the page open when loginRedirect()
 * gets called, so left at the default it just bounces you right back there); a successful
 * redirect is navigated to the dashboard explicitly here instead. No-op when there's no MSAL
 * instance (authMode === 'dev').
 */
async function initializeMsal(): Promise<void> {
  if (!msalInstance) {
    return;
  }

  const router = inject(Router);

  await msalInstance.initialize();

  try {
    const response = await msalInstance.handleRedirectPromise({ navigateToLoginRequestUrl: false });
    // Temporary diagnostic — remove once the post-login redirect is confirmed working.
    console.log('[msal] handleRedirectPromise result:', response, 'activeAccount before:', msalInstance.getActiveAccount(), 'allAccounts:', msalInstance.getAllAccounts());
    if (response?.account) {
      msalInstance.setActiveAccount(response.account);
      router.navigateByUrl('/dashboard');
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
