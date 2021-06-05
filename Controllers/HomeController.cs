using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AlarmController.Models;
using Microsoft.AspNetCore.Authorization;

namespace AlarmController.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Kontrola()
        {
            return View();
        }

        public IActionResult Upali()
        {
            PromjeniStatus(1);
            return RedirectToAction("Kontrola");
        }

        public IActionResult Ugasi()
        {
            PromjeniStatus(0);
            return RedirectToAction("Kontrola");
        }

        private void PromjeniStatus(int status)
        {
            if (status == 0 || status == 1)
            {
                var command = $"sudo uhubctl -l 2 -a {status.ToString()}";
                var result = "";
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = "/bin/bash";
                    proc.StartInfo.Arguments = "-c \" " + command + " \"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();

                    result += proc.StandardOutput.ReadToEnd();
                    result += proc.StandardError.ReadToEnd();

                    proc.WaitForExit();
                }

                Console.WriteLine("USB Hub Status Changed");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
