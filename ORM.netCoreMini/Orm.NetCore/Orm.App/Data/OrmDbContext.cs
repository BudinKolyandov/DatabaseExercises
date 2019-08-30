namespace Orm.App.Data
{
    using Entities;
    public class OrmDbContext : DbContext
    {
        public OrmDbContext(string connectionString) 
            : base(connectionString)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeesWorkingOnProjects> EmployeesWorkingOnProjects { get; set; }
    }
}
