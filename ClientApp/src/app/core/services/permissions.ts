import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PermissionMatrix, PermissionMatrixUpdateRequest } from '../models';

@Service()
export class PermissionsService {
  private readonly http = inject(HttpClient);

  getMatrix() {
    return this.http.get<PermissionMatrix>('/api/permissions');
  }

  updateMatrix(request: PermissionMatrixUpdateRequest) {
    return this.http.put<PermissionMatrix>('/api/permissions', request);
  }
}
