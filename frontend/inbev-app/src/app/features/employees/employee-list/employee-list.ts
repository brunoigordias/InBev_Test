import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { DatePipe } from '@angular/common';

import { EmployeeService } from '../../../core/services/employee.service';
import { Employee, EmployeeRole, EmployeeRoleLabels } from '../../../models';
import { CpfPipe } from '../../../shared/pipes/cpf-pipe';

@Component({
  selector: 'app-employee-list',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatCardModule,
    MatChipsModule,
    DatePipe,
    CpfPipe
  ],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.scss',
})
export class EmployeeList implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  employees = signal<Employee[]>([]);
  isLoading = signal(true);
  displayedColumns: string[] = ['firstName', 'email', 'role', 'birthDate', 'actions'];

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.isLoading.set(true);
    this.employeeService.getAll().subscribe({
      next: (employees) => {
        this.employees.set(employees);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.snackBar.open(
          error.message || 'Erro ao carregar funcionários',
          'Fechar',
          { duration: 5000, panelClass: ['error-snackbar'] }
        );
      }
    });
  }

  getRoleLabel(role: EmployeeRole): string {
    return EmployeeRoleLabels[role] || '';
  }

  getRoleColor(role: EmployeeRole): string {
    switch (role) {
      case EmployeeRole.Director:
        return 'warn';
      case EmployeeRole.Leader:
        return 'accent';
      default:
        return 'primary';
    }
  }

  viewEmployee(id: string): void {
    this.router.navigate(['/employees', id]);
  }

  editEmployee(id: string): void {
    this.router.navigate(['/employees', id, 'edit']);
  }

  deleteEmployee(employee: Employee): void {
    if (confirm(`Deseja realmente excluir ${employee.firstName} ${employee.lastName}?`)) {
      this.employeeService.delete(employee.id).subscribe({
        next: () => {
          this.snackBar.open('Funcionário excluído com sucesso!', 'Fechar', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.loadEmployees();
        },
        error: (error) => {
          this.snackBar.open(
            error.message || 'Erro ao excluir funcionário',
            'Fechar',
            { duration: 5000, panelClass: ['error-snackbar'] }
          );
        }
      });
    }
  }

  createNew(): void {
    this.router.navigate(['/employees/new']);
  }
}
