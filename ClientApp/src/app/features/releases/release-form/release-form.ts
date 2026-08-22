import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReleaseService } from '../../../core/services/release';
import { ReleaseTypeName } from '../../../core/models';

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-release-form',
  styleUrl: './release-form.css',
  templateUrl: './release-form.html',
})
export class ReleaseForm implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly releaseTypes: ReleaseTypeName[] = ['Major', 'Minor', 'Patch', 'Hotfix'];
  protected readonly isEdit = signal(false);
  protected readonly saving = signal(false);

  protected name = '';
  protected description = '';
  protected releaseType: ReleaseTypeName = 'Minor';
  protected status = 'Planned';
  protected targetDate = '';

  private editId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    this.editId = Number(idParam);
    this.isEdit.set(true);
    this.releaseService.get(this.editId).subscribe((release) => {
      this.name = release.name;
      this.description = release.description ?? '';
      this.releaseType = release.releaseType;
      this.status = release.status;
      this.targetDate = release.targetDate ? release.targetDate.substring(0, 10) : '';
    });
  }

  protected submit(): void {
    this.saving.set(true);
    const targetDate = this.targetDate ? new Date(this.targetDate).toISOString() : null;

    const request$ = this.isEdit()
      ? this.releaseService.update(this.editId!, {
          name: this.name,
          description: this.description || null,
          releaseType: this.releaseType,
          status: this.status,
          targetDate,
        })
      : this.releaseService.create({
          name: this.name,
          description: this.description || null,
          releaseType: this.releaseType,
          targetDate,
        });

    request$.subscribe((release) => {
      this.saving.set(false);
      this.router.navigate(['/releases', this.isEdit() ? this.editId : release.id]);
    });
  }
}
