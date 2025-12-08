/**
 * Enumeração de papéis/cargos de funcionários
 */
export enum EmployeeRole {
  Employee = 1,
  Manager = 2
}

/**
 * Mapeamento de nomes amigáveis para os papéis
 */
export const EmployeeRoleLabels: Record<EmployeeRole, string> = {
  [EmployeeRole.Employee]: 'Funcionário',
  [EmployeeRole.Manager]: 'Gerente'
};

