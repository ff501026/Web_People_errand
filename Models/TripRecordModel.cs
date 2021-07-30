using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class ReviewTripRecordModel : HttpResponse
    {
        public static async Task<List<TripRecord>> Get_ReviewTripRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewTripRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<TripRecord> review_triprecord = JsonConvert.DeserializeObject<List<TripRecord>>(GetResponse);

            return review_triprecord;
        }
    }
    class PassTripRecordModel : HttpResponse
    {
        public static async Task<List<TripRecord>> Get_PassTripRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyPassTripRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<TripRecord> pass_triprecord = JsonConvert.DeserializeObject<List<TripRecord>>(GetResponse);

            return pass_triprecord;
        }
        public static async Task<bool> RenewTripRecord(int id, DateTime startdate, DateTime enddate, string location, string reason, bool review)//更新公差資料
        {
            List<EditTripRecord> EditTripRecords = new List<EditTripRecord>();
            EditTripRecord editTripRecord = new EditTripRecord()//要寫進LIST的資料
            {
                TripRecordsId = id,//員工編碼
                StartDate = startdate,//開始時間
                EndDate = enddate,//結束時間
                Location = location,//地點
                Reason = reason,//備註(事由)
                Review = review//審核狀態
            };
            EditTripRecords.Add(editTripRecord);

            try
            {
                string jsonData = JsonConvert.SerializeObject(EditTripRecords);//序列化成JSON
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                response = await client.PutAsync(url + EmployeeEditTripRecord, content);
                if (response.StatusCode.ToString().Equals("OK"))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        public static async Task<List<TripRecord>> Search_TripRecord2(string company_hash, DateTime? date, string name)//兩條件篩選
        {
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> all_pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);
            List<TripRecord> search_TripRecord = new List<TripRecord>();
            for (int index = 0; index < all_pass_triprecord.Count; index++)
            {
                if (all_pass_triprecord[index].Name.Equals(name) && all_pass_triprecord[index].StartDate.Date.Equals(date))
                    search_TripRecord.Add(ListAddSearch(all_pass_triprecord, index));
            }

            return search_TripRecord;
        }//兩條件篩選
        public static async Task<List<TripRecord>> Search_TripRecord1(string company_hash, DateTime? date, string name)//一條件篩選
        {
            //輸入公司代碼取得已審核公出申請紀錄
            List<TripRecord> all_pass_triprecord = await PassTripRecordModel.Get_PassTripRecord(company_hash);
            List<TripRecord> search_TripRecord = new List<TripRecord>();
            for (int index = 0; index < all_pass_triprecord.Count; index++)
            {
                if (all_pass_triprecord[index].Name.Equals(name) || all_pass_triprecord[index].StartDate.Date.Equals(date))
                    search_TripRecord.Add(ListAddSearch(all_pass_triprecord, index));
            }

            return search_TripRecord;
        }//一條件篩選
        public static TripRecord ListAddSearch(List<TripRecord> tripRecords, int index)
        {
            TripRecord search = new TripRecord()
            {
               TripRecordId = tripRecords[index].TripRecordId,//公差編號
               HashAccount = tripRecords[index].HashAccount,//員工編號
               Name = tripRecords[index].Name,//員工姓名
               StartDate = tripRecords[index].StartDate,//開始時間
               EndDate = tripRecords[index].EndDate,//結束時間
               Location = tripRecords[index].Location,//地點
               Reason = tripRecords[index].Reason,//備註(事由)
               Review = tripRecords[index].Review,//審核狀態
               CreatedTime = tripRecords[index].CreatedTime//申請時間
            };
            return search;
        }
    }
    class Review_TripRecordModel : HttpResponse
    {

        public static async Task<bool> ReviewTripRecord(int id, bool review)//賦予職稱及部門
        {
            List<ReviewTripRecord> reviewTripRecord = new List<ReviewTripRecord>();
            ReviewTripRecord reviewTrip = new ReviewTripRecord()//要寫進LIST的資料
            {
                TripRecordsId = id,
                Review = review
            };
            reviewTripRecord.Add(reviewTrip);

            try
            {
                string jsonData = JsonConvert.SerializeObject(reviewTripRecord);//序列化成JSON
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                response = await client.PutAsync(url + EmployeeReviewTripRecord, content);
                if (response.StatusCode.ToString().Equals("OK"))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
       
    }//審核公差資料
    public class TripRecord //公差資料
    {
        public int TripRecordId { get; set; }//公差編號
        public string HashAccount { get; set; }//員工編號
        public string Name { get; set; }//員工姓名
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Location { get; set; }//地點
        public string Reason { get; set; }//備註(事由)
        public bool? Review { get; set; }//審核狀態
        public DateTime CreatedTime { get; set; }//申請時間
    }
    public class EditTripRecord //編輯公差資料
    {
        public int TripRecordsId { get; set; }//公差編號
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Location { get; set; }//地點
        public string Reason { get; set; }//備註(事由)
        public bool? Review { get; set; }//審核狀態
    }
    public class ReviewTripRecord //審核用
    {
        public int TripRecordsId { get; set; }
        public bool Review { get; set; }
    }
}