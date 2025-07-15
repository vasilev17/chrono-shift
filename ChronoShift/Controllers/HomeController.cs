using ChronoShift.Models;
using ChronoShift.Models.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Novell.Directory.Ldap;
using System;
using System.Diagnostics;
using System.Linq;

namespace ChronoShift.Controllers
{
    [Route("[action]/{id?}")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            AccountManager accountManager = new AccountManager();
            if (Request.Cookies["Token"] == null)
            {
                return RedirectToAction("Login");
            }

            User account = accountManager.GetUserByToken(Request.Cookies["Token"]);

            if (account != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            AccountManager accountManager = new AccountManager();

            if (accountManager.GetUserByToken(Request.Cookies["Token"]) != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Response.Cookies.Delete("Token");
                return View();
            }
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("Token");
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(Login account)
        {
            AccountManager accountManager = new AccountManager();
            bool result = false;

            //If input is invalid
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isThereExistingAccount = accountManager.GetUserByUsername(account.Username) != null;
            string userEmail = null;

            User userAccount = null;

            if (isThereExistingAccount)
            {
                userAccount = accountManager.GetUserByUsername(account.Username);
                accountManager.UnlockAccount(userAccount.Id);
                if (accountManager.isAccountLocked)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password! [Account Locked]");
                    RedirectToAction("Login");
                }
            }

            try
            {
                ADManager adManager = new ADManager();
                result = adManager.Login(account);
                userEmail = adManager.userEmail;
            }
            catch (LdapException ex)
            {
                if (!ex.ToString().Contains("49")) //invalid credentials
                {
                    ModelState.AddModelError(string.Empty, "Error code " + ex.ResultCode + "\nhttps://www.novell.com/documentation/developer/jldap/jldapenu/api/com/novell/ldap/LDAPException.html");
                    Console.WriteLine(ex.ToString());
                    View("Login");
                }
            }

            //If connected and credentials are correct
            if (result)
            {
                try
                {
                    CookieOptions option = accountManager.LoginCookieOptionExpiration(account.RememberPassword);

                    if (isThereExistingAccount)
                    {
                        accountManager.ResetUserAttempts(userAccount.Id);
                        Response.Cookies.Append("Token", userAccount.Token, option);
                    }
                    else
                    {
                        userAccount = accountManager.CreateUserAccount(account, userEmail);
                        Response.Cookies.Append("Token", userAccount.Token, option);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Server Error!");
                    Console.WriteLine(ex.ToString());
                    return View();
                }
            }
            else
            {
                if (isThereExistingAccount == true)
                {
                    accountManager.LockAccount(userAccount.Id);
                }
                if (!accountManager.isAccountLocked)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password!");
                }
                return View();
            }
        }

        [HttpGet]
        public IActionResult SelectRole(int role)
        {
            if (role <= (int)roleEnum.Manager && role >= (int)roleEnum.Intern)
            {
                try
                {
                    AccountManager accountManager = new AccountManager();
                    accountManager.SetRoleByToken(role, Request.Cookies["Token"]);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception caught!");
                    Console.WriteLine(ex.ToString());
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckRole()
        {
            try
            {
                AccountManager accountManager = new AccountManager();
                User account = accountManager.GetUserByToken(Request.Cookies["Token"]);

                if (account != null)
                {
                    if (account.Role == (int)roleEnum.NoRole)
                    {
                        //Show popup
                        return Json(new { showPopUp = true });
                    }
                    return new EmptyResult();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Token not found!");
                    return RedirectToAction("Login");
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Exception caught!");
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult SaveTableData(int activityNum, string date, string time, string activityDescription)
        {
            AccountManager accountManager = new AccountManager();
            ActivityManager activityManager = new ActivityManager();
            DateTime saveDate = DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string[] splitTime = time.Split(':');
            TimeSpan saveTime = new TimeSpan(int.Parse(splitTime[0]), int.Parse(splitTime[1]), 0);
            var user = accountManager.GetUserByToken(Request.Cookies["Token"]);
            int userID = user.Id;
            int recordID = activityManager.GetRecordID(userID, saveDate);

            //Look if there is any record for the date
            //If there is a record, update the record or create a new one if non-existant (in case of bug)
            if (recordID != -1)
            {
                var activities = activityManager.GetActivities(recordID);
                //Check if there is any activity
                if (activities.Any())
                {
                    //Check if there is activity with this ID (activityNum) and update it if exists
                    if (activityManager.isActivityExisting(recordID, activityNum))
                    {
                        activityManager.OverrideActivity(recordID, activityNum, saveTime, activityDescription);
                    }
                    //Creates activity
                    else
                    {
                        activityManager.CreateActivity(recordID, activityNum, saveTime, activityDescription);
                    }
                }
                //Create activity in case it doesn't exist
                else
                {
                    activityManager.CreateActivity(recordID, activityNum, saveTime, activityDescription);
                }
            }
            //Create record + activity
            else
            {
                activityManager.CreateRecord(userID, saveDate);
                recordID = activityManager.GetRecordID(userID, saveDate);

                if (recordID != -1)
                {
                    activityManager.CreateActivity(recordID, activityNum, saveTime, activityDescription);
                }
            }
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Connections()
        {
            return View("Connections");
        }

        [HttpGet]
        public IActionResult Holiday(int month, int year)
        {
            //send this method to separate class/manager
            DateManager d = new DateManager();

            int lastday = DateTime.DaysInMonth(year, month);
            int numberOfDays = d.GetNumberOfDates(new DateTime(year, month, 01), new DateTime(year, month, lastday));

            AccountManager accountManager = new AccountManager();
            
            int id = -1;
            User userAccount = accountManager.GetUserByToken(Request.Cookies["Token"]);
            
            if (userAccount != null)
            {
                id = userAccount.Id;
            }
            else
            {
                return RedirectToAction("Login");
            }

            TimeSpan totalHours = d.CalculateHoursForMonth(id, month);
            TimeSpan numberofdaysTimeSpan = new TimeSpan(numberOfDays, 0, 0, 0);

            int convertingDays = (int)(totalHours.TotalHours / 8);
            int convertingHours = (int)totalHours.TotalHours - (convertingDays * 8);
            int convertedTotalHours = totalHours.Days * 24 + totalHours.Hours;

            int minutes = totalHours.Minutes;
            TimeSpan workedHours = new TimeSpan(0, convertedTotalHours, minutes, 0);

            int convertedWorkedDays = (int)(workedHours.TotalHours / 8);
            int realWorkedHours = (int)workedHours.TotalHours - (convertedWorkedDays * 8);
            TimeSpan workedDays = new TimeSpan(convertedWorkedDays, realWorkedHours, workedHours.Minutes, 0);

            int holidayDays = numberOfDays - convertedWorkedDays;
            int holidayHours = 0;
            int holidayMinutes = 0;

            if (realWorkedHours != 0)
            {
                holidayHours = 8 - realWorkedHours;
                holidayDays -= 1;
            }

            int calculatedholidayHours = holidayHours;

            if (workedHours.Minutes != 0)
            {
                holidayMinutes = 60 - workedHours.Minutes;
                if (holidayHours == 0)
                {
                    holidayHours = 0;
                    holidayDays -= 1;
                    calculatedholidayHours = 8 - 1;
                }
                else
                {
                    calculatedholidayHours -= 1;
                }
            }


            TimeSpan holidays = new TimeSpan(holidayDays, calculatedholidayHours, holidayMinutes, 0);

            String nuberofDaysString = numberofdaysTimeSpan.Days.ToString();
            String workedHoursString = workedDays.ToString(@"dd\.hh\:mm");
            String holiDaysString = holidays.ToString(@"dd\.hh\:mm");



            String all = "  Working days:" + nuberofDaysString + " | " + "  Worked days:" + workedHoursString + " | " + " Holidays:" + holiDaysString;

            return Json(new { WorkedDays = all });
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        enum roleEnum
        {
            NoRole,
            Intern,
            Mentor,
            Manager,
            Administrator
        }
    }

}
