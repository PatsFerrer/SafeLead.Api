# SafeLead.Api

API REST em .NET 10 para captura de orçamentos (leads) com foco em segurança de borda, validação de entrada e proteção contra abuso.

## Camadas de Segurança

- **CORS Restrito:** Permite requisições apenas das origens configuradas.
- **Rate Limiting por IP:** Janela fixa configurável (suporte nativo ao header `CF-Connecting-IP` da Cloudflare), retornando `429 Too Many Requests`.
- **Security Headers (Helmet):** Adição automática de `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` e `Content-Security-Policy`.
- **Validação de Payload:** Limite de corpo de requisição em 10 KB no Kestrel e validação estrita via DataAnnotations no DTO.
- **Zero Hardcode:** Configurações validadas na inicialização (`ValidateOnStart`) via User Secrets ou Variáveis de Ambiente.

## Configuração Local

1. Copie a estrutura do arquivo `appsettings.Example.json` para o **User Secrets** do Visual Studio (`secrets.json`):

```json
{
  "SecuritySettings": {
    "AllowedOrigins": [ "http://localhost:5173" ],
    "RateLimitPermitLimit": 5,
    "RateLimitWindowSeconds": 60
  }
}
```

2. Execute a aplicação via Visual Studio (perfil `https` na porta `7070`) ou pelo terminal:

```bash
dotnet run --launch-profile https
```

## Testando a API com `curl`

> **Nota:** Execute os comandos abaixo utilizando o terminal **Git Bash** (no Windows) ou terminal padrão Linux/macOS. Evite acentuação direta na linha de comando do Windows para prevenir erros de codificação ANSI/UTF-8 no payload.

### 1. Health Check e Security Headers

```bash
curl -i -k https://localhost:7070/api/health
```

### 2. Enviar Lead Válido (`200 OK`)

```bash
curl -i -k -X POST https://localhost:7070/api/leads \
  -H "Content-Type: application/json" \
  -d '{"name":"Maria Silva","email":"maria@exemplo.com","phone":"85999999999","message":"Ola, gostaria de solicitar um orcamento para criacao de site."}'
```

### 3. Testar Validação de Dados (`400 Bad Request`)

```bash
curl -i -k -X POST https://localhost:7070/api/leads \
  -H "Content-Type: application/json" \
  -d '{"name":"A","email":"invalido","phone":"abc","message":"Oi"}'
```

### 4. Testar CORS (Origem Permitida vs. Bloqueada)

```bash
# Origem autorizada (retorna Access-Control-Allow-Origin: http://localhost:5173)
curl -i -k -X OPTIONS https://localhost:7070/api/leads \
  -H "Origin: http://localhost:5173" \
  -H "Access-Control-Request-Method: POST"

# Origem desconhecida (omite cabeçalho de permissão CORS)
curl -i -k -X OPTIONS https://localhost:7070/api/leads \
  -H "Origin: [https://site-hacker.com](https://site-hacker.com)" \
  -H "Access-Control-Request-Method: POST"
```