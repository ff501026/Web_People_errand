using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }//打卡資料
    class DepartmentModel : HttpResponse
    {
        public static async Task<List<Department>> Get_DepartmentAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyDepartment);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<Department> departments = JsonConvert.DeserializeObject<List<Department>>(GetResponse);

            return departments;
        }
    }//部門資料
    class JobtitleModel : HttpResponse
    {
        public static async Task<List<JobTitle>> Get_JobtitleAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyJobtitle);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<JobTitle> jobTitles = JsonConvert.DeserializeObject<List<JobTitle>>(GetResponse);
            
            return jobTitles;
        }


    }//職稱資料
    public class Work_Record
    {
        public int Num { get; set; }//編號
        public string Name { get; set; }//員工姓名
        public DateTime WorkTime { get; set; }//上班紀錄
        public DateTime RestTime { get; set; }//下班紀錄
    }//打卡
    public class Department 
    {
        public int DepartmentID { get; set; }//編號
        public string Name { get; set; }//部門名稱
    }//部門
    public class JobTitle
    {
        public int JobTitleID { get; set; }//編號
        public string Name { get; set; }//職稱
    }//職稱
}