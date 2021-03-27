using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortableManager.Web.Server.Models.Dto;

namespace PortableManager.Web.Server.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<TaskDto> Tasks { get; set; }
        public DbSet<TaskTypeDto> TaskTypes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
