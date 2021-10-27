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
        public static async Task<List<Detail_WorkRecord>> Get_WorkRecordAsync(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetWorkRecord + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyGetWorkRecord + company_hash}","",$"{ response.StatusCode.ToString()}",$"{GetResponse}");
            //解析打卡紀錄之JSON內容
            List<Work_Record> employee_workrecrd = JsonConvert.DeserializeObject<List<Work_Record>>(GetResponse);

            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(company_hash);
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(company_hash);
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            List<Detail_WorkRecord> detail_WorkRecords = new List<Detail_WorkRecord>();
            int num = 0;
            foreach (var work in employee_workrecrd)
            {
                num++;
            }

            for (int i = 0; i < num; i++)
            {
                string islate = "";
                string state = "";
                int index = passEmployees.FindIndex(t => t.HashAccount.Equals(employee_workrecrd[i].HashAccount));
                if (passEmployees[index].WorktimeId != null && passEmployees[index].WorktimeId != "0")
                {
                    int g_index = GeneralWorktime.FindIndex(t => t.GeneralWorktimeId.Equals(passEmployees[index].WorktimeId));
                    int f_index = FlexibleWorktime.FindIndex(t => t.FlexibleWorktimeId.Equals(passEmployees[index].WorktimeId));
                    if (passEmployees[index].WorktimeId.Substring(0, 1) == "G")
                    {
                        TimeSpan dt2 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        TimeSpan hour = new TimeSpan();//公司規定的上班時數
                        if (GeneralWorktime[g_index].RestTime > GeneralWorktime[g_index].WorkTime)
                        {
                            hour = GeneralWorktime[g_index].RestTime - GeneralWorktime[g_index].WorkTime; //公司規定的上班時數
                        }
                        else
                        {
                            hour = GeneralWorktime[g_index].WorkTime - GeneralWorktime[g_index].RestTime; //公司規定的上班時數
                        }
                        TimeSpan ts = employee_workrecrd[i].RestTime.Subtract(employee_workrecrd[i].WorkTime); //員工打卡的上班時數
                        TimeSpan time = new TimeSpan(1, 0, 0);//一小時


                        if (ts > hour + time)//員工的上班時數大於規定的上班時數
                        {
                            state = "異常";
                        }
                        else if (dt < GeneralWorktime[g_index].RestTime)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else if (ts < hour)//員工的上班時數比規定的上班時數小
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(GeneralWorktime[g_index].WorkTime, dt3) > 0)
                        {
                            islate = "準時";
                        }
                        else
                        {
                            islate = "遲到";
                        }
                    }
                    else if (passEmployees[index].WorktimeId.Substring(0, 1) == "F")
                    {
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        if (dt > FlexibleWorktime[f_index].RestTimeEnd)//員工的下班時間比規定的下班時間晚
                        {
                            state = "異常";
                        }
                        else if (dt < FlexibleWorktime[f_index].RestTimeStart)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt4 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, FlexibleWorktime[f_index].WorkTimeEnd) > 0)
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 || TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 && TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }

                    }
                }
                Detail_WorkRecord search = new Detail_WorkRecord()
                {
                    HashAccount = employee_workrecrd[i].HashAccount,
                    Num = employee_workrecrd[i].Num,//編號
                    Name = employee_workrecrd[i].Name,//員工姓名
                    WorkTime = employee_workrecrd[i].WorkTime,//上班紀錄
                    RestTime = employee_workrecrd[i].RestTime,//下班紀錄
                    isLate = islate,//是否遲到
                    state = state//下班狀態

                };
                detail_WorkRecords.Add(search);
            }

            return detail_WorkRecords;
        }

        public static async Task<List<Detail_WorkRecord>> Manager_Get_WorkRecordAsync2(string hash_account, string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetWorkRecord2 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerGetWorkRecord2 + hash_account}", "", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<Work_Record> employee_workrecrd = JsonConvert.DeserializeObject<List<Work_Record>>(GetResponse);
            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(company_hash);
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(company_hash);
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            List<Detail_WorkRecord> detail_WorkRecords = new List<Detail_WorkRecord>();
            int num = 0;
            foreach (var work in employee_workrecrd)
            {
                num++;
            }

            for (int i = 0; i < num; i++)
            {
                string islate = "";
                string state = "";
                int index = passEmployees.FindIndex(t => t.HashAccount.Equals(employee_workrecrd[i].HashAccount));
                if (passEmployees[index].WorktimeId != null && passEmployees[index].WorktimeId != "0")
                {
                    int g_index = GeneralWorktime.FindIndex(t => t.GeneralWorktimeId.Equals(passEmployees[index].WorktimeId));
                    int f_index = FlexibleWorktime.FindIndex(t => t.FlexibleWorktimeId.Equals(passEmployees[index].WorktimeId));
                    if (passEmployees[index].WorktimeId.Substring(0, 1) == "G")
                    {
                        TimeSpan dt2 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        TimeSpan hour = new TimeSpan();//公司規定的上班時數
                        if (GeneralWorktime[g_index].RestTime > GeneralWorktime[g_index].WorkTime)
                        {
                            hour = GeneralWorktime[g_index].RestTime - GeneralWorktime[g_index].WorkTime; //公司規定的上班時數
                        }
                        else
                        {
                            hour = GeneralWorktime[g_index].WorkTime - GeneralWorktime[g_index].RestTime; //公司規定的上班時數
                        }
                        TimeSpan ts = employee_workrecrd[i].RestTime.Subtract(employee_workrecrd[i].WorkTime); //員工打卡的上班時數
                        TimeSpan time = new TimeSpan(1, 0, 0);//一小時


                        if (ts > hour + time)//員工的上班時數大於規定的上班時數
                        {
                            state = "異常";
                        }
                        else if (dt < GeneralWorktime[g_index].RestTime)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else if (ts < hour)//員工的上班時數比規定的上班時數小
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(GeneralWorktime[g_index].WorkTime, dt3) > 0)
                        {
                            islate = "準時";
                        }
                        else
                        {
                            islate = "遲到";
                        }
                    }
                    else if (passEmployees[index].WorktimeId.Substring(0, 1) == "F")
                    {
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        if (dt > FlexibleWorktime[f_index].RestTimeEnd)//員工的下班時間比規定的下班時間晚
                        {
                            state = "異常";
                        }
                        else if (dt < FlexibleWorktime[f_index].RestTimeStart)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt4 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, FlexibleWorktime[f_index].WorkTimeEnd) > 0)
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 || TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 && TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }

                    }
                }
                Detail_WorkRecord search = new Detail_WorkRecord()
                {
                    HashAccount = employee_workrecrd[i].HashAccount,
                    Num = employee_workrecrd[i].Num,//編號
                    Name = employee_workrecrd[i].Name,//員工姓名
                    WorkTime = employee_workrecrd[i].WorkTime,//上班紀錄
                    RestTime = employee_workrecrd[i].RestTime,//下班紀錄
                    isLate = islate,//是否遲到
                    state = state//下班狀態

                };
                detail_WorkRecords.Add(search);
            }

            return detail_WorkRecords;
        }

        public static async Task<List<Detail_WorkRecord>> Manager_Get_WorkRecordAsync3(string hash_account,string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetWorkRecord3 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + ManagerGetWorkRecord3 + hash_account}", "", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<Work_Record> employee_workrecrd = JsonConvert.DeserializeObject<List<Work_Record>>(GetResponse);

            //取得公司一般上下班時間(新版)
            List<EmployeeGeneralWorktime> GeneralWorktime = await CompanyWorkTimeModel.Get_GeneralWorktime(company_hash);
            //取得公司彈性上下班時間(新版)
            List<EmployeeFlexibleWorktime> FlexibleWorktime = await CompanyWorkTimeModel.Get_FlexibleWorktime(company_hash);
            //輸入公司代碼取得全部的已審核資料
            List<PassEmployee> passEmployees = await PassEmployeeModel.PassEmployees(company_hash);
            List<Detail_WorkRecord> detail_WorkRecords = new List<Detail_WorkRecord>();
            int num = 0;
            foreach (var work in employee_workrecrd)
            {
                num++;
            }

            for (int i = 0; i < num; i++)
            {
                string islate = "";
                string state = "";
                int index = passEmployees.FindIndex(t => t.HashAccount.Equals(employee_workrecrd[i].HashAccount));
                if (passEmployees[index].WorktimeId != null && passEmployees[index].WorktimeId != "0")
                {
                    int g_index = GeneralWorktime.FindIndex(t => t.GeneralWorktimeId.Equals(passEmployees[index].WorktimeId));
                    int f_index = FlexibleWorktime.FindIndex(t => t.FlexibleWorktimeId.Equals(passEmployees[index].WorktimeId));
                    if (passEmployees[index].WorktimeId.Substring(0, 1) == "G")
                    {
                        TimeSpan dt2 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        TimeSpan hour = new TimeSpan();//公司規定的上班時數
                        if (GeneralWorktime[g_index].RestTime > GeneralWorktime[g_index].WorkTime)
                        {
                            hour = GeneralWorktime[g_index].RestTime - GeneralWorktime[g_index].WorkTime; //公司規定的上班時數
                        }
                        else
                        {
                            hour = GeneralWorktime[g_index].WorkTime - GeneralWorktime[g_index].RestTime; //公司規定的上班時數
                        }
                        TimeSpan ts = employee_workrecrd[i].RestTime.Subtract(employee_workrecrd[i].WorkTime); //員工打卡的上班時數
                        TimeSpan time = new TimeSpan(1, 0, 0);//一小時


                        if (ts > hour + time)//員工的上班時數大於規定的上班時數
                        {
                            state = "異常";
                        }
                        else if (dt < GeneralWorktime[g_index].RestTime)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else if (ts < hour)//員工的上班時數比規定的上班時數小
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(GeneralWorktime[g_index].WorkTime, dt3) > 0)
                        {
                            islate = "準時";
                        }
                        else
                        {
                            islate = "遲到";
                        }
                    }
                    else if (passEmployees[index].WorktimeId.Substring(0, 1) == "F")
                    {
                        TimeSpan dt3 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        TimeSpan dt = TimeSpan.Parse(employee_workrecrd[i].RestTime.ToString("HH:mm"));//員工打卡紀錄的下班時間
                        if (dt > FlexibleWorktime[f_index].RestTimeEnd)//員工的下班時間比規定的下班時間晚
                        {
                            state = "異常";
                        }
                        else if (dt < FlexibleWorktime[f_index].RestTimeStart)//員工的下班時間比規定的下班時間早
                        {
                            state = "早退";
                        }
                        else
                        {
                            state = "正常";
                        }
                        TimeSpan dt4 = TimeSpan.Parse(employee_workrecrd[i].WorkTime.ToString("HH:mm"));
                        if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, FlexibleWorktime[f_index].WorkTimeEnd) > 0)
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 || TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }
                        else
                        {
                            if (TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeStart, dt4) < 0 && TimeSpan.Compare(FlexibleWorktime[f_index].WorkTimeEnd, dt3) > 0)
                            {
                                islate = "準時";
                            }
                            else
                            {
                                islate = "遲到";
                            }
                        }

                    }
                }
                Detail_WorkRecord search = new Detail_WorkRecord()
                {
                    HashAccount = employee_workrecrd[i].HashAccount,
                    Num = employee_workrecrd[i].Num,//編號
                    Name = employee_workrecrd[i].Name,//員工姓名
                    WorkTime = employee_workrecrd[i].WorkTime,//上班紀錄
                    RestTime = employee_workrecrd[i].RestTime,//下班紀錄
                    isLate = islate,//是否遲到
                    state = state//下班狀態

                };
                detail_WorkRecords.Add(search);
            }

            return detail_WorkRecords;
        }
        public static async Task<List<Detail_WorkRecord>> Search_WorkRecord4(List<Detail_WorkRecord> all_work_Records, string company_hash, DateTime? date, string name, string islate, string state)//兩條件篩選
        {
            List<Detail_WorkRecord> searchWork_Record = new List<Detail_WorkRecord>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) && all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].isLate.Equals(islate) && all_work_Records[index].state.Equals(state))
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//兩條件篩選
        public static async Task<List<Detail_WorkRecord>> Search_WorkRecord3(List<Detail_WorkRecord> all_work_Records, string company_hash, DateTime? date, string name, string islate, string state)//兩條件篩選
        {
            List<Detail_WorkRecord> searchWork_Record = new List<Detail_WorkRecord>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) && all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].isLate.Equals(islate) ||
                    all_work_Records[index].Name.Equals(name) && all_work_Records[index].isLate.Equals(islate) && all_work_Records[index].state.Equals(state) ||
                    all_work_Records[index].Name.Equals(name) && all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].state.Equals(state) ||
                    all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].isLate.Equals(islate) && all_work_Records[index].state.Equals(state) 
                    )
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//兩條件篩選
        public static async Task<List<Detail_WorkRecord>> Search_WorkRecord2(List<Detail_WorkRecord> all_work_Records, string company_hash, DateTime? date, string name, string islate, string state)//兩條件篩選
        {
            List<Detail_WorkRecord> searchWork_Record = new List<Detail_WorkRecord>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) && all_work_Records[index].WorkTime.Date.Equals(date)||
                    all_work_Records[index].Name.Equals(name) && all_work_Records[index].isLate.Equals(islate) ||
                    all_work_Records[index].Name.Equals(name) && all_work_Records[index].state.Equals(state) ||
                    all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].isLate.Equals(islate) ||
                    all_work_Records[index].WorkTime.Date.Equals(date) && all_work_Records[index].state.Equals(state) ||
                    all_work_Records[index].isLate.Equals(islate) && all_work_Records[index].state.Equals(state) 
                    )
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//兩條件篩選
        public static async Task<List<Detail_WorkRecord>> Search_WorkRecord1(List<Detail_WorkRecord> all_work_Records, string company_hash, DateTime? date, string name,string islate,string state)//一條件篩選
        {
            List<Detail_WorkRecord> searchWork_Record = new List<Detail_WorkRecord>();
            for (int index = 0; index < all_work_Records.Count; index++)
            {
                if (all_work_Records[index].Name.Equals(name) || all_work_Records[index].WorkTime.Date.Equals(date) || all_work_Records[index].isLate.Equals(islate) || all_work_Records[index].state.Equals(state))
                    searchWork_Record.Add(ListAddSearch(all_work_Records, index));
            }

            return searchWork_Record;
        }//一條件篩選

        public static Detail_WorkRecord ListAddSearch(List<Detail_WorkRecord> work_Records, int index)
        {
            Detail_WorkRecord search = new Detail_WorkRecord()
            {
                HashAccount = work_Records[index].HashAccount,
                Num = work_Records[index].Num,//編號
                Name = work_Records[index].Name,//員工姓名
                WorkTime = work_Records[index].WorkTime,//上班紀錄
                RestTime = work_Records[index].RestTime,//下班紀錄
                isLate = work_Records[index].isLate,//是否遲到
                state = work_Records[index].state//下班狀態
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
            bool result = await LogModel.Add_Log($"{url + CompanyDepartment + company_hash}", "", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyAddDepartment}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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

            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyEditDepartment}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_Department(int id)//刪除部門
        {
            response = await client.DeleteAsync(url + CompanyDeleteDepartment + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyDeleteDepartment + id}", "", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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

            bool result = await LogModel.Add_Log($"{url + CompanyJobtitle + company_hash}", "", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyAddJobtitle}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyEditJobtitle}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_Jobtitle(int id)//刪除部門
        {
            response = await client.DeleteAsync(url + CompanyDeleteJobtitle + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyDeleteJobtitle + id}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

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

            bool result = await LogModel.Add_Log($"{url + CompanyGetTime + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");
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
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool result = await LogModel.Add_Log($"{url + CompanyEditTime + "companyhash=" + company_hash + "&worktime=" + worktime.ToString("hh") + "%3A" + worktime.ToString("mm") + "&resttime=" + resttime.ToString("hh") + "%3A" + resttime.ToString("mm")}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }

            return false;
        }
    }//公司上下班時間方法(初版)
    class CompanyManagerModel : HttpResponse
    {
        public static async Task<string> ForgetPassword_GetManagerHash(string code,string email)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerForegetPassword + code + "&email=" + email);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerForegetPassword + code + "&email=" + email}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                //取得API回傳的打卡紀錄內容
                GetResponse = await response.Content.ReadAsStringAsync();

                //解析打卡紀錄之JSON內容
                string managerhash = GetResponse.ToString();

                return managerhash;
            }
            
            return "";
        }//取得ManagerHash
        public static async Task<string> GetManagerKey(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerKey + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerKey + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            string managerkey = GetResponse.ToString();

            return managerkey;
        }//取得ManagerKey
        public static async Task<List<Manager>> GetAllManager(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerAll + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerAll + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<Manager> manager = JsonConvert.DeserializeObject<List<Manager>>(GetResponse);

            return manager;
        }//查看公司全部管理員

        public static async Task<List<ManagerKeyData>> ManagerKeyGetEmployee(string manager_key)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerKeyGetData + manager_key);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerKeyGetData + manager_key}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<ManagerKeyData> managerkeydata = JsonConvert.DeserializeObject<List<ManagerKeyData>>(GetResponse);

            return managerkeydata;
        }//ManagerKey取得員工資料

        public static async Task<bool> BoolManager(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerBool + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerBool + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            bool result = JsonConvert.DeserializeObject<bool>(GetResponse);

            return result;
        }//判斷是否有此管理員帳號

        public static async Task<bool> AddManager(string hash_account, string manager_password)
        {
            List<ManagerPassword> managerPasswords = new List<ManagerPassword>();
            ManagerPassword managerPassword = new ManagerPassword
            {
                HashAccount = hash_account,
                Password = manager_password
            };
            managerPasswords.Add(managerPassword);
            string jsonData = JsonConvert.SerializeObject(managerPasswords);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + ManagerAdd, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerAdd}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }//註冊管理員

        public static async Task<bool> UpdateManagerEnabled(string hash_account, bool enabled)
        {
            List<ManagerEnabled> managerEnableds = new List<ManagerEnabled>();
            ManagerEnabled managerEnabled = new ManagerEnabled
            {
                HashAccount = hash_account,
                Enabled = enabled
            };
            managerEnableds.Add(managerEnabled);
            string jsonData = JsonConvert.SerializeObject(managerEnableds);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + ManagerUpdateEnabled, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerUpdateEnabled}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }//

        public static async Task<bool> UpdateManagerAgent(string hash_account, string hash_agent)
        {
            List<ManagerAgent> managers = new List<ManagerAgent>();
            ManagerAgent manager = new ManagerAgent
            {
                HashAccount = hash_account,
                HashAgent = hash_agent
            };
            managers.Add(manager);
            string jsonData = JsonConvert.SerializeObject(managers);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + ManagerUpdateAgent, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerUpdateAgent}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<CompanyLogin> LoginCompany(string code, string manager_password)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyLogin + "code=" + code + "&password=" + manager_password);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyLogin + "code=" + code + "&password=" + manager_password}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            bool login = JsonConvert.DeserializeObject<bool>(GetResponse);

            CompanyLogin company = new Models.CompanyLogin();

            company.enabled = false;

            if (login)
            {
                //連上WebAPI
                response = await client.GetAsync(url + CompanyGetCompanyHash + "code=" + code + "&password=" + manager_password);
                //取得API回傳的打卡紀錄內容
                GetResponse = await response.Content.ReadAsStringAsync();
                bool result = await LogModel.Add_Log($"{url + CompanyGetCompanyHash + "code=" + code + "&password=" + manager_password}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

                company = JsonConvert.DeserializeObject<CompanyLogin>(GetResponse);
                
                company.enabled = true;
                return company;
            }
            return company;
        }//公司登入
        public static async Task<ManagerLogin> LoginManager(string code, string email ,string manager_password)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerLogin + "code=" + code + "&email=" + email + "&password=" + manager_password);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManagerLogin + "code=" + code + "&email=" + email + "&password=" + manager_password}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");
            //解析打卡紀錄之JSON內容
            bool login = JsonConvert.DeserializeObject<bool>(GetResponse);

            List<ManagerLogin> manager = new List<ManagerLogin>();
            ManagerLogin managerLogin = new ManagerLogin();
            manager.Add(managerLogin);
            manager[0].enabled = false;

            if (login)
            {
                //連上WebAPI
                response = await client.GetAsync(url + ManagerGetCompanyHash + "code=" + code + "&email=" + email + "&password=" + manager_password);
                //取得API回傳的打卡紀錄內容
                GetResponse = await response.Content.ReadAsStringAsync();
                bool result = await LogModel.Add_Log($"{url + ManagerGetCompanyHash + "code=" + code + "&email=" + email + "&password=" + manager_password}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

                manager = JsonConvert.DeserializeObject<List<ManagerLogin>>(GetResponse);

                manager[0].enabled = true;
                return manager[0];
            }
            return manager[0];
        }//管理員登入
        public static async Task<bool> EditManagerPassword(string hashaccount, string manager_password)
        {
            List<ManagerPassword> managerPasswords = new List<ManagerPassword>();
            ManagerPassword managerPassword = new ManagerPassword
            {
                HashAccount = hashaccount,
                Password = manager_password
            };
            managerPasswords.Add(managerPassword);
            string jsonData = JsonConvert.SerializeObject(managerPasswords);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + ManagerEditManagerPassword, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + ManagerEditManagerPassword}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }//變更管理員密碼
        public static async Task<bool> EditCompanyPassword(string companyhash, string manager_password)
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
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyEditManagerPassword}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }//變更公司密碼
    }//管理員方法
    class CompanyAddressModel : HttpResponse
    {
        public static async Task<List<CompanyAddress>> Get_CompanyAddress(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetCompanyAddress + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyGetCompanyAddress + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<CompanyAddress> companyAddresses = JsonConvert.DeserializeObject<List<CompanyAddress>>(GetResponse);

            return companyAddresses;
        }
        public static async Task<bool> EditCompanyAddress(string companyhash, string address)
        {
            string addressjson =GoogleMapApiModel.ConvertAddressToJsonString(address);
            double[] latLng = GoogleMapApiModel.ChineseAddressToLatLng(addressjson);

            List<CompanyAddress> companyAddresses = new List<CompanyAddress>();
            CompanyAddress companyAddress = new CompanyAddress
            {
                CompanyHash = companyhash,
                Address = address,
                CoordinateX = latLng[0],
                CoordinateY = latLng[1]
            };
            companyAddresses.Add(companyAddress);
            string jsonData = JsonConvert.SerializeObject(companyAddresses);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditCompanyAddress, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyEditCompanyAddress}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
    }//公司地址方法
    class CompanyWorkTimeModel : HttpResponse //公司上下班時間方法(新版)
    {
        public static async Task<List<EmployeeWorkTime>> Get_EmployeeWorkTime(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetEmployeeWorktime + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyGetEmployeeWorktime + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<EmployeeWorkTime> employeeWorkTimes = JsonConvert.DeserializeObject<List<EmployeeWorkTime>>(GetResponse);
            return employeeWorkTimes;
        }

        public static async Task<bool> Update_EmployeeWorkTime(int departmentid,int jobtitleid,string worktimeid)
        {

            List<EmployeeWorkTime> employeeWorkTimes = new List<EmployeeWorkTime>();
            EmployeeWorkTime employeeWorkTime = new EmployeeWorkTime
            {
                DepartmentId = departmentid,
                JobtitleId = jobtitleid,
                WorktimeId = worktimeid
            };
            employeeWorkTimes.Add(employeeWorkTime);
            string jsonData = JsonConvert.SerializeObject(employeeWorkTimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdateEmployeeWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyUpdateEmployeeWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Renew_EmployeeWorkTime(string old_id, string new_id)
        {

            List<RenewWorktime> renewWorktimes = new List<RenewWorktime>();
            RenewWorktime renewWorktime = new RenewWorktime
            {
                Worktime = old_id,
                NewWorktime = new_id
            };
            renewWorktimes.Add(renewWorktime);
            string jsonData = JsonConvert.SerializeObject(renewWorktimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyRenewEmployeeWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyRenewEmployeeWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<List<EmployeeGeneralWorktime>> Get_GeneralWorktime(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetAllGeneralWorktime + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyGetAllGeneralWorktime + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<EmployeeGeneralWorktime> employeeGeneralWorktimes = JsonConvert.DeserializeObject<List<EmployeeGeneralWorktime>>(GetResponse);
            return employeeGeneralWorktimes;
        }
        public static async Task<string> Add_GeneralWorktime(string companyhash, string Name, string WorkTime, string RestTime, int? BreakTime,string color)
        {

            List<GeneralWorkTime> generalWorkTimes = new List<GeneralWorkTime>();
            GeneralWorkTime generalWorkTime = new GeneralWorkTime
            {
                CompanyHash = companyhash,
                Name = Name,
                WorkTime = WorkTime,
                RestTime = RestTime,
                BreakTime = BreakTime,
                Color = color
            };
            generalWorkTimes.Add(generalWorkTime);
            string jsonData = JsonConvert.SerializeObject(generalWorkTimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddGeneralWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyAddGeneralWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                GetResponse = await response.Content.ReadAsStringAsync();
                return GetResponse;
            }
            return "";
        }
        public static async Task<bool> Edit_GeneralWorktime(string id, string Name, string WorkTime,string RestTime,int? BreakTime,string color)
        {

            List<GeneralWorkTime> generalWorkTimes = new List<GeneralWorkTime>();
            GeneralWorkTime generalWorkTime = new GeneralWorkTime
            {
                GeneralWorktimeId = id,
                Name = Name,
                WorkTime = WorkTime,
                RestTime = RestTime,
                BreakTime = BreakTime,
                Color = color
            };
            generalWorkTimes.Add(generalWorkTime);
            string jsonData = JsonConvert.SerializeObject(generalWorkTimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditGeneralWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyEditGeneralWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Delete_GeneralWorktime(string id)//刪除一般
        {
            response = await client.DeleteAsync(url + CompanyDeleteGeneralWorktime + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyDeleteGeneralWorktime + id}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<List<EmployeeFlexibleWorktime>> Get_FlexibleWorktime(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetAllFlexibleWorktime + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyGetAllFlexibleWorktime + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<EmployeeFlexibleWorktime> employeeFlexibleWorktimes = JsonConvert.DeserializeObject<List<EmployeeFlexibleWorktime>>(GetResponse);
            return employeeFlexibleWorktimes;
        }
        public static async Task<string> Add_FlexibleWorktime(string companyhash, string Name, string WorkTimeStart, string WorkTimeEnd,string RestTimeStart, string RestTimeEnd, int? BreakTime,string color)
        {

            List<FlexibleWorkTime> flexibleWorkTimes = new List<FlexibleWorkTime>();
            FlexibleWorkTime flexibleWorkTime = new FlexibleWorkTime
            {
                CompanyHash = companyhash,
                Name = Name,
                WorkTimeStart = WorkTimeStart,
                WorkTimeEnd = WorkTimeEnd,
                RestTimeStart = RestTimeStart,
                RestTimeEnd = RestTimeEnd,
                BreakTime = BreakTime,
                Color = color
            };
            flexibleWorkTimes.Add(flexibleWorkTime);
            string jsonData = JsonConvert.SerializeObject(flexibleWorkTimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddFlexibleWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyAddFlexibleWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                GetResponse = await response.Content.ReadAsStringAsync();
                return GetResponse;
            }
            return "";
        }

        public static async Task<bool> Edit_FlexibleWorktime(string id, string Name, string WorkTimeStart, string WorkTimeEnd, string RestTimeStart, string RestTimeEnd, int? BreakTime,string color)
        {

            List<FlexibleWorkTime> flexibleWorkTimes = new List<FlexibleWorkTime>();
            FlexibleWorkTime flexibleWorkTime = new FlexibleWorkTime
            {
                FlexibleWorktimeId = id,
                Name = Name,
                WorkTimeStart = WorkTimeStart,
                WorkTimeEnd = WorkTimeEnd,
                RestTimeStart = RestTimeStart,
                RestTimeEnd = RestTimeEnd,
                BreakTime = BreakTime,
                Color = color
            };
            flexibleWorkTimes.Add(flexibleWorkTime);
            string jsonData = JsonConvert.SerializeObject(flexibleWorkTimes);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyEditFlexibleWorktime, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyEditFlexibleWorktime}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_FlexibleWorktime(string id)//刪除彈性
        {
            response = await client.DeleteAsync(url + CompanyDeleteFlexibleWorktime + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool result = await LogModel.Add_Log($"{url + CompanyDeleteFlexibleWorktime + id}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

    }//公司上下班時間方法(新版)
    class CompanyManagerPermissionsModel : HttpResponse
    {
        public static async Task<bool> Manager_Bool_Agent(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManageBoolAgent + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManageBoolAgent + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            bool result = JsonConvert.DeserializeObject<bool>(GetResponse);
            return result;
        }
        public static async Task<List<BossSettingPermissions>> Manager_Get_BossPermissions(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManageGetBossPermissions + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + ManageGetBossPermissions + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<BossSettingPermissions> managerPermissions = JsonConvert.DeserializeObject<List<BossSettingPermissions>>(GetResponse);
            return managerPermissions;
        }
        public static async Task<List<ManagerPermissions>> Get_ManagerPermissions(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetManagerPermissions + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetManagerPermissions + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<ManagerPermissions> managerPermissions = JsonConvert.DeserializeObject<List<ManagerPermissions>>(GetResponse);
            return managerPermissions;
        }
        public static async Task<int> Add_ManagerPermissions(string companyhash, string Name, int employee_display,int? customization_display, int employee_review, int? customization_review, bool setting_worktime, bool setting_department_jobtitle, bool setting_location)
        {

            List<ManagerPermissions> managerPermissions = new List<ManagerPermissions>();
            ManagerPermissions managerPermission = new ManagerPermissions
            {
                CompanyHash = companyhash,
                Name = Name,
                EmployeeDisplay = employee_display,
                CustomizationDisplay = customization_display,
                EmployeeReview = employee_review,
                CustomizationReview = customization_review,
                SettingWorktime = setting_worktime,
                SettingDepartmentJobtitle = setting_department_jobtitle,
                SettingLocation = setting_location
            };
            managerPermissions.Add(managerPermission);
            string jsonData = JsonConvert.SerializeObject(managerPermissions);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddManagerPermissions, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyAddManagerPermissions}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                GetResponse = await response.Content.ReadAsStringAsync();
                int PermissionsId = JsonConvert.DeserializeObject<int>(GetResponse);
                return PermissionsId;
            }
            return -1;
        }

       public static async Task<bool> Edit_ManagerPermissions(string company_hash,int permissions_id, string Name, int employee_display, int? customization_display, int employee_review, int? customization_review, bool setting_worktime, bool setting_department_jobtitle, bool setting_location)
        {

            List<ManagerPermissions> managerPermissions = new List<ManagerPermissions>();
            ManagerPermissions managerPermission = new ManagerPermissions
            {
                PermissionsId = permissions_id,
                Name = Name,
                EmployeeDisplay = employee_display,
                CustomizationDisplay = customization_display,
                EmployeeReview = employee_review,
                CustomizationReview = customization_review,
                SettingWorktime = setting_worktime,
                SettingDepartmentJobtitle = setting_department_jobtitle,
                SettingLocation = setting_location
            };
            managerPermissions.Add(managerPermission);
            string jsonData = JsonConvert.SerializeObject(managerPermissions);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdateManagerPermissions, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyUpdateManagerPermissions}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_ManagerPermissions(string company_hash,int id)//刪除
        {
            response = await client.DeleteAsync(url + CompanyDeleteManagerPermissions + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyDeleteManagerPermissions + id}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<List<ManagerPermissionsCustomizations>> Get_ManagerPermissionsCustomizations(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetManagerPermissionsCustomizations);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetManagerPermissionsCustomizations}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<ManagerPermissionsCustomizations> managerPermissionsCustomizations = JsonConvert.DeserializeObject<List<ManagerPermissionsCustomizations>>(GetResponse);
            return managerPermissionsCustomizations;
        }
        public static async Task<int?> Add_ManagerPermissionsCustomizations(List<ManagerPermissionsCustomizations> managerPermissionsCustomizations)
        {

            List<ManagerPermissionsCustomizations> managerPermissionsCustomizations1 = new List<ManagerPermissionsCustomizations>();
            foreach (var input in managerPermissionsCustomizations) 
            {
                ManagerPermissionsCustomizations managerPermissionsCustomization = new ManagerPermissionsCustomizations
                {
                    DepartmentId = input.DepartmentId,
                    JobtitleId = input.JobtitleId
                };
                managerPermissionsCustomizations1.Add(managerPermissionsCustomization);
            }
            string jsonData = JsonConvert.SerializeObject(managerPermissionsCustomizations1);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PostAsync(url + CompanyAddManagerPermissionsCustomizations, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyAddManagerPermissionsCustomizations}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                GetResponse = await response.Content.ReadAsStringAsync();
                int PermissionsId = JsonConvert.DeserializeObject<int>(GetResponse);
                return PermissionsId;
            }
            return null;
        }
        public static async Task<bool> Delete_ManagerPermissionsCustomizations(string company_hash,int id)//刪除
        {
            response = await client.DeleteAsync(url + CompanyDeleteManagerPermissionsCustomizations + id);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyDeleteManagerPermissionsCustomizations + id}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<List<ManagerAccountPermissions>> Get_ManagerAccountPermissions(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetManagerAccountPermissions + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetManagerAccountPermissions + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<ManagerAccountPermissions> managerAccountPermissions = JsonConvert.DeserializeObject<List<ManagerAccountPermissions>>(GetResponse);
            return managerAccountPermissions;
        }

        public static async Task<bool> Edit_ManagerAccountPermissions(string company_hash,int department_id, int jobtitle_id, int? permissions_id)
        {

            List<ManagerAccountPermissions> managerAccountPermissions = new List<ManagerAccountPermissions>();
            ManagerAccountPermissions managerAccount = new ManagerAccountPermissions
            {
                DepartmentId = department_id,
                JobtitleId = jobtitle_id,
                PermissionsId = permissions_id
            };
            managerAccountPermissions.Add(managerAccount);
            string jsonData = JsonConvert.SerializeObject(managerAccountPermissions);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdateManagerAccountPermissions, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyUpdateManagerAccountPermissions}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<List<ManagerPermissions>> Get_ManagerRolePermissions(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetManagerRolePermissions + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetManagerRolePermissions + hash_account}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            //解析打卡紀錄之JSON內容
            List<ManagerPermissions> managerAccountPermissions = JsonConvert.DeserializeObject<List<ManagerPermissions>>(GetResponse);
            return managerAccountPermissions;
        }
    }
    class LogModel : HttpResponse
    {
        public static async Task<bool> Add_Log(string URL, string input, string RESPONSE, string output)
        {

            List<Log> logs = new List<Log>();
            Log log = new Log
            {
                Url = URL,
                Input = input,
                Response = RESPONSE,
                Output = output 
            };
            logs.Add(log);
            string jsonData = JsonConvert.SerializeObject(logs);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage logresponse = new HttpResponseMessage();
            logresponse = await client.PostAsync(url + AddLog, content);
            if (logresponse.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
    }

    class CompanySettingModel : HttpResponse
    {
        public static async Task<int> Get_CompanyPositionDifference(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetPositionDifference + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetPositionDifference + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");


            //解析打卡紀錄之JSON內容
            int PositionDifference = JsonConvert.DeserializeObject<int>(GetResponse);
            return PositionDifference;
        }

        public static async Task<bool> Edit_CompanyPositionDifference(string company_hash, int position_difference)
        {

            List<CompanySetting> companySettings = new List<CompanySetting>();
            CompanySetting companySetting = new CompanySetting
            {
               CompanyHash = company_hash,
               PositionDifference = position_difference
            };
            companySettings.Add(companySetting);
            string jsonData = JsonConvert.SerializeObject(companySettings);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdatePositionDifference, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyUpdatePositionDifference}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Get_CompanySettingTrip2Enabled(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetSettingTrip2Enabled + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetSettingTrip2Enabled + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");


            //解析打卡紀錄之JSON內容
            bool SettingTrip2Enabled = JsonConvert.DeserializeObject<bool>(GetResponse);
            return SettingTrip2Enabled;
        }

        public static async Task<bool> Edit_CompanySettingTrip2Enabled(string company_hash, bool setting_trip2_enabled)
        {

            List<CompanySetting> companySettings = new List<CompanySetting>();
            CompanySetting companySetting = new CompanySetting
            {
                CompanyHash = company_hash,
                SettingTrip2Enabled = setting_trip2_enabled
            };
            companySettings.Add(companySetting);
            string jsonData = JsonConvert.SerializeObject(companySettings);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdateSettingTrip2Enabled, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyUpdateSettingTrip2Enabled}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Get_CompanySettingWorkRecordEnabled(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + CompanyGetSettingWorkRecordEnabled + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();

            bool resultlog = await LogModel.Add_Log($"{url + CompanyGetSettingWorkRecordEnabled + company_hash}", $"", $"{ response.StatusCode.ToString()}", $"{GetResponse}");


            //解析打卡紀錄之JSON內容
            bool SettingWorkRecordEnabled = JsonConvert.DeserializeObject<bool>(GetResponse);
            return SettingWorkRecordEnabled;
        }

        public static async Task<bool> Edit_CompanySettingWorkRecordEnabled(string company_hash, bool setting_workrecord_enabled)
        {

            List<CompanySetting> companySettings = new List<CompanySetting>();
            CompanySetting companySetting = new CompanySetting
            {
                CompanyHash = company_hash,
                SettingWorkrecordEnabled = setting_workrecord_enabled
            };
            companySettings.Add(companySetting);
            string jsonData = JsonConvert.SerializeObject(companySettings);//序列化成JSON
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            response = await client.PutAsync(url + CompanyUpdateSettingWorkRecordEnabled, content);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            bool resultlog = await LogModel.Add_Log($"{url + CompanyUpdateSettingWorkRecordEnabled}", $"{jsonData}", $"{ response.StatusCode.ToString()}", $"{GetResponse}");

            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
    }

    public class CompanySetting
    {
        public string CompanyHash { get; set; }
        public int PositionDifference { get; set; }//誤差值
        public bool SettingTrip2Enabled { get; set; }//是否開啟到站
        public bool SettingWorkrecordEnabled { get; set; }//是否開啟定位
    }

    public partial class Log
    {
        public string Url { get; set; }
        public string Input { get; set; }
        public string Response { get; set; }
        public string Output { get; set; }
    }

    public class BossSettingPermissions
    {
        public bool SettingWorktime { get; set; }
        public bool SettingDepartmentJobtitle { get; set; }
        public bool SettingLocation { get; set; }
    }
    public class ManagerAgent 
    {
        public string HashAccount { get; set; }
        public string HashAgent { get; set; }
    }
    public class ManagerAccountPermissions 
    {
        public int DepartmentId { get; set; }
        public int JobtitleId { get; set; }
        public int? PermissionsId { get; set; }
    }
    public class RenewWorktime
    {
        public string Worktime { get; set; }
        public string NewWorktime { get; set; }
    }

    public partial class EmployeeWorkTime//員工上下班時間
    {
        public int DepartmentId { get; set; }
        public int JobtitleId { get; set; }
        public string WorktimeId { get; set; }
    }
    public partial class EmployeeGeneralWorktime//取得一般上下班設定
    {
        public string GeneralWorktimeId { get; set; }
        public string Name { get; set; }
        public TimeSpan WorkTime { get; set; }
        public TimeSpan RestTime { get; set; }
        public int? BreakTime { get; set; }
        public string Color { get; set; }
    }
    public class GeneralWorkTime //一般上下班設定(Post)(Put)
    {
        public string GeneralWorktimeId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public string WorkTime { get; set; }
        public string RestTime { get; set; }
        public int? BreakTime { get; set; }
        public string Color { get; set; }
    }
    public partial class EmployeeFlexibleWorktime //取得彈性上下班設定
    {
        public string FlexibleWorktimeId { get; set; }
        public string Name { get; set; }
        public TimeSpan WorkTimeStart { get; set; }
        public TimeSpan WorkTimeEnd { get; set; }
        public TimeSpan RestTimeStart { get; set; }
        public TimeSpan RestTimeEnd { get; set; }
        public int? BreakTime { get; set; }
        public string Color { get; set; }
    }
    public class FlexibleWorkTime //彈性上下班設定(Post)(Put)
    {
        public string FlexibleWorktimeId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public string WorkTimeStart { get; set; }
        public string WorkTimeEnd { get; set; }
        public string RestTimeStart { get; set; }
        public string RestTimeEnd { get; set; }
        public int? BreakTime { get; set; }
        public string Color { get; set; }
    }
    public class ManagerKeyData
    {
        public string HashAccount { get; set; }//員工編號
        public string Code { get; set; }//公司ID
        public string Name { get; set; }//員工姓名
        public DateTime ManagerKeyOverDate { get; set; }//Key到期時間
    }//ManagerKey取得的員工資料
    public class CompanyAddress
    {
        public string CompanyHash { get; set; }
        public string Address { get; set; }
        public double? CoordinateX { get; set; }
        public double? CoordinateY { get; set; }
    }//公司地址
    public class Work_Record
    {
        public string HashAccount { get; set; }
        public int Num { get; set; }//編號
        public string Name { get; set; }//員工姓名
        public DateTime WorkTime { get; set; }//上班紀錄
        public DateTime RestTime { get; set; }//下班紀錄
    }//打卡
    public class Detail_WorkRecord
    {
        public string HashAccount { get; set; }
        public int Num { get; set; }//編號
        public string Name { get; set; }//員工姓名
        public DateTime WorkTime { get; set; }//上班紀錄
        public DateTime RestTime { get; set; }//下班紀錄
        public string isLate { get; set; }//是否遲到
        public string state { get; set; }//下班狀態
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
    public class ManagerLogin
    {
        public string CompanyHash { get; set; }
        public string HashAccount { get; set; }
        public string Name { get; set; }
        public bool enabled { get; set; }
    }//管理員登入
    public class CompanyManagerPassword//公司密碼
    {
        public string CompanyHash { get; set; }
        public string ManagerPassword { get; set; }
    }
    public class ManagerPassword//管理員密碼
    {
        public string HashAccount { get; set; }
        public string Password { get; set; }
    }
    public class ManagerEnabled//管理員啟用或停用
    {
        public string HashAccount { get; set; }
        public bool Enabled { get; set; }
    }
    public class Manager//管理員
    {
        public string ManagerHash { get; set; }
        public string AgentHash { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }//員工電子郵件
        public string Department { get; set; }
        public string Jobtitle { get; set; }
        public int? PermissionsId { get; set; }
        public bool Enabled { get; set; }//使用狀態
    }

    public class ManagerPermissions //權限
    {
        public int PermissionsId { get; set; }
        public string CompanyHash { get; set; }
        public string Name { get; set; }
        public int EmployeeDisplay { get; set; }
        public int? CustomizationDisplay { get; set; }
        public int EmployeeReview { get; set; }
        public int? CustomizationReview { get; set; }
        public bool SettingWorktime { get; set; }
        public bool SettingDepartmentJobtitle { get; set; }
       public bool SettingLocation { get; set; }
    }

    public class ManagerPermissionsCustomizations //權限自訂
    {
        public int CustomizationId { get; set; }
        public int PermissionsId { get; set; }
        public int DepartmentId { get; set; }
        public int JobtitleId { get; set; }
    }
}
    