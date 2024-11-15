using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TriggerJob.Models;

namespace TriggerJob
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> op) : base(op)
        {

        }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Quiz> Quiz { get; set; }

    }
}
