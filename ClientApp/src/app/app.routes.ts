import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';
import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard';
import { ReleaseList } from './features/releases/release-list/release-list';
import { ReleaseDetail } from './features/releases/release-detail/release-detail';
import { ReleaseForm } from './features/releases/release-form/release-form';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: Dashboard },
      { path: 'my-releases', component: ReleaseList, data: { mineOnly: true } },
      { path: 'releases', component: ReleaseList, data: { mineOnly: false } },
      { path: 'releases/new', component: ReleaseForm },
      { path: 'releases/:id', component: ReleaseDetail },
      { path: 'releases/:id/edit', component: ReleaseForm },
    ],
  },
];
