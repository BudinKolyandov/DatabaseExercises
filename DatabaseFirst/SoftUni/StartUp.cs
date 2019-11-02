using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var employeesFullInfo = GetEmployeesFullInformation(context);
                Console.WriteLine(employeesFullInfo);
                
                var employeesWithSalary = GetEmployeesWithSalaryOver50000(context);
                Console.WriteLine(employeesWithSalary);
                
                var employeesFromRAD = GetEmployeesFromResearchAndDevelopment(context);
                Console.WriteLine(employeesFromRAD);
                
                var addAndUpdateEmployee = AddNewAddressToEmployee(context);
                Console.WriteLine(addAndUpdateEmployee);
                
                var employeesAndProjects = GetEmployeesInPeriod(context);
                Console.WriteLine(employeesAndProjects);
                
                var addressesByTown = GetAddressesByTown(context);
                Console.WriteLine(addressesByTown);
                
                var getEmployee = GetEmployee147(context);
                Console.WriteLine(getEmployee);
                
                var departmentsWithFiveOrMoreEmployees = GetDepartmentsWithMoreThan5Employees(context);
                Console.WriteLine(departmentsWithFiveOrMoreEmployees);
                
                var latestProjects = GetLatestProjects(context);
                Console.WriteLine(latestProjects);

                var increaceSalaries = IncreaseSalaries(context);
                Console.WriteLine(increaceSalaries);
                
                var employeesNamesStartingWithSa = GetEmployeesByFirstNameStartingWithSa(context);
                Console.WriteLine(employeesNamesStartingWithSa);

                var deleteProjectById = DeleteProjectById(context);
                Console.WriteLine(deleteProjectById);
                

                var removeTown = RemoveTown(context);
                Console.WriteLine(removeTown);

            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                    e.EmployeeId
                })
                .OrderBy(x => x.EmployeeId)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Name} - ${employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);

            var nakov = context.Employees.
                FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = address;
            context.SaveChanges();

            var employeesAddresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(a => a.Address.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employeeAddress in employeesAddresses)
            {
                sb.AppendLine(employeeAddress);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(p => p.EmployeesProjects.Any(s => s.Project.StartDate.Year >= 2001 && s.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    employeeName = x.FirstName + " " + x.LastName,
                    managerName = x.Manager.FirstName + " " + x.Manager.LastName,
                    projects = x.EmployeesProjects.Select(p => new
                    {
                        p.Project.Name,
                        p.Project.StartDate,
                        p.Project.EndDate
                    }).ToList()
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.employeeName} - Manager: {employee.managerName}");

                foreach (var project in employee.projects)
                {
                    var startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt");
                    string endDate = string.Empty;
                    if (project.EndDate.HasValue)
                    {
                        endDate = project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt");
                    }
                    else
                    {
                        endDate = "not finished";
                    }

                    sb.AppendLine($"--{project.Name} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context.Addresses
                .GroupBy(a => new { 
                    a.AddressId,
                    a.AddressText,
                    a.Town.Name
                },
                (key, group) => new
                {
                    AddressText = key.AddressText,
                    Town = key.Name,
                    Count = group.Sum(a => a.Employees.Count)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Town)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.Town} - {address.Count} employees");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    projects = e.EmployeesProjects.Select(p => new
                    {
                        p.Project.Name
                    })
                    .OrderBy(x => x.Name)
                })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                foreach (var project in employee.projects)
                {
                    sb.AppendLine($"{project.Name}");
                }
            }
                

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    managerFullName = d.Manager.FirstName + " " + d.Manager.LastName,
                    emplyees = d.Employees.Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(x=>x.FirstName)
                    .ThenBy(x=>x.LastName)
                });

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} - {department.managerFullName}");
                foreach (var employee in department.emplyees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
            .Where(e =>
                e.Department.Name == "Engineering"
                || e.Department.Name == "Tool Design"
                || e.Department.Name == "Marketing"
                || e.Department.Name == "Information Services")
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary*= 1.12m:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var emplyees = context.Employees
                .Where(e => e.FirstName.Substring(0, 2) == "Sa")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            foreach (var employee in emplyees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projectToRemove = context.Projects.First(p => p.ProjectId == 2);

            context.EmployeesProjects.ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));
            context.Projects.Remove(projectToRemove);

            context.SaveChanges();

            var projects = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            int removedAddresses = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .Count();

            var addresses = context.Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToList();

            foreach (var address in addresses)
            {
                context.Addresses.Remove(address);
            }

            context.Towns.Remove(
                context.Towns.SingleOrDefault(t => t.Name == "Seattle"));

            context.SaveChanges();

            sb.AppendLine($"{removedAddresses} addresses in Seattle were deleted");

            return sb.ToString().TrimEnd();
        }

    }
}
