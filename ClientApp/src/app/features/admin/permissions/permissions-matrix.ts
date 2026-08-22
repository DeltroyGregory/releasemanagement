import { Component, OnInit, inject, signal } from '@angular/core';
import { PermissionsService } from '../../../core/services/permissions';
import { PermissionMatrix } from '../../../core/models';

interface PermissionGroup {
  area: string;
  keys: { key: string; label: string }[];
}

@Component({
  selector: 'app-permissions-matrix',
  templateUrl: './permissions-matrix.html',
})
export class PermissionsMatrix implements OnInit {
  private readonly permissionsService = inject(PermissionsService);

  protected readonly matrix = signal<PermissionMatrix | null>(null);
  protected readonly groups = signal<PermissionGroup[]>([]);
  protected readonly saved = signal(false);

  ngOnInit(): void {
    this.reload();
  }

  private reload(): void {
    this.permissionsService.getMatrix().subscribe((matrix) => {
      this.matrix.set(matrix);
      this.groups.set(this.groupByArea(matrix));
    });
  }

  private groupByArea(matrix: PermissionMatrix): PermissionGroup[] {
    const byArea = new Map<string, { key: string; label: string }[]>();
    for (const p of matrix.permissions) {
      if (!byArea.has(p.area)) {
        byArea.set(p.area, []);
      }
      byArea.get(p.area)!.push({ key: p.key, label: p.label });
    }
    return [...byArea.entries()].map(([area, keys]) => ({ area, keys }));
  }

  protected isGranted(role: string, key: string): boolean {
    return this.matrix()?.grants[role]?.includes(key) ?? false;
  }

  protected toggle(role: string, key: string): void {
    if (role === 'Admin') {
      return;
    }

    const matrix = this.matrix();
    if (!matrix) {
      return;
    }

    const current = matrix.grants[role] ?? [];
    const next = current.includes(key) ? current.filter((k) => k !== key) : [...current, key];
    matrix.grants[role] = next;
    this.matrix.set({ ...matrix, grants: { ...matrix.grants } });
    this.saved.set(false);
  }

  protected save(): void {
    const matrix = this.matrix();
    if (!matrix) {
      return;
    }

    this.permissionsService.updateMatrix({ grants: matrix.grants }).subscribe((updated) => {
      this.matrix.set(updated);
      this.groups.set(this.groupByArea(updated));
      this.saved.set(true);
    });
  }
}
