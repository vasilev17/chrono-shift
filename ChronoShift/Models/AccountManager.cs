using ChronoShift.Models.Tables;
using Microsoft.AspNetCore.Http;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using System;
using System.Linq;

namespace ChronoShift.Models
{
    public class AccountManager
    {
        public bool isAccountLocked = false;

        public void UnlockAccount(int userID)
        {
            User userAccount = null;

            using (var context = new TraineeScheduleContext())
            {
                var user = context.User.Find(userID);
                userAccount = user;
            }

            try
            {
                if (userAccount.Attempts > 2)
                {
                    if (userAccount.LockExpiration.Value < DateTime.UtcNow)
                    {
                        ResetUserAttempts(userID);
                    }
                    else
                    {
                        isAccountLocked = true;
                    }
                }
                if (userAccount.LatestLoginAttempt.Value.AddMinutes(30) < DateTime.UtcNow)
                {
                    ResetUserAttempts(userID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR]: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("[ERROR]: " + ex.Message);
            }
        }

        public void LockAccount(int userID)
        {
            var context = new TraineeScheduleContext();
            var user = context.User.Find(userID);
            int attepmts = user.Attempts;
            attepmts++;
            user.Attempts = attepmts;
            user.LatestLoginAttempt = DateTime.UtcNow;
            
            if (attepmts >= 3)
            {
                DateTime lockDate = DateTime.UtcNow;
                lockDate = lockDate.AddMinutes(30);
                user.LockExpiration = lockDate;
            }

            context.SaveChanges();
            context.Dispose();
        }

        public User GetUserByUsername(string username)
        {
            using (var context = new TraineeScheduleContext())
            {
                var user = context.User.Where(s => s.Username == username);
                if(user.Any())
                {
                    return user.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public User GetUserByToken(string token)
        {
            using (var context = new TraineeScheduleContext())
            {
                var user = context.User.Where(s => s.Token == token);
                return null;
            }
        }

        public void SetRoleByToken(int role, string token)
        {
            User account = GetUserByToken(token);
            if (account != null)
            {
                //Set Role
                var context = new TraineeScheduleContext();
                //account.Role = role;
                var user = context.User.Find(account.Id);
                user.Role = role;

                context.SaveChanges();
                context.Dispose();
            }
        }

        public void ResetUserAttempts(int userID)
        {
            using (var context = new TraineeScheduleContext())
            {
                var user = context.User.Find(userID);
                user.Attempts = 0;
                context.SaveChanges();
            }
        }

        public User CreateUserAccount(Login account, string userEmail)
        {
            Guid newToken = Guid.NewGuid();
            var context = new TraineeScheduleContext();
            var newUser = new User
            {
                Username = account.Username,
                Email = userEmail,
                Token = newToken.ToString()
            };
            context.Add(newUser);
            context.SaveChanges();
            context.Dispose();

            return newUser;
        }

        //string to bool when possible
        public CookieOptions LoginCookieOptionExpiration(bool rememberPassword)
        {
            CookieOptions option = new CookieOptions();

            if (rememberPassword)
            {
                option.Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                option.Expires = DateTime.Now.AddMinutes(30);
            }
            option.IsEssential = true;

            return option;
        }
    }
}
