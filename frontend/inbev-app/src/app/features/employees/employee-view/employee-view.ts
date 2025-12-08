import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { DatePipe } from '@angular/common';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EmployeeService } from '../../../core/services/employee.service';
import { AuthService } from '../../../core/services/auth.service';
import { Employee, EmployeeRole, EmployeeRoleLabels, PhoneTypeLabels } from '../../../models';
import { CpfPipe } from '../../../shared/pipes/cpf-pipe';

@Component({
  selector: 'app-employee-view',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatDividerModule,
    MatListModule,
    DatePipe,
    MatSnackBarModule,
    CpfPipe
  ],
  templateUrl: './employee-view.html',
  styleUrl: './employee-view.scss',
})
export class EmployeeView implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  employee = signal<Employee | null>(null);
  isLoading = signal(true);
  isManager = signal(false);
  
  phoneTypeLabels = PhoneTypeLabels;

  ngOnInit(): void {
    // Verificar se o usuário logado é gerente
    const currentUser = this.authService.getCurrentUser();
    this.isManager.set(currentUser?.role === EmployeeRole.Manager);

    // Obter ID do funcionário da rota
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEmployee(id);
    } else {
      this.router.navigate(['/employees']);
    }
  }

  loadEmployee(id: string): void {
    this.isLoading.set(true);
    this.employeeService.getById(id).subscribe({
      next: (employee) => {
        this.employee.set(employee);
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

  getRoleLabel(role: EmployeeRole): string {
    return EmployeeRoleLabels[role] || '';
  }

  getRoleColor(role: EmployeeRole): string {
    return role === EmployeeRole.Manager ? 'warn' : 'primary';
  }

  getAge(birthDate: string | Date): number {
    const today = new Date();
    const birth = typeof birthDate === 'string' ? new Date(birthDate) : birthDate;
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age;
  }

  goBack(): void {
    this.router.navigate(['/employees']);
  }

  editEmployee(): void {
    if (this.employee()) {
      this.router.navigate(['/employees', this.employee()!.id, 'edit']);
    }
  }
}
