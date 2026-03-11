using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SaaS.Api.Data;
using SaaS.Api.Services;
using System;

namespace SaaS.Api
{
    // Esta clase es usada solo por EF Core en tiempo de diseño
    // para crear el DbContext cuando hacemos migraciones.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Configuramos las opciones del DbContext (ej. cadena de conexión)
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=ALBERTODIAZ\\SQLEXPRESS;Initial Catalog=SaaSManagementDB;Integrated Security=True;TrustServerCertificate=True;");

            // Pasamos un servicio “nulo” para CurrentUserService
            // Esto evita errores en tiempo de diseño
            return new ApplicationDbContext(optionsBuilder.Options, new NullCurrentUserService());
        }
    }

    // Servicio nulo para que el DbContext funcione en tiempo de diseño
    public class NullCurrentUserService : ICurrentUserService
    {
        public Guid? CompanyId => null;
    }
}