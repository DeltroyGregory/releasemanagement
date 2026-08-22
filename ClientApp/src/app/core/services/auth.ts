import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthMe } from '../models';

@Service()
export class AuthService {
  private readonly http = inject(HttpClient);

  me() {
    return this.http.get<AuthMe>('/api/auth/me');
  }
}
