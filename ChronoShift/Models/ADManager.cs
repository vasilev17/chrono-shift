using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChronoShift.Models
{
    public class ADManager
    {
        public string userEmail = null;
        private const string ADHost = "rodc.nemetschek.bg";
        private const string searchBase = "ou=MAIN,dc=nemetschek,dc=bg";
        private const string ADDomain = "w3knbg";

        public bool Login(Login account)
        {
            var saslRequest = new SaslDigestMd5Request(account.Username.Trim(), account.Password, ADDomain, ADHost);
            using (var conn = new LdapConnection())
            {
                conn.Connect(ADHost, 389);
                conn.Bind(saslRequest);
                if (conn.AuthenticationDn != null)
                {
                    string filter = $"(&(objectClass=*)(cn=" + conn.AuthenticationDn + "))";
                    var searchResults = conn.Search(searchBase, LdapConnection.ScopeSub, filter, null, false);
                    while (searchResults.HasMore())
                    {
                        var nextEntry = searchResults.Next();
                        nextEntry.GetAttributeSet();
                        var attr = nextEntry.GetAttribute("mail");

                        if (attr != null)
                        {
                            userEmail = nextEntry.GetAttribute("mail").StringValue;
                        }
                    }
                    conn.Disconnect();
                    return true;
                }
            }
            return false;
        }
    }
}
