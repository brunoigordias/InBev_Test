import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Employee, CreateEmployee, UpdateEmployee } from '../../models';

/**
 * Serviço para gerenciamento de funcionários
 */
@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly apiService = inject(ApiService);
  private readonly endpoint = 'employees';

  /**
   * Busca todos os funcionários
   */
  getAll(): Observable<Employee[]> {
    return this.apiService.get<Employee[]>(this.endpoint);
  }

  /**
   * Busca um funcionário por ID
   */
  getById(id: string): Observable<Employee> {
    return this.apiService.get<Employee>(`${this.endpoint}/${id}`);
  }

  /**
   * Cria um novo funcionário
   */
  create(employee: CreateEmployee): Observable<Employee> {
    return this.apiService.post<Employee>(this.endpoint, employee);
  }

  /**
   * Atualiza um funcionário existente
   */
  update(id: string, employee: UpdateEmployee): Observable<Employee> {
    return this.apiService.put<Employee>(`${this.endpoint}/${id}`, employee);
  }

  /**
   * Deleta um funcionário
   */
  delete(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }

  /**
   * Busca funcionários por role
   */
  getByRole(role: number): Observable<Employee[]> {
    return this.apiService.get<Employee[]>(`${this.endpoint}/role/${role}`);
  }

  /**
   * Busca subordinados de um gerente
   */
  getSubordinates(managerId: string): Observable<Employee[]> {
    return this.apiService.get<Employee[]>(`${this.endpoint}/${managerId}/subordinates`);
  }

  /**
   * Busca gerentes disponíveis (para seleção)
   */
  getAvailableManagers(): Observable<Employee[]> {
    return this.apiService.get<Employee[]>(`${this.endpoint}/managers`);
  }
}

