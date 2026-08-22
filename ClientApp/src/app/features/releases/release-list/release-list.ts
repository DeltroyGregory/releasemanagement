import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ReleaseService } from '../../../core/services/release';
import { AuthService } from '../../../core/services/auth';
import { Release } from '../../../core/models';

@Component({
  imports: [RouterLink, DatePipe],
  selector: 'app-release-list',
  styleUrl: './release-list.css',
  templateUrl: './release-list.html',
})
export class ReleaseList implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  protected readonly releases = signal<Release[]>([]);
  protected readonly title = signal('Releases');

  ngOnInit(): void {
    const mineOnly = this.route.snapshot.data['mineOnly'] === true;
    this.title.set(mineOnly ? 'My Releases' : 'Releases');

    this.releaseService.list().subscribe((releases) => {
      if (!mineOnly) {
        this.releases.set(releases);
        return;
      }

      this.authService.me().subscribe((me) => {
        this.releases.set(releases.filter((r) => r.createdByUserId === me.userId));
      });
    });
  }
}
