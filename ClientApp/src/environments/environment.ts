import { Environment } from './environment.model';

export const environment: Environment = {
  production: true,
  authMode: 'azuread',
  azureAd: { clientId: '', tenantId: '', redirectUri: '' },
};
