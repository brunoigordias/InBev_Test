import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray, AbstractControl, ValidationErrors } from '@angular/forms';
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
import { AuthService } from '../../../core/services/auth.service';
import { CreateEmployee, EmployeeRole, EmployeeRoleLabels, PhoneType, PhoneTypeLabels, Employee } from '../../../models';

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
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);

  employeeForm: FormGroup;
  isLoading = signal(false);
  isEditMode = signal(false);
  isManager = signal(false);
  employeeId: string | null = null;

  // Listas de opções
  roleLabels = EmployeeRoleLabels;
  phoneTypes = Object.values(PhoneType).filter(v => typeof v === 'number') as PhoneType[];
  phoneTypeLabels = PhoneTypeLabels;
  
  // Lista de gerentes disponíveis
  managers = signal<Employee[]>([]);

  constructor() {
    // Verificar se o usuário logado é gerente
    const currentUser = this.authService.getCurrentUser();
    const isManagerValue = currentUser?.role === EmployeeRole.Manager;
    this.isManager.set(isManagerValue);

    this.employeeForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      docNumber: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      birthDate: ['', [Validators.required, this.minAgeValidator(18)]],
      managerId: [null, isManagerValue ? [] : [Validators.required]], // Obrigatório para não-gerentes
      phoneNumbers: this.fb.array([this.createPhoneFormGroup()])
    });
  }

  /**
   * Validador customizado para idade mínima
   */
  minAgeValidator(minAge: number) {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null; // Se não há valor, o Validators.required cuida disso
      }

      const birthDate = new Date(control.value);
      const today = new Date();
      
      // Calcular a idade
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();
      
      // Ajustar se ainda não fez aniversário este ano
      if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
      }

      return age >= minAge ? null : { minAge: { requiredAge: minAge, actualAge: age } };
    };
  }

  ngOnInit(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    
    // Carregar lista de gerentes disponíveis
    this.loadManagers();
    
    if (this.employeeId) {
      this.isEditMode.set(true);
      
      // Tornar senha opcional no modo edição
      this.employeeForm.get('password')?.clearValidators();
      this.employeeForm.get('password')?.setValidators([Validators.minLength(6)]);
      this.employeeForm.get('password')?.updateValueAndValidity();
      
      this.loadEmployee(this.employeeId);
    }
  }

  /**
   * Carrega lista de gerentes disponíveis
   */
  loadManagers(): void {
    this.employeeService.getPaged(1, 100).subscribe({
      next: (response) => {
        this.managers.set(response.items);
      },
      error: (error) => {
        console.error('Erro ao carregar gerentes:', error);
      }
    });
  }

  loadEmployee(id: string): void {
    this.isLoading.set(true);
    this.employeeService.getById(id).subscribe({
      next: (employee) => {
        // Limpar os telefones existentes
        while (this.phoneNumbers.length > 0) {
          this.phoneNumbers.removeAt(0);
        }

        // Adicionar os telefones do funcionário
        employee.phoneNumbers.forEach(phone => {
          const phoneGroup = this.fb.group({
            number: [phone.number, [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
            type: [phone.type, Validators.required]
          });
          this.phoneNumbers.push(phoneGroup);
        });

        // Preencher o formulário (exceto senha)
        this.employeeForm.patchValue({
          firstName: employee.firstName,
          lastName: employee.lastName,
          email: employee.email,
          docNumber: employee.docNumber,
          birthDate: new Date(employee.birthDate),
          managerId: employee.managerId,
          password: '' // Senha vazia no modo edição
        });

        this.isLoading.set(false);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.snackBar.open(
          error.message || 'Erro ao carregar funcionário',
          'Fechar',
          { duration: 5000, panelClass: ['error-snackbar'] }
        );
        this.router.navigate(['/employees']);
      }
    });
  }

  get phoneNumbers(): FormArray {
    return this.employeeForm.get('phoneNumbers') as FormArray;
  }

  get phoneNumbersControls(): FormGroup[] {
    return this.phoneNumbers.controls as FormGroup[];
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
    
    if (this.isEditMode() && this.employeeId) {
      // Modo de edição
      const updateData: any = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        birthDate: new Date(formValue.birthDate).toISOString().split('T')[0],
        managerId: formValue.managerId || null,
        phoneNumbers: formValue.phoneNumbers
      };

      this.employeeService.update(this.employeeId, updateData).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.snackBar.open('Funcionário atualizado com sucesso!', 'Fechar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.router.navigate(['/employees']);
        },
        error: (error) => {
          this.isLoading.set(false);
          this.snackBar.open(
            error.message || 'Erro ao atualizar funcionário',
            'Fechar',
            { duration: 5000, panelClass: ['error-snackbar'] }
          );
        }
      });
    } else {
      // Modo de criação
      // Determinar role automaticamente: sem gerente = Manager, com gerente = Employee
      const role = formValue.managerId ? EmployeeRole.Employee : EmployeeRole.Manager;
      
      const employee: CreateEmployee = {
        ...formValue,
        role: role,
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
  }

  cancel(): void {
    this.router.navigate(['/employees']);
  }
}
