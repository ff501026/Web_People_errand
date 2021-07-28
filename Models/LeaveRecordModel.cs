using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class ReviewLeaveRecordModel : HttpResponse
    {
        public static async Task<List<LeaveRecord>> Get_ReviewLeaveRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewLeaveRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<LeaveRecord> review_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return review_leaverecord;
        }
    }
    class PassLeaveRecordModel : HttpResponse
    {
        public static async Task<List<LeaveRecord>> Get_PassLeaveRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyPassLeaveRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<LeaveRecord> pass_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return pass_leaverecord;
        }
    }
    public class LeaveRecord
    {
        public string HashAccount { get; set; }//員工編號
        public string Name { get; set; }//員工姓名
        public string LeaveType { get; set; }//假別
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Reason { get; set; }//備註(事由)
        public bool? Review { get; set; }//審核狀態
        public DateTime CreatedTime { get; set; }//申請時間
    }
}