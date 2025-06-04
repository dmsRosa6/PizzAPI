#!/bin/bash

set -e

CONTAINER_NAME="pizza-db"
DB_NAME="pizzastore"
DB_USER="postgres"
DB_PASSWORD="secret"
SQL_FILE="pizza_schema_postgres.sql"

echo "🧹 Cleaning up any existing container..."
docker rm -f $CONTAINER_NAME 2>/dev/null || true

echo "🐳 Starting PostgreSQL container..."
docker run --name $CONTAINER_NAME \
  -e POSTGRES_USER=$DB_USER \
  -e POSTGRES_PASSWORD=$DB_PASSWORD \
  -e POSTGRES_DB=$DB_NAME \
  -p 55432:5432 \
  -d postgis/postgis:latest

echo "⏳ Waiting for PostgreSQL to start..."
sleep 5

echo "📄 Copying SQL schema to container..."
docker cp $SQL_FILE $CONTAINER_NAME:/schema.sql

echo "📥 Running schema inside container..."
docker exec -u $DB_USER -it $CONTAINER_NAME \
  psql -d $DB_NAME -f /schema.sql

echo "✅ Done! PostgreSQL is running with your schema loaded."
echo "📦 Container: $CONTAINER_NAME"
echo "🔗 Connect with: psql -h localhost -U $DB_USER -d $DB_NAME"
