using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class ReviewEmployeeModel:HttpResponse
    {
        public ReviewEmployeeModel(string company_id, string name, string email, string phone_code,DateTime created_time)
        {
            CompanyId = company_id;
            Name = name;
            Email = email;
            PhoneCode = phone_code;
            CreatedTime = created_time;
        }

        public string CompanyId { get; set; }//公司ID
        public string Name { get; set; }//員工姓名
        public string Email{ get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
        public DateTime CreatedTime { get; set; }//申請時間

        public static async Task<List<ReviewEmployeeModel>> ReviewEmployees(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyReviewEmployee + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic review_employee = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆待審核資料
            var length = 0;
            foreach (var num in review_employee)
            {
                length++;
            }

            //用LIST存取每筆待審核資料
            List<ReviewEmployeeModel> list = new List<ReviewEmployeeModel>();
            for (int i = 0; i < length; i++)
            {
                string company_id = review_employee[i].Companyid;//公司ID
                string name = review_employee[i].Name;//員工姓名
                string email = review_employee[i].Email;//員工email
                string phone_code = review_employee[i].PhoneCode;//員工驗證碼(phone_code)
                DateTime created_time = review_employee[i].CreatedTime;//申請時間
                list.Add(new ReviewEmployeeModel(company_id, name, email, phone_code, created_time));
            }

            return list;
        }
    }

    class PassEmployeeModel : HttpResponse
    {
        public PassEmployeeModel( string name, string phone, string department,string jobtitle, string email, string phone_code)
        {
            Name = name;
            Phone = phone;
            Department = department;
            JobTitle = jobtitle;
            Email = email;
            PhoneCode = phone_code;
        }

        public string Name { get; set; }//員工姓名
        public string Phone { get; set; }//員工電話
        public string Department { get; set; }//員工部門
        public string JobTitle { get; set; }//員工職稱
        public string Email { get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
        

        public static async Task<List<PassEmployeeModel>> PassEmployees(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyPassEmployee + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic pass_employee = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆待審核資料
            var length = 0;
            foreach (var num in pass_employee)
            {
                length++;
            }

            //用LIST存取每筆待審核資料
            List<PassEmployeeModel> list = new List<PassEmployeeModel>();
            for (int i = 0; i < length; i++)
            {
                string name = pass_employee[i].Name;//員工姓名
                string phone = pass_employee[i].Phone;//公司ID
                string department = pass_employee[i].Department;//員工姓名
                string jobtitle = pass_employee[i].Jobtitle;//員工姓名
                string email = pass_employee[i].Email;//員工email
                string phone_code = pass_employee[i].PhoneCode;//員工驗證碼(phone_code)
                
                list.Add(new PassEmployeeModel(name, phone, department, jobtitle, email, phone_code));
            }

            return list;
        }
    }
}