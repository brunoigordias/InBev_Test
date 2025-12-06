# InBev Employee Management API

API REST desenvolvida em .NET 8 para gerenciamento de funcionários.

## 🏗️ Arquitetura

Projeto segue os princípios de **Clean Architecture** com separação em camadas:

- **InBev.Domain**: Entidades, Enums e Interfaces
- **InBev.Application**: DTOs, Validators e Lógica de Aplicação
- **InBev.Infrastructure**: Implementações (DbContext, Repositories, Services)
- **InBev.API**: Controllers, Configurações e Startup

## 🚀 Tecnologias

- **.NET 8.0**
- **Entity Framework Core** (SQL Server)
- **JWT Authentication**
- **Serilog** (Logging)
- **FluentValidation** (Validação)
- **AutoMapper** (Mapeamento de objetos)
- **BCrypt** (Hash de senhas)
- **Swagger/OpenAPI** (Documentação)
- **Docker**

## 📋 Pré-requisitos

- .NET 8 SDK
- SQL Server (ou Docker para rodar o SQL Server)
- Docker (opcional, mas recomendado)

## 🔧 Configuração

### 1. Clone o repositório

```bash
git clone <repository-url>
cd backend
```

### 2. Configurar Connection String

Edite o arquivo `src/InBev.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=InBevDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

### 3. Aplicar Migrations

```bash
cd src/InBev.API
dotnet ef migrations add InitialCreate --project ../InBev.Infrastructure
dotnet ef database update
```

### 4. Executar a aplicação

```bash
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

## 🐳 Docker

### Build da imagem

```bash
docker build -t inbev-api .
```

### Executar com Docker Compose (recomendado)

Na raiz do projeto principal:

```bash
docker-compose up -d
```

Isso irá subir:
- SQL Server na porta 1433
- API na porta 5000

## 📚 Documentação da API

Acesse a documentação Swagger em: `http://localhost:5000/swagger`

### Principais Endpoints

#### Autenticação
- `POST /api/auth/login` - Login

#### Funcionários
- `GET /api/employees` - Listar todos
- `GET /api/employees/{id}` - Buscar por ID
- `POST /api/employees` - Criar funcionário
- `PUT /api/employees/{id}` - Atualizar funcionário
- `DELETE /api/employees/{id}` - Remover funcionário
- `GET /api/employees/{id}/subordinates` - Listar subordinados

## 🔐 Autenticação

A API utiliza **JWT (JSON Web Token)** para autenticação.

### Usuário Administrador Padrão

Ao inicializar a aplicação pela primeira vez, um **usuário administrador padrão** é criado automaticamente:

```
Email: admin@inbev.com
Senha: Admin@123
Role: Director
```

> ⚠️ **IMPORTANTE**: Por questões de segurança, altere a senha padrão após o primeiro login em ambiente de produção.

### Como autenticar

1. Faça login em `/api/auth/login`
2. Copie o token retornado
3. No Swagger, clique em "Authorize" e cole o token: `Bearer {seu-token}`

## ✅ Regras de Negócio

1. **CPF único**: Não é permitido cadastrar funcionários com CPF duplicado
2. **Email único**: Não é permitido cadastrar funcionários com email duplicado
3. **Maior de idade**: Funcionários devem ter 18 anos ou mais
4. **Hierarquia**: Um funcionário não pode criar outro com permissões maiores que a sua:
   - Employee não pode criar Leader ou Director
   - Leader não pode criar Director
5. **Telefones**: Todo funcionário deve ter pelo menos um telefone
6. **Senha segura**: Mínimo 6 caracteres, com letra maiúscula, minúscula e número

## 🧪 Testes

### Executar testes unitários

```bash
dotnet test tests/InBev.UnitTests
```

### Executar testes de integração

```bash
dotnet test tests/InBev.IntegrationTests
```

## 📁 Estrutura de Pastas

```
backend/
├── src/
│   ├── InBev.API/              # Camada de apresentação
│   ├── InBev.Application/      # Lógica de aplicação
│   ├── InBev.Domain/           # Entidades e interfaces
│   └── InBev.Infrastructure/   # Implementações
├── tests/
│   ├── InBev.UnitTests/
│   └── InBev.IntegrationTests/
├── Dockerfile
├── .dockerignore
└── InBev.sln
```

## 📝 Logs

Os logs são salvos em:
- Console (desenvolvimento)
- Arquivo: `logs/inbev-api-{data}.txt`

## 🤝 Contribuindo

Este projeto segue o **GitFlow**:

1. Crie uma feature branch: `git flow feature start nome-da-feature`
2. Faça commits: `git commit -m "feat: descrição"`
3. Finalize a feature: `git flow feature finish nome-da-feature`

### Convenção de Commits

- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `refactor:` - Refatoração
- `test:` - Testes
- `chore:` - Manutenção

## 📄 Licença

Este projeto é parte de um desafio técnico da InBev.

