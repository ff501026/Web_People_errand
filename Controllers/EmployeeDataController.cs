using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using Newtonsoft.Json;


namespace AttendanceManagement.Controllers
{
    public class EmployeeDataController : Controller
    {
        // GET: EmployeeData
        public async Task<ActionResult> Index()//員工管理初始頁面
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得待審核資料
            List<ReviewEmployee> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(Session["company_hash"].ToString());
            //輸入公司代碼取得已審核資料(會變動)
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            //輸入公司代碼取得部門資料
            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得職稱資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得全部的已審核資料(不會變動)
            List<PassEmployee> all_passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());

            ViewBag.all_passemployee = all_passEmployees;//全部的已審核資料
            ViewBag.department = departments;//部門名稱
            ViewBag.jobtitle = jobtitles;//職稱
            ViewBag.review_employee = reviewEmployees;//待審核資料   
            ViewBag.pass_employee = passEmployees; //審核過資料
            return View();
        }

        public async Task<ActionResult> Edit(string id)//員工管理修改頁面
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());//已審核資料
            int num = passEmployees.FindIndex(item => item.HashAccount.Equals(id));//員工索引值

            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());//輸入公司代碼取得部門資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());//輸入公司代碼取得職稱資料

            ViewBag.department = departments;//部門名稱
            ViewBag.jobtitle = jobtitles;//職稱
            ViewBag.pass_employee = passEmployees;//待審核資料
            ViewBag.Num = num;//員工索引值  
            return View();
        }
   
        public async Task<ActionResult> Check(string id)//員工管理審核頁面
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            List<ReviewEmployee> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(Session["company_hash"].ToString());//待審核資料
            int num = reviewEmployees.FindIndex(item => item.HashAccount.Equals(id));//員工索引值
            
            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());//輸入公司代碼取得部門資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());//輸入公司代碼取得職稱資料

            ViewBag.department = departments;//部門名稱
            ViewBag.jobtitle = jobtitles;//職稱
            ViewBag.review_employee = reviewEmployees;//待審核資料
            ViewBag.Num = num;//員工索引值   
            return View("Review");
        }

        [HttpPost]//員工管理審核頁面，審核按鈕
        public async Task<ActionResult> SetEmployeeInformation(int num,string id,string Button, string phonecode, string department, string jobtitle)
        {
            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());//輸入公司代碼取得部門資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());//輸入公司代碼取得職稱資料
            int departmentIndex = departments.FindIndex(item => item.Name.Equals(department));//部門索引值
            int jobtitleIndex = jobtitles.FindIndex(item => item.Name.Equals(jobtitle));//職稱索引值

            List<ReviewEmployee> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(Session["company_hash"].ToString());//待審核資料

            bool result = false;

            if (Button.Equals("SaveButton"))
            {
                if (departmentIndex == -1 || jobtitleIndex == -1)
                {
                    return Content("<script>alert('審核失敗，請確認是否有賦予職稱及部門！');history.go(-1);</script>");
                }

                result = await SetEmployeeModel.SetEmployees(id, departments[departmentIndex].DepartmentId, jobtitles[jobtitleIndex].JobTitleId);//PUT部門及職稱

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(reviewEmployees[num].Email, "差勤打卡帳號審核通知","<h1>差勤打卡帳號審核成功</h1><p>請至差勤打卡APP重新進行登入，如有問題請連繫後台。</p>");
                    return RedirectToAction("index");
                }
                else
                    return Content("<script>alert('審核失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            result = await SetEmployeeModel.RejectEmployees(id);//Delete員工

            if (result)
            {
                AttendanceManagement.Models.HttpResponse.sendGmail(reviewEmployees[num].Email, "差勤打卡帳號審核通知", "<h1>差勤打卡帳號審核失敗</h1><p>如有問題請連繫後台。</p>");
                return RedirectToAction("index");
            }
            else
                return Content("<script>alert('審核失敗！如有問題請連繫後台');history.go(-1);</script>");
        }
        [HttpPost]//員工管理修改頁面，修改按鈕
        public async Task<ActionResult> EditEmployee(int num,string Button, string id, string name, string phone, string email, string phonecode, string department, string jobtitle)
        {
            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());//輸入公司代碼取得部門資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());//輸入公司代碼取得職稱資料
            int departmentIndex = departments.FindIndex(item => item.Name.Equals(department));//部門索引值
            int jobtitleIndex = jobtitles.FindIndex(item => item.Name.Equals(jobtitle));//職稱索引值

            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());//已審核資料

            bool result = false;

            if (Button.Equals("RenewButton"))
            {
                if (departmentIndex == -1 || jobtitleIndex == -1)
                {
                    return Content("<script>alert('更新失敗，請確認是否有賦予職稱及部門！');history.go(-1);</script>");
                }

                result = await PassEmployeeModel.RenewEmployees(id,name,phone,email, departments[departmentIndex].DepartmentId, jobtitles[jobtitleIndex].JobTitleId);//(PUT)更新員工資料

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[num].Email, "差勤打卡資料更新通知", "<h1>您的差勤打卡個人資料已更新</h1><p>請至差勤打卡APP個人資料確認更新內容，如有問題請連繫後台。</p>");
                    return RedirectToAction("index");
                }
                else
                    return Content("<script>alert('更新失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            return RedirectToAction("index");
        }
        [HttpPost]//員工管理修改頁面，停用或恢復按鈕
        public async Task<ActionResult> EnabledEmployee(int num,string id, bool Enabled)
        {
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());//已審核資料

            bool result = false;
            result = await PassEmployeeModel.EnabledEmployees(id,(bool)Enabled);//(PUT)更新員工資料

            if (result)
            {
                if (Enabled)
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[num].Email, "差勤打卡帳號狀態更新通知", "<h1>您的差勤打卡帳號已恢復使用權限</h1><p>請至差勤打卡APP確認，如有問題請連繫後台。</p>");

                else
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[num].Email, "差勤打卡帳號狀態更新通知", "<h1>您的差勤打卡帳號已遭停用</h1><p>如有問題請連繫後台。</p>");
                return RedirectToAction("index");
            }
            else
                return Content("<script>alert('狀態更新失敗！如有問題請連繫後台');history.go(-1);</script>");
        }
            [HttpGet]//已審核員工資料篩選
        public async Task<ActionResult> SearchEmployee(string department, string jobtitle, string employee_name)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得待審核資料
            List<ReviewEmployee> reviewEmployees = await ReviewEmployeeModel.ReviewEmployees(Session["company_hash"].ToString());
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> all_passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            //輸入公司代碼取得部門資料
            List<Department> departments = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得職稱資料
            List<JobTitle> jobtitles = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());

            //放入篩選後的已審核資料
            List<PassEmployee> searchEmployees = new List<PassEmployee>();

            int departmentIndex = departments.FindIndex(item => item.Name.Equals(department));//部門索引值
            int jobtitleIndex = jobtitles.FindIndex(item => item.Name.Equals(jobtitle));//職稱索引值
            int num = all_passEmployees.FindIndex(item => item.Name.Equals(employee_name));//員工索引值

            if (departmentIndex == -1 && jobtitleIndex == -1 && num == -1)//沒有輸入篩選條件就按搜尋，顯示全部資料
                return RedirectToAction("index");
            else if(departmentIndex != -1 && jobtitleIndex != -1 && num != -1)//三個篩選條件都輸入
                searchEmployees = await PassEmployeeModel.SearchEmployees3(Session["company_hash"].ToString(), department, jobtitle, employee_name);
            else if ((departmentIndex != -1 && jobtitleIndex != -1) || (departmentIndex!=-1 && num != -1) || (jobtitleIndex!=-1 && num!= -1))//只輸入兩個篩選條件
                searchEmployees = await PassEmployeeModel.SearchEmployees2(Session["company_hash"].ToString(), department, jobtitle, employee_name);
            else searchEmployees = await PassEmployeeModel.SearchEmployees1(Session["company_hash"].ToString(), department, jobtitle, employee_name);//只輸入一個篩選條件

            ViewBag.all_passemployee = all_passEmployees;//全部的已審核資料
            ViewBag.department = departments;//部門名稱
            ViewBag.jobtitle = jobtitles;//職稱
            ViewBag.review_employee = reviewEmployees;//待審核資料   
            ViewBag.pass_employee = searchEmployees; //篩選後的審核過資料
            return View("Index");
        }
    }
}