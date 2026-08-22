import { Environment } from './environment.model';

// authMode is 'dev' (not 'azuread') until a real Azure AD app registration exists — matching
// the deployed backend, which also falls back to DevAuthHandler while AzureAd:TenantId is unset
// (see dev.tfvars). Flip both to 'azuread' together once a real app registration is set up:
// otherwise this shows a "Sign in with Microsoft" button that fails silently (empty
// clientId/tenantId) against a backend that isn't actually validating Azure AD tokens anyway.
export const environment: Environment = {
  production: true,
  authMode: 'dev',
  azureAd: { clientId: '', tenantId: '', redirectUri: '' },
};
