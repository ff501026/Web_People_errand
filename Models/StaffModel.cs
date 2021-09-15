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

        public static async Task<List<Work_Record>> Manager_Get_WorkRecordAsync2(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetWorkRecord2 + hash_account);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
            //解析打卡紀錄之JSON內容
            List<Work_Record> employee_workrecrd = JsonConvert.DeserializeObject<List<Work_Record>>(GetResponse);
            return employee_workrecrd;
        }

        public static async Task<List<Work_Record>> Manager_Get_WorkRecordAsync3(string hash_account)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerGetWorkRecord3 + hash_account);
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
    }//公司上下班時間方法(初版)
    class CompanyManagerModel : HttpResponse
    {
        public static async Task<string> GetManagerKey(string company_hash)
        {
            //連上WebAPI
            response = await client.GetAsync(url + ManagerKey + company_hash);
            //取得API回傳的打卡紀錄內容
            GetResponse = await response.Content.ReadAsStringAsync();
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
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Delete_GeneralWorktime(string id)//刪除一般
        {
            response = await client.DeleteAsync(url + CompanyDeleteGeneralWorktime + id);
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
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_FlexibleWorktime(string id)//刪除彈性
        {
            response = await client.DeleteAsync(url + CompanyDeleteFlexibleWorktime + id);
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
            if (response.StatusCode.ToString().Equals("OK"))
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> Delete_ManagerPermissions(string company_hash,int id)//刪除
        {
            response = await client.DeleteAsync(url + CompanyDeleteManagerPermissions + id);
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
            //解析打卡紀錄之JSON內容
            List<ManagerPermissions> managerAccountPermissions = JsonConvert.DeserializeObject<List<ManagerPermissions>>(GetResponse);
            return managerAccountPermissions;
        }
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
    