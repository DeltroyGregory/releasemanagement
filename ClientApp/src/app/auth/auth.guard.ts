import { CanActivateFn } from '@angular/router';
import { environment } from '../../environments/environment';
import { msalInstance } from './msal.instance';

export const authGuard: CanActivateFn = () => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return true;
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0];
  if (account) {
    return true;
  }

  msalInstance.loginRedirect();
  return false;
};

export const adminGuard: CanActivateFn = () => {
  if (environment.authMode === 'dev' || !msalInstance) {
    return true;
  }

  const account = msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0];
  const roles = (account?.idTokenClaims?.['roles'] as string[] | undefined) ?? [];
  return roles.includes('Admin');
};
