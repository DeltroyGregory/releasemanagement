import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Release, ReleaseCreateRequest, ReleaseDetail, ReleaseUpdateRequest } from '../models';

@Service()
export class ReleaseService {
  private readonly http = inject(HttpClient);

  list(status?: string) {
    const url = status ? `/api/releases?status=${encodeURIComponent(status)}` : '/api/releases';
    return this.http.get<Release[]>(url);
  }

  get(id: number) {
    return this.http.get<ReleaseDetail>(`/api/releases/${id}`);
  }

  create(request: ReleaseCreateRequest) {
    return this.http.post<Release>('/api/releases', request);
  }

  update(id: number, request: ReleaseUpdateRequest) {
    return this.http.put<Release>(`/api/releases/${id}`, request);
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/releases/${id}`);
  }
}
