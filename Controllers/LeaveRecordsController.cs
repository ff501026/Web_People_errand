using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class LeaveRecordsController : Controller
    {
        private string company_hash = Models.HttpResponse.CompanyHash;
        // GET: LeaveRecords
        public async Task<ActionResult> Index()
        {
            //輸入公司代碼取得待審核公出申請紀錄
            List<LeaveRecord> review_leaverecord = await ReviewLeaveRecordModel.Get_ReviewLeaveRecord(company_hash);
            //輸入公司代碼取得已審核公出申請紀錄
            List<LeaveRecord> pass_leaverecord = await PassLeaveRecordModel.Get_PassLeaveRecord(company_hash);

            ViewBag.review_leaverecord = review_leaverecord;//待審核公出申請紀錄
            ViewBag.pass_leaverecord = pass_leaverecord;//待審核公出申請紀錄
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