export type AuthMode = 'dev' | 'azuread';

export interface Environment {
  production: boolean;
  authMode: AuthMode;
  azureAd: { clientId: string; tenantId: string; redirectUri: string };
}
