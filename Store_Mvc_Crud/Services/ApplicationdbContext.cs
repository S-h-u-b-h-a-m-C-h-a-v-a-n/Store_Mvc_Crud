using Microsoft.EntityFrameworkCore;
using Store_Mvc_Crud.Models;

namespace Store_Mvc_Crud.Services
{
    public class ApplicationdbContext : DbContext
    {
        public ApplicationdbContext(DbContextOptions options) : base (options)
        {
                
        }

        public DbSet<Product> Products { get; set; }
    }
}
