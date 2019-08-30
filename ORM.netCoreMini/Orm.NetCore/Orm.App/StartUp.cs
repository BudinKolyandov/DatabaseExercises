namespace Orm.App
{
    using System.Linq;
    using Data;
    using Data.Entities;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=DESKTOP-J1971AU\SQLEXPRESS;Database=OrmTest;Integrated Security = true";

            var context = new OrmDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Lucas",
                LastName = "Jones",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });

            var employee = context.Employees.Last();

            employee.FirstName = "Jessica";
            context.SaveChanges();
        }
    }
}
