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
    }
}
