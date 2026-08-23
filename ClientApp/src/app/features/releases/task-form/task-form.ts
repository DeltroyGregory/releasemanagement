import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { TaskService } from '../../../core/services/task';
import { UserService } from '../../../core/services/user';
import { LookupsService } from '../../../core/services/lookups';
import { LookupItem, TaskItem, User } from '../../../core/models';

@Component({
  imports: [FormsModule],
  selector: 'app-task-form',
  styleUrl: './task-form.css',
  templateUrl: './task-form.html',
})
export class TaskForm implements OnInit {
  private readonly taskService = inject(TaskService);
  private readonly userService = inject(UserService);
  private readonly lookupsService = inject(LookupsService);

  readonly releaseId = input.required<number>();
  readonly created = output<TaskItem>();
  readonly closed = output<void>();

  protected readonly users = signal<User[]>([]);
  protected readonly types = signal<LookupItem[]>([]);
  protected readonly components = signal<LookupItem[]>([]);
  protected readonly appNames = signal<LookupItem[]>([]);
  protected readonly versions = signal<LookupItem[]>([]);

  protected title = '';
  protected description = '';
  protected assigneeUserId = '';
  protected startDate = '';
  protected startTime = '';
  protected endDate = '';
  protected endTime = '';
  protected typeId = '';
  protected componentId = '';
  protected appNameId = '';
  protected versionId = '';

  ngOnInit(): void {
    this.userService.list().subscribe((users) => this.users.set(users));
    forkJoin({
      types: this.lookupsService.list('TaskType'),
      components: this.lookupsService.list('Component'),
      appNames: this.lookupsService.list('AppName'),
      versions: this.lookupsService.list('Version'),
    }).subscribe(({ types, components, appNames, versions }) => {
      this.types.set(types);
      this.components.set(components);
      this.appNames.set(appNames);
      this.versions.set(versions);
    });
  }

  protected submit(): void {
    this.taskService
      .create({
        releaseId: this.releaseId(),
        title: this.title,
        description: this.description || null,
        assigneeUserId: this.assigneeUserId || null,
        startDate: combineDateTime(this.startDate, this.startTime),
        endDate: combineDateTime(this.endDate, this.endTime),
        typeId: this.typeId ? Number(this.typeId) : null,
        componentId: this.componentId ? Number(this.componentId) : null,
        appNameId: this.appNameId ? Number(this.appNameId) : null,
        versionId: this.versionId ? Number(this.versionId) : null,
      })
      .subscribe((task) => this.created.emit(task));
  }

  protected cancel(): void {
    this.closed.emit();
  }
}

function combineDateTime(date: string, time: string): string | null {
  if (!date) {
    return null;
  }
  return new Date(`${date}T${time || '00:00'}`).toISOString();
}
