using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class ReviewTripRecordModel : HttpResponse
    {
        public ReviewTripRecordModel(string name, DateTime start_date, DateTime end_date,string location, string reason, DateTime created_time)
        {
            Name = name;
            StartDate = start_date;
            EndDate = end_date;
            Location = location;
            Reason = reason;
            CreatedTime = created_time;
        }

        public string Name { get; set; }//員工姓名
        public DateTime StartDate { get; set; }//開始時間
        public DateTime EndDate { get; set; }//結束時間
        public string Location { get; set; }//地點
        public string Reason { get; set; }//備註(事由)
        public DateTime CreatedTime { get; set; }//申請時間

        public static async Task<List<ReviewTripRecordModel>> Get_ReviewTripRecord(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewTripRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic review_triprecord = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆待審核資料
            var length = 0;
            foreach (var num in review_triprecord)
            {
                length++;
            }

            //用LIST存取每筆待審核資料
            List<ReviewTripRecordModel> list = new List<ReviewTripRecordModel>();
            for (int i = 0; i < length; i++)
            {
                string name = review_triprecord[i].Name;//員工姓名
                DateTime start_date = review_triprecord[i].StartDate;//開始時間
                DateTime end_date = review_triprecord[i].EndDate;//結束時間
                string location = review_triprecord[i].Location;//地點
                string reason = review_triprecord[i].Reason;//備註(事由)
                DateTime created_time = review_triprecord[i].CreatedTime;//申請時間
                list.Add(new ReviewTripRecordModel(name, start_date, end_date, location, reason, created_time));
            }

            return list;
        }
    }
}