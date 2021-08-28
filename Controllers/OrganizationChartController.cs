using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceManagement.Controllers
{
    public class OrganizationChartController : Controller
    {
        // GET: OrganizationChart
        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }
    }
}