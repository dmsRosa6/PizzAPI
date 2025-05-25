using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PizzAPI.Models;

public partial class PizzaStoreContext : DbContext
{
    public PizzaStoreContext()
    {
    }

    public PizzaStoreContext(DbContextOptions<PizzaStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<DeliveryDriver> DeliveryDrivers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Motorcycle> Motorcycles { get; set; }

    public virtual DbSet<Municipality> Municipalities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Pizza> Pizzas { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreStaff> StoreStaffs { get; set; }

    public virtual DbSet<TakeawayOrder> TakeawayOrders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=55432;Database=pizzastore;Username=postgres;Password=secret");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("addresses_pkey");

            entity.ToTable("addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.DoorNumber)
                .HasMaxLength(10)
                .HasColumnName("door_number");
            entity.Property(e => e.MunicipalityId).HasColumnName("municipality_id");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .HasColumnName("postal_code");
            entity.Property(e => e.StreetName).HasColumnName("street_name");

            entity.HasOne(d => d.Municipality).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.MunicipalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("addresses_municipality_id_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.HasIndex(e => e.Nif, "clients_nif_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "clients_phone_number_key").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Nif)
                .HasMaxLength(20)
                .HasColumnName("nif");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("deliveries_pkey");

            entity.ToTable("deliveries");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.DeliveredAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("delivered_at");

            entity.HasOne(d => d.Address).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deliveries_address_id_fkey");

            entity.HasOne(d => d.Order).WithOne(p => p.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deliveries_order_id_fkey");
        });

        modelBuilder.Entity<DeliveryDriver>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("delivery_drivers_pkey");

            entity.ToTable("delivery_drivers");

            entity.HasIndex(e => e.Licence, "delivery_drivers_licence_key").IsUnique();

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employee_id");
            entity.Property(e => e.Licence)
                .HasMaxLength(20)
                .HasColumnName("licence");

            entity.HasOne(d => d.Employee).WithOne(p => p.DeliveryDriver)
                .HasForeignKey<DeliveryDriver>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("delivery_drivers_employee_id_fkey");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("districts_pkey");

            entity.ToTable("districts");

            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Salary)
                .HasPrecision(10, 2)
                .HasColumnName("salary");
            entity.Property(e => e.StoreId).HasColumnName("store_id");

            entity.HasOne(d => d.Store).WithMany(p => p.Employees)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_store_id_fkey");
        });

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.MotorcycleId).HasName("motorcycles_pkey");

            entity.ToTable("motorcycles");

            entity.HasIndex(e => e.LicensePlate, "motorcycles_license_plate_key").IsUnique();

            entity.Property(e => e.MotorcycleId).HasColumnName("motorcycle_id");
            entity.Property(e => e.Brand)
                .HasMaxLength(30)
                .HasColumnName("brand");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(15)
                .HasColumnName("license_plate");

            entity.HasOne(d => d.Driver).WithMany(p => p.Motorcycles)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("motorcycles_driver_id_fkey");
        });

        modelBuilder.Entity<Municipality>(entity =>
        {
            entity.HasKey(e => e.MunicipalityId).HasName("municipalities_pkey");

            entity.ToTable("municipalities");

            entity.Property(e => e.MunicipalityId).HasColumnName("municipality_id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.District).WithMany(p => p.Municipalities)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("municipalities_district_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_client_id_fkey");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.PizzaId }).HasName("order_items_pkey");

            entity.ToTable("order_items");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PizzaId).HasColumnName("pizza_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.Pizza).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.PizzaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_pizza_id_fkey");
        });

        modelBuilder.Entity<Pizza>(entity =>
        {
            entity.HasKey(e => e.PizzaId).HasName("pizzas_pkey");

            entity.ToTable("pizzas");

            entity.Property(e => e.PizzaId).HasColumnName("pizza_id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasMany(d => d.Products).WithMany(p => p.Pizzas)
                .UsingEntity<Dictionary<string, object>>(
                    "PizzaIngredient",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pizza_ingredients_product_id_fkey"),
                    l => l.HasOne<Pizza>().WithMany()
                        .HasForeignKey("PizzaId")
                        .HasConstraintName("pizza_ingredients_pizza_id_fkey"),
                    j =>
                    {
                        j.HasKey("PizzaId", "ProductId").HasName("pizza_ingredients_pkey");
                        j.ToTable("pizza_ingredients");
                        j.IndexerProperty<int>("PizzaId").HasColumnName("pizza_id");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                    });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("promotions_pkey");

            entity.ToTable("promotions");

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.DiscountPercent)
                .HasPrecision(5, 2)
                .HasColumnName("discount_percent");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemType)
                .HasMaxLength(10)
                .HasColumnName("item_type");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("stores_pkey");

            entity.ToTable("stores");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.Name)
                .HasMaxLength(52)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StoreStaff>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("store_staff_pkey");

            entity.ToTable("store_staff");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employee_id");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");

            entity.HasOne(d => d.Employee).WithOne(p => p.StoreStaff)
                .HasForeignKey<StoreStaff>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("store_staff_employee_id_fkey");
        });

        modelBuilder.Entity<TakeawayOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("takeaway_orders_pkey");

            entity.ToTable("takeaway_orders");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");

            entity.HasOne(d => d.Order).WithOne(p => p.TakeawayOrder)
                .HasForeignKey<TakeawayOrder>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("takeaway_orders_order_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
