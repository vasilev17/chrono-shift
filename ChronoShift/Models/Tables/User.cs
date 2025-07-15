using System;
using System.Collections.Generic;

namespace ChronoShift.Models.Tables
{
    public partial class User
    {
        public User()
        {
            Record = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public int Role { get; set; }
        public DateTime? LatestLoginAttempt { get; set; }
        public DateTime? LockExpiration { get; set; }
        public int Attempts { get; set; }

        public virtual Role RoleNavigation { get; set; }
        public virtual ICollection<Record> Record { get; set; }
    }
}
