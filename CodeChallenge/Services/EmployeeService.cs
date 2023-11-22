using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using System.Collections.Immutable;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructureById(string id)
        {
            // Create local instance of ReportingStructure to return
            ReportingStructure reportingStructure = new ReportingStructure();

            // Only worry about getting employee if id is provided
            if (!String.IsNullOrEmpty(id))
            {
                // Call existing GetById method in data layer to populate Employee object
                reportingStructure.Employee = _employeeRepository.GetById(id);
                // Since this should not persist, call local function to calculate number of reports
                int numberOfReports = 0;
                reportingStructure.NumberOfReports = GetNumberOfReports(reportingStructure.Employee, numberOfReports);
                return reportingStructure;
            }
            return null;
        }

        public Compensation CreateCompensation(Compensation compensation)
        {
            // Compensation must be present and employee must exist to add compensation record
            Employee employee = _employeeRepository.GetById(compensation.Employee.EmployeeId);

            // Could also go a step further here and try to add an Employee record if it doesn't exist.
            if (compensation != null && employee != null)
            {
                // Since this should persist, call data layer to Add compensation record and save
                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public Compensation GetCompensationById(string id)
        {
            // No need for any checks beyond null/empty. Compensation will be null/empty if employee does not exist
            if (!String.IsNullOrEmpty(id))
            {
                // Since this should persist, call data layer to get compensation record from database
                return _employeeRepository.GetCompensationById(id);
            }

            return null;
        }

        private int GetNumberOfReports(Employee employee, int numberOfReports)
        {
            // Return 0 if employee doesn't have direct reports
            if (employee.DirectReports != null && employee.DirectReports.Any())
            {
                numberOfReports = numberOfReports + employee.DirectReports.Count();

                // Make recursive call for each direct report to get their direct reports and increment count
                foreach(var report in employee.DirectReports)
                {
                    Employee reportingEmployee = GetById(report.EmployeeId);
                    GetNumberOfReports(reportingEmployee, numberOfReports);
                }
                return numberOfReports;
            }
            return numberOfReports;
        }
    }
}
