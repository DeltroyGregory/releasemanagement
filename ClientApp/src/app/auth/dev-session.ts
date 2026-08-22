const DEV_SESSION_KEY = 'rmp.devSignedIn';

/**
 * Stand-in for a real sign-in when no Azure AD app registration exists yet (authMode === 'dev').
 * DevAuthHandler on the backend authenticates every request as the seeded dev admin regardless,
 * so this only gates whether the in-app login screen has been passed — not real authentication.
 */
export function isDevSignedIn(): boolean {
  return sessionStorage.getItem(DEV_SESSION_KEY) === 'true';
}

export function setDevSignedIn(): void {
  sessionStorage.setItem(DEV_SESSION_KEY, 'true');
}
