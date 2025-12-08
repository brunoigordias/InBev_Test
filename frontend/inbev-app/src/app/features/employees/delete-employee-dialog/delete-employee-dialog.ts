import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { Employee } from '../../../models';

@Component({
  selector: 'app-delete-employee-dialog',
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './delete-employee-dialog.html',
  styleUrl: './delete-employee-dialog.scss',
})
export class DeleteEmployeeDialog {
  public dialogRef = inject(MatDialogRef<DeleteEmployeeDialog>);
  public data = inject<Employee>(MAT_DIALOG_DATA);

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
