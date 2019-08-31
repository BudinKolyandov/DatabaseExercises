namespace Orm.App
{
    using System.Linq;
    using Data;
    using Data.Entities;

    class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=DESKTOP-J1971AU\SQLEXPRESS;Database = MiniORM;Integrated Security = true";

            var context = new OrmDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Peter",
                LastName = "Pan",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            });

            var emplyee = context.Employees.First();
            emplyee.FirstName = "New";

            context.SaveChanges();


        }
    }
}
