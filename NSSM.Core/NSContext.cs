namespace NSSM2.Core
{
    using NSSM.Core.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;

    public class NSContextInitializer : CreateDatabaseIfNotExists<NSContext>
    {
        protected override void Seed(NSContext context)
        {
            context.Members.Add(new Member
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "Admin@admin.com",
            });

            base.Seed(context);
        }
    }

    public class NSContext : DbContext
    {
        //uncomment this when running code-first migration (model changes)
        //public NSContext()
        //    : base("NSCONTEXTCONNSTRING")
        //{
        //}

        public NSContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer(new NSContextInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>().HasRequired(n => n.AdminMember)
                .WithMany(m => m.AdminNodes).HasForeignKey(n => n.AdminMemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Node>().HasRequired(n => n.CreatedBy)
                .WithMany(m => m.CreatedNodes).HasForeignKey(n => n.CreatedbyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Properties<DateTime>()
                .Configure(property => property.HasColumnType("datetime2"));

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateBaseEntities();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            UpdateBaseEntities();
            return await base.SaveChangesAsync();
        }

        private void UpdateBaseEntities()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is ModelBase
                && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            foreach (var e in entities)
            {
                if (e.Entity is ModelBase)
                {
                    if (e.State == EntityState.Added)
                    {
                        ((ModelBase)e.Entity).IsActive = true;
                        ((ModelBase)e.Entity).IsDeleted = false;
                        ((ModelBase)e.Entity).CreatedDate = DateTime.Now;
                    }
                    else if (e.State == EntityState.Deleted)
                    {
                        e.State = EntityState.Modified;
                        ((ModelBase)e.Entity).IsDeleted = true;
                    }

                    ((ModelBase)e.Entity).ModifiedDate = DateTime.Now;
                }
            }
        }


        #region DBETS

        public DbSet<Member> Members { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Scan> Scans { get; set; }
        public DbSet<Project> Projects { get; set; }

        #endregion
    }
}