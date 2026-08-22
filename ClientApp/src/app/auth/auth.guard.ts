import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { msalInstance } from './msal.instance';
import { isDevSignedIn } from './dev-session';
import { AuthService } from '../core/services/auth';

export const authGuard: CanActivateFn = () => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return isDevSignedIn() ? true : inject(Router).createUrlTree(['/login']);
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0];
  // Temporary diagnostic — remove once the post-login redirect is confirmed working.
  console.log('[authGuard] account:', account, 'activeAccount:', msalInstance.getActiveAccount(), 'allAccounts:', msalInstance.getAllAccounts());
  return account ? true : inject(Router).createUrlTree(['/login']);
};

// Backend-authoritative: GET /api/auth/me reflects the AspNetUserRoles table (via
// DbRoleClaimsTransformation) in either auth mode, so there's no separate MSAL/dev-mode branch
// to keep in sync — an Azure AD ID token's own "roles" claim would need App Roles configured in
// Azure AD, which this app doesn't use.
export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.me().pipe(map((me) => (me.roles.includes('Admin') ? true : router.createUrlTree(['/dashboard']))));
};
