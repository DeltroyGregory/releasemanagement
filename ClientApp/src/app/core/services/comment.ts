import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Comment } from '../models';

@Service()
export class CommentService {
  private readonly http = inject(HttpClient);

  listByRelease(releaseId: number) {
    return this.http.get<Comment[]>(`/api/comments?releaseId=${releaseId}`);
  }

  create(releaseId: number, body: string) {
    return this.http.post<Comment>('/api/comments', { releaseId, body });
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/comments/${id}`);
  }
}
