import { Environment } from './environment.model';

export const environment: Environment = {
  production: false,
  authMode: 'dev',
  azureAd: { clientId: '', tenantId: '', redirectUri: 'http://localhost:4200' },
};
