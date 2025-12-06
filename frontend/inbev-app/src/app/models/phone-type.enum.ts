/**
 * Enumeração de tipos de telefone
 */
export enum PhoneType {
  Mobile = 1,
  Home = 2,
  Work = 3
}

/**
 * Mapeamento de nomes amigáveis para os tipos de telefone
 */
export const PhoneTypeLabels: Record<PhoneType, string> = {
  [PhoneType.Mobile]: 'Celular',
  [PhoneType.Home]: 'Residencial',
  [PhoneType.Work]: 'Comercial'
};

