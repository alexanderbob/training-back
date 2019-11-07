using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/")]
    public class IndexController : ControllerBase
    {
        private string _frontAddress;
        public IndexController(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("Application");
            _frontAddress = section["frontAddress"];
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Redirect(_frontAddress);
        }
    }
}
