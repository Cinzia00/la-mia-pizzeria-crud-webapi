using la_mia_pizzeria_static.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace la_mia_pizzeria_static.Database
{
    public class PizzaContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Pizza> Pizze {  get; set; }
        public DbSet<Categoria> Categorie { get; set; }
        public DbSet<Ingrediente> Ingredienti { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Pizze;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
