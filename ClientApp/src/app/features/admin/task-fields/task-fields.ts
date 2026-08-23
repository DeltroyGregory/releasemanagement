import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LookupsService } from '../../../core/services/lookups';
import { LookupCategory, LookupItem } from '../../../core/models';

interface CategoryTab {
  category: LookupCategory;
  label: string;
}

@Component({
  imports: [FormsModule],
  selector: 'app-task-fields',
  templateUrl: './task-fields.html',
})
export class TaskFields implements OnInit {
  private readonly lookupsService = inject(LookupsService);

  protected readonly tabs: CategoryTab[] = [
    { category: 'TaskType', label: 'Type' },
    { category: 'Component', label: 'Component' },
    { category: 'AppName', label: 'App name' },
    { category: 'Version', label: 'Version' },
  ];

  protected readonly selected = signal<LookupCategory>('TaskType');
  protected readonly items = signal<LookupItem[]>([]);
  protected newValue = '';
  protected error = '';

  ngOnInit(): void {
    this.reload();
  }

  protected selectCategory(category: LookupCategory): void {
    this.selected.set(category);
    this.newValue = '';
    this.error = '';
    this.reload();
  }

  private reload(): void {
    this.lookupsService.list(this.selected()).subscribe((items) => this.items.set(items));
  }

  protected add(): void {
    if (!this.newValue.trim()) {
      return;
    }

    this.error = '';
    this.lookupsService.create({ category: this.selected(), value: this.newValue.trim() }).subscribe({
      next: () => {
        this.newValue = '';
        this.reload();
      },
      error: (err) => (this.error = err?.error ?? 'Could not add that value.'),
    });
  }

  protected remove(item: LookupItem): void {
    this.lookupsService.delete(item.id).subscribe(() => this.reload());
  }
}
