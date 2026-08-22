import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FixVersion } from '../models';

@Service()
export class FixVersionService {
  private readonly http = inject(HttpClient);

  listByRelease(releaseId: number) {
    return this.http.get<FixVersion[]>(`/api/fix-versions?releaseId=${releaseId}`);
  }

  create(releaseId: number, name: string, startDate?: string | null, endDate?: string | null) {
    return this.http.post<FixVersion>('/api/fix-versions', { releaseId, name, startDate, endDate });
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/fix-versions/${id}`);
  }
}
