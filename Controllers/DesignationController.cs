using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerAPI.Models;

namespace CustomerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly Learn_DBContext context;
        public DesignationController(Learn_DBContext learn_DB)
        {
            context = learn_DB;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public IEnumerable<TblDesignation> Get()
        {
            return context.TblDesignation.ToList();
        }
        [HttpGet("{id}")]
        public TblDesignation Get(string id)
        {
            return context.TblDesignation.FirstOrDefault(o => o.Code == id);
        }
        [HttpPost]
        public APIResponse Post([FromBody] TblDesignation value)
        {
            string result = string.Empty;
            try
            {
                var _emp = context.TblDesignation.FirstOrDefault(o => o.Code == value.Code);
                if (_emp != null)
                {
                    _emp.Name = value.Name;
                    context.SaveChanges();
                    result = "pass";
                }
                else
                {
                    TblDesignation tblEmployee = new TblDesignation()
                    {
                        Name = value.Name,
                        Code = value.Code
                    };
                    context.TblDesignation.Add(tblEmployee);
                    context.SaveChanges();
                    result = "pass";
                }

            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }
    }
}
