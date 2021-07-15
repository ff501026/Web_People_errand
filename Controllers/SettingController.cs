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
            List<DepartmentModel> department = await DepartmentModel.Get_DepartmentAsync(company_hash);

            string[] Department_Name = new string[department.Count];//部門名稱


            for (int i = 0; i < department.Count; i++)//將部門名稱存入欄位
            {

                Department_Name[i] = department[i].Name;
            }


            //輸入公司代碼取得職稱資料
            List<JobtitleModel> jobtitle = await JobtitleModel.Get_JobtitleAsync(company_hash);

            string[] Jobtitle_Name = new string[jobtitle.Count];//職稱


            for (int i = 0; i < jobtitle.Count; i++)//將職稱分別存入欄位
            {

                Jobtitle_Name[i] = jobtitle[i].Name;
            }


            ViewBag.department_name = Department_Name;//部門名稱

            ViewBag.jobtitle_name = Jobtitle_Name;//職稱


            return View();
        }
    }
}