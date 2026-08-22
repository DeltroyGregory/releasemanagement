import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ReleaseSystem } from '../models';

@Service()
export class ReleaseSystemService {
  private readonly http = inject(HttpClient);

  listByRelease(releaseId: number) {
    return this.http.get<ReleaseSystem[]>(`/api/release-systems?releaseId=${releaseId}`);
  }

  create(releaseId: number, systemName: string, notes?: string) {
    return this.http.post<ReleaseSystem>('/api/release-systems', { releaseId, systemName, notes });
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/release-systems/${id}`);
  }
}
