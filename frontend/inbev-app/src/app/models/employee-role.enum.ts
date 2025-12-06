/**
 * Enumeração de papéis/cargos de funcionários
 */
export enum EmployeeRole {
  Employee = 1,
  Leader = 2,
  Director = 3
}

/**
 * Mapeamento de nomes amigáveis para os papéis
 */
export const EmployeeRoleLabels: Record<EmployeeRole, string> = {
  [EmployeeRole.Employee]: 'Funcionário',
  [EmployeeRole.Leader]: 'Líder',
  [EmployeeRole.Director]: 'Diretor'
};

