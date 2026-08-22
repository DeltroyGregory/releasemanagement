import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../../core/services/task';
import { TaskItem } from '../../../core/models';

@Component({
  imports: [FormsModule],
  selector: 'app-task-form',
  styleUrl: './task-form.css',
  templateUrl: './task-form.html',
})
export class TaskForm {
  private readonly taskService = inject(TaskService);

  readonly releaseId = input.required<number>();
  readonly created = output<TaskItem>();
  readonly closed = output<void>();

  protected title = '';
  protected description = '';
  protected dueDate = '';

  protected submit(): void {
    this.taskService
      .create({
        releaseId: this.releaseId(),
        title: this.title,
        description: this.description || null,
        dueDate: this.dueDate ? new Date(this.dueDate).toISOString() : null,
      })
      .subscribe((task) => this.created.emit(task));
  }

  protected cancel(): void {
    this.closed.emit();
  }
}
