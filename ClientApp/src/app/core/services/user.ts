import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models';

@Service()
export class UserService {
  private readonly http = inject(HttpClient);

  list() {
    return this.http.get<User[]>('/api/users');
  }
}
