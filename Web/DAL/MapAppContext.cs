using MapApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MapApp.DAL
{
    public class MapAppContext : DbContext
    {
        public MapAppContext() : base("MapAppContext")
        {
        }

        public DbSet<MapItem> MapItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}