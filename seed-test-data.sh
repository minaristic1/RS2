#!/bin/bash
# Puni sistem test podacima: Admin nalog, jedan restoran sa menijem,
# i probni Customer/Driver nalozi. Pokreni nakon "docker compose up -d",
# kad su svi servisi vec zdravi.
#
# Koristi:
#   cd RS2 (koren repozitorijuma)
#   ./seed-test-data.sh

set -e

GATEWAY="http://localhost:5029"
USER_API="http://localhost:5238"
RESTAURANT_API="http://localhost:5056"
SQL_CONTAINER="rs2-main-check-sqlserver-1"

ADMIN_EMAIL="admin@grizgo.rs"
ADMIN_PASSWORD="AdminGrizGo2026!"
# Ovo je vec izracunat ASP.NET Identity hash za lozinku iznad (isti kod, bilo koja baza).
ADMIN_HASH="AQAAAAEAACcQAAAAEHwgM3uQkur4Dma/gChWzftfIvMta8WfDUwsfxH/azM8VKwkcwANtBFGMGZR8pIvTw=="

echo "== 1/5: Admin nalog =="
EXISTS=$(docker exec "$SQL_CONTAINER" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -d UserDb -h -1 -Q \
  "SET NOCOUNT ON; SELECT COUNT(*) FROM Users WHERE Email = '$ADMIN_EMAIL'" 2>/dev/null | tr -d '[:space:]')

if [ "$EXISTS" = "0" ]; then
  docker exec "$SQL_CONTAINER" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -d UserDb -Q \
    "INSERT INTO Users (Id, Email, PasswordHash, FullName, Role, IsActive, CreatedAt, RestaurantId)
     VALUES (NEWID(), '$ADMIN_EMAIL', '$ADMIN_HASH', 'Admin GrizGo', 5, 1, SYSUTCDATETIME(), NULL)"
  echo "  Admin nalog kreiran: $ADMIN_EMAIL / $ADMIN_PASSWORD"
else
  echo "  Admin nalog vec postoji, preskacem."
fi

echo "== 2/5: Prijava kao Admin =="
ADMIN_TOKEN=$(curl -s -X POST "$USER_API/api/users/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$ADMIN_EMAIL\",\"password\":\"$ADMIN_PASSWORD\"}" \
  | python3 -c "import sys,json; print(json.load(sys.stdin)['token'])")
echo "  token dobijen"

echo "== 3/5: Test restoran =="
RESTAURANT=$(curl -s -X POST "$RESTAURANT_API/api/restaurants" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{
    "nameSr": "Burek Centar",
    "nameEn": "Burek Center",
    "descriptionSr": "Najbolji burek u gradu, sveze testo svakog jutra.",
    "descriptionEn": "The best burek in town.",
    "address": "Bulevar Kralja Aleksandra 73, Beograd",
    "imageUrl": "https://picsum.photos/seed/burek-centar/600/400",
    "isFeatured": true,
    "cuisineType": "Srpska"
  }')
RESTAURANT_ID=$(echo "$RESTAURANT" | python3 -c "import sys,json; print(json.load(sys.stdin)['id'])")
echo "  Restoran napravljen: $RESTAURANT_ID"

echo "== 4/5: Meni sa stavkama =="
MENU=$(curl -s -X POST "$RESTAURANT_API/api/restaurants/$RESTAURANT_ID/menus" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{"nameSr":"Glavni meni","nameEn":"Main menu","descriptionSr":"","descriptionEn":"","displayOrder":1}')
MENU_ID=$(echo "$MENU" | python3 -c "import sys,json; print(json.load(sys.stdin)['id'])")

CATEGORY=$(curl -s -X POST "$RESTAURANT_API/api/restaurants/$RESTAURANT_ID/menus/$MENU_ID/categories" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{"nameSr":"Pite i burek","nameEn":"Pies and burek","descriptionSr":"","descriptionEn":"","displayOrder":1}')
CATEGORY_ID=$(echo "$CATEGORY" | python3 -c "import sys,json; print(json.load(sys.stdin)['id'])")

curl -s -o /dev/null -X POST "$RESTAURANT_API/api/restaurants/$RESTAURANT_ID/menus/$MENU_ID/categories/$CATEGORY_ID/items" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{"nameSr":"Burek sa mesom","nameEn":"Meat burek","descriptionSr":"Klasican burek","descriptionEn":"","price":280,"imageUrl":"https://picsum.photos/seed/burek-meso/300/200","isAvailable":true,"isFeatured":false,"preparationTimeMinutes":10}'

curl -s -o /dev/null -X POST "$RESTAURANT_API/api/restaurants/$RESTAURANT_ID/menus/$MENU_ID/categories/$CATEGORY_ID/items" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{"nameSr":"Burek sa sirom","nameEn":"Cheese burek","descriptionSr":"Burek punjen sirom i jajima","descriptionEn":"","price":250,"imageUrl":"https://picsum.photos/seed/burek-sir/300/200","isAvailable":true,"isFeatured":false,"preparationTimeMinutes":10}'

echo "  Meni i 2 stavke dodati."

echo "== 5/5: Test nalozi (Customer, Driver, Vlasnik restorana) =="
curl -s -o /dev/null -X POST "$USER_API/api/users/register" -H "Content-Type: application/json" \
  -d '{"email":"kupac@grizgo.rs","password":"Test1234!","fullName":"Test Kupac","role":"Customer"}'

curl -s -o /dev/null -X POST "$USER_API/api/users/register" -H "Content-Type: application/json" \
  -d '{"email":"dostavljac@grizgo.rs","password":"Test1234!","fullName":"Test Dostavljac","role":"Driver"}'

curl -s -o /dev/null -X POST "$USER_API/api/users/admin/staff" \
  -H "Content-Type: application/json" -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d "{\"email\":\"vlasnik@grizgo.rs\",\"password\":\"Test1234!\",\"fullName\":\"Test Vlasnik\",\"role\":\"RestaurantOwner\",\"restaurantId\":\"$RESTAURANT_ID\"}"

echo "  Nalozi napravljeni (lozinka za sve testne naloge: Test1234!)"
echo
echo "Gotovo. Kredencijali:"
echo "  Admin:      $ADMIN_EMAIL / $ADMIN_PASSWORD"
echo "  Kupac:      kupac@grizgo.rs / Test1234!"
echo "  Dostavljac: dostavljac@grizgo.rs / Test1234!"
echo "  Vlasnik:    vlasnik@grizgo.rs / Test1234! (restoran: Burek Centar)"
