import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReleaseService } from '../../../core/services/release';
import { CommentService } from '../../../core/services/comment';
import { TaskForm } from '../task-form/task-form';
import { ReleaseDetail as ReleaseDetailModel } from '../../../core/models';

@Component({
  imports: [DatePipe, FormsModule, RouterLink, TaskForm],
  selector: 'app-release-detail',
  styleUrl: './release-detail.css',
  templateUrl: './release-detail.html',
})
export class ReleaseDetail implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly commentService = inject(CommentService);
  private readonly route = inject(ActivatedRoute);

  protected readonly release = signal<ReleaseDetailModel | null>(null);
  protected readonly showTaskForm = signal(false);
  protected newComment = '';

  private releaseId!: number;

  ngOnInit(): void {
    this.releaseId = Number(this.route.snapshot.paramMap.get('id'));
    this.reload();
  }

  private reload(): void {
    this.releaseService.get(this.releaseId).subscribe((release) => this.release.set(release));
  }

  protected onTaskCreated(): void {
    this.showTaskForm.set(false);
    this.reload();
  }

  protected addComment(): void {
    if (!this.newComment.trim()) {
      return;
    }

    this.commentService.create(this.releaseId, this.newComment).subscribe(() => {
      this.newComment = '';
      this.reload();
    });
  }
}
