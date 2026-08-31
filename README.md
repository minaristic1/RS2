Grupni projekat iz predmeta Razvoj softvera 2 na master studijama na Matematičkom fakultetu, Univerziteta u Beogradu. 
Tema projekta: Aplikacija za dostavu hrane - GrizGo

Članovi tima:
Mina Ristić, 1119/2025
Ilinka Bibić, 1114/2025
Kristijan Petronijević, 1031/2025
Alma Hodžić, 1120/2025

## Lokalno pokretanje

Za pokretanje API Gateway-a, Billing servisa, Identity servisa i PostgreSQL baza:

```bash
cp .env.example .env
docker compose up --build
```

Dostupne adrese:

- API Gateway: `http://localhost:5029`
- Billing Swagger: `http://localhost:5005/swagger`
- Billing gRPC: `http://localhost:5001`
- Identity Swagger: `http://localhost:5100/swagger`

Prvo registrujte korisnika preko `POST /api/auth/register`, kopirajte
`accessToken`, a zatim u Billing Swagger-u izaberite **Authorize** i unesite
token. Za pozive kroz API Gateway potrebno je poslati i `ClientId` zaglavlje.
