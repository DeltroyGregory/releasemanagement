import { Environment } from './environment.model';

// Reuses releasemgmtport-dev (the GitHub Actions OIDC deployment app registration) as the
// sign-in app too — see .devops/terraform/dev.tfvars for the same values on the backend side.
export const environment: Environment = {
  production: true,
  authMode: 'azuread',
  azureAd: {
    clientId: '49eb83bf-411b-4ab6-bda2-c7afe12f41b0',
    tenantId: 'f36628fb-a459-4a87-a3bf-ea3aede4d7eb',
    redirectUri: 'https://dg-use-nonprod-rmp-app-01.azurewebsites.net',
  },
};
