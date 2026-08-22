import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../../core/services/user';
import { User } from '../../../../core/models';

@Component({
  imports: [FormsModule],
  selector: 'app-invite-user-form',
  templateUrl: './invite-user-form.html',
})
export class InviteUserForm {
  private readonly userService = inject(UserService);

  readonly roles = input.required<string[]>();
  readonly invited = output<User>();
  readonly closed = output<void>();

  protected email = '';
  protected role = '';
  protected error = '';

  protected submit(): void {
    this.error = '';
    this.userService.invite({ email: this.email, role: this.role }).subscribe({
      next: (user) => this.invited.emit(user),
      error: (err) => (this.error = err?.error ?? 'Could not invite that user.'),
    });
  }

  protected cancel(): void {
    this.closed.emit();
  }
}
