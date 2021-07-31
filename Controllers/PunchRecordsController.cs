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

        [HttpGet]//打卡資料篩選
        public async Task<ActionResult> SearchWorkRecord(DateTime? date, string employee_name)
        {
            //輸入公司代碼取得打卡紀錄
            List<Work_Record> all_work_Records = await StaffModel.Get_WorkRecordAsync(company_hash);

            //放入篩選後的已審核資料
            List<Work_Record> search_work_Records = new List<Work_Record>();

            if (date.Equals(null) && employee_name.Equals(""))//沒有輸入篩選條件就按搜尋，顯示全部資料
                return Redirect("/PunchRecords/Index");
            else if (!date.Equals(null) && !employee_name.Equals(""))//兩個篩選條件都輸入
                search_work_Records = await StaffModel.Search_WorkRecord2(company_hash, date, employee_name);
            else search_work_Records = await StaffModel.Search_WorkRecord1(company_hash, date, employee_name);//只輸入一個篩選條件

            ViewBag.workrecord = search_work_Records;//篩選後打卡紀錄

            return View("Index");
        }
    }
}