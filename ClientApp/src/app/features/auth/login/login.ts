import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { msalInstance } from '../../../auth/msal.instance';
import { setDevSignedIn } from '../../../auth/dev-session';

@Component({
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  private readonly router = inject(Router);

  protected readonly isDevMode = environment.authMode === 'dev' || !msalInstance;

  protected signInWithMicrosoft(): void {
    msalInstance?.loginRedirect();
  }

  protected continueAsDevAdmin(): void {
    setDevSignedIn();
    this.router.navigateByUrl('/');
  }
}
