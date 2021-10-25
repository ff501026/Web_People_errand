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
    class ReviewLeaveRecordModel : HttpResponse
    {
        public static async Task<List<LeaveRecord>> Get_ReviewLeaveRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewLeaveRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyReviewLeaveRecord + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<LeaveRecord> review_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return review_leaverecord;
        }
        public static async Task<List<LeaveRecord>> Manager_Get_ReviewLeaveRecord(string company_hash,string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetReviewLeaveRecord + company_hash+ "&hash_account="+hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerGetReviewLeaveRecord + company_hash + "&hash_account=" + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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
            bool resultlog = await LogModel.Add_Log($"{url + CompanyPassLeaveRecord + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<LeaveRecord> pass_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return pass_leaverecord;
        }
        public static async Task<List<LeaveRecord>> Manager_Get_PassLeaveRecord2(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetPassLeaveRecord2 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerGetPassLeaveRecord2 + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<LeaveRecord> pass_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return pass_leaverecord;
        }
        public static async Task<List<LeaveRecord>> Manager_Get_PassLeaveRecord3(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetPassLeaveRecord3 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerGetPassLeaveRecord3 + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<LeaveRecord> pass_leaverecord = JsonConvert.DeserializeObject<List<LeaveRecord>>(GetResponse);

            return pass_leaverecord;
        }
        public static async Task<List<LeaveRecord>> Search_LeaveRecord2(List<LeaveRecord> all_pass_leaverecord, string company_hash, DateTime? date, string name)//兩條件篩選
        {
            List<LeaveRecord> search_LeaveRecord = new List<LeaveRecord>();
            for (int index = 0; index < all_pass_leaverecord.Count; index++)
            {
                if (all_pass_leaverecord[index].Name.Equals(name) && all_pass_leaverecord[index].StartDate.Date.Equals(date))
                    search_LeaveRecord.Add(ListAddSearch(all_pass_leaverecord, index));
            }

            return search_LeaveRecord;
        }//兩條件篩選
        public static async Task<List<LeaveRecord>> Search_LeaveRecord1(List<LeaveRecord> all_pass_leaverecord, string company_hash, DateTime? date, string name)//一條件篩選
        {
            List<LeaveRecord> search_LeaveRecord = new List<LeaveRecord>();
            for (int index = 0; index < all_pass_leaverecord.Count; index++)
            {
                if (all_pass_leaverecord[index].Name.Equals(name) || all_pass_leaverecord[index].StartDate.Date.Equals(date))
                    search_LeaveRecord.Add(ListAddSearch(all_pass_leaverecord, index));
            }

            return search_LeaveRecord;
        }//一條件篩選
        public static LeaveRecord ListAddSearch(List<LeaveRecord> leaveRecords, int index)
        {
            LeaveRecord search = new LeaveRecord()
            {
                LeaveRecordId = leaveRecords[index].LeaveRecordId,//請假編號
                HashAccount = leaveRecords[index].HashAccount,//員工編號
                Name = leaveRecords[index].Name,//員工姓名
                LeaveType = leaveRecords[index].LeaveType,//假別
                StartDate = leaveRecords[index].StartDate,//開始時間
                EndDate = leaveRecords[index].EndDate,//結束時間
                Reason = leaveRecords[index].Reason,//備註(事由)
                Review = leaveRecords[index].Review,//審核狀態
                CreatedTime = leaveRecords[index].CreatedTime//申請時間
            };
            return search;
        }
    }
    class Review_LeaveRecordModel : HttpResponse
    {

        public static async Task<bool> ReviewLeaveRecord(int id, bool review)//賦予職稱及部門
        {
            List<ReviewLeaveRecord> reviewLeaveRecords = new List<ReviewLeaveRecord>();
            ReviewLeaveRecord reviewLeave = new ReviewLeaveRecord()//要寫進LIST的資料
            {
                LeaveRecordsId = id,
                Review = review
            };
            reviewLeaveRecords.Add(reviewLeave);

            string jsonData = JsonConvert.SerializeObject(reviewLeaveRecords);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + EmployeeReviewLeaveRecord, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + EmployeeReviewLeaveRecord}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }

    }
    class LeaveTypeModel : HttpResponse
    {
        public static async Task<List<LeaveType>> Get_LeaveType(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyLeaveType);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyLeaveType}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<LeaveType> leaveTypes = JsonConvert.DeserializeObject<List<LeaveType>>(GetResponse);

            return leaveTypes;
        }


    }//假別
    public class LeaveType
    {
        public int LeaveTypeID { get; set; }//編號
        public string Name { get; set; }//部門名稱
    }//部門
    public class LeaveRecord
    {
        public int LeaveRecordId { get; set; }//請假編號
        public string HashAccount { get; set; }//員工編號
        public string Name { get; set; }//員工姓名
        public string LeaveType { get; set; }//假別
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Reason { get; set; }//備註(事由)
        public bool? Review { get; set; }//審核狀態
        public DateTime CreatedTime { get; set; }//申請時間
    }
    public class ReviewLeaveRecord //審核用
    {
        public int LeaveRecordsId { get; set; }
        public bool Review { get; set; }
    }
}