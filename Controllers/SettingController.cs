using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class SettingController : Controller
    {
         private string company_hash = Models.HttpResponse.CompanyHash;
        // GET: Setting
        public async Task<ActionResult> Index()
        {

            //輸入公司代碼取得部門資料
            List<Department> department = await DepartmentModel.Get_DepartmentAsync(company_hash);
            //輸入公司代碼取得職稱資料
            List<JobTitle> jobtitle = await JobtitleModel.Get_JobtitleAsync(company_hash);

            ViewBag.departments = department;//部門名稱
            ViewBag.jobtitles = jobtitle;//職稱


            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCompanyPassword(string NewPassword)
        {
            Response.Write(NewPassword);
            return View("index");
        }
    }
}