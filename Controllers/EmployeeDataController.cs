using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;



namespace AttendanceManagement.Controllers
{
    public class EmployeeDataController : Controller
    {
        private string company_hash = Models.HttpResponse.CompanyHash;
        // GET: EmployeeData
        public async Task<ActionResult> Index()
        {
            //輸入公司代碼取得待審核資料
            List<ReviewEmployee> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(company_hash);    
            //輸入公司代碼取得已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
           
            ViewBag.review_employee = reviewEmployees;//待審核資料   
            ViewBag.pass_employee = passEmployees; //審核過資料

            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Check()
        {
            return View();
        }
    }
}