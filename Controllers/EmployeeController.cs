using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace CustomerAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly Learn_DBContext context;
        public EmployeeController(Learn_DBContext learn_DB)
        {
            context = learn_DB;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<TblEmployee> Get()
        {
            return context.TblEmployee.ToList();
        }
        [HttpGet("{id}")]
        public TblEmployee Get(int id)
        {
            return context.TblEmployee.FirstOrDefault(o => o.Code == id);
        }

        [HttpPost]
        public APIResponse Post([FromBody] TblEmployee value)
        {
            string result = string.Empty;
            try
            {
                var _emp = context.TblEmployee.FirstOrDefault(o => o.Code == value.Code);
                if (_emp != null)
                {
                    _emp.Designation = value.Designation;
                    _emp.Email = value.Email;
                    _emp.Name = value.Name;
                    _emp.Phone = value.Phone;
                    context.SaveChanges();
                    result= "pass";
                }
                else
                {
                    TblEmployee tblEmployee = new TblEmployee()
                    {
                        Name = value.Name,
                        Email = value.Email,
                        Phone = value.Phone,
                        Designation = value.Designation
                    };
                    context.TblEmployee.Add(tblEmployee);
                    context.SaveChanges();
                    result= "pass";
                }
                
            }
            catch(Exception ex)
            {
                result= string.Empty;
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }

        [HttpDelete("{id}")]
        public APIResponse Delete(int id)
        {
            string result = string.Empty;
            var _emp=context.TblEmployee.FirstOrDefault(o => o.Code == id);
            if (_emp != null)
            {
                context.TblEmployee.Remove(_emp);
                context.SaveChanges();
                result= "pass";
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }
    }
}
