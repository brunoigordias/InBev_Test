import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { EmployeeService } from '../../../core/services/employee.service';
import { CreateEmployee, EmployeeRole, EmployeeRoleLabels, PhoneType, PhoneTypeLabels } from '../../../models';

@Component({
  selector: 'app-employee-form',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.scss',
})
export class EmployeeForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);

  employeeForm: FormGroup;
  isLoading = signal(false);
  isEditMode = signal(false);
  employeeId: string | null = null;

  roles = Object.values(EmployeeRole).filter(v => typeof v === 'number') as EmployeeRole[];
  roleLabels = EmployeeRoleLabels;
  phoneTypes = Object.values(PhoneType).filter(v => typeof v === 'number') as PhoneType[];
  phoneTypeLabels = PhoneTypeLabels;

  constructor() {
    this.employeeForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      docNumber: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      birthDate: ['', Validators.required],
      role: [EmployeeRole.Employee, Validators.required],
      phoneNumbers: this.fb.array([this.createPhoneFormGroup()])
    });
  }

  ngOnInit(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    if (this.employeeId) {
      this.isEditMode.set(true);
      // TODO: Carregar dados do funcionário para edição
    }
  }

  get phoneNumbers(): FormArray {
    return this.employeeForm.get('phoneNumbers') as FormArray;
  }

  createPhoneFormGroup(): FormGroup {
    return this.fb.group({
      number: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
      type: [PhoneType.Mobile, Validators.required]
    });
  }

  addPhone(): void {
    this.phoneNumbers.push(this.createPhoneFormGroup());
  }

  removePhone(index: number): void {
    if (this.phoneNumbers.length > 1) {
      this.phoneNumbers.removeAt(index);
    }
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const formValue = this.employeeForm.value;
    
    const employee: CreateEmployee = {
      ...formValue,
      birthDate: new Date(formValue.birthDate).toISOString().split('T')[0]
    };

    this.employeeService.create(employee).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.snackBar.open('Funcionário criado com sucesso!', 'Fechar', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/employees']);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.snackBar.open(
          error.message || 'Erro ao criar funcionário',
          'Fechar',
          { duration: 5000, panelClass: ['error-snackbar'] }
        );
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }
}
