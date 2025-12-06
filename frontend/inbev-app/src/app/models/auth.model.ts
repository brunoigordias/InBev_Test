import { EmployeeRole } from './employee-role.enum';

/**
 * Interface para requisição de login
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Interface para resposta de login
 */
export interface LoginResponse {
  token: string;
  employee: AuthUser;
}

/**
 * Interface para o usuário autenticado
 */
export interface AuthUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: EmployeeRole;
}

/**
 * Interface para o payload do token JWT decodificado
 */
export interface TokenPayload {
  sub: string; // User ID
  email: string;
  role: string;
  unique_name: string;
  nbf: number;
  exp: number;
  iat: number;
}

