using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AttendanceManagement.Models;
using Newtonsoft.Json;

namespace AttendanceManagement.Models
{
    class StaffModel : HttpResponse
    {
        public StaffModel(dynamic num, string name, DateTime worktime, DateTime resttime)
        {
            Num = num;
            Name = name;
            WorkTime = worktime;
            RestTime = resttime;
        }

        public int Num { get; set; }//編號
        public string Name { get; set; }//員工姓名
        public DateTime WorkTime { get; set; }//上班紀錄
        public DateTime RestTime { get; set; }//下班紀錄


        public static async Task<List<StaffModel>> Get_WorkRecordAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetWorkRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic employee_workRecord = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆上下班紀錄
            var length = 0;
            foreach (var num in employee_workRecord)
            {
                length++;
            }

            //用LIST存取每筆打卡紀錄
            List<StaffModel> list = new List<StaffModel>();
            for (int i = 0; i < length; i++)
            {
                int num = employee_workRecord[i].Num;//編號
                string name = employee_workRecord[i].Name;//員工姓名
                DateTime worktime = employee_workRecord[i].WorkTime;//上班紀錄
                DateTime resttime = employee_workRecord[i].RestTime;//下班紀錄
                list.Add(new StaffModel(num, name, worktime, resttime));
            }

            return list;
        }

    }
    class DepartmentModel : HttpResponse
    {
        public DepartmentModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }//部門名稱



        public static async Task<List<DepartmentModel>> Get_DepartmentAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyDepartment);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic department = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆部門
            var length = 0;
            foreach (var num in department)
            {
                length++;
            }

            //用LIST存取每筆部門
            List<DepartmentModel> list = new List<DepartmentModel>();
            for (int i = 0; i < length; i++)
            {
                string name = department[i].name;//部門名稱
                list.Add(new DepartmentModel(name));
            }

            return list;
        }


    }
    class JobtitleModel : HttpResponse
    {
        public JobtitleModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }//職稱



        public static async Task<List<JobtitleModel>> Get_JobtitleAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyJobtitle);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            dynamic jobtitle = Newtonsoft.Json.Linq.JArray.Parse(GetResponse);

            //計算總共有幾筆職稱
            var length = 0;
            foreach (var num in jobtitle)
            {
                length++;
            }

            //用LIST存取每筆部職稱
            List<JobtitleModel> list = new List<JobtitleModel>();
            for (int i = 0; i < length; i++)
            {
                string name = jobtitle[i].name;//職稱
                list.Add(new JobtitleModel( name));
            }

            return list;
        }


    }
}