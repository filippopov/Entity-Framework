using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_FrameworkHomework
{
    class Program
    {
        static void Main(string[] args)
        {
            var employee = new Employee();
            employee.FirstName = "Filip";
            employee.LastName = "Ivanov";
            employee.JobTitle = "dr.";
            employee.DepartmentID = 3;
            employee.HireDate = DateTime.Now;
            employee.Salary = 1234;
            EmployeeDAO.Add(employee);
            var emp=EmployeeDAO.FindByKey(3);
            Console.WriteLine(emp.FirstName);
            EmployeeDAO.Modify(employee,"Ivan");
            //EmployeeDAO.Delete(employee);

            //Problem 3.	Database Search Queries

            var context = new SoftUniEntities();
            var employees = context.Employees.Select(e => new
            {
                e.FirstName,
                e.LastName,
                Projects=e.Projects
                            .Where(p=>p.StartDate.Year>=2001 && p.StartDate.Year<=2003)
                            .Select(p=>new {p.Name,p.StartDate,p.EndDate})
            });

            foreach (var em in employees)
            {
                foreach (var pro in em.Projects)
                {
                    //Console.WriteLine("{0} {1}, Project Name:{2}---{3}-{4}",em.FirstName,em.LastName,pro.Name,pro.StartDate,pro.EndDate);
                }
            }

            //Task 2

            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .Take(10)
                .Select(a => new
                {
                    a.AddressText,
                    a.Town.Name,
                    a.Employees.Count
                });

            foreach (var address in addresses)
            {
                Console.WriteLine("{0}, {1} - {2}",address.AddressText,address.Name,address.Count);
            }
            Console.WriteLine();
            // Task 3

            var empl = context.Employees.Select(e => new
            {
                e.EmployeeID,
                e.FirstName,
                e.LastName,
                e.JobTitle,
                ProjectsName=e.Projects.Select(p=>new {p.Name})
            }).FirstOrDefault(e => e.EmployeeID == 147);

            Console.WriteLine("{0} {1} {2} {3}",empl.FirstName,empl.LastName,empl.JobTitle,empl.ProjectsName.First().Name);


            //Task 4
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Employees.Count,
                    d.Name,
                    ManagerFullName = d.ManagerID,
                    d.Employee.HireDate,
                    d.Employee.JobTitle

                })
                .OrderBy(p => p.Count)
                .GroupBy(d => d.Name);
            Console.WriteLine(departments.Count());
            foreach (var department in departments)
            {
                foreach (var dep in department)
                {
                    Console.WriteLine("--{0} - Manger:{1}, Employees:{2}",dep.Name,dep.ManagerFullName,dep.Count);
                }
            }
            Console.WriteLine();

            //Problem 4.Native SQL Query

            var totalCount = context.Employees.Count();
            var sw = new Stopwatch();
            sw.Start();
            PrintNameWithNativeQuery();
            Console.WriteLine("Native: {0}", sw.Elapsed);

            sw.Restart();
            PrintNameWithLinqQuery();
            Console.WriteLine("Linq: {0}", sw.Elapsed);

            //Problem 6.	Call a Stored Procedure

            Console.WriteLine();
            var projects = context.usp_GetProjectsByEmployee("Guy", "Gilbert");

            foreach (var project in projects)
            {
                Console.WriteLine("{0}, {1}, {2}",project.Name,project.Description,project.StartDate);
            }

        }

        private static void PrintNameWithLinqQuery()
        {
            var context = new SoftUniEntities();

            var employees = context.Employees.Select(e => new
            {
                e.FirstName,
                Projects = e.Projects.Where(p => p.StartDate.Year == 2002)
            });

            //foreach (var employee in employees)
            //{
            //    Console.WriteLine(employee.FirstName);
            //}
        }

        private static void PrintNameWithNativeQuery()
        {
            var context = new SoftUniEntities();

           var employees= context.Database.SqlQuery<String>
                ("SELECT e.FirstName from Employees e join EmployeesProjects ep on e.EmployeeID=ep.EmployeeID join Projects p on ep.ProjectID=p.ProjectID where Year(p.StartDate)='2002'");
            //foreach (var employee in employees)
            //{
            //    Console.WriteLine(employee);
            //}
        }
    }
}
