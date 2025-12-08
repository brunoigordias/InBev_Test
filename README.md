# 🍺 InBev Employee Management System

Sistema completo de gerenciamento de funcionários desenvolvido com **.NET 8** e **Angular 19**, demonstrando práticas modernas de desenvolvimento fullstack.

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-19-DD0031?logo=angular)](https://angular.io/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.7-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

</div>

---

## 📋 Sobre o Projeto

Sistema web fullstack para gerenciamento de funcionários com autenticação JWT, hierarquia de permissões e operações CRUD completas. Desenvolvido seguindo as melhores práticas de Clean Architecture, SOLID e padrões de desenvolvimento modernos.

### ✨ Principais Funcionalidades

- 🔐 **Autenticação JWT** com refresh token
- 👥 **Gestão de Funcionários** (CRUD completo)
- 🏢 **Hierarquia Organizacional** (Employee → Leader → Director)
- 📱 **Gerenciamento de Telefones** (múltiplos por funcionário)
- 🔑 **Troca de Senha** segura
- 📊 **Paginação** e filtros avançados
- ✅ **Validação de CPF** brasileiro
- 🎨 **Interface moderna** com Material Design
- 📝 **Logs estruturados** com Serilog
- 🧪 **Testes unitários** com alta cobertura

---

## 🏗️ Arquitetura

### Backend - Clean Architecture

```
InBev.API/
├── Domain/              # Entidades, Enums, Interfaces (núcleo da aplicação)
├── Application/         # DTOs, Validators, Regras de negócio
├── Infrastructure/      # DbContext, Repositories, Services
└── API/                # Controllers, Middlewares, Configurações
```

**Principais Padrões Implementados:**
- ✅ Clean Architecture
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ SOLID Principles
- ✅ Unit of Work (implícito via EF Core)

### Frontend - Angular 19

```
inbev-app/
├── core/               # Guards, Interceptors, Services
├── features/           # Módulos de funcionalidades
├── models/            # Interfaces e Enums TypeScript
└── shared/            # Componentes, Pipes, Layouts compartilhados
```

**Padrões Frontend:**
- ✅ Standalone Components (Angular 19)
- ✅ Reactive Forms
- ✅ Route Guards
- ✅ HTTP Interceptors
- ✅ Lazy Loading
- ✅ Signal-based State (Angular Signals)

---

## 🚀 Tecnologias Utilizadas

### Backend

| Tecnologia | Versão | Descrição |
|-----------|--------|-----------|
| .NET | 8.0 | Framework principal |
| Entity Framework Core | 8.0 | ORM para acesso a dados |
| SQL Server | 2022 | Banco de dados relacional |
| JWT Bearer | 8.0 | Autenticação e autorização |
| FluentValidation | 11.10 | Validações de DTOs |
| AutoMapper | 13.0 | Mapeamento de objetos |
| Serilog | 4.2 | Logging estruturado |
| BCrypt.Net | 0.1.0 | Hash de senhas |
| Swagger/OpenAPI | 6.6 | Documentação da API |
| xUnit | 2.9 | Testes unitários |
| Moq | 4.20 | Mock para testes |
| FluentAssertions | 7.0 | Assertions fluentes |

### Frontend

| Tecnologia | Versão | Descrição |
|-----------|--------|-----------|
| Angular | 19.1 | Framework SPA |
| TypeScript | 5.7 | Superset tipado do JavaScript |
| Angular Material | 19.0 | Biblioteca de componentes UI |
| RxJS | 7.8 | Programação reativa |
| Jasmine | 5.5 | Framework de testes |
| Karma | 6.4 | Test runner |

### DevOps

- **Docker** & **Docker Compose** - Containerização
- **Git Flow** - Versionamento
- **Conventional Commits** - Padronização de commits

---

## 📦 Pré-requisitos

Certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) e npm
- [Angular CLI 19](https://angular.io/cli): `npm install -g @angular/cli`
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (recomendado)
- [SQL Server 2022](https://www.microsoft.com/sql-server) ou Docker
- Editor: [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

---

## ⚙️ Configuração e Instalação

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/seu-usuario/InBev_Test.git
cd InBev_Test
```

### 2️⃣ Configuração do Banco de Dados

#### Opção A: Usando Docker (Recomendado)

```bash
# Subir apenas o SQL Server
docker-compose up -d sqlserver

# Ou subir todo o ambiente (SQL Server + Backend)
docker-compose --profile full up -d
```

#### Opção B: SQL Server Local

Edite a connection string em `backend/src/InBev.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=InBevDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3️⃣ Backend - API .NET

```bash
cd backend

# Restaurar dependências
dotnet restore

# Aplicar migrations
cd src/InBev.API
dotnet ef database update

# Executar a aplicação
dotnet run

# Ou com hot reload
dotnet watch run
```

A API estará disponível em:
- **HTTP**: http://localhost:5141
- **Swagger**: http://localhost:5141/swagger

### 4️⃣ Frontend - Angular

```bash
cd frontend/inbev-app

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm start
# ou
ng serve

# Build de produção
npm run build
```

A aplicação estará disponível em: http://localhost:4200

---

## 🎯 Como Usar

### 1. Primeiro Acesso

Ao inicializar o backend pela primeira vez, um **usuário administrador** é criado automaticamente:

```
Email: admin@inbev.com
Senha: Admin@123
Perfil: Director
```

### 2. Login

Acesse http://localhost:4200 e faça login com as credenciais acima.

### 3. Operações Disponíveis

- ➕ **Criar Funcionários** (com validação de CPF e idade)
- ✏️ **Editar Funcionários** (respeitando hierarquia)
- 🗑️ **Excluir Funcionários**
- 👁️ **Visualizar Detalhes** e subordinados
- 🔐 **Trocar Senha** (própria)
- 🔍 **Buscar e Filtrar** com paginação

---

## 🧪 Testes

### Backend - Testes Unitários

```bash
cd backend

# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html

# Executar apenas testes unitários
dotnet test tests/InBev.UnitTests

# Executar apenas testes de integração
dotnet test tests/InBev.IntegrationTests
```

**Cobertura Atual**: 43 testes unitários (100% passando)

**Principais testes:**
- ✅ Validadores (CreateEmployee, Login, ChangePassword)
- ✅ PasswordHasher (hash e verificação)
- ✅ TokenService (geração JWT)
- ✅ EmployeeRepository (operações CRUD)

### Frontend - Testes Unitários

```bash
cd frontend/inbev-app

# Executar testes
npm test

# Executar com cobertura
npm run test:coverage

# Executar em modo watch
ng test --watch
```

---

## 📚 Documentação da API

### Swagger UI

Acesse a documentação interativa em: **http://localhost:5141/swagger**

### Principais Endpoints

#### 🔐 Autenticação

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@inbev.com",
  "password": "Admin@123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiration": "2025-12-09T00:00:00Z"
}
```

#### 👥 Funcionários

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/employees` | Listar com paginação | ✅ |
| GET | `/api/employees/{id}` | Buscar por ID | ✅ |
| POST | `/api/employees` | Criar funcionário | ✅ |
| PUT | `/api/employees/{id}` | Atualizar funcionário | ✅ |
| DELETE | `/api/employees/{id}` | Remover funcionário | ✅ |
| GET | `/api/employees/{id}/subordinates` | Listar subordinados | ✅ |
| POST | `/api/auth/change-password` | Trocar senha | ✅ |

### Exemplo de Request - Criar Funcionário

```http
POST /api/employees
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "João Silva",
  "cpf": "12345678901",
  "email": "joao.silva@inbev.com",
  "password": "Senha@123",
  "birthDate": "1990-05-15",
  "role": "Employee",
  "managerId": 1,
  "phoneNumbers": [
    {
      "number": "11987654321",
      "type": "Mobile"
    }
  ]
}
```

---

## ✅ Regras de Negócio

### Validações Implementadas

1. **CPF**
   - ✅ Deve ser válido (validação de dígitos verificadores)
   - ✅ Deve ser único no sistema

2. **Email**
   - ✅ Formato válido
   - ✅ Único no sistema

3. **Idade**
   - ✅ Funcionário deve ter 18 anos ou mais

4. **Hierarquia**
   - ✅ Employee não pode criar Leader ou Director
   - ✅ Leader não pode criar Director
   - ✅ Apenas Director tem permissão total

5. **Senha**
   - ✅ Mínimo 6 caracteres
   - ✅ Pelo menos 1 letra maiúscula
   - ✅ Pelo menos 1 letra minúscula
   - ✅ Pelo menos 1 número
   - ✅ Hash BCrypt (nunca armazenada em texto plano)

6. **Telefones**
   - ✅ Pelo menos 1 telefone obrigatório
   - ✅ Tipos: Mobile, Home, Work

---

## 📁 Estrutura do Projeto

```
InBev_Test/
│
├── backend/                          # Backend .NET 8
│   ├── src/
│   │   ├── InBev.API/               # Controllers, Program.cs
│   │   │   ├── Controllers/
│   │   │   │   ├── AuthController.cs
│   │   │   │   └── EmployeesController.cs
│   │   │   ├── Program.cs
│   │   │   └── appsettings.json
│   │   │
│   │   ├── InBev.Application/       # DTOs, Validators
│   │   │   ├── DTOs/
│   │   │   │   ├── EmployeeDto.cs
│   │   │   │   ├── PagedRequest.cs
│   │   │   │   └── PagedResponse.cs
│   │   │   └── Validators/
│   │   │       ├── CreateEmployeeValidator.cs
│   │   │       ├── LoginValidator.cs
│   │   │       └── ChangePasswordValidator.cs
│   │   │
│   │   ├── InBev.Domain/            # Entidades, Enums, Interfaces
│   │   │   ├── Entities/
│   │   │   │   ├── Employee.cs
│   │   │   │   └── PhoneNumber.cs
│   │   │   ├── Enums/
│   │   │   │   ├── EmployeeRole.cs
│   │   │   │   └── PhoneType.cs
│   │   │   └── Interfaces/
│   │   │       ├── IEmployeeRepository.cs
│   │   │       ├── IPasswordHasher.cs
│   │   │       └── ITokenService.cs
│   │   │
│   │   └── InBev.Infrastructure/    # DbContext, Repositories, Services
│   │       ├── Data/
│   │       │   ├── ApplicationDbContext.cs
│   │       │   └── DataSeeder.cs
│   │       ├── Migrations/
│   │       ├── Repositories/
│   │       │   └── EmployeeRepository.cs
│   │       └── Services/
│   │           ├── PasswordHasher.cs
│   │           └── TokenService.cs
│   │
│   ├── tests/
│   │   ├── InBev.UnitTests/         # 43 testes unitários
│   │   └── InBev.IntegrationTests/  # Testes de integração
│   │
│   ├── Dockerfile
│   └── InBev.sln
│
├── frontend/                         # Frontend Angular 19
│   └── inbev-app/
│       └── src/
│           └── app/
│               ├── core/            # Guards, Interceptors, Services
│               │   ├── guards/
│               │   │   ├── auth.guard.ts
│               │   │   ├── no-auth.guard.ts
│               │   │   └── role.guard.ts
│               │   ├── interceptors/
│               │   │   ├── auth.interceptor.ts
│               │   │   ├── error.interceptor.ts
│               │   │   └── loading.interceptor.ts
│               │   └── services/
│               │       ├── auth.service.ts
│               │       ├── employee.service.ts
│               │       ├── loading.service.ts
│               │       └── storage.service.ts
│               │
│               ├── features/        # Funcionalidades
│               │   ├── auth/
│               │   │   ├── login/
│               │   │   └── register/
│               │   ├── employees/
│               │   │   ├── employee-list/
│               │   │   ├── employee-form/
│               │   │   ├── employee-detail/
│               │   │   └── employee-delete-dialog/
│               │   └── change-password/
│               │
│               ├── models/          # Interfaces e Enums
│               │   ├── employee.model.ts
│               │   ├── auth.model.ts
│               │   ├── employee-role.enum.ts
│               │   └── phone-type.enum.ts
│               │
│               └── shared/          # Componentes compartilhados
│                   ├── layouts/
│                   │   └── main-layout/
│                   └── pipes/
│                       └── cpf.pipe.ts
│
├── docker-compose.yml               # Orquestração Docker
├── LICENSE
└── README.md                        # Este arquivo
```

---

## 🔐 Segurança

### Implementações de Segurança

- ✅ **JWT Authentication** com expiração configurável
- ✅ **BCrypt** para hash de senhas (custo 12)
- ✅ **HTTPS** habilitado
- ✅ **CORS** configurado
- ✅ **SQL Injection** protegido (EF Core parametrizado)
- ✅ **XSS** protegido (Angular sanitization)
- ✅ **CSRF** protegido (Angular HttpClient)
- ✅ **Validação de entrada** em todas as camadas
- ✅ **Logs estruturados** (sem dados sensíveis)

### Variáveis de Ambiente Sensíveis

⚠️ **NUNCA commite** informações sensíveis. Use variáveis de ambiente:

```bash
# .env (exemplo)
JWT_SECRET=SuaChaveSuperSecretaAqui_MinimoDe32Caracteres
CONNECTION_STRING=Server=...
```

---

## 📊 Decisões Técnicas

### Por que Clean Architecture?

- ✅ **Separação de responsabilidades** clara
- ✅ **Testabilidade** facilitada
- ✅ **Manutenibilidade** a longo prazo
- ✅ **Independência de frameworks** externos
- ✅ **Facilidade para mudanças** de infraestrutura

### Por que Angular 19?

- ✅ **Standalone Components** (sem modules)
- ✅ **Signals** para gerenciamento de estado
- ✅ **Performance otimizada**
- ✅ **TypeScript** com tipagem forte
- ✅ **Angular Material** para UI consistente

### Por que Entity Framework Core?

- ✅ **Migrations** automáticas
- ✅ **LINQ** para queries type-safe
- ✅ **Lazy Loading** e **Eager Loading**
- ✅ **Change Tracking** automático

---

## 🚧 Melhorias Futuras

Possíveis evoluções do sistema:

- [ ] **Refresh Token** para renovação automática
- [ ] **Upload de fotos** de funcionários
- [ ] **Relatórios** em PDF/Excel
- [ ] **Dashboard** com gráficos
- [ ] **Notificações** em tempo real (SignalR)
- [ ] **Multi-tenant** para várias empresas
- [ ] **Auditoria** completa de ações
- [ ] **CI/CD** com GitHub Actions
- [ ] **Kubernetes** para deploy
- [ ] **Redis** para cache
- [ ] **RabbitMQ** para mensageria
- [ ] **Elasticsearch** para busca avançada

---

## 📝 Logs

Logs são salvos automaticamente:

- **Console**: desenvolvimento
- **Arquivo**: `backend/src/InBev.API/logs/inbev-api-{data}.txt`
- **Níveis**: Information, Warning, Error, Critical

Exemplo:
```
2025-12-08 00:05:23.145 [INF] Employee created successfully. EmployeeId: 5
2025-12-08 00:06:15.987 [WRN] Invalid login attempt for email: wrong@email.com
2025-12-08 00:07:42.321 [ERR] Database connection failed. Retrying...
```

---

## 🤝 Contribuindo

Este projeto segue o **GitFlow** e **Conventional Commits**.

### Fluxo de Trabalho

```bash
# Criar feature
git flow feature start nome-da-feature

# Fazer alterações
git add .
git commit -m "feat: adiciona nova funcionalidade X"

# Finalizar feature
git flow feature finish nome-da-feature
```

### Tipos de Commit

| Tipo | Descrição |
|------|-----------|
| `feat:` | Nova funcionalidade |
| `fix:` | Correção de bug |
| `docs:` | Documentação |
| `style:` | Formatação (sem mudança de código) |
| `refactor:` | Refatoração |
| `test:` | Adição/correção de testes |
| `chore:` | Manutenção/configuração |

---



