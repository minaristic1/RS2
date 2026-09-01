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

## API Gateway
API Gateway je implementiran pomoću Ocelot biblioteke i predstavlja jedinstvenu 
ulaznu tačku ka mikroservisima sistema.
Implementiran je pomoću Ocelot biblioteke i zadužen je za rutiranje zahteva ka odgovarajućim servisima, 
tako da klijent ne mora direktno da zna njihove interne portove i adrese.

Lokalno je dostupan na `http://localhost:5029`

Tok komunikacije izgleda ovako:
```text
Client
  ↓
API Gateway
  ├── CartService
  ├── RestaurantService
  ├── Billing/PaymentService
  └── DeliveryService
```
Za svaki mikroservis u `ApiGateway/ocelot.json` definišu se:
- upstream ruta koju koristi klijent;
- downstream ruta mikroservisa;
- host i port servisa;
- dozvoljene HTTP metode;
- opciona pravila za autentifikaciju i rate limiting.

Primer toka zahteva:
```text
Client
  ↓
http://localhost:5029/api/carts/...
  ↓
API Gateway
  ↓
CartService
```

Na isti način Gateway prosleđuje zahteve ka Restaurant, Billing/Payment i Delivery servisima.
Gateway podržava JWT/Bearer autentifikaciju za zaštićene rute.
Takođe je podešen rate limiting, kojim se ograničava broj zahteva koje jedan klijent može da pošalje 
u određenom vremenskom periodu. Klijent se identifikuje pomoću ClientId HTTP zaglavlja.
Za rute na kojima je rate limiting uključen, prekoračenje dozvoljenog broja zahteva vraća:

```bash
429 Too Many Requests
```

Gateway ima i health check endpoint:
```bash
GET /health
```
koji pri ispravnom radu vraća:
```bash
Healthy
```
Konfiguracija svih ruta nalazi se u `ApiGateway/ocelot.json`

Dodavanje novog mikroservisa u Gateway svodi se na dodavanje nove Ocelot rute u ovaj
konfiguracioni fajl, bez potrebe da se menja kod postojećih mikroservisa.

## Cart servis

Cart servis je zadužen za upravljanje korisničkom korpom. Podaci o korpi čuvaju se u Redis-u, 
dok se podaci o proizvodima proveravaju preko Restaurant servisa.

Podržane su operacije:
- pregled korpe;
- dodavanje proizvoda;
- promena količine;
- uklanjanje proizvoda;
- pražnjenje korpe;
- checkout.

Cart API:

```text
GET    /api/carts/{userId}
POST   /api/carts/{userId}/items
PUT    /api/carts/{userId}/items/{productId}
DELETE /api/carts/{userId}/items/{productId}
DELETE /api/carts/{userId}
POST   /api/carts/{userId}/checkout
```

Prilikom dodavanja proizvoda Cart servis poziva Restaurant servis (GET /api/menu-items/{id}) i od njega preuzima naziv, 
cenu, restoran i dostupnost proizvoda. Korpa može da sadrži proizvode samo iz jednog restorana.

Cart endpointi su zaštićeni JWT autentifikacijom. Korisnik može da pristupi samo svojoj korpi, dok administrator može 
da pristupi svim korpama.

Pri checkout-u Cart servis kreira CartCheckedOutEvent, koji sadrži podatke o porudžbini i adresi dostave. Događaj se 
preko RabbitMQ-a objavljuje na:

```text
cart.exchange → cart.checked-out → payment.queue
```

Billing servis konzumira događaj i nastavlja obradu porudžbine. Korpa se briše tek nakon uspešnog objavljivanja događaja.
Cart servis ima unit testove implementirane pomoću xUnit i Moq.

## Billing servis

Billing se pokreće zajedno sa ostatkom sistema komandom:

```bash
docker compose up --build -d
```

Za rad Billing servisa potrebni su `billing-db`, `rabbitmq` i `user-api`.
Ako se testira samo ovaj deo projekta, mogu se pokrenuti ovako:

```bash
docker compose up --build -d billing-db rabbitmq sqlserver user-api billing-api
```

Testiranje preko Swagger-a:

1. Na `http://localhost:5238/swagger` registrujte korisnika i prijavite se.
2. Kopirajte polje `token` iz odgovora za prijavu.
3. Otvorite `http://localhost:5005/swagger`, kliknite **Authorize** i unesite token.
4. Kreirajte račun preko `POST /api/invoices`, a zatim ga platite preko
   `POST /api/invoices/{id}/payments`.

Billing koristi PostgreSQL na portu `5433`, REST API na `5005` i gRPC na `5001`.
Račun se može kreirati i automatski kada Cart servis pošalje checkout događaj.

## Restaurant servis

Restaurant servis je zadužen za katalog restorana: same restorane, njihove
menije (meniji → kategorije → stavke), radno vreme i praznične izuzetke od
radnog vremena.

Lokalno je dostupan na `http://localhost:5056`, Swagger na
`http://localhost:5056/swagger`.

Podržane su operacije:
- CRUD nad restoranima (kreiranje, izmena, brisanje, pretraga);
- postavljanje radnog vremena i praznih dana (holiday exceptions);
- CRUD nad menijima, kategorijama menija i stavkama menija.

Pregled restorana, pretraga, meni i stavka menija (`GET /api/menu-items/{id}`)
su javno dostupni, bez tokena — koristi ih i Cart servis da proveri naziv,
cenu i dostupnost proizvoda pri dodavanju u korpu. Sve izmene zahtevaju JWT
token i podležu dvostrukoj proveri:

- provera uloge — samo `RestaurantOwner`, `RestaurantEmployee` ili `Admin`;
- provera vlasništva — `RestaurantOwner` sme da menja samo restoran koji je
  sam kreirao ili koji mu je Admin dodelio preko `/api/users/admin/staff`,
  a `RestaurantEmployee` samo restoran za koji je vezan (`restaurantId` u
  JWT tokenu). Admin nema ograničenje.

Pokušaj izmene tuđeg restorana vraća `403 Forbidden`, a ne samo `401`.

## User servis

User servis vodi evidenciju korisničkih naloga (kupci, dostavljači, vlasnici
i zaposleni restorana, administratori), autentifikaciju i izdavanje JWT
tokena.

Lokalno je dostupan na `http://localhost:5238`, Swagger na
`http://localhost:5238/swagger`.

Samostalna registracija (`POST /api/users/register`) je dozvoljena samo za
role `Customer` i `Driver`. Naloge za `RestaurantOwner` i
`RestaurantEmployee` kreira isključivo Admin preko `POST /api/users/admin/staff`,
gde se zaposlenom obavezno, a vlasniku opciono, dodeljuje `restaurantId`.

Prijava (`POST /api/users/login`) vraća JWT token sa claim-ovima: id
korisnika, email, uloga, ime i (ako postoji) `restaurantId` — ovaj poslednji
koriste Restaurant servis i Gateway da utvrde kojim resursima korisnik sme
da pristupi.

Za zaustavljanje sistema:

```bash
docker compose down
```
