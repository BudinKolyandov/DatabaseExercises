namespace Orm.App.Data
{
    using Entities;
    class OrmDbContext : DbContext
    {
        public OrmDbContext(string connectionString) 
            : base(connectionString)
        {
        }

        public DbSet<Employee> Employees { get; }

        public DbSet<Department> Departments { get; }

        public DbSet<Project> Projects { get; }

        public DbSet<EmployeesProjects> EmployeesProjects { get; }
    }
}
