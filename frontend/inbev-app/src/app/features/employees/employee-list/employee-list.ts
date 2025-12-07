import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';

import { EmployeeService } from '../../../core/services/employee.service';
import { Employee, EmployeeRole, EmployeeRoleLabels } from '../../../models';
import { CpfPipe } from '../../../shared/pipes/cpf-pipe';
import { PagedResponse } from '../../../models/paged-response.model';

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
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    DatePipe,
    CpfPipe,
    FormsModule
  ],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.scss',
})
export class EmployeeList implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private searchSubject = new Subject<string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  employees = signal<Employee[]>([]);
  isLoading = signal(true);
  displayedColumns: string[] = ['firstName', 'email', 'role', 'birthDate', 'actions'];
  
  // Paginação
  totalCount = signal(0);
  pageSize = signal(10);
  pageNumber = signal(1);
  pageSizeOptions = [5, 10, 25, 50, 100];
  
  // Busca
  searchTerm = '';

  ngOnInit(): void {
    this.loadEmployees();
    
    // Configurar debounce para busca
    this.searchSubject.pipe(
      debounceTime(500)
    ).subscribe(() => {
      this.pageNumber.set(1);
      this.loadEmployees();
    });
  }

  loadEmployees(): void {
    this.isLoading.set(true);
    
    this.employeeService.getPaged(
      this.pageNumber(),
      this.pageSize(),
      this.searchTerm
    ).subscribe({
      next: (response: PagedResponse<Employee>) => {
        this.employees.set(response.items);
        this.totalCount.set(response.totalCount);
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

  onPageChange(event: PageEvent): void {
    this.pageSize.set(event.pageSize);
    this.pageNumber.set(event.pageIndex + 1);
    this.loadEmployees();
  }

  onSearchChange(value: string): void {
    this.searchTerm = value;
    this.searchSubject.next(value);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.pageNumber.set(1);
    this.loadEmployees();
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
