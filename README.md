Grupni projekat iz predmeta Razvoj softvera 2 na master studijama na Matematičkom fakultetu, Univerziteta u Beogradu. 
Tema projekta: Aplikacija za dostavu hrane - GrizGo

Članovi tima:
Mina Ristić, 1119/2025
Ilinka Bibić, 1114/2025
Kristijan Petronijević, 1031/2025
Alma Hodžić, 1120/2025

## Lokalno pokretanje celog sistema

Docker Compose pokreće API Gateway, User, Restaurant, Cart, Delivery i Billing
servise, kao i SQL Server, PostgreSQL, Redis i RabbitMQ:

```bash
cp .env.example .env
docker compose up --build -d
```

Dostupne adrese:

- API Gateway: `http://localhost:5029`
- Billing Swagger: `http://localhost:5005/swagger`
- Billing gRPC: `http://localhost:5001`
- User Swagger: `http://localhost:5238/swagger`
- Restaurant Swagger: `http://localhost:5056/swagger`
- Delivery Swagger: `http://localhost:5121/swagger`
- RabbitMQ administracija: `http://localhost:15672`

Korisnik se registruje preko `POST /api/users/register`, a prijavljuje preko
`POST /api/users/login`. Dobijeni `token` koristi se u Billing Swagger-u preko
dugmeta **Authorize**. Frontend automatski šalje JWT i `ClientId` zaglavlja.

Frontend se pokreće posebno:

```bash
cd grizgo-web
npm install
npm start
```

Web aplikacija je dostupna na `http://localhost:4200`.

Checkout korpe objavljuje `CartCheckedOutEvent` preko RabbitMQ-a. Billing servis
konzumira događaj sa `payment.queue` i automatski kreira račun za porudžbinu.

Za zaustavljanje sistema:

```bash
docker compose down
```
