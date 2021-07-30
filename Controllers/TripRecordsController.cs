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

        public async Task<ActionResult> Edit(int id)
        {
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);
            int num = pass_triprecord.FindIndex(item => item.TripRecordId == id);//員工索引值

            ViewBag.pass_tripRecord = pass_triprecord;//待審核資料
            ViewBag.Num = num;//員工索引值
            return View("Edit");
        }

        public async Task<ActionResult> Check(int id)//員工管理審核頁面
        {
            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(company_hash);
            int num = review_triprecord.FindIndex(item => item.TripRecordId==id);//員工索引值

            ViewBag.review_tripRecord = review_triprecord;//待審核資料
            ViewBag.Num = num;//員工索引值   
            return View("Check");
        }

        [HttpPost]//公差審核頁面，審核按鈕
        public async Task<ActionResult> ReviewTripRecord(int id,string Button)
        {

            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(company_hash);
            int num = review_triprecord.FindIndex(item => item.TripRecordId == id); ;//員工索引值，用來找出員工編號
            string hashaccount = review_triprecord[num].HashAccount;//員工編號

            //輸入公司代碼取得已審核員工資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            int Index = passEmployees.FindIndex(item => item.HashAccount.Equals(hashaccount)); ;//員工索引值，用來找出員工EMAIL

            bool result = false;

            if (Button.Equals("SaveButton"))
            {
                result = await Review_TripRecordModel.ReviewTripRecord(id,true);//PUT更新審核狀態

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[Index].Email, "差勤打卡公差審核通知", "<h1>差勤打卡公差審核成功</h1><p>請至差勤打卡APP公差紀錄進行確認，如有問題請連繫後台。</p>");
                    return Redirect("/TripRecords/Index");
                }
                else
                    return Content("<script>alert('審核失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            else
            {
                result = await Review_TripRecordModel.ReviewTripRecord(id,false);//PUT更新審核狀態

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[Index].Email, "差勤打卡公差審核通知", "<h1>差勤打卡公差審核失敗</h1><p>請至差勤打卡APP公差紀錄進行確認，如有問題請連繫後台。</p>");
                    return Redirect("/TripRecords/Index");
                }
                else
                    return Content("<script>alert('審核失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            
        }

        [HttpGet]//公差紀錄篩選
        public async Task<ActionResult> SearchTripRecord(string employee_name,DateTime? date)
        {

            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(company_hash);
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> all_pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);
            //放入篩選後的已審核資料
            List<TripRecord> search_triprecord = new List<TripRecord>();

            if (date.Equals(null) && employee_name.Equals(""))//沒有輸入篩選條件就按搜尋，顯示全部資料
                return Redirect("/TripRecords/Index");
            else if (!date.Equals(null) && !employee_name.Equals(""))//兩個篩選條件都輸入
                search_triprecord = await PassTripRecordModel.Search_TripRecord2(company_hash, date, employee_name);
            else search_triprecord = await PassTripRecordModel.Search_TripRecord1(company_hash, date, employee_name);//只輸入一個篩選條件

            ViewBag.review_triprecord = review_triprecord;//待審核公出申請紀錄
            ViewBag.pass_triprecord = search_triprecord;//待審核公出申請紀錄

            return View("Index");
        }
        [HttpPost]//員工管理修改頁面，修改按鈕
        public async Task<ActionResult> EditTripRecord(int id, string Button, bool? review, DateTime startdate,DateTime enddate, string location,string reason)
        {

            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);
            int num = pass_triprecord.FindIndex(item => item.TripRecordId == id); ;//員工索引值，用來找出員工編號
            string hashaccount = pass_triprecord[num].HashAccount;//員工編號

            //輸入公司代碼取得已審核員工資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            int Index = passEmployees.FindIndex(item => item.HashAccount.Equals(hashaccount)); ;//員工索引值，用來找出員工EMAIL
            bool result = false;

            if (Button.Equals("RenewButton"))
            {

                result = await PassTripRecordModel.RenewTripRecord(id, startdate, enddate, location, reason, (bool)review);//PUT更新審核狀態

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[num].Email, "差勤打卡公差紀錄變更通知", "<h1>您的公差紀錄已變更</h1><p>請至差勤打卡APP公差紀錄確認變更內容，如有問題請連繫後台。</p>");
                    return Redirect("/TripRecords/Index");
                }
                else
                    return Content("<script>alert('更新失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            //string url = "/TripRecords/Index?id=" + id + "&startdate=" + startdate + "&enddate=" + enddate + "location=" + location + "reason=" + reason;
            return Redirect("/TripRecords/Index");
        }
    }
}