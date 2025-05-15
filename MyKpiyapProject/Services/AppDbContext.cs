using MyKpiyapProject.NewModels;
using System.Data.Entity;

namespace MyKpiyapProject.Services
{
    public class AppDbContext : DbContext
    {
        private readonly static string ConnectionString = "Data Source=HOME-PC\\SQLEXPRESS;Initial Catalog=dbRspoManager;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;";

        public DbSet<tbEmployee> Employees { get; set; }
        public DbSet<tbReport> Reports { get; set; }
        public DbSet<tbProject> Projects { get; set; }
        public DbSet<tbTask> Tasks { get; set; }
        public DbSet<tbAdminLog> AdminLogs { get; set; }

        public AppDbContext() : base(ConnectionString)
        {
            //Configuration.LazyLoadingEnabled = true;
            //Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Пример: Если Проект удаляется, все связанные Задачи также должны быть удалены
            modelBuilder.Entity<tbProject>()
                .HasMany(p => p.Tasks)
                .WithRequired(t => t.Project)
                .HasForeignKey(t => t.ProjectID)
                .WillCascadeOnDelete(true);

            // Если сотрудник удаляется, связанные задачи удаляются
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.Tasks)
                .WithRequired(t => t.Executor)
                .HasForeignKey(t => t.ExecutorID)
                .WillCascadeOnDelete(false); 

            // Если сотрудник удаляется, связанные отчеты остаются
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.Reports)
                .WithRequired(r => r.Employee)
                .HasForeignKey(r => r.EmployeeID)
                .WillCascadeOnDelete(false);

            // Если сотрудник удаляется, связанные проекты остаются
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.Projects)
                .WithRequired(p => p.Creator)
                .HasForeignKey(p => p.CreatorID)
                .WillCascadeOnDelete(false);

            // Если сотрудник удаляется, связанные логи админа остаются
            modelBuilder.Entity<tbEmployee>()
                .HasMany(e => e.AdminLogs)
                .WithRequired(a => a.Employee)
                .HasForeignKey(a => a.EmployeeID)
                .WillCascadeOnDelete(false);
        }
    }
}
