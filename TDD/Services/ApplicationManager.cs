using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using Microsoft.Practices.Unity;
using SMS.Core;
using SMS.Data;
using SMS.Extensions;
using SMS.Models;
using SMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SMS.Services
{
    internal sealed class ApplicationManager : ObservableObject
    {
        #region Fields

        private string applicationName;
        private byte passwordFormat;
        private int maxPasswordFailures;
        private int passwordAttemptWindow;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private string passwordStrengthRegularExpression;
        private int minRequiredSearchWordLength;
        private int maxUsageCountToRefresh;
        private long maxImageFileSize;
        private long maxLogFileSize;
        private bool showAllErrors;
        private bool reverseErrors;
        private bool writeToLog;
        private bool checkTimeZone;
        private TimeSpan timeZone;
        private bool checkDateTime;
        private bool alreadyCreated;
        private bool canCreateNew;

        private IUnityContainer container;
        private GenericPrincipal user;
        private Guid? userId;
        private Mutex mutex;

        public static readonly string ImageOpenFileFilter;
        public static readonly string ImageSaveFileFilter;
        public static readonly string ImageFileSizeError;

        #endregion

        #region Properties

        public static ApplicationManager Current { get; private set; }
        public List<KeyValues<short, string, List<KeyValues<Guid, string, Uri>>>> GroupMenus;
        public bool IsFakeLogon { get; private set; }

        public IUnityContainer Container
        {
            get { return this.container; }
        }

        public bool IsAuthenticated
        {
            get { return this.User.Identity.IsAuthenticated; }
        }

        public GenericPrincipal User
        {
            get { return this.user; }
            private set
            {
                if (this.user != value)
                {
                    this.user = value;
                    RaisePropertyChanged("User");
                }
            }
        }

        public Guid? UserId
        {
            get { return this.userId; }
            private set
            {
                Repair(ref value);
                if (this.userId != value)
                {
                    this.userId = value;
                    RaisePropertyChanged("UserId");
                }
            }
        }

        public bool CanCreateNew
        {
            get
            {
                CreateMutex();
                return this.canCreateNew;
            }
        }

        public string ApplicationName
        {
            get { return this.applicationName; }
            set
            {
                if (this.applicationName != value)
                {
                    this.applicationName = value;
                    RaisePropertyChanged("ApplicationName");
                }
            }
        }

        public byte PasswordFormat
        {
            get { return this.passwordFormat; }
            set
            {
                ValidPasswordFormat(value);
                if (this.passwordFormat != value)
                {
                    this.passwordFormat = value;
                    RaisePropertyChanged("PasswordFormat");
                }
            }
        }

        public int MaxPasswordFailures
        {
            get { return this.maxPasswordFailures; }
            set
            {
                ValidMaxPasswordFailures(value);
                if (this.maxPasswordFailures != value)
                {
                    this.maxPasswordFailures = value;
                    RaisePropertyChanged("MaxPasswordFailures");
                }
            }
        }

        public int PasswordAttemptWindow
        {
            get { return this.passwordAttemptWindow; }
            set
            {
                ValidPasswordAttemptWindow(value);
                if (this.passwordAttemptWindow != value)
                {
                    this.passwordAttemptWindow = value;
                    RaisePropertyChanged("PasswordAttemptWindow");
                }
            }
        }

        public int MinRequiredNonAlphanumericCharacters
        {
            get { return this.minRequiredNonAlphanumericCharacters; }
            set
            {
                ValidMinRequiredNonAlphanumericCharacters(value);
                if (this.minRequiredNonAlphanumericCharacters != value)
                {
                    this.minRequiredNonAlphanumericCharacters = value;
                    RaisePropertyChanged("MinRequiredNonAlphanumericCharacters");
                }
            }
        }

        public int MinRequiredPasswordLength
        {
            get { return this.minRequiredPasswordLength; }
            set
            {
                ValidMinRequiredPasswordLength(value);
                if (this.minRequiredPasswordLength != value)
                {
                    this.minRequiredPasswordLength = value;
                    RaisePropertyChanged("MinRequiredPasswordLength");
                }
            }
        }

        public string PasswordStrengthRegularExpression
        {
            get { return this.passwordStrengthRegularExpression; }
            set
            {
                //var expressionString = value.Trim();
                //if (string.IsNullOrEmpty(expressionString) == false && expressionString.Length > 0)
                //{
                //    try
                //    {
                //        Regex regex = new Regex(expressionString);
                //        if (this.passwordStrengthRegularExpression != expressionString)
                //        {
                //            this.passwordStrengthRegularExpression = expressionString;
                //            RaisePropertyChanged("PasswordStrengthRegularExpression");
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}
                value = value.Trim();
                ValidPasswordStrengthRegularExpression(value);
                if (this.passwordStrengthRegularExpression != value)
                {
                    this.passwordStrengthRegularExpression = value;
                    RaisePropertyChanged("PasswordStrengthRegularExpression");
                }
            }
        }

        public int MinRequiredSearchWordLength
        {
            get { return this.minRequiredSearchWordLength; }
            set
            {
                ValidMinRequiredSearchWordLength(value);
                if (this.minRequiredSearchWordLength != value)
                {
                    this.minRequiredSearchWordLength = value;
                    RaisePropertyChanged("MinRequiredSearchWordLength");
                }
            }
        }

        public int MaxUsageCountToRefresh
        {
            get { return this.maxUsageCountToRefresh; }
            set
            {
                ValidMaxUsageCountToRefresh(value);
                if (this.maxUsageCountToRefresh != value)
                {
                    this.maxUsageCountToRefresh = value;
                    RaisePropertyChanged("MaxUsageCountToRefresh");
                }
            }
        }

        public long MaxImageFileSize
        {
            get { return this.maxImageFileSize; }
            set
            {
                ValidMaxImageFileSize(value);
                if (this.maxImageFileSize != value)
                {
                    this.maxImageFileSize = value;
                    RaisePropertyChanged("MaxImageFileSize");
                }
            }
        }

        public long MaxLogFileSize
        {
            get { return this.maxLogFileSize; }
            set
            {
                ValidMaxLogFileSize(value);
                if (this.maxLogFileSize != value)
                {
                    this.maxLogFileSize = value;
                    RaisePropertyChanged("MaxLogFileSize");
                }
            }
        }

        public bool ShowAllErrors
        {
            get { return this.showAllErrors; }
            set
            {
                if (this.showAllErrors != value)
                {
                    this.showAllErrors = value;
                    RaisePropertyChanged("ShowAllErrors");
                }
            }
        }

        public bool ReverseErrors
        {
            get { return this.reverseErrors; }
            set
            {
                if (this.reverseErrors != value)
                {
                    this.reverseErrors = value;
                    RaisePropertyChanged("ReverseErrors");
                }
            }
        }

        public bool WriteToLog
        {
            get { return this.writeToLog; }
            set
            {
                if (this.writeToLog != value)
                {
                    this.writeToLog = value;
                    RaisePropertyChanged("WriteToLog");
                }
            }
        }

        public bool CheckTimeZone
        {
            get { return this.checkTimeZone; }
            set
            {
                if (this.checkTimeZone != value)
                {
                    this.checkTimeZone = value;
                    RaisePropertyChanged("CheckTimeZone");
                }
            }
        }

        public TimeSpan TimeZone
        {
            get { return this.timeZone; }
            set
            {
                if (this.timeZone != value)
                {
                    this.timeZone = value;
                    RaisePropertyChanged("TimeZone");
                }
            }
        }

        public bool CheckDateTime
        {
            get { return this.checkDateTime; }
            set
            {
                if (this.checkDateTime != value)
                {
                    this.checkDateTime = value;
                    RaisePropertyChanged("CheckDateTime");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        static ApplicationManager()
        {
            ImageFileSizeError = "Image file size cannot be more then 200 KB.";
            ImageOpenFileFilter = "Image Files|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.wdp";
            ImageSaveFileFilter = "JPEG|*.jpg;*.jpeg|Bitmap|*.bmp|GIF|*.gif|PNG|*.png|TIFF|*.tif|WMP|*.wdp";

            Current = new ApplicationManager();
        }

        public ApplicationManager()
        {
            RegisterTypes();
            LoadDefaultSettings(false);
            ResetUser();
        }

        #endregion

        #region Methods

        public void CheckDbRequirements()
        {
            using (var uow = ApplicationManager.Resolve<ISMSUnitOfWork>())
            {
                int count;
                string userName = "Administrator";

                count = uow.Count<User>();
                if (count == 0)
                {
                    var value = new User
                    {
                        UserName = userName,
                        EmailAddress = "administrator.sms@codeflexsystems.com",
                        UserMembership = uow.CreateMemebership("sms@2017", null, null)
                    };
                    uow.Add(value);
                    uow.SaveChanges();
                }
                SystemLogon();

                var administrator = uow.GetUserByUserName(userName);

                count = uow.Count<User>();
                if (administrator != null)
                {
                    Permission v = Permission.CanView | Permission.CanAdd | Permission.CanEdit
                        | Permission.CanDelete | Permission.CanPrint | Permission.CanResetPassword
                        | Permission.CanSetPermission;
                    if (count == 1 && administrator.UserMembership.Disabled)
                        uow.UnlockUser(administrator);
                    InitialFixUserPermissions(administrator.UserPermissions);
                    FinalFixUserPermissions(administrator.UserPermissions);
                    administrator.UserPermissions.ForEach(m =>
                    {
                        m.Permission = v;
                    });
                    uow.SaveChanges();
                }

                count = uow.Count<AddressType>();
                if (count == 0)
                {
                    foreach (var line in SR.GetAddressTypes())
                        uow.Add(new AddressType() { AddressTypeName = line });
                    uow.SaveChanges();
                }

                //count = uow.Count<Country>();
                //if (count == 0)
                //{
                //    var countries = SR.GetCountries();
                //    if (countries.Count == 0)
                //    {
                //        var country = new Country();

                //        country.CountryCode = "MM";
                //        country.CountryName = "Myanmar (မြန်မာ)";
                //        country.Code = "95";
                //        uow.Add(country);
                //    }
                //    else
                //    {
                //        foreach (var line in countries)
                //        {
                //            var country = new Country();

                //            country.CountryName = line.Item1;
                //            country.CountryCode = line.Item2;
                //            country.Code = line.Item3;
                //            uow.Add(country);
                //        }
                //    }
                //    uow.SaveChanges();
                //}

                //count = uow.Count<Currency>();
                //if (count == 0)
                //{
                //    foreach (var line in SR.GetCurrencies())
                //        uow.Add(new Currency() { CurrencyCode = line.Item1, CurrencyName = line.Item2 });
                //    uow.SaveChanges();
                //}

                count = uow.Count<EmailType>();
                if (count == 0)
                {
                    foreach (var line in SR.GetEmailTypes())
                        uow.Add(new EmailType() { EmailTypeName = line });
                    uow.SaveChanges();
                }

                count = uow.Count<FeeType>();
                if (count == 0)
                {
                    foreach (var line in SR.GetFeeTypes())
                        uow.Add(new FeeType() { FeeTypeName = line });
                    uow.SaveChanges();
                }

                count = uow.Count<PhoneType>();
                if (count == 0)
                {
                    foreach (var line in SR.GetPhoneTypesRaw())
                        uow.Add(new PhoneType() { PhoneTypeName = line });
                    uow.SaveChanges();
                }

                count = uow.Count<GuardianType>();
                if (count == 0)
                {
                    foreach (var line in SR.GetGuardianTypies())
                        uow.Add(new GuardianType { GuardianTypeName = line.Item1, Gender = line.Item2 });
                    uow.SaveChanges();
                }

                count = uow.Count<Religion>();
                if (count == 0)
                {
                    foreach (var line in SR.GetReligions())
                        uow.Add(new Religion { ReligionName = line });
                    uow.SaveChanges();
                }
                Logoff();
            }
        }

        public void CreateMutex()
        {
            if (!this.alreadyCreated)
            {
                this.mutex = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out this.canCreateNew);
                this.alreadyCreated = true;
            }
        }

        public void ReleaseMutex()
        {
            try
            {
                if (this.canCreateNew)
                    this.mutex.ReleaseMutex();
            }
            catch { }
        }

        public static T Resolve<T>()
        {
            return Current.Container.Resolve<T>();
        }

        #endregion

        #region Validation Methods

        public bool IsDbOK()
        {
            bool flag = false;

            try
            {
                using (var cox = new SMSContext())
                {
                    cox.Database.Connection.Open();
                    cox.Database.Connection.Close();
                    flag = true;
                }
            }
            catch { }

            return flag;
        }

        public bool IsValidTimeZone()
        {
            return TimeZoneInfo.Local.BaseUtcOffset == this.timeZone;
        }

        //public bool IsValidDateTime(DateTime dt)
        //{
        //    bool flag = false;

        //    try
        //    {
        //        using (var uow = this.container.Resolve<ISMSUnitOfWork>())
        //        {
        //            var sdt = Library.Data.DbHelper.GetSqlServerUtcDate(uow.Context.Database.Connection.ConnectionString);
        //            var diff = sdt.Item1 - dt;

        //            flag = Math.Abs(diff.TotalSeconds) < sdt.Item2;
        //        }
        //    }
        //    catch { }

        //    return flag;
        //}

        public bool IsValidEmail(string email)
        {
            if (!email.IsValidEmail())
                return false;
            if (email.Length > 128)
                return false;
            return true;
        }

        public void ValidateEmail(string email)
        {
            if (!email.IsValidEmail())
                throw new ArgumentException(string.Format("Email {0} is not valid.", email));
            if (email.Length > 128)
                throw new ArgumentException(SR.Get("IsTooLong", "Email", 128));
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            //if (password.Length > 128)
            //    return false;
            if (password.Length < this.minRequiredPasswordLength)
                return false;
            if (password.Where(c => !char.IsLetterOrDigit(c)).Count() < this.minRequiredNonAlphanumericCharacters)
                return false;
            if (!string.IsNullOrWhiteSpace(this.passwordStrengthRegularExpression))
            {
                bool flag = false;
                try
                {
                    flag = Regex.IsMatch(password, this.passwordStrengthRegularExpression);
                }
                catch { }
                return flag;
            }
            return true;
        }

        public void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(SR.Get("IsNotAllowNull", "Password"));
            //if (password.Length > 128)
            //    throw new ArgumentException(SR.Get("IsTooLong", "Password", 128));
            if (password.Length < this.minRequiredPasswordLength)
                throw new ArgumentException(string.Format("Password do not meet minimum required length.{0}Minimum lenght is {1}",
                    Environment.NewLine, this.minRequiredPasswordLength));
            if (password.Where(c => !char.IsLetterOrDigit(c)).Count() < this.minRequiredNonAlphanumericCharacters)
                throw new ArgumentException(string.Format("Password do not meet minimum required non alpha numeric characters.{0}Minimum lenght is {1}",
                    Environment.NewLine, this.minRequiredNonAlphanumericCharacters));
            if (!string.IsNullOrWhiteSpace(this.passwordStrengthRegularExpression))
            {
                bool flag = false;
                try
                {
                    flag = Regex.IsMatch(password, this.passwordStrengthRegularExpression);
                }
                catch { }
                if (!flag)
                    throw new ArgumentException("Password do not meet minimum required strength.");
            }
        }

        public bool IsValidPasswordAnswer(string passwordAnswer)
        {
            if (string.IsNullOrWhiteSpace(passwordAnswer))
                return false;
            if (passwordAnswer.Length > 128)
                return false;
            return true;
        }

        public void ValidatePasswordAnswer(string passwordAnswer)
        {
            if (string.IsNullOrWhiteSpace(passwordAnswer))
                throw new ArgumentException(SR.Get("IsNotAllowNull", "Password Answer"));
            if (passwordAnswer.Length > 128)
                throw new ArgumentException(SR.Get("IsTooLong", "Password Answer", 128));
        }

        public bool IsValidPasswordQuestion(string passwordQuestion, bool allowNotNull = true)
        {
            if (string.IsNullOrWhiteSpace(passwordQuestion))
                return false;
            if (passwordQuestion.Length > 128)
                return false;
            return true;
        }

        public void ValidatePasswordQuestion(string passwordQuestion, bool allowNotNull = true)
        {
            if (string.IsNullOrWhiteSpace(passwordQuestion))
                throw new ArgumentException(SR.Get("IsNotAllowNull", "Password Question"));
            if (passwordQuestion.Length > 128)
                throw new ArgumentException(SR.Get("IsTooLong", "Password Question", 128));
        }

        public bool IsValidUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;
            if (userName.Length > 128)
                return false;
            return true;
        }

        public void ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(SR.Get("IsNotAllowNullOrSpace", "User Name"));
            if (userName.Length > 128)
                throw new ArgumentException(SR.Get("IsTooLong", "User Name", 128));
        }

        public bool IsValidPasswordFormat(byte value)
        {
            if (value == 1)
                return true;
            return false;
        }

        public void ValidPasswordFormat(byte value)
        {
            //if (value < 1 || value > 2)
            //    throw new NotSupportedException("Password format must be Hashed or Encrypted.");
            if (value != 1)
                throw new NotSupportedException(SR.Get("PwMustHashed"));
        }

        public bool IsValidMaxPasswordFailures(int value)
        {
            if (value < 3)
                return false;
            return true;
        }

        public void ValidMaxPasswordFailures(int value)
        {
            if (value < 3)
                throw new ArgumentException(SR.Get("InvalidMaxPwFailure"));
        }

        public bool IsValidPasswordAttemptWindow(int value)
        {
            if (value < 5)
                return false;
            return true;
        }

        public void ValidPasswordAttemptWindow(int value)
        {
            if (value < 5)
                throw new ArgumentException(SR.Get("InvalidPwAttemptWin"));
        }

        public bool IsValidMinRequiredNonAlphanumericCharacters(int value)
        {
            if (value < 1)
                return false;
            if (value > this.minRequiredPasswordLength)
                return false;
            return true;
        }

        public void ValidMinRequiredNonAlphanumericCharacters(int value)
        {
            if (value < 1)
                throw new ArgumentException(SR.Get("InvalidMinRequired", "non alpha numeric characters"));
            if (value > this.minRequiredPasswordLength)
                throw new ArgumentException(SR.Get("CannotBeMoreThan", "MinRequiredNonalphanumericCharacters", "MinRequiredPasswordLength."));
        }

        public bool IsValidMinRequiredPasswordLength(int value)
        {
            if (value < 6)
                return false;
            if (this.minRequiredNonAlphanumericCharacters > value)
                return false;
            return true;
        }

        public void ValidMinRequiredPasswordLength(int value)
        {
            if (value < 6)
                throw new ArgumentException(SR.Get("InvalidMinRequired", "password length"));
            if (this.minRequiredNonAlphanumericCharacters > value)
                throw new ArgumentException(SR.Get("CannotBeMoreThan", "MinRequiredNonalphanumericCharacters", "MinRequiredPasswordLength."));
        }

        public bool IsValidPasswordStrengthRegularExpression(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    Regex regex = new Regex(value);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ValidPasswordStrengthRegularExpression(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    Regex regex = new Regex(value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsValidMinRequiredSearchWordLength(int value)
        {
            if (value < 1)
                return false;
            if (value > 20)
                return false;
            return true;
        }

        public void ValidMinRequiredSearchWordLength(int value)
        {
            if (value < 1)
                throw new ArgumentException(SR.Get("InvalidMinRequired", "search word length"));
            if (value > 20)
                throw new ArgumentException("Minimum required search word length can not be more than 20.");
        }

        public bool IsValidMaxUsageCountToRefresh(int value)
        {
            if (value < 1)
                return false;
            return true;
        }

        public void ValidMaxUsageCountToRefresh(int value)
        {
            if (value < 1)
                throw new ArgumentException("Maximum usage count to refresh is invalid.");
        }

        public bool IsValidMaxImageFileSize(long value)
        {
            if (value < 1)
                return false;
            return true;
        }

        public void ValidMaxImageFileSize(long value)
        {
            if (value < 1)
                throw new ArgumentException("Invalid maximum image file size.");
        }

        public bool IsValidMaxLogFileSize(long value)
        {
            if (value < 1)
                return false;
            return true;
        }

        public void ValidMaxLogFileSize(long value)
        {
            if (value < 1)
                throw new ArgumentException("Invalid maximum log file size.");
        }

        #endregion

        #region Logon, Logout, Menus

        public void Logoff()
        {
            if (this.IsAuthenticated)
            {
                ResetUser();
                //if (this.Options.IsDefault == false)
                //    this.Options.LoadDefault();
            }
        }

        public void FakeLogon(string userName)
        {
            //var user = new User { UserId = Guid.Parse("EDDA758A-C3DA-E411-B7D6-FF0411E1DDD9"), UserName = userName };

            //this.IsFakeLogon = true;
            //this.SetLogonInfo(user);
            using (var uow = this.container.Resolve<ISMSUnitOfWork>())
            {
                var user = uow.GetUserByUserName(userName, asNoTracking: true);

                if (user == null)
                    user = uow.FirstOrDefaultAsNoTracking<User>();
                this.IsFakeLogon = true;
                this.SetLogonInfo(user);
            }
        }

        public void Logon(string userName, string password)
        {
            using (var uow = this.container.Resolve<ISMSUnitOfWork>())
            {
                var user = uow.GetUserByUserName(userName);
                bool flag = uow.ValidateUser(user, password);

                this.IsFakeLogon = false;
                if (flag)
                    SetLogonInfo(user);
            }
        }

        private void SetLogonInfo(User user)
        {
            this.User = CreatePrincipal(user.UserName);
            this.UserId = user.UserId;
            //if (user.UserOption == null)
            //    this.Options.Load(user.UserId);
            //else
            //    this.Options.Load(user.UserOption);
        }

        public bool IsLogonUser(User user)
        {
            //return object.Equals(this.user.Identity.Name, user.UserName);
            return this.user.Identity.Name.Equals(user.UserName, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsLogonUser(string userName)
        {
            //return object.Equals(this.user.Identity.Name, userName);
            return this.user.Identity.Name.Equals(userName, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsLogonUser(Guid id)
        {
            if (this.UserId.HasValue)
                return this.UserId.Value == id;

            return false;
        }

        public ICollection<UserPermission> GetUserPermissions()
        {
            List<UserPermission> list = new List<UserPermission>();

            if (this.IsAuthenticated && this.UserId.HasValue)
            {
                Guid uid = this.UserId.Value;

                using (var sms = this.container.Resolve<SMS.Data.ISMSUnitOfWork>())
                {
                    list.AddRange(sms.WhereAsNoTracking<SMS.Models.UserPermission>(m => m.UserId == uid));
                }
            }

            return list;
        }

        public ICollection<UserPermission> GetFakeUserPermissions()
        {
            List<UserPermission> list = new List<UserPermission>();

            if (this.IsAuthenticated && this.UserId.HasValue)
            {
                Guid uid = this.UserId.Value;

                if (this.user.Identity.Name == "Administrator")
                {
                    Permission v = Permission.CanView | Permission.CanAdd | Permission.CanEdit
                        | Permission.CanDelete | Permission.CanPrint | Permission.CanResetPassword
                        | Permission.CanSetPermission;
                    this.GroupMenus.ForEach(gm =>
                    {
                        gm.Value2.ForEach(m => list.Add(new UserPermission
                        {
                            UserId = uid,
                            MenuId = m.Key,
                            Permission = v
                        }));
                    });
                }
                else if (this.user.Identity.Name == "Reception")
                {
                    SetUserPermission(list, uid, 1000, 3000);
                    SetUserPermission(list, uid, 5000,
                        Guid.Parse("690B5E1E-E544-4576-B2EA-1480D31E7FC9"),
                        Guid.Parse("659D6623-DA74-4311-95FB-EA1CD656AEEC"),
                        Guid.Parse("026E81D2-1E0C-451D-995F-1A056D36A301"),
                        Guid.Parse("8813BBA3-6E1C-4E68-A508-EDC90C7646ED"),
                        Guid.Parse("02607F25-31D3-4681-BB77-4039E7A75969"),
                        Guid.Parse("8C17A322-E821-4B6A-AA98-3CC2DE464E9F"));
                }
                else if (this.user.Identity.Name == "Admin")
                {
                    SetUserPermission(list, uid, 3000, 4000);
                }
                else if (this.user.Identity.Name == "Account")
                {
                    SetUserPermission(list, uid, 2000, 3000);
                    SetUserPermission(list, uid, 5000,
                        Guid.Parse("9507F3CF-A54E-4895-8A51-589501D910DB"),
                        Guid.Parse("173B036B-5FAF-42BA-A401-F438D2DF4BCF"));
                }
                else
                {
                    SetUserPermission(list, uid, 3000);
                }
            }

            return list;
        }

        public void InitialFixUserPermissions(ICollection<UserPermission> list)
        {
            if (list == null)
                return;

            this.GroupMenus.ForEach(gm => gm.Value2.ForEach(lm =>
            {
                if (list.Any(m => m.MenuId == lm.Key) == false)
                    list.Add(new UserPermission { MenuId = lm.Key });
            }));
        }

        public void FinalFixUserPermissions(ICollection<UserPermission> list)
        {
            var gkeys = this.GroupMenus.SelectMany(gm => gm.Value2.Select(lm => lm.Key)).ToList();
            var pkeys = list.Select(up => up.MenuId).ToList();

            pkeys.ForEach(p =>
            {
                if (gkeys.Any(g => g == p) == false)
                {
                    var up = list.FirstOrDefault(m => m.MenuId == p);

                    if (up != null)
                        list.Remove(up);
                }
            });
        }

        #endregion

        #region Internal Operations

        private void CreateApplicationMenus()
        {
            this.GroupMenus = new List<KeyValues<short, string, List<KeyValues<Guid, string, Uri>>>>();

            short gid, gstep;
            gid = 1000;

            // 1000
            gstep = 1000;
            var homeLinks = new List<KeyValues<Guid, string, Uri>>();
            var home = KeyValues.Create(gid, "Home", homeLinks);

            homeLinks.Add(KeyValues.Create(Guid.Parse("C77E879B-31E1-405C-A815-841EEB074FBD"),
                "Main", new Uri("/Views/MainView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("08DBF10A-4B9C-4D06-9788-B5727B50028C"),
                "Students", new Uri("/Views/StudentListView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("C1AD9A10-BAFD-4021-BD6B-C896F1EAD1B1"),
                "Search Students", new Uri("/Views/StudentSearchListView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("49CF2ADF-0BC1-475A-BFA8-BA6123A331F3"),
                "Addresses", new Uri("/Views/AddressListView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("A0A3D470-DED4-43F9-8040-B6B257DD14D8"),
                "Search Addresses", new Uri("/Views/AddressSearchListView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("03DD7230-61D6-4A63-AA30-4739D44D232E"),
                "Guardians", new Uri("/Views/GuardianListView.xaml", UriKind.Relative)));

            homeLinks.Add(KeyValues.Create(Guid.Parse("C1034874-1FFB-4FB4-B3D8-4BF1626EFEA3"),
                "Search Guardians", new Uri("/Views/GuardianSearchListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(home);

            // 2000
            gid += gstep;
            var accLinks = new List<KeyValues<Guid, string, Uri>>();
            var acc = KeyValues.Create(gid, "Account", accLinks);

            accLinks.Add(KeyValues.Create(Guid.Parse("0CD926B6-D747-47C2-B371-2DB62FEDB61B"),
                "Receipts", new Uri("/Views/ReceiptListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("1AED0776-B6C4-4CFF-9300-CADC22A84E99"),
                "Search Receipts", new Uri("/Views/ReceiptSearchListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("1751C1A0-38D8-4A14-9D0B-7048796471B2"),
                "Discounts", new Uri("/Views/StudentDiscountListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("CCE3FA49-2447-48C4-B348-FF3001D9B151"),
                "Search Discounts", new Uri("/Views/StudentDiscountSearchListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("FFFF566F-D6F1-4DF1-9D91-193C58F3F740"),
                "Ferry Fees", new Uri("/Views/StudentFerryFeeListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("5DBE1E3C-C1F3-4699-A8F3-C50DE18B90A0"),
                "Search Ferry Fees", new Uri("/Views/StudentFerryFeeSearchListView.xaml", UriKind.Relative)));

            accLinks.Add(KeyValues.Create(Guid.Parse("030FAB7A-9BDF-4489-8BE2-4B92AF3FE028"),
                "Reports", new Uri("/Views/AccountReportListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(acc);

            // 3000
            gid += gstep;
            var rptLinks = new List<KeyValues<Guid, string, Uri>>();
            var rpt = KeyValues.Create(gid, "Reports", rptLinks);

            rptLinks.Add(KeyValues.Create(Guid.Parse("661D019F-76E3-4A32-B20E-5ADA4F7773EF"),
                "Schools", new Uri("/Views/SchoolReportListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(rpt);

            // 4000
            gid += gstep;
            var schLinks = new List<KeyValues<Guid, string, Uri>>();
            var sch = KeyValues.Create(gid, "Schools", schLinks);

            schLinks.Add(KeyValues.Create(Guid.Parse("313E4FA5-F4DB-431B-96EA-B3924B8F3DE8"),
                "Schools", new Uri("/Views/SchoolListView.xaml", UriKind.Relative)));

            schLinks.Add(KeyValues.Create(Guid.Parse("895A6F8D-E250-4C63-BA5B-743839FDB9B3"),
                "Academic Years", new Uri("/Views/AcademicYearListView.xaml", UriKind.Relative)));

            schLinks.Add(KeyValues.Create(Guid.Parse("90AE6525-24C9-46F4-96BB-119B07B158C3"),
                "Courses", new Uri("/Views/CourseListView.xaml", UriKind.Relative)));

            schLinks.Add(KeyValues.Create(Guid.Parse("88FA1447-C3CC-4156-A2C6-5C71139B2BAE"),
                "Classes", new Uri("/Views/ClassListView.xaml", UriKind.Relative)));

            schLinks.Add(KeyValues.Create(Guid.Parse("89FD3AD7-ECDC-4D79-8614-64AD299E4ED2"),
                "Sections", new Uri("/Views/SectionListView.xaml", UriKind.Relative)));

            schLinks.Add(KeyValues.Create(Guid.Parse("989694AE-245D-44C1-8033-598AAE7F6E5D"),
                "Search Sections", new Uri("/Views/SectionSearchListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(sch);

            // 5000
            gid += gstep;
            var typeLinks = new List<KeyValues<Guid, string, Uri>>();
            var type = KeyValues.Create(gid, "Types", typeLinks);

            typeLinks.Add(KeyValues.Create(Guid.Parse("690B5E1E-E544-4576-B2EA-1480D31E7FC9"),
                "Address Types", new Uri("/Views/AddressTypeListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("659D6623-DA74-4311-95FB-EA1CD656AEEC"),
                "Email Types", new Uri("/Views/EmailTypeListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("026E81D2-1E0C-451D-995F-1A056D36A301"),
                "Phone Types", new Uri("/Views/PhoneTypeListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("8813BBA3-6E1C-4E68-A508-EDC90C7646ED"),
                "Guardian Types", new Uri("/Views/GuardianTypeListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("02607F25-31D3-4681-BB77-4039E7A75969"),
                "Countries", new Uri("/Views/CountryListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("8C17A322-E821-4B6A-AA98-3CC2DE464E9F"),
                "Religions", new Uri("/Views/ReligionListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("9507F3CF-A54E-4895-8A51-589501D910DB"),
                "Currencies", new Uri("/Views/CurrencyListView.xaml", UriKind.Relative)));

            typeLinks.Add(KeyValues.Create(Guid.Parse("173B036B-5FAF-42BA-A401-F438D2DF4BCF"),
                "Fee Types", new Uri("/Views/FeeTypeListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(type);

            // 6000
            gid += gstep;
            var secLinks = new List<KeyValues<Guid, string, Uri>>();
            var sec = KeyValues.Create(gid, "Security", secLinks);

            secLinks.Add(KeyValues.Create(Guid.Parse("FABF9DFC-5B30-4A9C-9370-BC04D092A3C6"),
                "Users", new Uri("/Views/UserListView.xaml", UriKind.Relative)));

            secLinks.Add(KeyValues.Create(Guid.Parse("2B7124A6-C26C-45B4-946D-FFEB391835F8"),
                "Search Users", new Uri("/Views/UserSearchListView.xaml", UriKind.Relative)));
            this.GroupMenus.Add(sec);
        }

        private void SetUserPermission(List<UserPermission> list, Guid uid, params short[] gmids)
        {
            Permission v = Permission.CanView;
            foreach (var sgm in this.GroupMenus.Where(gm => gmids.Contains(gm.Key)))
            {
                foreach (var gm in sgm.Value2)
                {
                    var p = new UserPermission { UserId = uid, MenuId = gm.Key };

                    p.SetPermission(v, true);
                    list.Add(p);
                }
            }
        }

        private void SetUserPermission(List<UserPermission> list, Guid uid, short gmid, params Guid[] ids)
        {
            var sgm = this.GroupMenus.FirstOrDefault(gm => gmid == gm.Key);
            if (sgm != null)
            {
                Permission v = Permission.CanView;
                foreach (var gm in sgm.Value2.Where(gm => ids.Contains(gm.Key)))
                {
                    var p = new UserPermission { UserId = uid, MenuId = gm.Key };

                    p.SetPermission(v, true);
                    list.Add(p);
                }
            }
        }

        private GenericPrincipal CreatePrincipal(string userName, string[] roles = null)
        {
            return new GenericPrincipal(new GenericIdentity(userName), roles);
        }

        private void LoadDefaultSettings(bool raisePropertyChanged = true)
        {
            this.applicationName = "SMS";
            this.passwordFormat = 1;
            this.maxPasswordFailures = 5;
            this.passwordAttemptWindow = 10;
            this.minRequiredNonAlphanumericCharacters = 1;
            this.minRequiredPasswordLength = 7;
            this.passwordStrengthRegularExpression = string.Empty;
            this.minRequiredSearchWordLength = 2;
            this.maxUsageCountToRefresh = 40;
            this.maxImageFileSize = 204800L;
            this.maxLogFileSize = 102400L;
            this.showAllErrors = true;
            this.reverseErrors = true;
            this.writeToLog = true;
            this.checkTimeZone = true;
            this.timeZone = new TimeSpan(6, 30, 0);
            this.checkDateTime = true;
            if (raisePropertyChanged)
            {
                //RaisePropertyChanged("ApplicationName");
                //RaisePropertyChanged("PasswordFormat");
                //RaisePropertyChanged("MaxPasswordFailures");
                //RaisePropertyChanged("PasswordAttemptWindow");
                //RaisePropertyChanged("MinRequiredNonAlphanumericCharacters");
                //RaisePropertyChanged("MinRequiredPasswordLength");
                //RaisePropertyChanged("PasswordStrengthRegularExpression");
                RaisePropertyChanged("MinRequiredSearchWordLength");
                RaisePropertyChanged("MaxUsageCountToRefresh");
                RaisePropertyChanged("MaxImageFileSize");
                RaisePropertyChanged("MaxLogFileSize");
                RaisePropertyChanged("ShowAllErrors");
                RaisePropertyChanged("ReverseErrors");
                RaisePropertyChanged("WriteToLog");
                RaisePropertyChanged("CheckTimeZone");
                RaisePropertyChanged("TimeZone");
                RaisePropertyChanged("CheckDateTime");
            }
        }

        private void RegisterTypes()
        {
            this.container = new UnityContainer();

            //this.container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            //this.container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<ISMSUnitOfWork, SMSUnitOfWork>();
        }

        private void ResetUser()
        {
            this.User = CreatePrincipal(string.Empty);
            this.UserId = null;
        }

        private void SystemLogon()
        {
            string userName = "Administrator";

            using (var uow = this.container.Resolve<ISMSUnitOfWork>())
            {
                var user = uow.GetUserByUserName(userName, asNoTracking: true);

                if (user == null)
                    user = uow.FirstOrDefaultAsNoTracking<User>();
                if (user == null)
                    FakeLogon(userName);
                else
                    SetLogonInfo(user);
            }
        }

        #endregion
    }
}
