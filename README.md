# 🏢 ProjectCondo

Bem-vindo ao **ProjectCondo**!  

Este aplicativo está em desenvolvimento e foi criado como **portfólio** para demonstrar minhas habilidades em:

- ⚡ **.NET 8**
- 🖥️ **ASP.NET Core MVC**
- 💻 **C#**
- 🎨 **HTML, CSS, JavaScript**
- 🗄️ **SQL Server**
- 🧩 **Arquitetura limpa**
- ✅ **Clean Code**

---

## 🚧 Observações

Algumas funcionalidades estão desabilitadas no projeto por questões técnicas ou de privacidade:

- 📷 **Leitura de QR Code de acesso** (limitação física)  
- 🌐 **Leitura do IP público** para geração de logs, envio de e-mails e WhatsApp (restrição de privacidade)  

O projeto foi **intencionalmente simplificado**. Várias complexidades poderiam ser implementadas, mas foram retiradas para manter o foco em **demonstrar competências técnicas**.

---

## ✨ Funcionalidades

### 🔑 Login
- Cadastro de login  
- Recuperação de senha  
- Realização de login  

### 🏠 Home Principal
- Cadastro, edição e exclusão de visitantes  
- Liberação de visitante (representativo)  
- Baixa de acesso do visitante  
- Cadastro de festa (liberação múltipla de visitantes)  
- Envio de convite por e-mail ou WhatsApp (representativo)  
- Edição de festa (mensagem adaptada conforme o visitante e a festa)  
- Exclusão de festa  
- Recebimento de encomenda *(em desenvolvimento)*  
- Baixa de encomenda *(em desenvolvimento)*  

### ⚙️ Home Admin *(em desenvolvimento)*
- Cadastro, edição e exclusão de moradores  
- Configuração de e-mail  
- Configuração do **Zapi**  
- Cadastro de encomendas  

---

## 🛠️ Instalação

1. Instale o **SQL Server 2022** junto com o **SSMS (20 ou superior)**.  
2. Execute o script disponível na pasta `Sql` para criar a base de dados.  
3. Para testar localmente:  
   - Você pode configurar o **IIS**, ou  
   - Basta abrir o arquivo especificado para iniciar o site.  

---

## 🔐 Login Padrão do Administrador
- **Usuário:** `admin`  
- **Senha:** `admin`  

Após o login inicial:
- Cadastre um morador e faça login na tela de login.  
- Como o envio de e-mails está desativado, o código para redefinir senha será sempre:  
