import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ReleaseService } from '../../core/services/release';
import { TaskService } from '../../core/services/task';
import { AuthService } from '../../core/services/auth';
import { Release, TaskItem } from '../../core/models';

interface AssignedTask extends TaskItem {
  releaseName: string;
}

interface StatusCount {
  status: string;
  count: number;
}

@Component({
  imports: [DatePipe, RouterLink],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly taskService = inject(TaskService);
  private readonly authService = inject(AuthService);

  protected readonly myTasks = signal<AssignedTask[]>([]);
  protected readonly myReleaseCount = signal(0);
  protected readonly statusCounts = signal<StatusCount[]>([]);

  ngOnInit(): void {
    this.authService.me().subscribe((me) => {
      if (!me.userId) {
        return;
      }

      forkJoin({
        releases: this.releaseService.list(),
        tasks: this.taskService.listAll(),
      }).subscribe(({ releases, tasks }) => {
        const releaseNames = new Map<number, string>(releases.map((r) => [r.id, r.name]));

        const assigned = tasks
          .filter((t) => t.assigneeUserId === me.userId)
          .map((t) => ({ ...t, releaseName: releaseNames.get(t.releaseId) ?? `#${t.releaseId}` }))
          .sort((a, b) => (a.dueDate ?? '9999').localeCompare(b.dueDate ?? '9999'));
        this.myTasks.set(assigned);

        const myReleases = releases.filter((r) => r.createdByUserId === me.userId);
        this.myReleaseCount.set(myReleases.length);
        this.statusCounts.set(this.countByStatus(myReleases));
      });
    });
  }

  private countByStatus(releases: Release[]): StatusCount[] {
    const counts = new Map<string, number>();
    for (const release of releases) {
      counts.set(release.status, (counts.get(release.status) ?? 0) + 1);
    }
    return [...counts.entries()].map(([status, count]) => ({ status, count }));
  }
}
