using System;
using System.Collections.Generic;

namespace ChronoShift.Models.Tables
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
