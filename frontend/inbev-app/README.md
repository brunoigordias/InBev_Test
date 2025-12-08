# 🎨 InBev Employee Management - Frontend

Interface web moderna desenvolvida em **Angular 19** para o sistema de gerenciamento de funcionários da InBev.

## 📋 Sobre

Frontend responsivo e moderno utilizando as últimas funcionalidades do Angular, incluindo Standalone Components, Signals e Angular Material para uma experiência de usuário consistente e profissional.

## 🚀 Tecnologias

- **Angular 19.1** - Framework SPA
- **TypeScript 5.7** - Superset tipado do JavaScript
- **Angular Material 19.0** - Biblioteca de componentes UI
- **RxJS 7.8** - Programação reativa
- **SCSS** - Pré-processador CSS
- **Jasmine + Karma** - Framework de testes

## ✨ Funcionalidades

### Autenticação
- ✅ Login com JWT
- ✅ Registro de novos usuários
- ✅ Troca de senha
- ✅ Auto-logout em caso de token expirado
- ✅ Guards de rota para proteção

### Gerenciamento de Funcionários
- ✅ Listagem com paginação
- ✅ Criação de funcionários
- ✅ Edição de dados
- ✅ Exclusão com confirmação
- ✅ Visualização de detalhes
- ✅ Gerenciamento de telefones
- ✅ Validação de CPF brasileiro
- ✅ Filtros e busca

### Interface
- ✅ Design responsivo (mobile-first)
- ✅ Material Design
- ✅ Loading states
- ✅ Mensagens de erro/sucesso
- ✅ Confirmações de ações críticas
- ✅ Navegação intuitiva

## 📦 Pré-requisitos

- Node.js 20.x ou superior
- npm 10.x ou superior
- Angular CLI 19.x

## ⚙️ Instalação

```bash
# Instalar dependências
npm install

# Instalar Angular CLI globalmente (se necessário)
npm install -g @angular/cli
```

## 🏃 Executar Aplicação

### Modo Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm start

# Ou com ng serve
ng serve

# Com host específico
ng serve --host 0.0.0.0

# Com porta customizada
ng serve --port 4201
```

A aplicação estará disponível em: **http://localhost:4200**

### Build de Produção

```bash
# Build para produção
npm run build

# Build com otimizações
ng build --configuration production

# Build com análise de bundle
ng build --stats-json
npm run analyze
```

Os arquivos de build serão gerados em `dist/inbev-app/`.

## 🧪 Testes

### Testes Unitários

```bash
# Executar testes
npm test

# Executar com cobertura
npm run test:coverage

