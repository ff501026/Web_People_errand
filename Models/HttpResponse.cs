using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace AttendanceManagement.Models
{
    class HttpResponse
    {
        public static HttpClient client = new HttpClient();
        public static HttpResponseMessage response = new HttpResponseMessage();
        public static readonly string url = "http://163.18.110.100/api/"; //主機的URL
        public static readonly string CompanyGetWorkRecord = "Companies/GetWorkRecord/"; //查看公司員工打卡紀錄api
        public static readonly string CompanyReviewEmployee = "Companies/Review_Employee/"; //查看公司待審核之員工帳號資料api
        public static readonly string CompanyPassEmployee = "Companies/Pass_Employee/";//查看公司審核通過之員工資料api
        public static readonly string CompanyDepartment = "EmployeeDepartmentTypes";//查看公司部門資料api
        public static readonly string CompanyJobtitle = "EmployeeJobtitleTypes";//查看公司職稱資料api
        public static readonly string CompanyReviewTripRecord = "Companies/Review_TripRecord/";//查看公司職稱資料api
        public static readonly string CompanyHash = "4D7F9C66F7D796DB03284D632B5A8F";//公司

        public static string GetResponse { get; set; } //存取API回傳的內容

    }
}