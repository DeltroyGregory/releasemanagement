import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task';
import { UserService } from '../../../core/services/user';
import { TaskItem, User } from '../../../core/models';

@Component({
  imports: [FormsModule],
  selector: 'app-task-form',
  styleUrl: './task-form.css',
  templateUrl: './task-form.html',
})
export class TaskForm implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly userService = inject(UserService);

  readonly releaseId = input.required<number>();
  readonly created = output<TaskItem>();
  readonly closed = output<void>();

  protected readonly users = signal<User[]>([]);

  protected title = '';
  protected description = '';
  protected dueDate = '';
  protected assigneeUserId = '';

  ngOnInit(): void {
    this.userService.list().subscribe((users) => this.users.set(users));
  }

  protected submit(): void {
    this.taskService
      .create({
        releaseId: this.releaseId(),
        title: this.title,
        description: this.description || null,
        assigneeUserId: this.assigneeUserId || null,
        dueDate: this.dueDate ? new Date(this.dueDate).toISOString() : null,
      })
      .subscribe((task) => this.created.emit(task));
  }

  protected cancel(): void {
    this.closed.emit();
  }
}
