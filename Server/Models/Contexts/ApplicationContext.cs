using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace netcore.Models.Contexts
{

    public class ApplicationContext: DbContext
    {
        // public virtual DbSet<User> Users { get; set; }
        // public virtual DbSet<UserState> UserStates { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        public override Int32 SaveChanges() {
            return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess: true);
        }
        public override Int32 SaveChanges(Boolean acceptAllChangesOnSuccess) {
            return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
        }
        public override Task<Int32> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess: true, cancellationToken: cancellationToken);
        }
        public override Task<Int32> SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
            return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess, cancellationToken);
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //     => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<UserState>( t => {
            //     t.HasKey( s => new { s.Type, s.UserId });
            // });

            // modelBuilder.Entity<User>( t => {
            //     t.HasKey( u => u.Id );
            //     t.HasIndex( u => u.Signature )
            //         .IsUnique();
            //     t.HasMany( u => u.States )
            //         .WithOne( s => s.User )
            //         .HasForeignKey( s => s.UserId );
            //     // this stamp is used as row version
            //     t.Property( p => p.ConcurrencyStamp )
            //         .IsConcurrencyToken();
            // });
        }

    }

}