import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User, UserInviteRequest, UserRoleUpdateRequest } from '../models';

@Service()
export class UserService {
  private readonly http = inject(HttpClient);

  list() {
    return this.http.get<User[]>('/api/users');
  }

  listRoles() {
    return this.http.get<string[]>('/api/users/roles');
  }

  invite(request: UserInviteRequest) {
    return this.http.post<User>('/api/users/invite', request);
  }

  updateRole(id: string, request: UserRoleUpdateRequest) {
    return this.http.put<User>(`/api/users/${id}/role`, request);
  }
}
