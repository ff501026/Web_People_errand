using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceManagement.Controllers
{
    public class AttendanceManagementController : Controller
    {
        // GET: AttendanceManagement
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult TripRecords()
        {
            return View();
        }

        public ActionResult EmployeeData()
        {
            return View();
        }

        public ActionResult Setting()
        {
            return View();
        }

        public ActionResult DataExport()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}