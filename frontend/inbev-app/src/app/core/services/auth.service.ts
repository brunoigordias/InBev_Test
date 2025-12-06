import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { ApiService } from './api.service';
import { LoginRequest, LoginResponse, AuthUser, TokenPayload } from '../../models';

/**
 * Serviço de autenticação
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiService = inject(ApiService);
  private readonly router = inject(Router);

  private readonly TOKEN_KEY = 'inbev_token';
  private readonly USER_KEY = 'inbev_user';

  // Estado do usuário autenticado
  private currentUserSubject = new BehaviorSubject<AuthUser | null>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  // Signal para o estado de autenticação
  public isAuthenticated = signal<boolean>(this.hasToken());

  constructor() {
    // Verificar se o token ainda é válido ao inicializar
    if (this.hasToken() && this.isTokenExpired()) {
      this.logout();
    }
  }

  /**
   * Realiza o login do usuário
   */
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.apiService.post<LoginResponse>('auth/login', credentials).pipe(
      tap(response => {
        if (response.token) {
          this.setToken(response.token);
          this.setUser(response.employee);
          this.currentUserSubject.next(response.employee);
          this.isAuthenticated.set(true);
        }
      })
    );
  }

  /**
   * Realiza o logout do usuário
   */
  logout(): void {
    this.removeToken();
    this.removeUser();
    this.currentUserSubject.next(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['/auth/login']);
  }

  /**
   * Obtém o token armazenado
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Verifica se há um token armazenado
   */
  hasToken(): boolean {
    return !!this.getToken();
  }

  /**
   * Armazena o token
   */
  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  /**
   * Remove o token
   */
  private removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  /**
   * Armazena os dados do usuário
   */
  private setUser(user: AuthUser): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  /**
   * Obtém os dados do usuário do storage
   */
  private getUserFromStorage(): AuthUser | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
  }

  /**
   * Obtém os dados do usuário atual
   */
  getCurrentUser(): AuthUser | null {
    return this.currentUserSubject.value;
  }

  /**
   * Remove os dados do usuário
   */
  private removeUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  /**
   * Decodifica o token JWT
   */
  private decodeToken(token: string): TokenPayload | null {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (error) {
      return null;
    }
  }

  /**
   * Verifica se o token está expirado
   */
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decoded = this.decodeToken(token);
    if (!decoded) return true;

    const expirationDate = new Date(decoded.exp * 1000);
    return expirationDate < new Date();
  }

  /**
   * Verifica se o usuário tem uma role específica
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user ? user.role.toString() === role : false;
  }
}

