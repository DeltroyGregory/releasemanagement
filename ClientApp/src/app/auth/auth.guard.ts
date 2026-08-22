import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { msalInstance } from './msal.instance';
import { isDevSignedIn } from './dev-session';

export const authGuard: CanActivateFn = () => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return isDevSignedIn() ? true : inject(Router).createUrlTree(['/login']);
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0];
  return account ? true : inject(Router).createUrlTree(['/login']);
};

export const adminGuard: CanActivateFn = () => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return true;
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0];
  const roles = (account?.idTokenClaims?.['roles'] as string[] | undefined) ?? [];
  return roles.includes('Admin');
};
