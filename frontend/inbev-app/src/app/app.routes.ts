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
    path: 'employees',
    canActivate: [authGuard],
    loadComponent: () => import('./app').then(m => m.App), // Temporário - substituir depois
  },
  {
    path: '**',
    redirectTo: 'auth/login'
  }
];
