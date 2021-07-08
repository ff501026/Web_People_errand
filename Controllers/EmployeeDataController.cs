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
            List<ReviewEmployeeModel> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(company_hash);
            string[] CompanyId = new string[reviewEmployees.Count];//公司ID
            string[] Name = new string[reviewEmployees.Count];//員工名稱
            string[] Email = new string[reviewEmployees.Count];//員工電子郵件
            string[] PhoneCode = new string[reviewEmployees.Count]; //員工驗證碼(phone_code)
            DateTime[] CreatedTime = new DateTime[reviewEmployees.Count];//申請時間

            for (int i = 0; i < reviewEmployees.Count; i++)//將待審核資料分別存入欄位
            {
                CompanyId[i] = reviewEmployees[i].CompanyId;
                Name[i] = reviewEmployees[i].Name;
                Email[i] = reviewEmployees[i].Email;
                PhoneCode[i] = reviewEmployees[i].PhoneCode;
                CreatedTime[i] = reviewEmployees[i].CreatedTime;
            }

            //輸入公司代碼取得已審核資料
            List<PassEmployeeModel> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            string[] name = new string[passEmployees.Count];//員工姓名
            string[] phone = new string[passEmployees.Count];//員工電話
            string[] department = new string[passEmployees.Count];//員工部門
            string[] jobtitle = new string[passEmployees.Count];//員工職稱
            string[] email = new string[passEmployees.Count];//員工email
            string[] phone_code = new string[passEmployees.Count];//員工驗證碼(phone_code)


            for (int i = 0; i < passEmployees.Count; i++)//將已審核資料分別存入欄位
            {
                name[i] = passEmployees[i].Name;//員工姓名
                phone[i] = passEmployees[i].Phone;//員工電話
                department[i] = passEmployees[i].Department;//員工部門
                jobtitle[i] = passEmployees[i].JobTitle;//員工職稱
                email[i] = passEmployees[i].Email;//員工email
                phone_code[i] = passEmployees[i].PhoneCode;//員工驗證碼(phone_code)
            }

            //待審核資料
            ViewBag.reivew_companyid = CompanyId;//公司ID
            ViewBag.reivew_name = Name;//員工名稱
            ViewBag.reivew_eamil = Email;//員工電子郵件
            ViewBag.reivew_phonecode = PhoneCode;//員工驗證碼(phone_code)
            ViewBag.reivew_createdtime = CreatedTime;//申請時間

            //審核過資料
            ViewBag.Name = name;//員工名稱
            ViewBag.Phone = phone;//員工電話
            ViewBag.Department = department;//員工部門
            ViewBag.Jobtitle = jobtitle;//員工職稱
            ViewBag.Email = email;//員工電子郵件
            ViewBag.Phone_code = phone_code;//員工驗證碼(phone_code)

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