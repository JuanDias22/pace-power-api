## 🌐 Acesse o projeto

🔗 https://seu-projeto.vercel.app

## 📸 Preview

https://github.com/user-attachments/assets/6e3c9804-355e-4662-aca7-c216ee993e9b

# 🚀 Pace Power

Sistema completo de captura de leads e processamento de pagamentos integrado ao Mercado Pago.

## 📌 Sobre o projeto

O **Pace Power** é uma aplicação fullstack desenvolvida para:

* Captura de leads
* Escolha de planos (Básico, Intermediário, Premium)
* Geração de pagamento via Mercado Pago
* Atualização automática do plano do usuário via webhook

O projeto simula um fluxo real de SaaS com checkout integrado.

---

## 🧱 Arquitetura

Frontend → Backend API → Mercado Pago → Webhook → Banco de Dados

```
[ Next.js ]
     ↓
[ .NET API ]
     ↓
[ Mercado Pago ]
     ↓
[ Webhook ]
     ↓
[ PostgreSQL ]
```

---

## 🛠️ Tecnologias utilizadas

### Frontend

* Next.js
* React
* TailwindCSS

### Backend

* .NET 8
* ASP.NET Core
* Dapper
* PostgreSQL

### Integrações

* Mercado Pago (Checkout + Webhook)

### Deploy

* Frontend: Vercel
* Backend: Render

---

## ⚙️ Funcionalidades

* ✅ Cadastro de leads
* ✅ Seleção de plano
* ✅ Geração de pagamento
* ✅ Redirecionamento para checkout
* ✅ Webhook para confirmação de pagamento
* ✅ Atualização automática do plano do usuário
* ✅ Persistência em banco de dados

---

## 🔗 Endpoints principais

### Criar pagamento

```
POST /api/payments
```

**Body:**

```json
{
  "plano": "Basico",
  "nome": "Juan",
  "email": "juan@email.com"
}
```

---

### Webhook do Mercado Pago

```
POST /api/payments/webhook
```

Responsável por atualizar o status do pagamento e liberar o plano.

---

## 🗄️ Banco de dados

### Tabela: `pagamentos`

* email
* plano
* valor
* status
* mercado_pago_id
* created_at
* updated_at

---

### Tabela: `usuarios`

* nome
* email
* senha_hash
* plano
* updated_at

---

## 🔐 Variáveis de ambiente

### Backend (.NET)

```env
ConnectionStrings__DefaultConnection=
MercadoPago__AccessToken=
MercadoPago__WebhookUrl=
```

---

### Frontend (Next.js)

```env
NEXT_PUBLIC_API_URL=
```

---

## ▶️ Como rodar o projeto

### 🔹 Backend

```bash
dotnet restore
dotnet run
```

---

### 🔹 Frontend

```bash
npm install
npm run dev
```

---

## 🌐 Deploy

### Backend (Render)

* Configurar variáveis de ambiente
* Subir repositório

### Frontend (Vercel)

* Adicionar `NEXT_PUBLIC_API_URL`
* Deploy automático via GitHub

---

## 🔄 Fluxo de pagamento

1. Usuário preenche formulário
2. Lead é salvo no backend
3. API cria pagamento no Mercado Pago
4. Usuário é redirecionado para pagamento
5. Mercado Pago envia webhook
6. Backend atualiza status
7. Plano do usuário é liberado

---

## 🚧 Melhorias futuras

* [ ] Implementar camada de Services
* [ ] Criar endpoint único de checkout
* [ ] Adicionar autenticação JWT
* [ ] Implementar rate limiting
* [ ] Logs estruturados (Serilog)
* [ ] Dashboard administrativo

---

## 👨‍💻 Autor

Desenvolvido por **Juan Dias**

---

## 📄 Licença

Este projeto é para fins de estudo e portfólio.
