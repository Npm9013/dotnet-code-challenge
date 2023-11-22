using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost()]
        [Route("createEmployee")]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        [Route("getEmployeeById/{id}")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        [Route("replaceEmployee/{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        // New REST endpoint to return ReportingStructure
        [HttpGet("{id}")]
        [Route("getReportingStructureById/{id}")]
        public IActionResult GetReportingStructureById(String id)
        {
            _logger.LogDebug($"Recieved reporting structure get request for '{id}'");
            // Call service layer
            var reportingStructure = _employeeService.GetReportingStructureById(id);
            // return Not Found if null is returned
            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }

        // New REST endpoint to create Compensation for an Employee Id
        [HttpPost]
        [Route("createCompensation")]
        public IActionResult CreateCompensation(Compensation compensation)
        {
            //_logger.LogDebug($"Received compensation create request for '{compensation.Employee.EmployeeId}'");
            //Call service layer
            _employeeService.CreateCompensation(compensation);
            // Call Get Compensation to return the newly created compensation record
            return CreatedAtRoute("getCompensationById", new { id = compensation.Employee.EmployeeId }, compensation);
        }

        // New REST endpoint to return Compensation based on Employee Id
        [HttpGet("{id}", Name = "getCompensationById")]
        [Route("getCompensationById/{id}")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Recieved compensation get request for '{id}'");

            var compensation = _employeeService.GetCompensationById(id);
            // If no compensation records are found, return Not Found
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
