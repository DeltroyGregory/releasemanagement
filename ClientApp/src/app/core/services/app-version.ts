import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppVersion } from '../models';

@Service()
export class AppVersionService {
  private readonly http = inject(HttpClient);

  list() {
    return this.http.get<AppVersion[]>('/api/app-versions');
  }

  create(systemName: string, versionLabel: string) {
    return this.http.post<AppVersion>('/api/app-versions', { systemName, versionLabel });
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/app-versions/${id}`);
  }
}
