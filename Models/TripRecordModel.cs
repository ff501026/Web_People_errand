using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static async Task<List<PassTripRecord>> Get_PassTripRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyPassTripRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<PassTripRecord> pass_triprecord = JsonConvert.DeserializeObject<List<PassTripRecord>>(GetResponse);

            return pass_triprecord;
        }
    }
    public class TripRecord 
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
    public class PassTripRecord
    {
        public int TripRecordId { get; set; }//公差編號
        public string HashAccount { get; set; }//員工編號
        public string Name { get; set; }//員工姓名
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Location { get; set; }//地點
        public string Reason { get; set; }//備註(事由)
        public bool Review { get; set; }//審核狀態
        public DateTime CreatedTime { get; set; }//申請時間
    }
}