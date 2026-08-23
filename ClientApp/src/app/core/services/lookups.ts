import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LookupCategory, LookupItem, LookupItemCreateRequest, LookupItemUpdateRequest } from '../models';

@Service()
export class LookupsService {
  private readonly http = inject(HttpClient);

  list(category?: LookupCategory) {
    const url = category ? `/api/lookups?category=${encodeURIComponent(category)}` : '/api/lookups';
    return this.http.get<LookupItem[]>(url);
  }

  create(request: LookupItemCreateRequest) {
    return this.http.post<LookupItem>('/api/lookups', request);
  }

  update(id: number, request: LookupItemUpdateRequest) {
    return this.http.put<LookupItem>(`/api/lookups/${id}`, request);
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/lookups/${id}`);
  }
}
