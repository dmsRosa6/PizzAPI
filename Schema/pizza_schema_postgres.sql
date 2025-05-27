-- =====================================================================
-- TABLE DEFINITIONS
-- =====================================================================

-- Stores of the pizza chain
CREATE TABLE stores (
    store_id      SERIAL PRIMARY KEY,
    name          VARCHAR(52) NOT NULL
);

Create TYPE product_type as ENUM (    
  'Beverages', 'PizzaBases', 'SaltyIngredients', 'SweetIngredients', 'Snacks', 'Desserts', 'Sauces' 
);

-- Products (ingredients, beverages, extras)
CREATE TABLE products (
    product_id    SERIAL PRIMARY KEY,
    name          VARCHAR(100) NOT NULL,
    price         NUMERIC(10,2) NOT NULL CHECK (price >= 0),
    type          product_type NOT NULL,
    is_deleted    BOOLEAN   NOT NULL
);

-- Pizzas (composite items)
CREATE TABLE pizzas (
    pizza_id      SERIAL PRIMARY KEY,
    name          VARCHAR(100) NOT NULL,
    base_price    NUMERIC(10,2) NOT NULL CHECK (base_price >= 0),
    is_deleted    BOOLEAN NOT NULL
);

-- Link ingredients to pizzas
CREATE TABLE pizza_ingredients (
    pizza_id      INTEGER NOT NULL REFERENCES pizzas(pizza_id) ON DELETE CASCADE,
    product_id    INTEGER NOT NULL REFERENCES products(product_id),
    PRIMARY KEY (pizza_id, product_id)
);

-- Clients (customers)
CREATE TABLE clients (
    client_id     SERIAL PRIMARY KEY,
    name          VARCHAR(60) NOT NULL,
    phone_number  VARCHAR(15) UNIQUE,
    nif           VARCHAR(20) UNIQUE,
    is_deleted BOOLEAN NOT NULL
);

-- Orders placed by clients
CREATE TABLE orders (
    order_id      SERIAL PRIMARY KEY,
    order_date    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    client_id     INTEGER NOT NULL REFERENCES clients(client_id)
);

-- Items in each order
CREATE TABLE order_items (
    order_id      INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    pizza_id      INTEGER NOT NULL REFERENCES pizzas(pizza_id),
    quantity      INTEGER NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (order_id, pizza_id)
);

-- Districts and Municipalities for address normalization
CREATE TABLE districts (
    district_id   SERIAL PRIMARY KEY,
    name          VARCHAR(50) NOT NULL
);

CREATE TABLE municipalities (
    municipality_id SERIAL PRIMARY KEY,
    name             VARCHAR(50) NOT NULL,
    district_id      INTEGER NOT NULL REFERENCES districts(district_id)
);

-- Addresses for deliveries
CREATE TABLE addresses (
    address_id      SERIAL PRIMARY KEY,
    street_name     TEXT NOT NULL,
    postal_code     VARCHAR(10) NOT NULL,
    door_number     VARCHAR(10),
    municipality_id INTEGER NOT NULL REFERENCES municipalities(municipality_id)
);

-- Delivery orders (to be delivered to an address)
CREATE TABLE deliveries (
    order_id      INTEGER PRIMARY KEY REFERENCES orders(order_id),
    address_id    INTEGER NOT NULL REFERENCES addresses(address_id),
    delivered_at  TIMESTAMP       -- NULL until delivered
);

-- Take-away orders (client picks up)
CREATE TABLE takeaway_orders (
    order_id      INTEGER PRIMARY KEY REFERENCES orders(order_id)
);

-- Employees and subtypes
CREATE TABLE employees (
    employee_id   SERIAL PRIMARY KEY,
    name          VARCHAR(60) NOT NULL,
    salary        NUMERIC(10,2) CHECK (salary >= 0),
    store_id      INTEGER NOT NULL REFERENCES stores(store_id)
    is_active    BOOLEAN NOT NULL
);

-- Employees who drive deliveries
CREATE TABLE delivery_drivers (
    employee_id INTEGER PRIMARY KEY REFERENCES employees(employee_id),
    licence     VARCHAR(20) UNIQUE  -- driver's licence
);

-- Motorcycles assigned to drivers
CREATE TABLE motorcycles (
    motorcycle_id SERIAL PRIMARY KEY,
    license_plate VARCHAR(15) UNIQUE NOT NULL,
    brand         VARCHAR(30),
    driver_id     INTEGER NOT NULL REFERENCES delivery_drivers(employee_id)
);

-- Employees working in-store (cashiers, cooks, managers)
CREATE TABLE store_staff (
    employee_id INTEGER PRIMARY KEY REFERENCES employees(employee_id),
    role        VARCHAR(30) NOT NULL CHECK (role IN ('cashier','cook','manager'))
);

CREATE TABLE promotions (
    promotion_id     SERIAL PRIMARY KEY,
    item_type        VARCHAR(10) NOT NULL CHECK (item_type IN ('product','pizza')),
    item_id          INTEGER NOT NULL,
    discount_percent NUMERIC(5,2) NOT NULL CHECK (discount_percent > 0 AND discount_percent <= 100),
    start_date       DATE NOT NULL,
    end_date         DATE NOT NULL,
    CHECK (start_date <= end_date)
);

-- =====================================================================
-- TRIGGERS
-- =====================================================================

-- Trigger: on inserting into promotions, check that the referenced item exists
CREATE OR REPLACE FUNCTION check_promotion_item() RETURNS TRIGGER AS $$
BEGIN
    IF NEW.item_type = 'product' THEN
        PERFORM 1 FROM products WHERE product_id = NEW.item_id;
    ELSE
        PERFORM 1 FROM pizzas WHERE pizza_id = NEW.item_id;
    END IF;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Promotion refers to non-existent % id %', NEW.item_type, NEW.item_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_check_promotion_item
    BEFORE INSERT OR UPDATE ON promotions
    FOR EACH ROW EXECUTE FUNCTION check_promotion_item();

-- =====================================================================
-- VIEWS
-- =====================================================================


-- =====================================================================
-- ACCESS CONTROL
-- =====================================================================

-- Create roles
CREATE ROLE client_role;
CREATE ROLE staff_role;
CREATE ROLE driver_role;
CREATE ROLE manager_role;

-- Grant minimal privileges
GRANT USAGE ON SCHEMA public TO client_role, staff_role, driver_role, manager_role;

-- Clients: can view menu and place orders
GRANT SELECT ON products, pizzas, pizza_ingredients TO client_role;
GRANT INSERT ON clients, orders, order_items, takeaway_orders TO client_role;

-- Staff: can view and manage orders
GRANT SELECT, UPDATE ON orders, order_items, deliveries TO staff_role;

-- Drivers: can view assigned deliveries
GRANT SELECT ON deliveries TO driver_role;
GRANT UPDATE ON deliveries TO driver_role;

-- Managers: full access to reporting and staff
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO manager_role;
