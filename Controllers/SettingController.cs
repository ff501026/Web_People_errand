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
            //取得公司上下班時間(舊版)
            Company_Time company_Times = await CompanyTimeModel.GetCompany_Times(Session["company_hash"].ToString());
            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(Session["company_hash"].ToString());
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(Session["company_hash"].ToString());
            //取得員工上下班時間(新版)
            List<EmployeeWorkTime> employeeWorkTimes = await CompanyWorkTimeModel.Get_EmployeeWorkTime(Session["company_hash"].ToString());


            ViewBag.departments = department;//部門名稱
            ViewBag.jobtitles = jobtitle;//職稱
            ViewBag.company_time = company_Times;//公司上下班時間(舊版)
            ViewBag.company_address = companyAddresses;//公司地址
            ViewBag.general_worktime = GeneralWorktime;//公司一般上下班時間(新版)
            ViewBag.flexible_worktime = FlexibleWorktime;//公司彈性上下班時間(新版)
            ViewBag.employeeWorkTimes = employeeWorkTimes;//員工上下班時間(新版)
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
                    bool result = await CompanyManagerModel.EditCompanyPassword(Session["company_hash"].ToString(), NewPassword);
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
                    bool result = await CompanyManagerModel.EditManagerPassword(Session["hash_account"].ToString(), NewPassword);
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
        public async Task<ActionResult> UpdateEmployeeWorkTime(int n_department, int n_jobtitle, string updateworktime)
        {
            bool result = await CompanyWorkTimeModel.Update_EmployeeWorkTime(n_department, n_jobtitle, updateworktime);
            if(result)
                return Content($"<script>window.location='/Setting/index';</script>");
            else
                return Content($"<script>alert('更新失敗！請重新再試！{n_department},{n_jobtitle},{updateworktime}');history.go(-1);</script>");

        }

        [HttpPost]
        public async Task<ActionResult> DeleteWorktime(string DeleteId)
        {
            if (DeleteId.Substring(0, 1) == "G")
            {
                bool result3 = await CompanyWorkTimeModel.Renew_EmployeeWorkTime(DeleteId, "");
                if (result3)
                {
                    bool result = await CompanyWorkTimeModel.Delete_GeneralWorktime(DeleteId);
                    if (result)
                        return RedirectToAction("index");
                    else
                        return Content($"<script>alert('刪除失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");
                }
                else
                    return Content($"<script>alert('更新失敗！請重新再試！');history.go(-1);</script>");

            }
            else 
            {
                bool result3 = await CompanyWorkTimeModel.Renew_EmployeeWorkTime(DeleteId, "");
                if (result3) 
                {
                    bool result = await CompanyWorkTimeModel.Delete_FlexibleWorktime(DeleteId);
                    if (result)
                        return RedirectToAction("index");
                    else
                        return Content($"<script>alert('刪除失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");

                }
                else
                    return Content($"<script>alert('更新失敗！請重新再試！');history.go(-1);</script>");

                
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddWorktime(string Name, int auditstate, string WorkTime, string RestTime, int? BreakTime, string WorkTimeStart, string WorkTimeEnd, string RestTimeStart, string RestTimeEnd, int? BreakTime2)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (BreakTime == null)
            {
                BreakTime = 0;
            }
            if (BreakTime2 == null)
            {
                BreakTime2 = 0;
            }
            DateTime dt1 = Convert.ToDateTime(WorkTimeStart);
            DateTime dt2 = Convert.ToDateTime(WorkTimeEnd);
            DateTime dt3 = Convert.ToDateTime(RestTimeStart);
            DateTime dt4 = Convert.ToDateTime(RestTimeEnd);
            if (auditstate == 1)
            {
                string result2 = await CompanyWorkTimeModel.Add_GeneralWorktime(Session["company_hash"].ToString(), Name, WorkTime, RestTime, BreakTime);
                if (!result2.Equals(""))
                    return Content($"<script>alert('新增成功！');window.location='/Setting/index';</script>");
                else
                    return Content($"<script>alert('新增失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");
            }
            else 
            {
                if (DateTime.Compare(dt1, dt2) == 0 || DateTime.Compare(dt3, dt4) == 0)
                {
                    return Content($"<script>alert('開始時間不能等於結束時間!');window.location='/Setting/index';</script>");
                }
                string result2 = await CompanyWorkTimeModel.Add_FlexibleWorktime(Session["company_hash"].ToString(), Name, WorkTimeStart, WorkTimeEnd, RestTimeStart, RestTimeEnd, BreakTime2);
                if (!result2.Equals(""))
                    return Content($"<script>alert('新增成功！');window.location='/Setting/index';</script>");
                else
                    return Content($"<script>alert('新增失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");

            }
        }

            [HttpPost]
        public async Task<ActionResult> EditWorktime(string id, string old_state,string Name,string auditstate, string WorkTime,string RestTime,int? BreakTime,string WorkTimeStart,string WorkTimeEnd,string RestTimeStart,string RestTimeEnd,int? BreakTime2)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (BreakTime == null) 
            {
                BreakTime = 0;
            }
            if (BreakTime2 == null)
            {
                BreakTime2 = 0;
            }
            DateTime dt1 = Convert.ToDateTime(WorkTimeStart);
            DateTime dt2 = Convert.ToDateTime(WorkTimeEnd);
            DateTime dt3 = Convert.ToDateTime(RestTimeStart);
            DateTime dt4 = Convert.ToDateTime(RestTimeEnd);

            if (auditstate.Equals(old_state))
            {
                if (old_state.Substring(5, 1).Equals("G"))//更新一般
                {
                    bool result = await CompanyWorkTimeModel.Edit_GeneralWorktime(id, Name, WorkTime, RestTime, BreakTime);
                    if (result)
                        return Content($"<script>alert('更新成功！');window.location='/Setting/index';</script>");
                    else
                        return Content($"<script>alert('更新失敗！如有問題請連繫後台!');window.location='/Setting/index';</script>");
                }
                else //更新彈性
                {
                    if (DateTime.Compare(dt1, dt2) == 0 || DateTime.Compare(dt3, dt4) == 0)
                    {
                        return Content($"<script>alert('開始時間不能等於結束時間!');window.location='/Setting/index';</script>");
                    }
                    bool result = await CompanyWorkTimeModel.Edit_FlexibleWorktime(id, Name, WorkTimeStart, WorkTimeEnd, RestTimeStart, RestTimeEnd, BreakTime2);
                    if (result)
                        return Content($"<script>alert('更新成功！');window.location='/Setting/index';</script>");
                    else
                        return Content($"<script>alert('更新失敗！如有問題請連繫後台!');window.location='/Setting/index';</script>");

                }

            }
            else 
            {
                if (old_state.Substring(5, 1).Equals("G"))//刪除一般新增彈性
                {
                    if (DateTime.Compare(dt1, dt2) == 0 || DateTime.Compare(dt3, dt4) == 0)
                    {
                        return Content($"<script>alert('開始時間不能等於結束時間!');window.location='/Setting/index';</script>");
                    }
                    string new_worktimeid = await CompanyWorkTimeModel.Add_FlexibleWorktime(Session["company_hash"].ToString(), Name, WorkTimeStart, WorkTimeEnd, RestTimeStart, RestTimeEnd, BreakTime2);
                    if (!new_worktimeid.Equals(""))
                    {
                        bool result3 = await CompanyWorkTimeModel.Renew_EmployeeWorkTime(id, new_worktimeid);
                        if (result3) 
                        {
                            bool result = await CompanyWorkTimeModel.Delete_GeneralWorktime(id);
                            if (result)
                                return Content($"<script>alert('更新成功！');window.location='/Setting/index';</script>");
                            else
                                return Content($"<script>alert('更新失敗！如有問題請連繫後台!{Session["company_hash"].ToString()},{Name},{WorkTimeStart},{WorkTimeEnd},{RestTimeStart},{RestTimeEnd},{BreakTime2}');window.location='/Setting/index';</script>");
                        }
                        else
                            return Content($"<script>alert('更新失敗！請重新再試！');history.go(-1);</script>");
                    }
                    else
                        return Content($"<script>alert('更新失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");

                }
                else //刪除彈性新增一般
                {
                    string new_worktimeid = await CompanyWorkTimeModel.Add_GeneralWorktime(Session["company_hash"].ToString(), Name, WorkTime, RestTime, BreakTime);
                    if (!new_worktimeid.Equals(""))
                    {
                        bool result3 = await CompanyWorkTimeModel.Renew_EmployeeWorkTime(id, new_worktimeid);
                        if (result3) 
                        {
                            bool result = await CompanyWorkTimeModel.Delete_FlexibleWorktime(id);
                            if (result)
                                return Content($"<script>alert('更新成功！');window.location='/Setting/index';</script>");
                            else
                                return Content($"<script>alert('更新失敗！如有問題請連繫後台!{Session["company_hash"].ToString()},{Name},{WorkTimeStart},{WorkTimeEnd},{RestTimeStart},{RestTimeEnd},{BreakTime2}');window.location='/Setting/index';</script>");

                        }
                        else
                            return Content($"<script>alert('更新失敗！請重新再試！');history.go(-1);</script>");

                    }
                    else
                        return Content($"<script>alert('更新失敗！如有問題請連繫後台!');window.location='/Setting/index'</script>");

                 }
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
                    return Content($"<script>alert('新增失敗！如有問題請連繫後台!');history.go(-1);</script>");
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