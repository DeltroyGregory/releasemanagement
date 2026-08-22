import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TaskItem, TaskItemCreateRequest } from '../models';

@Service()
export class TaskService {
  private readonly http = inject(HttpClient);

  listByRelease(releaseId: number) {
    return this.http.get<TaskItem[]>(`/api/tasks?releaseId=${releaseId}`);
  }

  listAll() {
    return this.http.get<TaskItem[]>('/api/tasks');
  }

  create(request: TaskItemCreateRequest) {
    return this.http.post<TaskItem>('/api/tasks', request);
  }

  delete(id: number) {
    return this.http.delete<void>(`/api/tasks/${id}`);
  }
}