# Executar em modo headless (CI)
npm run test:ci
```

### Testes E2E

```bash
# Executar testes end-to-end
npm run e2e
```

## 📁 Estrutura do Projeto

```
src/
├── app/
│   ├── core/                      # Serviços core, guards, interceptors
│   │   ├── guards/
│   │   │   ├── auth.guard.ts      # Proteção de rotas autenticadas
│   │   │   ├── no-auth.guard.ts   # Redireciona usuários logados
│   │   │   └── role.guard.ts      # Controle de permissões
│   │   ├── interceptors/
│   │   │   ├── auth.interceptor.ts    # Adiciona token JWT
│   │   │   ├── error.interceptor.ts   # Tratamento de erros HTTP
│   │   │   └── loading.interceptor.ts # Controle de loading
│   │   └── services/
│   │       ├── auth.service.ts        # Serviço de autenticação
│   │       ├── employee.service.ts    # Serviço de funcionários
│   │       ├── loading.service.ts     # Controle de loading global
│   │       └── storage.service.ts     # Armazenamento local
│   │
│   ├── features/                  # Funcionalidades da aplicação
│   │   ├── auth/
│   │   │   ├── login/            # Página de login
│   │   │   └── register/         # Página de registro
│   │   ├── employees/
│   │   │   ├── employee-list/    # Lista de funcionários
│   │   │   ├── employee-form/    # Formulário criar/editar
│   │   │   ├── employee-detail/  # Detalhes do funcionário
│   │   │   └── employee-delete-dialog/  # Diálogo de confirmação
│   │   └── change-password/      # Troca de senha
│   │
│   ├── models/                    # Interfaces e tipos TypeScript
│   │   ├── employee.model.ts
│   │   ├── auth.model.ts
│   │   ├── employee-role.enum.ts
│   │   ├── phone-type.enum.ts
│   │   └── paged-response.model.ts
│   │
│   ├── shared/                    # Componentes e utilitários compartilhados
│   │   ├── layouts/
│   │   │   └── main-layout/      # Layout principal com header
│   │   └── pipes/
│   │       └── cpf.pipe.ts       # Formatação de CPF
│   │
│   ├── app.config.ts             # Configuração da aplicação
│   ├── app.routes.ts             # Configuração de rotas
│   └── app.ts                    # Componente raiz
│
├── environments/                  # Variáveis de ambiente
│   ├── environment.ts            # Desenvolvimento
│   └── environment.prod.ts       # Produção
│
├── styles.scss                   # Estilos globais
└── index.html                    # HTML principal
```

## 🔐 Configuração de Ambiente

### Desenvolvimento

Edite `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5141/api'
};
```

### Produção

Edite `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.inbev.com/api'
};
```

## 🎨 Arquitetura Frontend

### Standalone Components

Todos os componentes utilizam a nova abordagem standalone do Angular 19, eliminando a necessidade de NgModules.

### Reactive Forms

Formulários construídos com FormBuilder e validações customizadas.

### HTTP Interceptors

- **AuthInterceptor**: Adiciona token JWT automaticamente
- **ErrorInterceptor**: Tratamento centralizado de erros
- **LoadingInterceptor**: Controle de estados de carregamento

### Route Guards

- **AuthGuard**: Protege rotas que requerem autenticação
- **NoAuthGuard**: Redireciona usuários logados (ex: página de login)
- **RoleGuard**: Controle de acesso baseado em permissões

### Services

Serviços injetáveis utilizando `providedIn: 'root'` para singleton global.

## 📱 Responsividade

Interface totalmente responsiva com breakpoints:

- **Mobile**: < 600px
- **Tablet**: 600px - 960px
- **Desktop**: > 960px

## 🎯 Padrões de Código

### Nomenclatura

- **Componentes**: PascalCase (ex: `EmployeeListComponent`)
- **Serviços**: PascalCase com sufixo Service (ex: `AuthService`)
- **Interfaces**: PascalCase (ex: `Employee`)
- **Enums**: PascalCase (ex: `EmployeeRole`)
- **Arquivos**: kebab-case (ex: `employee-list.component.ts`)

### Estrutura de Componente

```typescript
@Component({
  selector: 'app-example',
  standalone: true,
  imports: [...],
  templateUrl: './example.component.html',
  styleUrls: ['./example.component.scss']
})
export class ExampleComponent {
  // Signals
  data = signal<Data[]>([]);
  
  // Injeção de dependências
  constructor(private service: Service) {}
  
  // Lifecycle hooks
  ngOnInit(): void {}
  
  // Métodos
  loadData(): void {}
}
```

## 🛠️ Scripts Disponíveis

| Script | Descrição |
|--------|-----------|
| `npm start` | Inicia servidor de desenvolvimento |
| `npm run build` | Build de produção |
| `npm test` | Executa testes unitários |
| `npm run test:coverage` | Testes com cobertura |
| `npm run lint` | Verifica problemas de código |
| `npm run format` | Formata código com Prettier |

## 🐛 Debugging

### VS Code

Adicione ao `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "chrome",
      "request": "launch",
      "name": "Angular",
      "url": "http://localhost:4200",
      "webRoot": "${workspaceFolder}/frontend/inbev-app"
    }
  ]
}
```

### Chrome DevTools

1. Abra as DevTools (F12)
2. Use a aba **Sources** para debugar TypeScript
3. **Angular DevTools** (extensão recomendada)

## 📊 Performance

### Otimizações Implementadas

- ✅ Lazy Loading de rotas
- ✅ OnPush Change Detection
- ✅ TrackBy em *ngFor
- ✅ Async Pipe para Observables
- ✅ Standalone Components (menor bundle)
- ✅ Tree-shaking automático

## 🔄 Fluxo de Autenticação

```
1. Usuário faz login → AuthService
2. Backend retorna JWT → Armazenado no localStorage
3. Todas requests HTTP → AuthInterceptor adiciona token
4. Token expirado → ErrorInterceptor → Logout automático
5. Navegação protegida → AuthGuard verifica token
```

## 🌐 Internacionalização (i18n)

Preparado para suportar múltiplos idiomas:

```bash
# Extrair textos para tradução
ng extract-i18n

# Build com locale específico
ng build --configuration production --localize
```

## 📝 Próximos Passos

- [ ] Adicionar testes E2E com Cypress
- [ ] Implementar PWA (Service Workers)
- [ ] Adicionar suporte a tema escuro
- [ ] Implementar notificações push
- [ ] Cache de requisições com service worker

## 🤝 Contribuindo

1. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
2. Faça commit: `git commit -m 'feat: adiciona nova funcionalidade'`
3. Push: `git push origin feature/nova-funcionalidade`
4. Abra um Pull Request

## 📚 Recursos Úteis

- [Documentação Angular](https://angular.dev)
- [Angular Material](https://material.angular.io)
- [RxJS](https://rxjs.dev)
- [TypeScript](https://www.typescriptlang.org)
