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
            List<ReviewTripRecordModel> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(company_hash);
            string[] Name = new string[review_triprecord.Count];//員工姓名
            DateTime[] StartDate = new DateTime[review_triprecord.Count];//開始時間
            DateTime[] EndDate = new DateTime[review_triprecord.Count];//結束時間
            string[] Location = new string[review_triprecord.Count];//地點
            string[] Reason = new string[review_triprecord.Count];//備註(事由)
            DateTime[] CreatedTime = new DateTime[review_triprecord.Count];//申請時間

            for (int i = 0; i < review_triprecord.Count; i++)//將打卡紀錄分別存入欄位
            {
                Name[i] = review_triprecord[i].Name;
                StartDate[i] = review_triprecord[i].StartDate;
                EndDate[i] = review_triprecord[i].EndDate;
                Location[i] = review_triprecord[i].Location;
                Reason[i] = review_triprecord[i].Reason;
                CreatedTime[i] = review_triprecord[i].CreatedTime;

            }

            ViewBag.name = Name;//員工名稱
            ViewBag.startdate = StartDate;//開始時間
            ViewBag.enddate = EndDate;//結束時間
            ViewBag.location = Location;//地點
            ViewBag.reason = Reason;//備註(事由)
            ViewBag.createtime = CreatedTime;//申請時間

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