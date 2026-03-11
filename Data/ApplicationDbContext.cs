using Microsoft.EntityFrameworkCore;
using SaaS.Api.Models;
using SaaS.Api.Common;
using System.Linq;
using SaaS.Api.Services;

namespace SaaS.Api.Data
{
    // DbContext principal de la aplicación
    // Se encarga de conectar las entidades con la base de datos
    public class ApplicationDbContext : DbContext
    {
        // Servicio que permite obtener el CompanyId del usuario autenticado
        // Esto se usa para aplicar el filtrado multi-tenant
        private readonly ICurrentUserService _currentUserService;

        // Constructor del DbContext para la aplicación normal
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        // Constructor adicional solo para migraciones
        // EF Core lo usará cuando instancie el DbContext para crear la DB
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Creamos un CurrentUserService falso para que las migraciones no fallen
            _currentUserService = new FakeCurrentUserService();
        }

        // Tablas de la base de datos
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; } // ejemplo de entidad multi-tenant
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }       // Ventas
        public DbSet<SaleItem> SaleItems { get; set; } // Items de ventas

        // Este método se ejecuta cuando EF construye el modelo de datos
        // Aquí configuramos reglas globales como filtros y precisión de decimales
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente el filtro de empresa (tenant)
            // a todas las entidades que implementen ITenantEntity
            ApplyTenantQueryFilter(modelBuilder);

            // Configuración de precisión para campos decimales (dinero)
            modelBuilder.Entity<Products>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 dígitos, 2 decimales

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalAmount)
                .HasPrecision(18, 2); // igualmente la misma regla

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.UnitPrice)
                .HasPrecision(18, 2);
        }

        // Este método aplica filtros automáticos por CompanyId
        private void ApplyTenantQueryFilter(ModelBuilder modelBuilder)
        {
            // Recorremos todas las entidades registradas en EF
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Verificamos si la entidad implementa ITenantEntity
                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Llamamos dinámicamente al método genérico
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetTenantFilter),
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance)
                        .MakeGenericMethod(entityType.ClrType);

                    method.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        // Método genérico que aplica el filtro a cada entidad tenant
        private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ITenantEntity
        {
            // Esto hace que TODAS las consultas incluyan:
            // WHERE CompanyId = usuarioActual.CompanyId
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => e.CompanyId == _currentUserService.CompanyId);
        }

        // Override para guardar cambios de forma asincrónica
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Antes de guardar aplicamos información del tenant
            ApplyTenantInformation();

            return base.SaveChangesAsync(cancellationToken);
        }

        // Override para guardar cambios de forma síncrona
        public override int SaveChanges()
        {
            ApplyTenantInformation();

            return base.SaveChanges();
        }

        // Este método asegura que los datos siempre tengan CompanyId correcto
        private void ApplyTenantInformation()
        {
            var companyId = _currentUserService.CompanyId;

            // Si no hay empresa asociada al usuario no hacemos nada
            if (companyId == null)
                return;

            // Obtenemos todas las entidades modificadas o agregadas
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is ITenantEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (ITenantEntity)entry.Entity;

                // Si la entidad es nueva
                if (entry.State == EntityState.Added)
                {
                    // Se asigna automáticamente el CompanyId
                    entity.CompanyId = companyId.Value;
                }

                // Si la entidad fue modificada
                if (entry.State == EntityState.Modified)
                {
                    // Evitamos que se cambie el CompanyId
                    entry.Property(nameof(ITenantEntity.CompanyId)).IsModified = false;
                }
            }
        }
    }

    // Servicio fake para migraciones
    public class FakeCurrentUserService : ICurrentUserService
    {
        // Retorna un CompanyId ficticio para migraciones
        public Guid? CompanyId => Guid.NewGuid();
    }
}