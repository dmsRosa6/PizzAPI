#!/usr/bin/env bash
# -------------------------------------------------
# Vars
# -------------------------------------------------
CONTAINER_NAME="pizza-db"
DB_NAME="pizzastore"
DB_USER="postgres"
DB_PASSWORD="secret"
SQL_FILE="pizza_schema_postgres.sql"
# -------------------------------------------------

# Build the connection string on the fly
CONN_STR="Host=localhost;Port=55432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"

# Make sure dotnet-ef is available
dotnet tool install --global dotnet-ef 2>/dev/null

# Pull the schema into your ASP.NET Core project
dotnet ef dbcontext scaffold \
  "$CONN_STR" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --schema public \
  --context PizzaStoreContext \
  --output-dir ./Models \
  --force

echo "✅  Schema pulled.  Check out the generated Models/ folder."
