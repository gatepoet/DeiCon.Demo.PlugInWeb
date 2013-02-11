using System;
using System.Configuration.Provider;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Web.Security;

namespace org.theGecko.Utilities
{
    public class LocalAccountMembershipProvider : MembershipProvider
    {
        #region Imports

        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out int phToken);

        [DllImport("Kernel32.dll")]
        public static extern int GetLastError();

        #endregion

        private string _applicationName;
        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length > 256)
                {
                    throw new ProviderException("Provider application name is too long");
                }

                _applicationName = value;
            }
        }

        private string _domainName;
        public string DomainName
        {
            get
            {
                return _domainName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (value.Length > 256)
                {
                    throw new ProviderException("Provider domain name is too long");
                }

                _domainName = value;
            }
        }

        public string[] AccountsToIgnore
        {
            get;
            private set;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _applicationName = config["applicationName"];
            _domainName = config["domainName"];

            if (String.IsNullOrEmpty(name))
            {
                name = Environment.MachineName;
            }

            if (string.IsNullOrEmpty(_domainName))
            {
                _domainName = name;
            }

            string accountsToIgnore = config["accountsToIgnore"];

            AccountsToIgnore = (string.IsNullOrEmpty(accountsToIgnore)) ? new[] { "Administrator", "Guest" } : accountsToIgnore.Split(';');

            base.Initialize(name, config);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            int totalRecords;
            MembershipUserCollection users = FindUsersByName(username, 0, 1, out totalRecords);

            if (users.Count == 0)
            {
                return null;
            }

            return users.OfType<MembershipUser>().First();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            if (string.IsNullOrEmpty(usernameToMatch))
            {
                throw new ArgumentException("username must be supplied");
            }

            if (pageIndex < 0)
            {
                throw new ArgumentException("pageIndex must be 0 or above");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("pageSize must be above 0");
            }

            long upperBound = (long)pageIndex * pageSize + pageSize - 1;

            if (upperBound > Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize will create a record set which is too large");
            }

            string[] splitUsername = SplitUsername(usernameToMatch);
            string domain = (splitUsername.Length == 2) ? splitUsername[1] : DomainName;

            // Could use Name LIKE '%" + splitUsername[0] + "%'
            return SelectUsers("LocalAccount = True AND Name = '" + splitUsername[0] + "' AND Domain = '" + domain + "'", out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("pageIndex must be 0 or above");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("pageSize must be above 0");
            }

            long upperBound = (long)pageIndex * pageSize + pageSize - 1;

            if (upperBound > Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize will create a record set which is too large");
            }

            return SelectUsers("LocalAccount = True AND Domain = '" + DomainName + "'", out totalRecords);
        }

        public override bool ValidateUser(string username, string password)
        {
            string[] splitUsername = SplitUsername(username);

            // Only validate full UPN username to act like existing active directory provider
            if (splitUsername.Length != 2)
            {
                return false;
            }

            // Ignore specified accounts
            if (AccountsToIgnore.Contains(splitUsername[0]))
            {
                return false;
            }

            // User token for specified user is returned here
            int phToken;

            // Servername = "." (local)
            // Logon type = LOGON32_LOGON_NETWORK_CLEARTEXT (3)
            // Logon provider = LOGON32_PROVIDER_DEFAULT (0)
            bool loggedOn = LogonUser(splitUsername[0], splitUsername[1], password, 3, 0, out phToken);

            // Call GetLastError to try to determine why logon failed if it did not succeed.
            int returnCode = 0;
            if (!loggedOn)
            {
                returnCode = GetLastError();
            }

            if (returnCode != 0)
            {
                // throw new Exception ("Invalid Username or Password");
            }

            return loggedOn;
        }

        #region Helper Methods

        private static string[] SplitUsername(string username)
        {
            return username.Split('@');
        }

        private MembershipUserCollection SelectUsers(string where, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            SelectQuery query = new SelectQuery("Win32_UserAccount", where);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            totalRecords = collection.Count;

            foreach (ManagementObject user in collection)
            {
                string username = user["Name"].ToString();
                string domain = user["Domain"].ToString();

                if (!AccountsToIgnore.Contains(username))
                {
                    users.Add(CreateMembershipUser(username, domain));
                }
            }

            return users;
        }

        private MembershipUser CreateMembershipUser(string username, string domain)
        {
            return new MembershipUser(Name, username + "@" + domain, null, null, null, null, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
        }

        #endregion

        #region Not Implemented

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
