using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class OrganizationChartModel :HttpResponse
    {
        public static async Task<List<OrganizationChart>> GetOrganization(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetOrganization + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<OrganizationChart> organization = JsonConvert.DeserializeObject<List<OrganizationChart>>(GetResponse);

            return organization;
        }
    }
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
    } //顯示未審核員工資料
    class PassEmployeeModel : HttpResponse //已審核員工資料
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
        public static async Task<List<PassEmployee>> ManagerPassEmployees2(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetPassEmployee2 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<PassEmployee> pass_employee = JsonConvert.DeserializeObject<List<PassEmployee>>(GetResponse);

            return pass_employee;
        }
        public static async Task<List<PassEmployee>> ManagerPassEmployees3(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetPassEmployee3 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<PassEmployee> pass_employee = JsonConvert.DeserializeObject<List<PassEmployee>>(GetResponse);

            return pass_employee;
        }
        public static async Task<bool> EmployeeBoolEmail(string company_hash,string email)//判斷此EMAIL是否被用過
        {
            //連上WebAPI
            response = await client.GetAsync(url + EmployeeBoolRepeatEmail + "hash_company="+company_hash+ "&email="+email);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            bool result = JsonConvert.DeserializeObject<bool>(GetResponse);

            return result;
        }
            public static async Task<bool> EnabledEmployees(string hashaccount,bool enabled)//停用或恢復員工帳號
        {
            List<EnabledEmployee> enabledEmployees = new List<EnabledEmployee>();
            EnabledEmployee enabledEmployee = new EnabledEmployee()//要寫進LIST的資料
            {
                hashAccount = hashaccount,//員工編碼
                Enabled = enabled//使用狀態
            };
            enabledEmployees.Add(enabledEmployee);
            string jsonData = JsonConvert.SerializeObject(enabledEmployees);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + EmployeeEnabled, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> RenewEmployees(string hashaccount,string managerhash,string name,string phone,string email,int departmentID,int jobtitleID)//更新員工資料
        {
            List<EditEmployee> editEmployees = new List<EditEmployee>();
            EditEmployee editEmployee = new EditEmployee()//要寫進Json的資料
            {
                HashAccount = hashaccount,//員工編碼
                ManagerHash = managerhash,//管理員編號
                Name = name,
                Phone = phone,
                Email = email,
                DepartmentId = departmentID,//部門ID
                JobTitleId = jobtitleID//職稱ID
            };
            editEmployees.Add(editEmployee);

            string jsonData = JsonConvert.SerializeObject(editEmployees);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + EmployeeEditInformation, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }
        public static async Task<List<PassEmployee>> SearchEmployees3(List<PassEmployee> passEmployees, string company_hash,string department,string jobtitle ,string name)//三條件篩選
        {
            List<PassEmployee> searchEmployee = new List<PassEmployee>();
            for(int index = 0;index < passEmployees.Count; index++) 
            {
                if (passEmployees[index].HashAccount.Equals(name) && passEmployees[index].Department.Equals(department) && passEmployees[index].JobTitle.Equals(jobtitle))
                    searchEmployee.Add(ListAddSearch(passEmployees,index));
            }
           
            return searchEmployee;
        }//三條件篩選
        public static async Task<List<PassEmployee>> SearchEmployees2(List<PassEmployee> passEmployees, string company_hash, string department, string jobtitle, string name)//兩條件篩選
        {
            List<PassEmployee> searchEmployee = new List<PassEmployee>();
            for (int index = 0; index < passEmployees.Count; index++)
            {
                if (passEmployees[index].HashAccount.Equals(name) && passEmployees[index].Department.Equals(department))
                    searchEmployee.Add(ListAddSearch(passEmployees, index));
                else if (passEmployees[index].Department.Equals(department) && passEmployees[index].JobTitle.Equals(jobtitle))
                    searchEmployee.Add(ListAddSearch(passEmployees, index));
                else if (passEmployees[index].HashAccount.Equals(name) && passEmployees[index].JobTitle.Equals(jobtitle))
                    searchEmployee.Add(ListAddSearch(passEmployees, index));
            }

            return searchEmployee;
        }//兩條件篩選
        public static async Task<List<PassEmployee>> SearchEmployees1(List<PassEmployee> passEmployees,string company_hash, string department, string jobtitle, string name)//一條件篩選
        {
            List<PassEmployee> searchEmployee = new List<PassEmployee>();
            for (int index = 0; index < passEmployees.Count; index++)
            {
                    if (passEmployees[index].HashAccount.Equals(name) || passEmployees[index].Department.Equals(department) || passEmployees[index].JobTitle.Equals(jobtitle))
                    searchEmployee.Add(ListAddSearch(passEmployees, index));
            }

            return searchEmployee;
        }//一條件篩選
        public static PassEmployee ListAddSearch(List<PassEmployee> passEmployee, int index)
        {
            PassEmployee search = new PassEmployee()
            {
                HashAccount = passEmployee[index].HashAccount,//員工編號
                Name = passEmployee[index].Name,//員工姓名
                Phone = passEmployee[index].Phone,//員工電話
                Department = passEmployee[index].Department,//員工部門
                JobTitle = passEmployee[index].JobTitle,//員工職稱
                Email = passEmployee[index].Email, //員工電子郵件
                PhoneCode = passEmployee[index].PhoneCode,//員工驗證碼(phone_code)
                Enabled = passEmployee[index].Enabled,
                WorktimeId = passEmployee[index].WorktimeId,
                ManagerHash = passEmployee[index].ManagerHash
            };
            return search;
        }
    }//顯示已審核員工資料
    class SetEmployeeModel : HttpResponse //審核員工帳號
    {
        
        public static async Task<bool> SetEmployees(string hashaccount ,string managerhash,int departmentID ,int jobtitleID)//賦予職稱及部門
        {
            List<SetEmployee> setEmployees = new List<SetEmployee>();
            SetEmployee setEmployee = new SetEmployee()//要寫進LIST的資料
            {
                HashAccount = hashaccount,//員工編碼
                ManagerHash = managerhash,//管理員編號
                DepartmentId = departmentID,//部門ID
                JobtitleId = jobtitleID//職稱ID
            };
            setEmployees.Add(setEmployee);

            string jsonData = JsonConvert.SerializeObject(setEmployees);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + EmployeeSetInformation, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }
        public static async Task<bool> RejectEmployees(string hashaccount)//拒絕核准
        {

            response = await client.DeleteAsync(url + EmployeeRejectInformation + hashaccount);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }
    }//審核員工帳號
    public class ReviewEmployee
    {
        public string HashAccount { get; set; }//員工編號
        public string CompanyId { get; set; }//公司ID
        public string Name { get; set; }//員工姓名
        public string Email { get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
        public DateTime CreatedTime { get; set; }//申請時間
        public bool? Enabled { get; set; }//審核狀態
    }//未審核員工資料
    public class PassEmployee
    {
        public string HashAccount { get; set; }//員工編號
        public string ManagerHash { get; set; }//管理員編號
        public string Name { get; set; }//員工姓名
        public string Phone { get; set; }//員工電話
        public string Department { get; set; }//員工部門
        public string JobTitle { get; set; }//員工職稱
        public string Email { get; set; }//員工電子郵件
        public string PhoneCode { get; set; }//員工驗證碼(phone_code)
        public string WorktimeId { get; set; }//上下班代碼
        public bool? Enabled { get; set; }//使用狀態
    }//已審核員工資料
    public class SetEmployee
    {
        public string HashAccount { get; set; }//員工編號
        public string ManagerHash { get; set; }//管理員編號
        public int DepartmentId { get; set; }//員工部門
        public int JobtitleId { get; set; }//員工職稱
    }//賦予職稱及部門
    public class EnabledEmployee
    {
        public string hashAccount { get; set; }//員工編號
        public bool Enabled { get; set; }//使用狀態
    }//賦予職稱及部門
    public class EditEmployee
    {
        public string HashAccount { get; set; }//員工編號
        public string ManagerHash { get; set; }//管理員編號
        public string Name { get; set; }//員工姓名
        public string Phone { get; set; }//員工電話
        public string Email { get; set; }//員工電子郵件
        public int DepartmentId { get; set; }//員工部門代號
        public int JobTitleId { get; set; }//員工職稱代號
    }//已審核員工資料編輯
    public class OrganizationChart//組織圖
    {
        public string HashAccount { get; set; }
        public string Name { get; set; }
        public string ManagerHash { get; set; }
        public string Department { get; set; }
        public string Jobtitle{get;set;}
    }
}