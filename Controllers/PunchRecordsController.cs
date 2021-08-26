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
        
        // GET: PunchRecords
        public async Task<ActionResult> Index()
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得打卡紀錄
            List<Work_Record> work_record = await StaffModel.Get_WorkRecordAsync(Session["company_hash"].ToString());
            //取得公司上下班時間
            Company_Time company_Times = await CompanyTimeModel.GetCompany_Times(Session["company_hash"].ToString());
            int num=0;
            foreach (var work in work_record) 
            {
                num++;
            }
            ViewBag.Num = num;//打卡記錄總共有幾筆
            ViewBag.workrecord = work_record;//打卡紀錄
            ViewBag.company_time = company_Times;
            return View("Index");
        }

        [HttpGet]//打卡資料篩選
        public async Task<ActionResult> SearchWorkRecord(DateTime? date, string employee_name)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }

            //輸入公司代碼取得打卡紀錄
            List<Work_Record> all_work_Records = await StaffModel.Get_WorkRecordAsync(Session["company_hash"].ToString());

            //放入篩選後的已審核資料
            List<Work_Record> search_work_Records = new List<Work_Record>();
            //取得公司上下班時間
            Company_Time company_Times = await CompanyTimeModel.GetCompany_Times(Session["company_hash"].ToString());

            if (date.Equals(null) && employee_name.Equals(""))//沒有輸入篩選條件就按搜尋，顯示全部資料
                return RedirectToAction("index");
            else if (!date.Equals(null) && !employee_name.Equals(""))//兩個篩選條件都輸入
                search_work_Records = await StaffModel.Search_WorkRecord2(Session["company_hash"].ToString(), date, employee_name);
            else search_work_Records = await StaffModel.Search_WorkRecord1(Session["company_hash"].ToString(), date, employee_name);//只輸入一個篩選條件

            int num = 0;
            foreach (var work in search_work_Records)
            {
                num++;
            }
            ViewBag.Num = num;//打卡記錄總共有幾筆
            ViewBag.workrecord = search_work_Records;//篩選後打卡紀錄
            ViewBag.company_time = company_Times;
            return View("Index");
        }
    }
}