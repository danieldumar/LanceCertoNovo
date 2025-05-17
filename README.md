# LanceCerto.WebApp

**Lance Certo** é uma aplicação web desenvolvida em **ASP.NET Core MVC (.NET 8.0)** voltada para leilões de imóveis. Permite o cadastro, consulta e gerenciamento de imóveis, usuários e lances, com foco em segurança, usabilidade e clareza nas funcionalidades.

---

## 🔧 Tecnologias Utilizadas

- ASP.NET Core MVC (.NET 8.0)
- Entity Framework Core (EF Core)
- Identity com suporte a PK `int`
- Azure SQL Database (SQL Server)
- Bootstrap 5
- LINQ
- Razor Pages

---

## ✅ Funcionalidades Implementadas

### 📄 CRUD de Imóveis
- Cadastrar, editar, visualizar e excluir imóveis
- Upload de URL de imagem (campo ImagemUrl)
- Campos obrigatórios com validação via DataAnnotations

### 🔍 Pesquisa de Imóveis
- Filtros por cidade, estado, tipo e preço máximo

### 👥 Autenticação e Identidade
- Cadastro de usuários com validação
- Login e logout com persistência (cookie)
- Campos extras no usuário: Nome, Data de Nascimento, CRECI, perfil (Corretor/Vendedor)

### 🕒 Leilões
- Cadastro de leilões com datas de início/fim, status e maior lance atual
- Vínculo entre imóvel e leilão
- Relacionamento com usuário vencedor

### 💬 Mensagens
- Entidade de mensagens com remetente, destinatário e imóvel relacionado

---

## 🚀 Como Executar Localmente

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/lancecerto.git
```

2. Configure a string de conexão no `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:seu-servidor.database.windows.net,1433;Database=LanceCertoDB;User ID=seu-usuario;Password=sua-senha;Encrypt=True;"
}
```

3. Aplique as migrations ao banco de dados:
```bash
Update-Database
```

4. Execute a aplicação no Visual Studio (`Ctrl + F5`) ou terminal:
```bash
dotnet run
```

---

## 📁 Estrutura do Projeto

```
LanceCerto.WebApp/
├── Controllers/
├── Models/
├── Views/
│   ├── Account/
│   ├── Imovel/
│   └── Leilao/
├── Data/
│   └── LanceCertoDbContext.cs
├── Migrations/
├── Program.cs
├── appsettings.json
└── README.md
```

---

## 🏗️ Próximos Passos

- Adicionar módulo de lances ao vivo com cronômetro e controle por JavaScript
- Implementar dashboard de usuário (perfil)
- Upload de imagem real com integração a Azure Blob Storage ou local
- Adicionar autenticação por roles (Admin, Vendedor, Comprador, Corretor)

---

## 👨‍💻 Desenvolvedor

**Daniel Lopes da Costa**  
Estudante de Análise e Desenvolvimento de Sistemas – PUC Minas  
Projeto acadêmico desenvolvido no 1º semestre de 2025  
