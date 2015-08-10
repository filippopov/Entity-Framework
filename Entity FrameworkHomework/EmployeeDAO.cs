using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_FrameworkHomework
{
    public static class EmployeeDAO
    {
        
        public static void Add(Employee employee)
        {
            var context = new SoftUniEntities();
            context.Employees.Add(employee);
            context.SaveChanges();
        }

        public static Employee FindByKey(object key)
        {
            var context = new SoftUniEntities();
            Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == (int)key);
            return employee;
        }

        public static void Modify(Employee employee,string firstName)
        {
            var context = new SoftUniEntities();
            var newEmployee=context.Employees.FirstOrDefault(e => e.FirstName == employee.FirstName);
            newEmployee.FirstName = firstName;
            context.SaveChanges();
        }

        public static void Delete(Employee employee)
        {
            var context = new SoftUniEntities();
            var departments = employee.Departments.ToList();
            foreach (var department in departments)
            {
                department.Employee = null;
            }

            context.Employees.Remove(employee);
            context.SaveChanges();

        }
    }
}
