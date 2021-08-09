using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CompanyLogin(string code, string manager_password)
        {
            
            CompanyLogin result = await CompanyManagerPasswordModel.Login(code,manager_password);
            if (result.enabled)
            {
                Session["company_hash"] = result.CompanyHash;
                Session["name"] = result.Name;

                return RedirectToAction("Index","PunchRecords",null);
            }
            else
                return Content($"<script>alert('公司ID或密碼錯誤');history.go(-1);</script>");
        }
    }
}