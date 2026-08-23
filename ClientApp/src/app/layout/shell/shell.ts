import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  selector: 'app-shell',
  styleUrl: './shell.css',
  templateUrl: './shell.html',
})
export class Shell {
  protected readonly navItems = [
    { label: 'Dashboard', path: 'dashboard' },
    { label: 'My Releases', path: 'my-releases' },
    { label: 'Releases', path: 'releases' },
  ];

  protected readonly adminNavItems = [
    { label: 'Users', path: 'admin/users' },
    { label: 'Permissions', path: 'admin/permissions' },
    { label: 'Task Fields', path: 'admin/task-fields' },
  ];
}
