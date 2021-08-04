using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web;

namespace AttendanceManagement.Models
{
    public class HttpResponse
    {
        public static HttpClient client = new HttpClient();
        public static HttpResponseMessage response = new HttpResponseMessage();
        public static readonly string url = "http://163.18.110.100/api/"; //主機的URL
        public static readonly string CompanyGetCompanyHash = "Companies/Get_CompanyHash?"; //取得公司編號
        public static readonly string CompanyGetCompanyAddress = "Companies/Get_CompanyAddress?company_hash="; //取得公司地址
        public static readonly string CompanyEditCompanyAddress = "Companies/Update_CompanyAddress"; //(Put)修改公司地址
        public static readonly string CompanyLogin = "Companies/Login_Company?"; //管理員登入
        public static readonly string CompanyGetTime = "Companies/Get_WorkTime_RestTime/"; //查看公司上下班時間
        public static readonly string CompanyEditTime = "Companies/Update_WorkTime_RestTime?"; //(PUT)編輯公司上下班時間
        public static readonly string CompanyEditManagerPassword = "Companies/UpdateManagerPassword"; //(PUT)編輯公司上下班時間
        public static readonly string CompanyGetWorkRecord = "Companies/GetWorkRecord/"; //查看公司員工打卡紀錄api
        public static readonly string CompanyReviewEmployee = "Companies/Review_Employee/"; //查看公司待審核之員工帳號資料api
        public static readonly string CompanyPassEmployee = "Companies/Pass_Employee/";//查看公司審核通過之員工資料api
        public static readonly string CompanyDepartment = "EmployeeDepartmentTypes/";//查看公司部門資料api
        public static readonly string CompanyJobtitle = "EmployeeJobtitleTypes/";//查看公司職稱資料api
        public static readonly string CompanyLeaveType = "EmployeeLeaveTypes";//查看公司請假資料api
        public static readonly string CompanyReviewTripRecord = "Companies/Review_TripRecord/";//查看公司待審核公差資料api
        public static readonly string CompanyPassTripRecord = "Companies/Pass_TripRecord/";//查看公司已審核公差資料api
        public static readonly string CompanyReviewLeaveRecord = "Companies/Review_LeaveRecord/";//查看公司待審核請假資料api
        public static readonly string CompanyPassLeaveRecord = "Companies/Pass_LeaveRecord/";//查看公司已審核請假資料api
        public static readonly string EmployeeSetInformation = "EmployeeInformations/set_information";//(PUT)賦予審核通過的員工部門及職稱API
        public static readonly string EmployeeRejectInformation = "EmployeeInformations/DeleteInformation/";//(Delete)拒絕審核員工帳號
        public static readonly string EmployeeEditInformation = "EmployeeInformations/edit_information";//(PUT)編輯員工資料
        public static readonly string EmployeeEnabled = "Employees/enabled_employee";//(PUT)停用或恢復員工帳號
        public static readonly string EmployeeReviewTripRecord = "EmployeeTripRecords/review_tripRecord";//(PUT)審核公差API
        public static readonly string EmployeeEditTripRecord = "EmployeeTripRecords/update_tripRecord";//(PUT)編輯公差API
        public static readonly string EmployeeReviewLeaveRecord = "EmployeeLeaveRecords/review_leaveRecord";//(PUT)審核請假API
        public static readonly string EmployeeEditLeaveRecord = "EmployeeLeaveRecords​/update_leaveRecord";//(PUT)編輯請假API
        public static readonly string CompanyAddDepartment = "EmployeeDepartmentTypes/add_department";//(POST)新增部門API
        public static readonly string CompanyEditDepartment = "EmployeeDepartmentTypes/UpdateDepartment";//(PUT)編輯部門名稱API
        public static readonly string CompanyDeleteDepartment = "EmployeeDepartmentTypes/DeleteDepartment/";//(Delete)刪除部門
        public static readonly string CompanyAddJobtitle = "EmployeeJobtitleTypes/add_jobtitle";//(POST)新增職稱API
        public static readonly string CompanyEditJobtitle = "EmployeeJobtitleTypes/UpdateJobtitle";//(PUT)編輯職稱API
        public static readonly string CompanyDeleteJobtitle = "EmployeeJobtitleTypes/DeleteJobtitle/";//(Delete)刪除職稱

        public static string CompanyHash="";//公司
        public static string CompanyName = "";//公司

        public static void sendGmail(string to_email,string email_subject, string email_body)//寄EMAIL
        {
            try
            {
                MailMessage mail = new MailMessage();
                //前面是發信email後面是顯示的名稱
                mail.From = new MailAddress("C108118221@nkust.edu.tw", "差勤打卡");

                //收信者email
                mail.To.Add(to_email);

                //設定優先權
                mail.Priority = MailPriority.Normal;

                //標題
                mail.Subject = email_subject;

                //內容
                mail.Body = email_body;

                //內容使用html
                mail.IsBodyHtml = true;

                //設定gmail的smtp (這是google的)
                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

                //您在gmail的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential("like3yy@gmail.com", "nkust.edu.tw");

                //開啟ssl
                MySmtp.EnableSsl = true;

                //發送郵件
                MySmtp.Send(mail);

                //放掉宣告出來的MySmtp
                MySmtp = null;

                //放掉宣告出來的mail
                mail.Dispose();
                Console.WriteLine("成功發送EMAIL通知!");
            }
            catch (Exception) 
            {
                Console.WriteLine("發送EMAIL通知失敗!");
            }
        }
        public static string GetResponse { get; set; } //存取API回傳的內容

    }
}