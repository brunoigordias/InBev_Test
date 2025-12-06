import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { EmployeeRole } from '../../models';

/**
 * Guard para proteger rotas baseado em role (permissões)
 */
export const roleGuard = (allowedRoles: EmployeeRole[]): CanActivateFn => {
  return (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const currentUser = authService.getCurrentUser();

    if (!currentUser) {
      router.navigate(['/auth/login']);
      return false;
    }

    if (allowedRoles.includes(currentUser.role)) {
      return true;
    }

    // Usuário não tem permissão - redirecionar para página de acesso negado
    router.navigate(['/access-denied']);
    return false;
  };
};

