using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceManagement.Controllers
{
    public class PunchRecordsController : Controller
    {
        // GET: PunchRecords
        public ActionResult Index()
        {
            return View();
        }
    }
}