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
        // GET: TripRecords
        public async Task<ActionResult> Index()
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(Session["company_hash"].ToString());
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(Session["company_hash"].ToString());
            //輸入公司代碼取得公出2申請紀錄
            List<Trip2Record> trip2record = await Trip2RecordModel.Get_Trip2Record(Session["company_hash"].ToString());
            //輸入公司代碼取得詳細公出2申請紀錄
            List<DetailTrip2Record> detailtrip2record = await Trip2RecordModel.Detail_Trip2Record(Session["company_hash"].ToString());
            //取得權限
            List<ManagerPermissions> managerPermissions = await CompanyManagerPermissionsModel.Get_ManagerPermissions(Session["company_hash"].ToString());

            if (Session["hash_account"] != null)
            {
                //輸入公司取得全部的管理員
                List<Manager> managers = await CompanyManagerModel.GetAllManager(Session["company_hash"].ToString());
                int index = managers.FindIndex(item => item.ManagerHash.Equals(Session["hash_account"].ToString()));
                if (managers[index].PermissionsId == null) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 1) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 2)
                {
                    trip2record = await Trip2RecordModel.Manager_Get_Trip2Record2(Session["hash_account"].ToString());
                }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 3)
                {
                    trip2record = await Trip2RecordModel.Manager_Get_Trip2Record3(Session["hash_account"].ToString());
                }
                else
                {
                    if (await CompanyManagerPermissionsModel.Manager_Bool_Agent(Session["hash_account"].ToString()))
                    {
                        int bossindex = managers.FindIndex(item => item.AgentHash.Equals(Session["hash_account"].ToString()));

                        if (managers[bossindex].PermissionsId == null) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 1) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 2)
                        {
                            trip2record = await Trip2RecordModel.Manager_Get_Trip2Record2(managers[bossindex].ManagerHash);
                        }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 3)
                        {
                            trip2record = await Trip2RecordModel.Manager_Get_Trip2Record3(managers[bossindex].ManagerHash);
                        }
                        else
                        {
                            trip2record = await Trip2RecordModel.Get_Trip2Record("n");
                        }
                    }
                    else
                    {
                        trip2record = await Trip2RecordModel.Get_Trip2Record("n");
                    }
                    
                }
                
            }

            ViewBag.review_triprecord = review_triprecord;//待審核公出申請紀錄
            ViewBag.pass_triprecord = pass_triprecord;//已審核公出申請紀錄
            ViewBag.trip2record = trip2record;//公出紀錄2
            ViewBag.detailtrip2record = detailtrip2record;//詳細公出紀錄2

            return View();
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(Session["company_hash"].ToString());
            int num = pass_triprecord.FindIndex(item => item.TripRecordId == id);//員工索引值

            ViewBag.pass_tripRecord = pass_triprecord;//待審核資料
            ViewBag.Num = num;//員工索引值
            return View("Edit");
        }

        public async Task<ActionResult> Check(int id)//員工管理審核頁面
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(Session["company_hash"].ToString());
            int num = review_triprecord.FindIndex(item => item.TripRecordId==id);//員工索引值

            ViewBag.review_tripRecord = review_triprecord;//待審核資料
            ViewBag.Num = num;//員工索引值   
            return View("Check");
        }

        [HttpPost]//公差審核頁面，審核按鈕
        public async Task<ActionResult> ReviewTripRecord(int num,int id,string Button)
        {

            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(Session["company_hash"].ToString());
            string hashaccount = review_triprecord[num].HashAccount;//員工編號

            //輸入公司代碼取得已審核員工資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            int Index = passEmployees.FindIndex(item => item.HashAccount.Equals(hashaccount)); ;//員工索引值，用來找出員工EMAIL

            bool result = false;

            if (Button.Equals("SaveButton"))
            {
                result = await Review_TripRecordModel.ReviewTripRecord(id,true);//PUT更新審核狀態

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[Index].Email, "差勤打卡公差審核通知", "<h1>差勤打卡公差審核成功</h1><p>請至差勤打卡APP公差紀錄進行確認，如有問題請連繫後台。</p>");
                    return RedirectToAction("index");
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
                    return RedirectToAction("index");
                }
                else
                    return Content("<script>alert('審核失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            
        }

        [HttpGet]//公差紀錄篩選
        public async Task<ActionResult> SearchTripRecord(string employee_name,DateTime? date)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得待審核公出申請紀錄
            List<TripRecord> review_triprecord = await ReviewTripRecordModel.Get_ReviewTripRecord(Session["company_hash"].ToString());
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> all_pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(Session["company_hash"].ToString());
            //輸入公司代碼取得公出2申請紀錄
            List<Trip2Record> trip2record = await Trip2RecordModel.Get_Trip2Record(Session["company_hash"].ToString());
            //輸入公司代碼取得詳細公出2申請紀錄
            List<DetailTrip2Record> detailtrip2record = await Trip2RecordModel.Detail_Trip2Record(Session["company_hash"].ToString());


            //公出記錄第二版的篩選
            //放入篩選後的已審核資料
            List<Trip2Record> search_trip2record = new List<Trip2Record>();
            if (date.Equals(null) && employee_name.Equals(""))//沒有輸入篩選條件就按搜尋，顯示全部資料
                 return RedirectToAction("index");
            else if (!date.Equals(null) && !employee_name.Equals(""))//兩個篩選條件都輸入
                 search_trip2record = await Trip2RecordModel.Search_Trip2Record2(Session["company_hash"].ToString(), date, employee_name);
            else search_trip2record = await Trip2RecordModel.Search_Trip2Record1(Session["company_hash"].ToString(), date, employee_name);//只輸入一個篩選條件
            
            //公出紀錄第一版的篩選
            ////放入篩選後的已審核資料
            //List<TripRecord> search_triprecord = new List<TripRecord>();
            //if (date.Equals(null) && employee_name.Equals(""))//沒有輸入篩選條件就按搜尋，顯示全部資料
            //    return RedirectToAction("index");
            //else if (!date.Equals(null) && !employee_name.Equals(""))//兩個篩選條件都輸入
            //    search_triprecord = await PassTripRecordModel.Search_TripRecord2(Session["company_hash"].ToString(), date, employee_name);
            //else search_triprecord = await PassTripRecordModel.Search_TripRecord1(Session["company_hash"].ToString(), date, employee_name);//只輸入一個篩選條件

             ViewBag.review_triprecord = review_triprecord;//待審核公出申請紀錄
            //ViewBag.pass_triprecord = search_triprecord;//待審核公出申請紀錄//公出記錄第一版的篩選
            ViewBag.trip2record = search_trip2record;//待審核公出申請紀錄
            ViewBag.detailtrip2record = detailtrip2record;//詳細公出紀錄2

            return View("Index");
        }
        [HttpPost]//公出管理改頁面，修改按鈕
        public async Task<ActionResult> EditTripRecord(int num,int id, string Button, bool? review, DateTime startdate,DateTime enddate, string location,string reason)
        {

            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(Session["company_hash"].ToString());

            string hashaccount = pass_triprecord[num].HashAccount;//員工編號

            //輸入公司代碼取得已審核員工資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            int Index = passEmployees.FindIndex(item => item.HashAccount.Equals(hashaccount)); ;//員工索引值，用來找出員工EMAIL
            bool result = false;

            if (Button.Equals("RenewButton"))
            {

                result = await PassTripRecordModel.RenewTripRecord(id, startdate, enddate, location, reason, (bool)review);//PUT更新審核狀態

                if (result)
                {
                    AttendanceManagement.Models.HttpResponse.sendGmail(passEmployees[Index].Email, "差勤打卡公差紀錄變更通知", "<h1>您的公差紀錄已變更</h1><p>請至差勤打卡APP公差紀錄確認變更內容，如有問題請連繫後台。</p>");
                    return RedirectToAction("index");
                }
                else
                    return Content("<script>alert('更新失敗！如有問題請連繫後台');history.go(-1);</script>");
            }
            //string url = "/TripRecords/Index?id=" + id + "&startdate=" + startdate + "&enddate=" + enddate + "location=" + location + "reason=" + reason;
            return RedirectToAction("index");
        }
    }
}