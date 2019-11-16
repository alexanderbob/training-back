using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Controllers
{
    public class IndexController : Controller
    {
        private string _frontAddress;
        private bool isDevelopment;
        public IndexController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _frontAddress = configuration.GetValue<string>("Application:frontAddress");
            isDevelopment = environment.IsDevelopment();
        }

        [HttpGet]
        public ActionResult Get()
        {
            if (isDevelopment)
            {
                return Redirect(_frontAddress);
            }
            else
            {
                return new PhysicalFileResult(Path.Combine(Directory.GetCurrentDirectory(), "Content/index.html"), "text/html");
            }
        }
    }
}
