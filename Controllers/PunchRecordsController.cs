using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class PunchRecordsController : Controller
    {
        private string company_hash = Models.HttpResponse.CompanyHash;
        // GET: PunchRecords
        public async Task<ActionResult> Index()
        {

            //輸入公司代碼取得打卡紀錄
            List<Work_Record> work_record = await StaffModel.Get_WorkRecordAsync(company_hash);

            ViewBag.workrecord = work_record;//打卡紀錄

            return View();
        }
    }
}