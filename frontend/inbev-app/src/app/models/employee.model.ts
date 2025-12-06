import { EmployeeRole } from './employee-role.enum';
import { PhoneNumber } from './phone-number.model';

/**
 * Interface para funcionário completo
 */
export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  birthDate: string | Date;
  role: EmployeeRole;
  managerId?: string | null;
  manager?: Employee | null;
  phoneNumbers: PhoneNumber[];
  subordinates?: Employee[];
  createdAt: string | Date;
  updatedAt?: string | Date | null;
  isActive: boolean;
}

/**
 * Interface para criação de funcionário (DTO)
 */
export interface CreateEmployee {
  firstName: string;
  lastName: string;
  email: string;
  docNumber: string;
  password: string;
  birthDate: string;
  role: EmployeeRole;
  managerId?: string | null;
  phoneNumbers: {
    number: string;
    type: number;
  }[];
}

/**
 * Interface para atualização de funcionário (DTO)
 */
export interface UpdateEmployee {
  firstName?: string;
  lastName?: string;
  email?: string;
  birthDate?: string;
  role?: EmployeeRole;
  managerId?: string | null;
  phoneNumbers?: {
    number: string;
    type: number;
  }[];
  isActive?: boolean;
}

/**
 * Interface para o nome completo
 */
export interface EmployeeNameDisplay {
  fullName: string;
  firstName: string;
  lastName: string;
}

