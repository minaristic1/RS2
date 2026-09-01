# GrizGo Web

Angular frontend za GrizGo, aplikaciju za dostavu hrane. Pokriva ekrane za sve
role u sistemu: Kupac, Vlasnik restorana, Zaposleni u restoranu, Dostavljač i
Admin.

## Šta je urađeno

- **Prijava/registracija** — samostalna registracija je dozvoljena samo za
  Kupca i Dostavljača; naloge za Vlasnika restorana i Zaposlenog kreira Admin
- **Restorani** — pregled, pretraga i filter po tipu kuhinje; Admin dodaje
  nove restorane; vlasnik/zaposleni uređuju podatke i meni svog restorana
- **Korpa i plaćanje** — dodavanje stavki, unos adrese dostave, checkout, pa
  plaćanje računa koji Billing servis automatski napravi
- **Praćenje dostave** — status porudžbine se prati preko ID-ja porudžbine,
  dostupno i bez prijave
- **Porudžbine restorana** — vlasnik/zaposleni pomeraju status porudžbine
  kroz pripremu
- **Dostavljač** — pregled dostupnih dostava, preuzimanje i pomeranje statusa
  do isporuke
- **Admin panel** — kreiranje naloga za vlasnike/zaposlene restorana

## Pokretanje

Backend servisi (Gateway, User, Restaurant, Cart, Delivery, Billing) se
pokreću preko Docker Compose-a iz korena repozitorijuma — pogledaj glavni
`README.md`. Frontend se pokreće posebno:

```bash
npm install
npm start
```

Aplikacija je dostupna na `http://localhost:4200`.

## Tehnologije

Angular 17 (standalone komponente, signali, novi `@if`/`@for` sintaksa),
Bootstrap za stilizovanje.
