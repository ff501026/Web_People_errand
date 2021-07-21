using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class ReviewEmployeeModel:HttpResponse
    {
        public static async Task<List<ReviewEmployee>> ReviewEmployees(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewEmployee + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List < ReviewEmployee > review_employee= JsonConvert.DeserializeObject<List<ReviewEmployee>>(GetResponse);

            return review_employee;
        }
    }
    class PassEmployeeModel : HttpResponse
    {
        public static async Task<List<PassEmployee>> PassEmployees(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyPassEmployee + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<PassEmployee> pass_employee = JsonConvert.DeserializeObject<List<PassEmployee>>(GetResponse);

            return pass_employee;
        }
    }
    public class ReviewEmployee
    {
        public string CompanyId { get; set; }//公司ID
        public string Name { get; set; }//員工姓名
        public string Email { get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
        public DateTime CreatedTime { get; set; }//申請時間
    }
    public class PassEmployee
    {
        public string Name { get; set; }//員工姓名
        public string Phone { get; set; }//員工電話
        public string Department { get; set; }//員工部門
        public string JobTitle { get; set; }//員工職稱
        public string Email { get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
    }
}