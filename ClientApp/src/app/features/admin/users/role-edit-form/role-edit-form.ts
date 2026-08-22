import { Component, OnInit, inject, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { UserService } from '../../../../core/services/user';
import { User } from '../../../../core/models';

@Component({
  imports: [DatePipe],
  selector: 'app-role-edit-form',
  templateUrl: './role-edit-form.html',
})
export class RoleEditForm implements OnInit {
  private readonly userService = inject(UserService);

  readonly user = input.required<User>();
  readonly roles = input.required<string[]>();
  readonly saved = output<User>();
  readonly closed = output<void>();

  protected selectedRole = '';

  ngOnInit(): void {
    this.selectedRole = this.user().role ?? '';
  }

  private static readonly roleDescriptions: Record<string, string> = {
    Admin: 'Full access, including user and permission management.',
    'Release Coordinator': 'Manage releases, tasks, and checklists across teams.',
    'Power User': 'Create and edit releases and tasks; no admin access.',
    Reader: 'View-only access to releases and tasks.',
  };

  protected initials(): string {
    const source = this.user().email ?? this.user().userName ?? '?';
    return source.slice(0, 2).toUpperCase();
  }

  protected roleDescription(role: string): string {
    return RoleEditForm.roleDescriptions[role] ?? '';
  }

  protected save(): void {
    this.userService.updateRole(this.user().id, { role: this.selectedRole }).subscribe((user) => this.saved.emit(user));
  }

  protected cancel(): void {
    this.closed.emit();
  }
}
