import { Pipe, PipeTransform } from '@angular/core';

/**
 * Pipe para formatar CPF
 * Transforma "12345678900" em "123.456.789-00"
 */
@Pipe({
  name: 'cpf',
  standalone: true
})
export class CpfPipe implements PipeTransform {

  transform(value: string | null | undefined): string {
    if (!value) return '';

    // Remove tudo que não é número
    const cleanValue = value.replace(/\D/g, '');

    // Verifica se tem 11 dígitos
    if (cleanValue.length !== 11) {
      return value; // Retorna o valor original se não for válido
    }

    // Formata: 123.456.789-00
    return cleanValue.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  }

}
