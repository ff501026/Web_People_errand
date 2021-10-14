using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;

namespace AttendanceManagement.Controllers
{
    public class PunchRecordsController : Controller
    {
        
        // GET: PunchRecords
        public async Task<ActionResult> Index()
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //輸入公司代碼取得打卡紀錄
            List<Detail_WorkRecord> work_record = await StaffModel.Get_WorkRecordAsync(Session["company_hash"].ToString());
            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(Session["company_hash"].ToString());
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(Session["company_hash"].ToString());
            //取得員工上下班時間(新版)
            List<EmployeeWorkTime> employeeWorkTimes = await CompanyWorkTimeModel.Get_EmployeeWorkTime(Session["company_hash"].ToString());
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            //取得權限
            List<ManagerPermissions> managerPermissions = await CompanyManagerPermissionsModel.Get_ManagerPermissions(Session["company_hash"].ToString());

            if (Session["hash_account"] != null)
            {
                //輸入公司取得全部的管理員
                List<Manager> managers = await CompanyManagerModel.GetAllManager(Session["company_hash"].ToString());
                int index = managers.FindIndex(item => item.ManagerHash.Equals(Session["hash_account"].ToString()));
                if (managers[index].PermissionsId == null) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 1) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 2)
                {
                    work_record = await StaffModel.Manager_Get_WorkRecordAsync2(Session["hash_account"].ToString(), Session["company_hash"].ToString());
                }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 3)
                {
                    work_record = await StaffModel.Manager_Get_WorkRecordAsync3(Session["hash_account"].ToString(), Session["company_hash"].ToString());
                }
                else
                {
                    if (await CompanyManagerPermissionsModel.Manager_Bool_Agent(Session["hash_account"].ToString()))
                    {
                        int bossindex = managers.FindIndex(item => item.AgentHash.Equals(Session["hash_account"].ToString()));

                        if (managers[bossindex].PermissionsId == null) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 1) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 2)
                        {
                            work_record = await StaffModel.Manager_Get_WorkRecordAsync2(managers[bossindex].ManagerHash, Session["company_hash"].ToString());
                        }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 3)
                        {
                            work_record = await StaffModel.Manager_Get_WorkRecordAsync3(managers[bossindex].ManagerHash, Session["company_hash"].ToString());
                        }
                        else
                        {
                            work_record = await StaffModel.Get_WorkRecordAsync("n");
                        }
                    }
                    else
                    {
                        work_record = await StaffModel.Get_WorkRecordAsync("n");
                    }
                    
                }
               
            }

            int num =0;
            foreach (var work in work_record) 
            {
                num++;
            }
            ViewBag.Num = num;//打卡記錄總共有幾筆
            ViewBag.workrecord = work_record;//打卡紀錄
            ViewBag.general_worktime = GeneralWorktime;//公司一般上下班時間(新版)
            ViewBag.flexible_worktime = FlexibleWorktime;//公司彈性上下班時間(新版)
            ViewBag.employeeWorkTimes = employeeWorkTimes;//員工上下班時間(新版)
            ViewBag.pass_employee = passEmployees; //審核過資料
            return View("Index");
        }

        [HttpGet]//打卡資料篩選
        public async Task<ActionResult> SearchWorkRecord(DateTime? date, string employee_name,string islate,string state)
        {
            if (Session["company_hash"] == null)
            {
                return RedirectToAction("Index", "Account", null);
            }
            //取得權限
            List<ManagerPermissions> managerPermissions = await CompanyManagerPermissionsModel.Get_ManagerPermissions(Session["company_hash"].ToString());
            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(Session["company_hash"].ToString());
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(Session["company_hash"].ToString());
            //取得員工上下班時間(新版)
            List<EmployeeWorkTime> employeeWorkTimes = await CompanyWorkTimeModel.Get_EmployeeWorkTime(Session["company_hash"].ToString());
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(Session["company_hash"].ToString());
            //輸入公司代碼取得打卡紀錄
            List<Detail_WorkRecord> all_work_Records = await StaffModel.Get_WorkRecordAsync(Session["company_hash"].ToString());
            //放入篩選後的已審核資料
            List<Detail_WorkRecord> search_work_Records = new List<Detail_WorkRecord>();
            if (Session["hash_account"] != null)
            {
                //輸入公司取得全部的管理員
                List<Manager> managers = await CompanyManagerModel.GetAllManager(Session["company_hash"].ToString());
                int index = managers.FindIndex(item => item.ManagerHash.Equals(Session["hash_account"].ToString()));
                if (managers[index].PermissionsId == null) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 1) { }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 2)
                {
                    all_work_Records = await StaffModel.Manager_Get_WorkRecordAsync2(Session["hash_account"].ToString(),Session["company_hash"].ToString());
                }
                else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[index].PermissionsId)].EmployeeDisplay == 3)
                {
                    all_work_Records = await StaffModel.Manager_Get_WorkRecordAsync3(Session["hash_account"].ToString(), Session["company_hash"].ToString());
                }
                else
                {
                    if (await CompanyManagerPermissionsModel.Manager_Bool_Agent(Session["hash_account"].ToString()))
                    {
                        int bossindex = managers.FindIndex(item => item.AgentHash.Equals(Session["hash_account"].ToString()));

                        if (managers[bossindex].PermissionsId == null) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 1) { }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 2)
                        {
                            all_work_Records = await StaffModel.Manager_Get_WorkRecordAsync2(managers[bossindex].ManagerHash, Session["company_hash"].ToString());
                        }
                        else if (managerPermissions[managerPermissions.FindIndex(item => item.PermissionsId == managers[bossindex].PermissionsId)].EmployeeDisplay == 3)
                        {
                            all_work_Records = await StaffModel.Manager_Get_WorkRecordAsync3(managers[bossindex].ManagerHash, Session["company_hash"].ToString());
                        }
                        else
                        {
                            all_work_Records = await StaffModel.Get_WorkRecordAsync("n");
                        }
                    }
                    else
                    {
                        all_work_Records = await StaffModel.Get_WorkRecordAsync("n");
                    }

                }

            }
            //取得公司上下班時間
            Company_Time company_Times = await CompanyTimeModel.GetCompany_Times(Session["company_hash"].ToString());


            if (date.Equals(null) && employee_name.Equals("") && islate.Equals("是否遲到") && state.Equals("下班狀態"))//沒有輸入篩選條件就按搜尋，顯示全部資料
            { return RedirectToAction("index"); }
            else if (!date.Equals(null) && !employee_name.Equals("") && !islate.Equals("是否遲到") && !state.Equals("下班狀態"))//兩個篩選條件都輸入
            { search_work_Records = await StaffModel.Search_WorkRecord4(all_work_Records, Session["company_hash"].ToString(), date, employee_name, islate, state); }
            else if (!date.Equals(null) && !employee_name.Equals("") && !islate.Equals("是否遲到") ||
                    !date.Equals(null) && !employee_name.Equals("") && !state.Equals("下班狀態") ||
                    !date.Equals(null) && !islate.Equals("是否遲到") && !state.Equals("下班狀態") ||
                   !employee_name.Equals("") && !islate.Equals("是否遲到") && !state.Equals("下班狀態"))//兩個篩選條件都輸入
            { search_work_Records = await StaffModel.Search_WorkRecord3(all_work_Records, Session["company_hash"].ToString(), date, employee_name, islate, state); }//只輸入一個篩選條件
            else if (!date.Equals(null) && !employee_name.Equals("")  ||
                    !date.Equals(null) && !state.Equals("下班狀態") ||
                    !date.Equals(null) && !islate.Equals("是否遲到") ||
                    !islate.Equals("是否遲到") && !employee_name.Equals("") ||
                   !employee_name.Equals("")  && !state.Equals("下班狀態") ||
                   !islate.Equals("是否遲到") && !state.Equals("下班狀態") ) //兩個篩選條件都輸入
            { search_work_Records = await StaffModel.Search_WorkRecord2(all_work_Records, Session["company_hash"].ToString(), date, employee_name, islate, state);}//只輸入一個篩選條件
            else search_work_Records = await StaffModel.Search_WorkRecord1(all_work_Records, Session["company_hash"].ToString(), date, employee_name, islate, state);


            int num = 0;
            foreach (var work in search_work_Records)
            {
                num++;
            }

            ViewBag.Num = num;//打卡記錄總共有幾筆
            ViewBag.workrecord = search_work_Records;//篩選後打卡紀錄
            ViewBag.company_time = company_Times;
            ViewBag.general_worktime = GeneralWorktime;//公司一般上下班時間(新版)
            ViewBag.flexible_worktime = FlexibleWorktime;//公司彈性上下班時間(新版)
            ViewBag.employeeWorkTimes = employeeWorkTimes;//員工上下班時間(新版)
            ViewBag.pass_employee = passEmployees; //審核過資料
            return View("Index");
        }
    }
}