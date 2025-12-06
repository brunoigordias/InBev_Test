import { PhoneType } from './phone-type.enum';

/**
 * Interface para número de telefone
 */
export interface PhoneNumber {
  id?: string;
  number: string;
  type: PhoneType;
  employeeId?: string;
}

/**
 * Interface para criação de telefone (DTO)
 */
export interface CreatePhoneNumber {
  number: string;
  type: PhoneType;
}

