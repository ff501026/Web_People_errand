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
        public async Task<ActionResult> CompanyLogin(string code, string email,string manager_password)
        {
            if (email.Equals(""))
            {
                CompanyLogin result = await CompanyManagerPasswordModel.Login(code, manager_password);
                if (result.enabled)
                {
                    Session["company_hash"] = result.CompanyHash;
                    Session["name"] = result.Name;

                    return RedirectToAction("Index", "PunchRecords", null);
                }
                else
                    return Content($"<script>alert('公司ID或Email或密碼錯誤');history.go(-1);</script>");
            }
            else 
            {
                ManagerLogin result = await CompanyManagerPasswordModel.CompanyManagerLogin(code,email, manager_password);
                if (result.enabled)
                {
                    Session["company_hash"] = result.CompanyHash;
                    Session["hash_account"] = result.HashAccount;
                    Session["name"] = result.Name;
                    return RedirectToAction("Index", "PunchRecords", null);
                }
                else
                    return Content($"<script>alert('公司ID或Email或密碼錯誤');history.go(-1);</script>");
            }
        }
    }
}