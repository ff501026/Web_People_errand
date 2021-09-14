using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class AuthoritySettingController : Controller
    {
        // GET: Authority
        public async Task<ActionResult> Index()
        {
            if (Session["hash_account"] != null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司取得全部的管理員
            List<Manager> managers = await CompanyManagerModel.GetAllManager(Session["company_hash"].ToString());
            //輸入公司代碼取得部門資料
            List<Department> department = await DepartmentModel.Get_DepartmentAsync(Session["company_hash"].ToString());
            //輸入公司代碼取得職稱資料
            List<JobTitle> jobtitle = await JobtitleModel.Get_JobtitleAsync(Session["company_hash"].ToString());
            //取得權限
            List<ManagerPermissions> managerPermissions = await CompanyManagerPermissionsModel.Get_ManagerPermissions(Session["company_hash"].ToString());
            //取得權限自訂
            List<ManagerPermissionsCustomizations> customizations = await CompanyManagerPermissionsModel.Get_ManagerPermissionsCustomizations(Session["company_hash"].ToString());
            //取得角色套用權限資料
            List<ManagerAccountPermissions> managerAccountPermissions = await CompanyManagerPermissionsModel.Get_ManagerAccountPermissions(Session["company_hash"].ToString());

            ViewBag.managerAccountPermissions = managerAccountPermissions;//角色權限
            ViewBag.customizations = customizations;//權限自訂
            ViewBag.managerPermissions = managerPermissions;//權限
            ViewBag.departments = department;//部門名稱
            ViewBag.jobtitles = jobtitle;//職稱
            ViewBag.managers = managers;//全部的管理員
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateManagerAccountPermissions(int department_id, int jobtitle_id, int? permission_id)
        {
            if (Session["hash_account"] != null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            bool result = false;

            result = await CompanyManagerPermissionsModel.Edit_ManagerAccountPermissions(Session["company_hash"].ToString(), department_id,jobtitle_id, permission_id);
            if (result)
            {
                return RedirectToAction("index");
            }
            else
                return Content($"<script>alert('變更失敗！如有問題請連繫後台!');history.go(-1);</script>");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteManagerPermissions(int id, int? old_display, int? old_review)
        {
            if (Session["hash_account"] != null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            bool result = false;

            result = await CompanyManagerPermissionsModel.Delete_ManagerPermissions(Session["company_hash"].ToString(), id);
            if (result)
            {
                if (old_display != null)
                {
                    bool deleteresult = await CompanyManagerPermissionsModel.Delete_ManagerPermissionsCustomizations(Session["company_hash"].ToString(), (int)old_display);
                }
                if (old_review != null)
                {
                    bool deleteresult = await CompanyManagerPermissionsModel.Delete_ManagerPermissionsCustomizations(Session["company_hash"].ToString(),(int)old_review);
                }
                return RedirectToAction("index");
            }
            else
                return Content($"<script>alert('刪除失敗！如有問題請連繫後台!');history.go(-1);</script>");
        }

        [HttpPost]
        public async Task<ActionResult> AddManagerPermissions(string[] check3, string[] check4,string name, int employee_display,int employee_review,bool setting_worktime,bool setting_department_jobtitle, bool setting_location)
        {
            if (Session["hash_account"] != null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            int permissions_id = -1;

            List<ManagerPermissionsCustomizations> CustomizationDisplay = new List<ManagerPermissionsCustomizations>();
            List<ManagerPermissionsCustomizations> CustomizationReview = new List<ManagerPermissionsCustomizations>();


            int? display_id = null;
            int? review_id = null;

            if (employee_display == 3)
            {
                if (check3 == null)
                {
                    return Content($"<script>alert('請勾選對象!');history.go(-1);</script>");
                }
                else
                {
                    int[,] display = new int[check3.Length, 2];
                    int wordsindex = 0;
                    foreach (var str in check3)
                    {
                        string[] word = str.Split(' ');
                        for (int i = 0; i < 2; i++)
                        {
                            display[wordsindex, i] = int.Parse(word[i]);
                        }
                        wordsindex++;
                    }

                    for (int i = 0; i < check3.Length; i++)
                    {
                        ManagerPermissionsCustomizations managerPermissionsCustomization = new ManagerPermissionsCustomizations
                        {
                            DepartmentId = display[i, 0],
                            JobtitleId = display[i, 1]
                        };
                        CustomizationDisplay.Add(managerPermissionsCustomization);
                    }

                    display_id = await CompanyManagerPermissionsModel.Add_ManagerPermissionsCustomizations(CustomizationDisplay);
                }
            }//查看對象自訂
            if (employee_review == 3)
            {
                if (check4 == null)
                {
                    return Content($"<script>alert('請勾選對象!');history.go(-1);</script>");
                }
                else
                {
                    int[,] review = new int[check4.Length, 2];

                    int wordsindex = 0;
                    foreach (var str in check4)
                    {
                        string[] word = str.Split(' ');
                        for (int i = 0; i < 2; i++)
                        {
                            review[wordsindex, i] = int.Parse(word[i]);
                        }
                        wordsindex++;
                    }

                    for (int i = 0; i < check4.Length; i++)
                    {
                        ManagerPermissionsCustomizations managerPermissionsCustomization = new ManagerPermissionsCustomizations
                        {
                            DepartmentId = review[i, 0],
                            JobtitleId = review[i, 1]
                        };
                        CustomizationReview.Add(managerPermissionsCustomization);
                    }

                    review_id = await CompanyManagerPermissionsModel.Add_ManagerPermissionsCustomizations(CustomizationReview);
                }
            }//審核對象自訂

            permissions_id = await CompanyManagerPermissionsModel.Add_ManagerPermissions(Session["company_hash"].ToString(), name,employee_display, display_id, employee_review, review_id, setting_worktime, setting_department_jobtitle, setting_location);
            if (permissions_id != -1)
            {
                return Content($"<script>alert('新增成功！');window.location='/AuthoritySetting/index';</script>");
            }
            else
                return Content($"<script>alert('新增失敗！如有問題請連繫後台!');history.go(-1);</script>");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateManagerPermissions(int id,string name, int employee_display, string[] check1,string[] check2,int employee_review, bool setting_worktime, bool setting_department_jobtitle, bool setting_location,int? old_display, int? old_review)
        {
            if (Session["hash_account"] != null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }

            List<ManagerPermissionsCustomizations> CustomizationDisplay = new List<ManagerPermissionsCustomizations>();
            List<ManagerPermissionsCustomizations> CustomizationReview = new List<ManagerPermissionsCustomizations>();


            int? display_id = null;
            int? review_id = null;

            if (employee_display == 3)
            {
                if (check1 == null)
                {
                    return Content($"<script>alert('請勾選對象!');history.go(-1);</script>");
                }
                else 
                {
                    int[,] display = new int[check1.Length, 2];
                    int wordsindex = 0;
                    foreach (var str in check1)
                    {
                        string[] word = str.Split(' ');
                        for (int i = 0; i < 2; i++)
                        {
                            display[wordsindex, i] = int.Parse(word[i]);
                        }
                        wordsindex++;
                    }

                    for (int i = 0; i < check1.Length; i++)
                    {
                        ManagerPermissionsCustomizations managerPermissionsCustomization = new ManagerPermissionsCustomizations
                        {
                            DepartmentId = display[i, 0],
                            JobtitleId = display[i, 1]
                        };
                        CustomizationDisplay.Add(managerPermissionsCustomization);
                    }

                    display_id = await CompanyManagerPermissionsModel.Add_ManagerPermissionsCustomizations(CustomizationDisplay);
                }
            }//查看對象自訂
            if (employee_review == 3)
            {
                if (check2 == null)
                {
                    return Content($"<script>alert('請勾選對象!');history.go(-1);</script>");
                }
                else 
                {
                    int[,] review = new int[check2.Length, 2];

                    int wordsindex = 0;
                    foreach (var str in check2)
                    {
                        string[] word = str.Split(' ');
                        for (int i = 0; i < 2; i++)
                        {
                            review[wordsindex, i] = int.Parse(word[i]);
                        }
                        wordsindex++;
                    }

                    for (int i = 0; i < check2.Length; i++)
                    {
                        ManagerPermissionsCustomizations managerPermissionsCustomization = new ManagerPermissionsCustomizations
                        {
                            DepartmentId = review[i, 0],
                            JobtitleId = review[i, 1]
                        };
                        CustomizationReview.Add(managerPermissionsCustomization);
                    }

                    review_id = await CompanyManagerPermissionsModel.Add_ManagerPermissionsCustomizations(CustomizationReview);
                }
            }//審核對象自訂

            bool result = await CompanyManagerPermissionsModel.Edit_ManagerPermissions(Session["company_hash"].ToString(),id, name, employee_display, display_id, employee_review, review_id, setting_worktime, setting_department_jobtitle, setting_location);
            if (result)
            {
                if (old_display != null)
                {
                    bool deleteresult = await CompanyManagerPermissionsModel.Delete_ManagerPermissionsCustomizations(Session["company_hash"].ToString(),(int)old_display);
                }
                if (old_review != null)
                {
                    bool deleteresult = await CompanyManagerPermissionsModel.Delete_ManagerPermissionsCustomizations(Session["company_hash"].ToString(),(int)old_review);
                }
                return RedirectToAction("index");
            }
            else
                return Content($"<script>alert('編輯失敗！如有問題請連繫後台!');history.go(-1);</script>");
        }
    }
}