import { StatusChipComponent } from './status-chip.component';

describe('StatusChipComponent', () => {
  it('formats InTransit nicely', () => {
    const chip = new StatusChipComponent();
    chip.status = 'InTransit';
    expect(chip.label).toBe('In transit');
  });

  it('passes other statuses through', () => {
    const chip = new StatusChipComponent();
    chip.status = 'Delivered';
    expect(chip.label).toBe('Delivered');
  });
});
