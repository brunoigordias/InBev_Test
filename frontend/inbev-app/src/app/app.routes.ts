import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadComponent: () => import('./features/auth/auth-layout/auth-layout').then(m => m.AuthLayout),
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then(m => m.Login)
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./shared/layouts/main-layout/main-layout').then(m => m.MainLayout),
    children: [
      {
        path: 'employees',
        loadComponent: () => import('./features/employees/employee-list/employee-list').then(m => m.EmployeeList)
      },
      {
        path: 'employees/new',
        loadComponent: () => import('./features/employees/employee-form/employee-form').then(m => m.EmployeeForm)
      },
      {
        path: 'employees/:id',
        loadComponent: () => import('./features/employees/employee-form/employee-form').then(m => m.EmployeeForm)
      },
      {
        path: 'employees/:id/edit',
        loadComponent: () => import('./features/employees/employee-form/employee-form').then(m => m.EmployeeForm)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];
