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
        // GET: Setting
        public async Task<ActionResult> Index()
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得部門資料
            List<Department> department = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得職稱資料
            List<JobTitle> jobtitle = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得地址
            List<CompanyAddress> companyAddresses = await CompanyAddressModel.Get_CompanyAddress(Session["company_hash"].ToString());
            //取得公司上下班時間
            Company_Time company_Times = await CompanyTimeModel.GetCompany_Times(Session["company_hash"].ToString());

            ViewBag.departments = department;//部門名稱
            ViewBag.jobtitles = jobtitle;//職稱
            ViewBag.company_time = company_Times;
            ViewBag.company_address = companyAddresses;

            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeCompanyPassword(string NewPassword, string NewPassword2)
        {
            if (Session["hash_account"]==null)
            {
                if (NewPassword.Equals(NewPassword2))
                {
                    bool result = await CompanyManagerPasswordModel.EditCompanyManagerPassword(Session["company_hash"].ToString(), NewPassword);
                    if (result)
                        return Content($"<script>alert('公司密碼更新成功！');window.location='index';</script>");
                    else
                        return Content($"<script>alert('公司密碼更新失敗！如有問題請連繫後台!');history.go(-1);</script>");
                }
                else
                    return Content($"<script>alert('密碼輸入錯誤！請重新再試！');history.go(-1);</script>");
            }
            else 
            {
                if (NewPassword.Equals(NewPassword2))
                {
                    bool result = await CompanyManagerPasswordModel.EditManagerPassword(Session["hash_account"].ToString(), NewPassword);
                    if (result)
                        return Content($"<script>alert('管理員密碼更新成功！');window.location='index';</script>");
                    else
                        return Content($"<script>alert('管理員密碼更新失敗！如有問題請連繫後台!');history.go(-1);</script>");
                }
                else
                    return Content($"<script>alert('密碼輸入錯誤！請重新再試！');history.go(-1);</script>");
            }
            
        }

        [HttpPost]
        public async Task<ActionResult> EditAddress(string address) 
        {
            if (!address.Equals(""))
            {
                bool result = await CompanyAddressModel.EditCompanyAddress(Session["company_hash"].ToString(), address);
                if (result)
                    return Content($"<script>alert('更新成功！');window.location='index';</script>");
                else
                    return Content($"<script>alert('更新失敗！如有問題請連繫後台!{address}');history.go(-1);</script>");
            }
            else
                return Content($"<script>alert('密碼輸入錯誤！請重新再試！');history.go(-1);</script>");

        }

        [HttpPost]
        public async Task<ActionResult> UpdateCompanyTime(TimeSpan? WorkTime, TimeSpan? RestTime) 
        {
            bool result = await CompanyTimeModel.Edit_CompanyTime(Session["company_hash"].ToString(), WorkTime, RestTime);
            if (result)
                return Content($"<script>alert('更新成功！');window.location='index';</script>");
            else
                return Content($"<script>alert('更新失敗！請確認上下班時間是否均有填寫，如有問題請連繫後台!');history.go(-1);</script>");
        }

        [HttpPost]
        public async Task<ActionResult> AddDepartment(string department_name, string Button)
        {
            bool result = false;
            if (Button.Equals("AddButton"))
            {
                result = await DepartmentModel.Add_Department(Session["company_hash"].ToString(), department_name);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('新增失敗！如有問題請連繫後台!');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<ActionResult> AddJobtitle(string jobtitle_name, string Button)
        {
            bool result = false;
            if (Button.Equals("AddButton"))
            {
                result = await JobtitleModel.Add_Jobtitle(Session["company_hash"].ToString(), jobtitle_name);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('新增失敗！如有問題請連繫後台!');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }


        [HttpPost]
        public async Task<ActionResult> EditDepartment(int id,string department_name, string Button)
        {
            bool result = false;
            if (Button.Equals("EditButton"))
            {
                result = await DepartmentModel.Edit_Department(id,department_name);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('編輯失敗！如有問題請連繫後台!{department_name}');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }
       


        [HttpPost]
        public async Task<ActionResult> EditJobtitle( int id,string jobtitle_name, string Button)
        {
            bool result = false;
            if (Button.Equals("EditButton"))
            {
                result = await JobtitleModel.Edit_Jobtitle(id, jobtitle_name);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('編輯失敗！如有問題請連繫後台!');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteJobtitle(int id,string Button)
        {
            bool result = false;
            if (Button.Equals("DeleteButton"))
            {
                result = await JobtitleModel.Delete_Jobtitle(id);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('刪除失敗！如有問題請連繫後台!');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<ActionResult> DeleteDepartment(int id, string Button)
        {
            bool result = false;
            if (Button.Equals("DeleteButton"))
            {
                result = await DepartmentModel.Delete_Department(id);
                if (result)
                {
                    return RedirectToAction("index");
                }
                else
                    return Content($"<script>alert('刪除失敗！如有問題請連繫後台!');history.go(-1);</script>");
            }
            else
                return RedirectToAction("index");
        }
    }
}