using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class PasswordSettingController : Controller
    {
        // GET: PasswordSetting
        public async Task<ActionResult> Index(string key)
        {
            List<ManagerKeyData> managerKeyData = await CompanyManagerModel.ManagerKeyGetEmployee(key);

            ViewBag.ManagerKeyData = managerKeyData;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ManagerPassword(string id, string manager_password, string manager_password2)
        {
            bool ExistManager = await CompanyManagerModel.BoolManager(id);
            if (manager_password.Equals(manager_password2))
            {
                if (ExistManager)
                {
                    bool result = await CompanyManagerModel.EditManagerPassword(id, manager_password);
                    if (result)
                        return Content($"<script>alert('管理員密碼更新成功！');window.location='/Account/index';</script>");
                    else
                        return Content($"<script>alert('管理員密碼更新失敗！如有問題請連繫後台!');history.go(-1);</script>");
                }
                else
                {
                    bool result = await CompanyManagerModel.AddManager(id, manager_password);
                    if (result)
                        return Content($"<script>alert('管理員註冊成功！');window.location='/Account/index';</script>");
                    else
                        return Content($"<script>alert('管理員註冊失敗！如有問題請連繫後台!');history.go(-1);</script>");
                }
            }
            else
                return Content($"<script>alert('密碼輸入錯誤！請重新再試！');history.go(-1);</script>");
        }//密碼送出按鈕
    }
}