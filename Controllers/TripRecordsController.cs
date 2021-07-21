using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class TripRecordsController : Controller
    {
        private string company_hash = Models.HttpResponse.CompanyHash;
        // GET: TripRecords
        public async Task<ActionResult> Index()
        {

            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(company_hash);
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);

            ViewBag.review_triprecord = review_triprecord;//待審核公出申請紀錄
            ViewBag.pass_triprecord = pass_triprecord;//待審核公出申請紀錄

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