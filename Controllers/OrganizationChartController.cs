using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class OrganizationChartController : Controller
    {
        // GET: OrganizationChart
        public async Task<ActionResult> Index()
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            List<OrganizationChart> organizations = await OrganizationChartModel.GetOrganization(Session["company_hash"].ToString());//組織資料
            ViewBag.Organization = organizations;
            return View();
        }
    }
}