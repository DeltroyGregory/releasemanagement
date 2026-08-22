import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { UserService } from '../../../../core/services/user';
import { InviteUserForm } from '../invite-user-form/invite-user-form';
import { RoleEditForm } from '../role-edit-form/role-edit-form';
import { User } from '../../../../core/models';

@Component({
  imports: [DatePipe, InviteUserForm, RoleEditForm],
  selector: 'app-user-list',
  templateUrl: './user-list.html',
})
export class UserList implements OnInit {
  private readonly userService = inject(UserService);

  protected readonly users = signal<User[]>([]);
  protected readonly roles = signal<string[]>([]);
  protected readonly showInviteForm = signal(false);
  protected readonly editingUser = signal<User | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  private reload(): void {
    forkJoin({ users: this.userService.list(), roles: this.userService.listRoles() }).subscribe(
      ({ users, roles }) => {
        this.users.set(users);
        this.roles.set(roles);
      },
    );
  }

  protected initials(user: User): string {
    const source = user.email ?? user.userName ?? '?';
    return source.slice(0, 2).toUpperCase();
  }

  protected onInvited(): void {
    this.showInviteForm.set(false);
    this.reload();
  }

  protected onRoleSaved(): void {
    this.editingUser.set(null);
    this.reload();
  }
}
