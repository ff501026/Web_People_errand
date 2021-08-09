using AttendanceManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceManagement.Models
{
    class StaffModel : HttpResponse
    {
        public static async Task<List<Work_Record>> Get_WorkRecordAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetWorkRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<Work_Record> employee_workrecrd = JsonConvert.DeserializeObject<List<Work_Record>>(GetResponse);
            return employee_workrecrd;
        }
        public static async Task<List<Work_Record>> Search_WorkRecord2(string company_hash, DateTime? date, string name)//兩條件篩選
        {
            //輸入公司代碼取得打卡紀錄
            List<Work_Record> all_work_Records = await StaffModel.Get_WorkRecordAsync(company_hash);
            List<Work_Record> searchWork_Record = new List<Work_Record>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) && all_work_Records[index].WorkTime.Date.Equals(date))
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//兩條件篩選
        public static async Task<List<Work_Record>> Search_WorkRecord1(string company_hash, DateTime? date, string name)//一條件篩選
        {
            //輸入公司代碼取得打卡紀錄
            List<Work_Record> all_work_Records = await StaffModel.Get_WorkRecordAsync(company_hash);
            List<Work_Record> searchWork_Record = new List<Work_Record>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) || all_work_Records[index].WorkTime.Date.Equals(date))
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//一條件篩選

        public static Work_Record ListAddSearch(List<Work_Record> work_Records, int index)
        {
            Work_Record search = new Work_Record()
            {
                Num = work_Records[index].Num,//編號
                Name = work_Records[index].Name,//員工姓名
                WorkTime = work_Records[index].WorkTime,//上班紀錄
                RestTime = work_Records[index].RestTime//下班紀錄
            };
            return search;
        }
    }//打卡資料方法
    class DepartmentModel : HttpResponse
    {
        public static async Task<List<Department>> Get_DepartmentAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyDepartment + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<Department> departments = JsonConvert.DeserializeObject<List<Department>>(GetResponse);

            return departments;
        }

        public static async Task<bool> Add_Department(string company_hash,string name)
        {
            List<Add> addDepartments = new List<Add>();
            Add addDepartment = new Add
            {
                Name = name,
                CompanyHash = company_hash
            };
            addDepartments.Add(addDepartment);
            string jsonData = JsonConvert.SerializeObject(addDepartments);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddDepartment, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Edit_Department(int id, string name)
        {
            List<Department> editDepartments = new List<Department>();
            Department editDepartment = new Department
            {
                DepartmentId = id,
                Name = name
            };
            editDepartments.Add(editDepartment);

            string jsonData = JsonConvert.SerializeObject(editDepartments);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditDepartment, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_Department(int id)//刪除部門
        {
            response = await client.DeleteAsync(url + CompanyDeleteDepartment + id);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
    }//部門資料方法
    class JobtitleModel : HttpResponse
    {
        public static async Task<List<JobTitle>> Get_JobtitleAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyJobtitle + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<JobTitle> jobTitles = JsonConvert.DeserializeObject<List<JobTitle>>(GetResponse);

            return jobTitles;
        }

        public static async Task<bool> Add_Jobtitle(string company_hash,string name)
        {
            List<Add> addJobtitles = new List<Add>();
            Add addJobtitle = new Add
            {
                Name = name,
                CompanyHash = company_hash
            };
            addJobtitles.Add(addJobtitle);
            string jsonData = JsonConvert.SerializeObject(addJobtitles);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddJobtitle, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Edit_Jobtitle(int id, string name)
        {
            List<JobTitle> editjobTitles = new List<JobTitle>();
            JobTitle editJobtitle = new JobTitle
            {
                JobTitleId = id,
                Name = name
            };
            editjobTitles.Add(editJobtitle);
            string jsonData = JsonConvert.SerializeObject(editjobTitles);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditJobtitle, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_Jobtitle(int id)//刪除部門
        {
            response = await client.DeleteAsync(url + CompanyDeleteJobtitle + id);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }


    }//職稱資料方法
    class CompanyTimeModel : HttpResponse
    {
        public static async Task<Company_Time> GetCompany_Times(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetTime + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic company_time = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);
            Company_Time company_Times = new Company_Time()
            {
                WorkTime = company_time[0].WorkTime,
                RestTime = company_time[0].RestTime
            };
            return company_Times;
        }
        public static async Task<bool> Edit_CompanyTime(string company_hash, TimeSpan? WorkTime, TimeSpan? RestTime)
        {
            TimeSpan worktime = (TimeSpan)WorkTime;
            TimeSpan resttime = (TimeSpan)RestTime;
            response = await client.GetAsync(url + CompanyEditTime + "companyhash=" + company_hash + "&worktime=" + worktime.ToString("hh") + "%3A" + worktime.ToString("mm") + "&resttime=" + resttime.ToString("hh") + "%3A" + resttime.ToString("mm"));
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }
    }//公司上下班時間方法
    class CompanyManagerPasswordModel : HttpResponse
    {
        public static async Task<CompanyLogin> Login(string code, string manager_password)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyLogin + "code=" + code + "&password=" + manager_password);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            bool login = JsonConvert.DeserializeObject<bool>(GetResponse);

            List<CompanyLogin> company = new List<CompanyLogin>();
            CompanyLogin company_data = new Models.CompanyLogin();
            company.Add(company_data);

            company[0].enabled = false;

            if (login)
            {
                //連上WebAPI
                response = await client.GetAsync(url + CompanyGetCompanyHash + "code=" + code + "&password=" + manager_password);
                //取得API回傳的打卡紀錄內容
                GetResponse = await response.Content.ReadAsStringAsync();
                company = JsonConvert.DeserializeObject<List<CompanyLogin>>(GetResponse);
                
                company[0].enabled = true;
                return company[0];
            }
            return company[0];
        }
        public static async Task<bool> EditCompanyManagerPassword(string companyhash, string manager_password)
        {
            List<CompanyManagerPassword> managerPasswords = new List<CompanyManagerPassword>();
            CompanyManagerPassword managerPassword = new CompanyManagerPassword
            {
                CompanyHash = companyhash,
                ManagerPassword = manager_password
            };
            managerPasswords.Add(managerPassword);
            string jsonData = JsonConvert.SerializeObject(managerPasswords);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditManagerPassword, content);
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
    }//公司登入方法包刮變更密碼
    class CompanyAddressModel : HttpResponse
    {
        public static async Task<List<CompanyAddress>> Get_CompanyAddress(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetCompanyAddress + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<CompanyAddress> companyAddresses = JsonConvert.DeserializeObject<List<CompanyAddress>>(GetResponse);

            return companyAddresses;
        }
    }//公司地址方法
    public class CompanyAddress
    {
        public string CompanyHash { get; set; }
        public string Address { get; set; }
        public double? CoordinateX { get; set; }
        public double? CoordinateY { get; set; }
    }//公司地址
    public class Work_Record
    {
        public int Num { get; set; }//編號
        public string Name { get; set; }//員工姓名
        public DateTime WorkTime { get; set; }//上班紀錄
        public DateTime RestTime { get; set; }//下班紀錄
    }//打卡
    public class Department
    {
        public int DepartmentId { get; set; }//編號
        public string Name { get; set; }//部門名稱
    }//部門
    public class Company_Time
    {
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? RestTime { get; set; }
    }//公司上下班時間
    public class Update_Company_Time
    {
        public string CompanyHash { get; set; }
        public TimeSpan? WorkTime { get; set; }
        public TimeSpan? RestTime { get; set; }
    }//變更公司上下班時間
    public class JobTitle
    {
        public int JobTitleId { get; set; }//編號
        public string Name { get; set; }//職稱
    }//職稱
    public class Add
    {
        public string Name { get; set; }//名稱
        public string CompanyHash { get; set; }//公司編號
    }//新增部門或職稱
    public class CompanyLogin
    {
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public bool enabled { get; set; }
    }//公司登入
    public class CompanyManagerPassword//公司密碼
    {
        public string CompanyHash { get; set; }
        public string ManagerPassword { get; set; }
    }
}
    