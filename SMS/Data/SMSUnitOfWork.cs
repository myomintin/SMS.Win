using CodeflexSystems.Library.Converters;
using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.Library.Security;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Data
{
    [DataObject]
    internal sealed class SMSUnitOfWork : UnitOfWork, ISMSUnitOfWork
    {
        #region Fields

        private Lazy<ObservableCollection<AddressType>> addressTypes;
        private Lazy<ObservableCollection<Country>> countries;
        private Lazy<ObservableCollection<Country>> countriesAsNoTracking;
        private Lazy<ObservableCollection<Currency>> currencies;
        private Lazy<ObservableCollection<Currency>> currenciesAsNoTracking;
        private Lazy<ObservableCollection<EmailType>> emailTypes;
        private Lazy<ObservableCollection<FeeType>> feeTypes;
        private Lazy<ObservableCollection<GuardianType>> guardianTypes;
        private Lazy<ObservableCollection<GuardianType>> guardianTypesAsNoTracking;
        private Lazy<ObservableCollection<PhoneType>> phoneTypes;
        private Lazy<ObservableCollection<Religion>> religions;
        private Lazy<ObservableCollection<Religion>> religionsAsNoTracking;
        private Lazy<ObservableCollection<School>> schools;
        private Lazy<ObservableCollection<School>> schoolsAsNoTracking;

        #endregion

        #region Properties

        public ObservableCollection<AddressType> AddressTypes
        {
            get { return this.addressTypes.Value; }
        }

        public ObservableCollection<Country> Countries
        {
            get { return this.countries.Value; }
        }

        public ObservableCollection<Country> CountriesAsNoTracking
        {
            get { return this.countriesAsNoTracking.Value; }
        }

        public ObservableCollection<Currency> Currencies
        {
            get { return this.currencies.Value; }
        }

        public ObservableCollection<Currency> CurrenciesAsNoTracking
        {
            get { return this.currenciesAsNoTracking.Value; }
        }

        public ObservableCollection<EmailType> EmailTypes
        {
            get { return this.emailTypes.Value; }
        }

        public ObservableCollection<FeeType> FeeTypes
        {
            get { return this.feeTypes.Value; }
        }

        public ObservableCollection<GuardianType> GuardianTypes
        {
            get { return this.guardianTypes.Value; }
        }

        public ObservableCollection<GuardianType> GuardianTypesAsNoTracking
        {
            get { return this.guardianTypesAsNoTracking.Value; }
        }

        public ObservableCollection<PhoneType> PhoneTypes
        {
            get { return this.phoneTypes.Value; }
        }

        public ObservableCollection<Religion> Religions
        {
            get { return this.religions.Value; }
        }

        public ObservableCollection<Religion> ReligionsAsNoTracking
        {
            get { return this.religionsAsNoTracking.Value; }
        }

        public ObservableCollection<School> Schools
        {
            get { return this.schools.Value; }
        }

        public ObservableCollection<School> SchoolsAsNoTracking
        {
            get { return this.schoolsAsNoTracking.Value; }
        }

        #endregion

        #region Ctor, Dtor

        public SMSUnitOfWork(SMSContext context = null)
            : base(context)
        {
            this.addressTypes = new Lazy<ObservableCollection<AddressType>>(() =>
                this.SelectAll<AddressType>().OrderedList().ToObservableCollection());
            this.countries = new Lazy<ObservableCollection<Country>>(() =>
                this.SelectAll<Country>().OrderedList().ToObservableCollection());
            this.countriesAsNoTracking = new Lazy<ObservableCollection<Country>>(() =>
                this.SelectAllAsNoTracking<Country>().OrderedList().ToObservableCollection());
            this.currencies = new Lazy<ObservableCollection<Currency>>(() =>
                this.GetCurrencies().Ordered().ToObservableCollection());
            this.currenciesAsNoTracking = new Lazy<ObservableCollection<Currency>>(() =>
                this.GetCurrencies().Ordered().ToObservableCollection());
            this.emailTypes = new Lazy<ObservableCollection<EmailType>>(() =>
                this.SelectAll<EmailType>().OrderedList().ToObservableCollection());
            this.feeTypes = new Lazy<ObservableCollection<FeeType>>(() =>
                this.SelectAll<FeeType>().OrderedList().ToObservableCollection());
            this.guardianTypes = new Lazy<ObservableCollection<GuardianType>>(() =>
                this.SelectAll<GuardianType>().OrderedList().ToObservableCollection());
            this.guardianTypesAsNoTracking = new Lazy<ObservableCollection<GuardianType>>(() =>
                this.SelectAllAsNoTracking<GuardianType>().OrderedList().ToObservableCollection());
            this.phoneTypes = new Lazy<ObservableCollection<PhoneType>>(() =>
                this.SelectAll<PhoneType>().OrderedList().ToObservableCollection());
            this.religions = new Lazy<ObservableCollection<Religion>>(() =>
                this.SelectAll<Religion>().OrderedList().ToObservableCollection());
            this.religionsAsNoTracking = new Lazy<ObservableCollection<Religion>>(() =>
                this.SelectAllAsNoTracking<Religion>().OrderedList().ToObservableCollection());
            this.schools = new Lazy<ObservableCollection<School>>(() =>
                this.GetSchools().Ordered().ToObservableCollection());
            this.schoolsAsNoTracking = new Lazy<ObservableCollection<School>>(() =>
                this.GetSchools().Ordered().AsNoTracking().ToObservableCollection());
        }

        #endregion

        #region Methods

        public void Clear()
        {
            if (this.addressTypes.IsValueCreated)
                this.AddressTypes.Clear();
            if (this.countries.IsValueCreated)
                this.Countries.Clear();
            if (this.countriesAsNoTracking.IsValueCreated)
                this.CountriesAsNoTracking.Clear();
            if (this.currencies.IsValueCreated)
                this.Currencies.Clear();
            if (this.currenciesAsNoTracking.IsValueCreated)
                this.CurrenciesAsNoTracking.Clear();
            if (this.emailTypes.IsValueCreated)
                this.EmailTypes.Clear();
            if (this.feeTypes.IsValueCreated)
                this.FeeTypes.Clear();
            if (this.guardianTypes.IsValueCreated)
                this.GuardianTypes.Clear();
            if (this.guardianTypesAsNoTracking.IsValueCreated)
                this.GuardianTypesAsNoTracking.Clear();
            if (this.phoneTypes.IsValueCreated)
                this.PhoneTypes.Clear();
            if (this.religions.IsValueCreated)
                this.Religions.Clear();
            if (this.religionsAsNoTracking.IsValueCreated)
                this.ReligionsAsNoTracking.Clear();
            if (this.schools.IsValueCreated)
                this.Schools.Clear();
            if (this.schoolsAsNoTracking.IsValueCreated)
                this.SchoolsAsNoTracking.Clear();
        }

        public UserMembership CreateMemebership(string password, string passwordQuestion, string passwordAnswer)
        {
            var membership = new UserMembership();

            SetMemebership(membership, password, passwordQuestion, passwordAnswer);

            return membership;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool ChangePassword(User entity, string oldPassword, string newPassword, bool saveChanges = false)
        {
            //ApplicationManager.Current.ValidatePassword(oldPassword);
            ApplicationManager.Current.ValidatePassword(newPassword);

            if (entity == null || entity.UserMembership == null || entity.UserMembership.Disabled)
                return false;

            var result = SetPassword(entity, oldPassword, newPassword);
            SetPasswordFailure(entity, result);

            if (saveChanges)
                SaveChanges();

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool ResetPassword(User entity, string newPassword, bool saveChanges = false)
        {
            ApplicationManager.Current.ValidatePassword(newPassword);

            if (entity == null || entity.UserMembership == null || entity.UserMembership.Disabled)
                return false;

            var result = SetPassword(entity, newPassword);
            SetPasswordFailure(entity, result);

            if (saveChanges)
                SaveChanges();

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool ChangePasswordQuestionAndAnswer(User entity, string password, string newPasswordQuestion, string newPasswordAnswer, bool saveChanges = false)
        {
            ApplicationManager.Current.ValidatePassword(password);
            ApplicationManager.Current.ValidatePasswordQuestion(newPasswordQuestion);
            ApplicationManager.Current.ValidatePasswordAnswer(newPasswordAnswer);

            if (entity == null || entity.UserMembership == null || entity.UserMembership.Disabled)
                return false;

            var result = SetPasswordQuestionAndAnswer(entity, password, newPasswordQuestion, newPasswordAnswer);
            SetPasswordAnswerFailure(entity, result);

            if (saveChanges)
                SaveChanges();

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public User Delete(User entity, bool saveChanges = false)
        {
            if (entity == null || !entity.CanDelete)
                return null;

            return Remove(entity, saveChanges);
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public IEnumerable<User> DeleteRange(Expression<Func<User, bool>> predicate, bool saveChanges = false)
        {
            var entities = Where(predicate).Where(m => m.CanDelete);

            return RemoveRange(entities, saveChanges);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public User GetUser(object userId)
        {
            return GetUser(ConverterHelper.As<object, Guid>(userId));
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public User GetUser(Guid userId)
        {
            return FirstOrDefault<User>(u => u.UserId == userId);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public User GetUserByUserName(string userName, StringMatchType matchType = StringMatchType.Equal, bool asNoTracking = false)
        {
            ApplicationManager.Current.ValidateUserName(userName);
            var predicate = ExpressionHelper.CreatePredicate<User>("UserName", userName, matchType);

            if (asNoTracking)
                return FirstOrDefaultAsNoTracking(predicate);
            else
                return FirstOrDefault(predicate);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<User> GetUsersByUserName(string userName, StringMatchType matchType = StringMatchType.Equal)
        {
            ApplicationManager.Current.ValidateUserName(userName);
            var predicate = ExpressionHelper.CreatePredicate<User>("UserName", userName, matchType);

            return Where(predicate);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<User> GetUsersByEmail(string email, StringMatchType matchType = StringMatchType.Equal)
        {
            ApplicationManager.Current.ValidateEmail(email);
            var predicate = ExpressionHelper.CreatePredicate<User>("EmailAddress", email, matchType);

            return Where(predicate);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public string GetUserNameById(Guid? id)
        {
            if (id.HasValue)
            {
                //var predicate = ExpressionHelper.CreatePredicate<User>("UserId", id.Value, ValueMatchType.Equal);
                //var entity = FirstOrDefaultAsNoTracking<User>(predicate);
                var entity = FirstOrDefaultAsNoTracking<User>(m => m.UserId == id.Value);

                if (entity == null)
                    return null;

                return entity.UserName;
            }

            return null;
        }

        public bool Logon(string userName, string password, bool persistCookie = false)
        {
            var entity = GetUserByUserName(userName);
            var result = ValidateUser(entity, password);

            //if (result)
            //{
            //    //ApplicationManager.Current.SetLogonInfo(entity);
            //    SecurityHelper.SetAuthCookie(entity.UserName, persistCookie);
            //}

            return result;
        }

        public void Logout()
        {
            ////ApplicationManager.Current.Logoff();
            //SecurityHelper.SignOut();
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool UnlockUser(User entity, bool saveChanges = false)
        {
            if (entity == null)
                return false;

            if (entity.UserMembership.Disabled)
            {
                entity.UserMembership.Disabled = false;
                entity.UserMembership.DisabledDate = DateTime.UtcNow;
            }

            if (saveChanges)
                SaveChanges();

            return true;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<Currency> GetAllCurrencies()
        {
            if (ApplicationManager.b)
                return this.Context.Currencies;
            else
                return this.Context.Currencies.Take(ApplicationManager.c);
            //else
            //{
            //    var currencyCode = ApplicationManager.Current.CurrencyCode;
            //    var query = this.Where<Currency>(m => m.CurrencyCode == currencyCode);

            //    if (query.Count() == 0)
            //        return this.Context.Currencies.Take(1);
            //    else
            //        return query;
            //}
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<Currency> GetCurrencies()
        {
            if (ApplicationManager.b)
                return this.Context.Currencies;
            else
                return this.Context.Currencies.Take(ApplicationManager.c);
            //else
            //{
            //    var currencyCode = ApplicationManager.Current.CurrencyCode;
            //    var query = this.Where<Currency>(m => m.CurrencyCode == currencyCode);

            //    if (query.Count() == 0)
            //        return this.Context.Currencies.Take(1);
            //    else
            //        return query;
            //}
        }

        public bool AddCurrencyById(ICollection<Currency> source, Guid id)
        {
            bool flag = false;

            if (source != null && !object.Equals(id, default(Guid)) &&
                !source.Any(m => m.CurrencyId == id))
            {
                var value = this.FirstOrDefault<Currency>(m => m.CurrencyId == id);

                if (value != null)
                {
                    source.Add(value);
                    flag = true;
                }
            }

            return flag;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<School> GetAllSchools()
        {
            if (ApplicationManager.d)
                return this.Context.Schools;
            else
                return this.Context.Schools.Take(ApplicationManager.e);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<School> GetSchools()
        {
            if (ApplicationManager.d)
                return this.Context.Schools;
            else
                return this.Context.Schools.Take(ApplicationManager.e);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<School> GetSchools(Expression<Func<School, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return GetSchools().Where(predicate);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentSection> GetStudentSections(Expression<Func<StudentSection, bool>> predicate, Guid? schoolId, bool? activeAcademicYear)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var query = Where(predicate);

            if (schoolId.HasValue)
                query = from t in query
                        where t.Section.Class.Course.AcademicYear.SchoolId == schoolId.Value
                        select t;
            if (activeAcademicYear.HasValue)
                query = from t in query
                        where t.Section.Class.Course.AcademicYear.Active == activeAcademicYear.Value
                        select t;

            return query;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentSection> GetStudentSections(Guid? schoolId, bool? activeAcademicYear)
        {
            IQueryable<StudentSection> query;

            if (schoolId.HasValue)
                query = from t in this.Context.StudentSections
                        where t.Section.Class.Course.AcademicYear.SchoolId == schoolId.Value
                        select t;
            else
                query = this.Context.StudentSections;
            if (activeAcademicYear.HasValue)
                query = from t in query
                        where t.Section.Class.Course.AcademicYear.Active == activeAcademicYear.Value
                        select t;

            return query;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentSectionInfo> GetStudentSectionInfos(Expression<Func<StudentSection, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return from t in Where(predicate)
                   select new StudentSectionInfo
                   {
                       StudentSection = t,
                       Student = t.Student,
                       Section = t.Section,
                       Class = t.Section.Class,
                       Course = t.Section.Class.Course,
                       AcademicYear = t.Section.Class.Course.AcademicYear,
                       School = t.Section.Class.Course.AcademicYear.School
                   };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentSectionInfo> GetStudentSectionInfosById(string id, Guid? schoolId, bool? activeAcademicYear)
        {
            return from t in GetStudentSections(m => m.Student.Id.Contains(id), schoolId, activeAcademicYear)
                   select new StudentSectionInfo
                   {
                       StudentSection = t,
                       Student = t.Student,
                       Section = t.Section,
                       Class = t.Section.Class,
                       Course = t.Section.Class.Course,
                       AcademicYear = t.Section.Class.Course.AcademicYear,
                       School = t.Section.Class.Course.AcademicYear.School
                   };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentSectionInfo> GetStudentSectionInfosByName(string name, Guid? schoolId, bool? activeAcademicYear)
        {
            var query1 = GetStudentSections(m => m.Student.StudentName.Contains(name), schoolId, activeAcademicYear);
            var query2 = GetStudentSections(m => m.Student.OtherName.Contains(name), schoolId, activeAcademicYear);

            return from t in query1.Union(query2)
                   select new StudentSectionInfo
                   {
                       StudentSection = t,
                       Student = t.Student,
                       Section = t.Section,
                       Class = t.Section.Class,
                       Course = t.Section.Class.Course,
                       AcademicYear = t.Section.Class.Course.AcademicYear,
                       School = t.Section.Class.Course.AcademicYear.School
                   };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentDiscount> GetStudentDiscounts(Guid? courseId)
        {
            IQueryable<StudentDiscount> query;

            if (courseId.HasValue)
                query = from t in this.Context.StudentDiscounts
                        where t.StudentSection.Section.Class.CourseId == courseId.Value
                        select t;
            else
                query = from t in this.Context.StudentDiscounts
                        select t;

            return query;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<StudentFerryFee> GetStudentFerryFees(Guid? courseId)
        {
            IQueryable<StudentFerryFee> query;

            if (courseId.HasValue)
                query = from t in this.Context.StudentFerryFees
                        where t.StudentSection.Section.Class.CourseId == courseId.Value
                        select t;
            else
                query = from t in this.Context.StudentFerryFees
                        select t;

            return query;
        }

        public IEnumerable<Currency> GetCurrenciesAsNoTracking(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.CurrenciesAsNoTracking.FirstOrDefault(m => m.CurrencyId == id);
                if (value != null)
                    yield return value;
            }
            yield break;
        }

        public IEnumerable<Currency> GetCurrenciesAsNoTracking(params Guid[] ids)
        {
            return GetCurrenciesAsNoTracking(ids.AsEnumerable());
        }

        public IEnumerable<FeeType> GetFeeTypesAsNoTracking(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.FirstOrDefaultAsNoTracking<FeeType>(m => m.FeeTypeId == id);
                if (value != null)
                    yield return value;
            }
            yield break;
        }

        public IEnumerable<FeeType> GetFeeTypesAsNoTracking(params Guid[] ids)
        {
            return GetFeeTypesAsNoTracking(ids.AsEnumerable());
        }

        public IEnumerable<School> GetSchoolsAsNoTracking(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.SchoolsAsNoTracking.FirstOrDefault(m => m.SchoolId == id);
                if (value != null)
                    yield return value;
            }
            yield break;
        }

        public IEnumerable<School> GetSchoolsAsNoTracking(params Guid[] ids)
        {
            return GetSchoolsAsNoTracking(ids.AsEnumerable());
        }

        #endregion

        #region IMembershipProvider

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool ChangePassword(string userName, string oldPassword, string newPassword, bool saveChanges = false)
        {
            return ChangePassword(GetUserByUserName(userName), oldPassword, newPassword, saveChanges);
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool ChangePasswordQuestionAndAnswer(string userName, string password, string newPasswordQuestion, string newPasswordAnswer, bool saveChanges = false)
        {
            return ChangePasswordQuestionAndAnswer(GetUserByUserName(userName), password, newPasswordQuestion, newPasswordAnswer, saveChanges);
        }

        //public User ConvertTo(System.Web.Security.MembershipUser mentity)
        //{
        //    if (mentity == null)
        //        return null;

        //    var entity = new User();
        //    var id = ConverterHelper.As<object, Guid>(mentity.ProviderUserKey);

        //    entity.UserId = id;
        //    entity.UserName = mentity.UserName;
        //    entity.EmailAddress = mentity.Email;
        //    entity.CreatedDate = mentity.CreationDate;
        //    entity.UserMembership.PasswordQuestion = mentity.PasswordQuestion;
        //    entity.UserMembership.Disabled = mentity.IsLockedOut;
        //    entity.UserMembership.LogonDate = mentity.LastLoginDate;
        //    entity.UserMembership.PasswordModifiedDate = mentity.LastPasswordChangedDate;
        //    entity.UserMembership.DisabledDate = mentity.LastLockoutDate;

        //    return entity;
        //}

        //public System.Web.Security.MembershipUser ConvertTo(User entity)
        //{
        //    if (entity == null)
        //        return null;

        //    var mentity = new System.Web.Security.MembershipUser(
        //        ApplicationManager.Current.ApplicationName,
        //        entity.UserName,
        //        entity.UserId,
        //        entity.EmailAddress,
        //        entity.UserMembership.PasswordQuestion,
        //        null,
        //        true,
        //        entity.UserMembership.Disabled,
        //        entity.CreatedDate,
        //        entity.UserMembership.LogonDate ?? DateTime.UtcNow,
        //        DateTime.UtcNow,
        //        entity.UserMembership.PasswordModifiedDate ?? DateTime.MinValue,
        //        entity.UserMembership.DisabledDate ?? DateTime.MinValue);

        //    return mentity;
        //}

        //public System.Web.Security.MembershipUserCollection ConvertTo(IQueryable<User> entities)
        //{
        //    var collection = new System.Web.Security.MembershipUserCollection();

        //    foreach (var entity in entities)
        //        collection.Add(ConvertTo(entity));

        //    return collection;
        //}

        //public void Initialize(System.Web.Security.MembershipProvider membershipProvider)
        //{
        //    ApplicationManager.Current.ApplicationName = membershipProvider.ApplicationName;
        //    ApplicationManager.Current.MaxPasswordFailures = membershipProvider.MaxInvalidPasswordAttempts;
        //    ApplicationManager.Current.MinRequiredNonAlphanumericCharacters = membershipProvider.MinRequiredNonAlphanumericCharacters;
        //    ApplicationManager.Current.MinRequiredPasswordLength = membershipProvider.MinRequiredPasswordLength;
        //    ApplicationManager.Current.PasswordAttemptWindow = membershipProvider.PasswordAttemptWindow;
        //    ApplicationManager.Current.PasswordFormat = (byte)membershipProvider.PasswordFormat;
        //    ApplicationManager.Current.PasswordStrengthRegularExpression = membershipProvider.PasswordStrengthRegularExpression;
        //}

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public User CreateUser(string userName, string email, string password, string passwordQuestion, string passwordAnswer, bool saveChanges = false)
        {
            ApplicationManager.Current.ValidateUserName(userName);
            ApplicationManager.Current.ValidateEmail(email);
            ApplicationManager.Current.ValidatePassword(password);
            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                ApplicationManager.Current.ValidatePasswordQuestion(passwordQuestion);
                ApplicationManager.Current.ValidatePasswordAnswer(passwordAnswer);
            }

            if (ExistsUserName(userName))
                throw new Exception(SR.GetString("AlreadyExists1", "User Name", userName));

            if (ExistsEmail(email))
                throw new Exception(SR.GetString("AlreadyExists1", "Email", email));

            var entity = new User();

            entity.UserName = userName;
            entity.EmailAddress = email;
            entity.UserMembership = CreateMemebership(password, passwordQuestion, passwordAnswer);

            return Add(entity, saveChanges);
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public bool DeleteUser(string userName, bool deleteAllRelatedData, bool saveChanges = false)
        {
            var entity = GetUserByUserName(userName);

            return Delete(entity, saveChanges) != null;
        }

        public bool ExistsEmail(string email)
        {
            return Any<User>(m => email.Equals(m.EmailAddress, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool ExistsUserId(object userId)
        {
            var uid = ConverterHelper.As<object, Guid>(userId);

            return Any<User>(m => uid == m.UserId);
        }

        public bool ExistsUserName(string userName)
        {
            return Any<User>(m => userName.Equals(m.UserName, StringComparison.InvariantCultureIgnoreCase));
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<User> FindUsersByEmail(string email, int pageIndex, int pageSize, out int totalRecords)
        {
            var entities = GetUsersByEmail(email);

            totalRecords = entities.Count();

            return entities.OrderBy(m => m.UserId).Skip(pageIndex * pageSize).Take(pageSize);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<User> FindUsersByName(string userName, int pageIndex, int pageSize, out int totalRecords)
        {
            var entities = GetUsersByUserName(userName);

            totalRecords = entities.Count();

            return entities.OrderBy(u => u.UserId).Skip(pageIndex * pageSize).Take(pageSize);
        }

        public string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            //return SecurityHelper.GeneratePassword(length, numberOfNonAlphanumericCharacters);
            return null;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IQueryable<User> GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = Count<User>();

            return OrderBy<User, Guid>(m => m.UserId).Skip(pageIndex * pageSize).Take(pageSize);
        }

        public int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException(SR.GetString("IsNotSupported", "Number of users online"));
        }

        public string GetPassword(string userName, string answer)
        {
            throw new NotSupportedException(SR.GetString("IsNotSupported", "Password retrieval"));
        }

        public DateTime GetPasswordAnswerChangedDate(string userName)
        {
            var entity = GetUserByUserName(userName, asNoTracking: true);
            var result = DateTime.MinValue;

            if (entity != null && entity.UserMembership.PasswordAnswerModifiedDate.HasValue)
                result = entity.UserMembership.PasswordAnswerModifiedDate.Value;

            return result;
        }

        public DateTime GetPasswordAnswerFailureDate(string userName)
        {
            var entity = GetUserByUserName(userName, asNoTracking: true);
            var result = DateTime.MinValue;

            if (entity != null && entity.UserMembership.PasswordAnswerFailureDate.HasValue)
                result = entity.UserMembership.PasswordAnswerFailureDate.Value;

            return result;
        }

        public DateTime GetPasswordChangedDate(string userName)
        {
            var entity = GetUserByUserName(userName, asNoTracking: true);
            var result = DateTime.MinValue;

            if (entity != null && entity.UserMembership.PasswordModifiedDate.HasValue)
                result = entity.UserMembership.PasswordModifiedDate.Value;

            return result;
        }

        public DateTime GetPasswordFailureDate(string userName)
        {
            var entity = GetUserByUserName(userName, asNoTracking: true);
            var result = DateTime.MinValue;

            if (entity != null && entity.UserMembership.PasswordFailureDate.HasValue)
                result = entity.UserMembership.PasswordFailureDate.Value;

            return result;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public User GetUser(string userName, bool userIsOnline)
        {
            return GetUserByUserName(userName);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public User GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetUser(providerUserKey);
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public string GetUserNameByEmail(string email)
        {
            ApplicationManager.Current.ValidateEmail(email);
            //var predicate = ExpressionHelper.CreatePredicate<User>("EmailAddress", email, StringMatchType.Equal);
            //var entity = FirstOrDefaultAsNoTracking<User>(predicate);
            var entity = FirstOrDefaultAsNoTracking<User>(m => email.Equals(m.EmailAddress, StringComparison.InvariantCultureIgnoreCase));

            if (entity == null)
                return null;

            return entity.UserName;
        }

        public bool IsValidEmail(string email)
        {
            return ApplicationManager.Current.IsValidEmail(email);
        }

        public bool IsValidPassword(string password)
        {
            return ApplicationManager.Current.IsValidPassword(password);
        }

        public bool IsValidPasswordAnswer(string passwordAnswer)
        {
            return ApplicationManager.Current.IsValidPasswordAnswer(passwordAnswer);
        }

        public bool IsValidPasswordQuestion(string passwordQuestion)
        {
            return ApplicationManager.Current.IsValidPasswordQuestion(passwordQuestion);
        }

        public bool IsValidProviderUserKey(object userId)
        {
            try
            {
                return ConverterHelper.Is<object, Guid>(userId);
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidUserName(string userName)
        {
            return ApplicationManager.Current.IsValidUserName(userName);
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public string ResetPassword(string userName, string answer)
        {
            throw new NotSupportedException(SR.GetString("IsNotSupported", "Password reset"));
        }

        public void SetMemebership(UserMembership membership, string password, string passwordQuestion, string passwordAnswer)
        {
            if (membership == null)
                throw new ArgumentNullException("membership");

            var utc = DateTime.UtcNow;

            membership.PasswordFormat = ApplicationManager.Current.PasswordFormat;
            if (membership.PasswordFormat == PasswordFormat.Rfc2898Hash)
                password = Crypto.HashRfc2898(password).ToBase64String();

            if (password.Length > 128)
                throw new ArgumentException(SR.GetString("IsTooLong", "Password hash", 128));

            membership.Password = password;
            membership.PasswordModifiedDate = utc;

            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                if (membership.PasswordFormat == PasswordFormat.Rfc2898Hash)
                    passwordAnswer = Crypto.HashRfc2898(passwordAnswer.Trim()).ToBase64String();

                if (passwordAnswer.Length > 128)
                    throw new ArgumentException(SR.GetString("IsTooLong", "Password Answer hash", 128));

                membership.PasswordAnswer = passwordAnswer;
                membership.PasswordQuestion = passwordQuestion;
                membership.PasswordAnswerModifiedDate = utc;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public bool UnlockUser(string userName, bool saveChanges = false)
        {
            return UnlockUser(GetUserByUserName(userName), saveChanges);
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateUser(User entity, bool saveChanges = false)
        {
            Modify(entity, saveChanges);
        }

        public bool ValidateUser(string userName, string password)
        {
            return ValidateUser(GetUserByUserName(userName), password);
        }

        public bool ValidateUser(User entity, string password)
        {
            if (entity == null || entity.UserMembership.Disabled)
                return false;

            var result = CheckPassword(entity, password);
            SetPasswordFailure(entity, result);
            if (result)
                entity.UserMembership.LogonDate = DateTime.UtcNow;
            SaveChanges();

            return result;
        }

        #endregion

        #region Internal Operations

        private bool CheckPassword(User entity, string password)
        {
            if (entity == null)
                return false;

            return Crypto.VerifyHashedRfc2898(entity.UserMembership.Password, password);
        }

        private bool CheckPasswordAnswer(User entity, string answer)
        {
            if (entity == null)
                return false;

            return Crypto.VerifyHashedRfc2898(
                entity.UserMembership.PasswordAnswer.ToLower(CultureInfo.InvariantCulture),
                answer.Trim().ToLower(CultureInfo.InvariantCulture));
        }

        private bool SetPassword(User entity, string oldPassword, string newPassword)
        {
            if (CheckPassword(entity, oldPassword))
                return SetPassword(entity, newPassword);

            return false;
        }

        private bool SetPassword(User entity, string newPassword)
        {
            if (entity.UserMembership.PasswordFormat == PasswordFormat.Rfc2898Hash)
                newPassword = Crypto.HashRfc2898(newPassword).ToBase64String();

            if (newPassword.Length > 128)
                throw new ArgumentException(SR.GetString("IsTooLong", "Password hash", 128));

            entity.UserMembership.Password = newPassword;
            entity.UserMembership.PasswordModifiedDate = DateTime.UtcNow;

            return true;
        }

        private bool SetPasswordQuestionAndAnswer(User entity, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (CheckPassword(entity, password))
            {
                if (entity.UserMembership.PasswordFormat == PasswordFormat.Rfc2898Hash)
                    newPasswordAnswer = Crypto.HashRfc2898(newPasswordAnswer.Trim()).ToBase64String();

                if (newPasswordAnswer.Length > 128)
                    throw new ArgumentException(SR.GetString("IsTooLong", "Password Answer hash", 128));

                entity.UserMembership.PasswordAnswer = newPasswordAnswer;
                entity.UserMembership.PasswordQuestion = newPasswordQuestion;
                entity.UserMembership.PasswordAnswerModifiedDate = DateTime.UtcNow;

                return true;
            }

            return false;
        }

        private void SetPasswordFailure(User entity, bool reset = false)
        {
            if (reset)
            {
                entity.UserMembership.PasswordFailures = 0;
                entity.UserMembership.PasswordFailureDate = null;
            }
            else
            {
                var count = entity.UserMembership.PasswordFailures + 1;
                var date = entity.UserMembership.PasswordFailureDate;
                var utc = DateTime.UtcNow;

                if (date.HasValue)
                {
                    if (date.Value.AddMinutes(ApplicationManager.Current.PasswordAttemptWindow) <= utc)
                        entity.UserMembership.PasswordFailures = 1;
                    else
                        entity.UserMembership.PasswordFailures = (byte)count;
                    entity.UserMembership.PasswordFailureDate = utc;
                }

                if (entity.UserMembership.PasswordFailures >= ApplicationManager.Current.MaxPasswordFailures)
                {
                    entity.UserMembership.Disabled = true;
                    entity.UserMembership.DisabledDate = utc;
                }
            }
        }

        private void SetPasswordAnswerFailure(User entity, bool reset = false)
        {
            if (reset)
            {
                entity.UserMembership.PasswordAnswerFailures = 0;
                entity.UserMembership.PasswordAnswerFailureDate = null;
            }
            else
            {
                var count = entity.UserMembership.PasswordAnswerFailures + 1;
                var date = entity.UserMembership.PasswordAnswerFailureDate;
                var utc = DateTime.UtcNow;

                if (date.HasValue)
                {
                    if (date.Value.AddMinutes(ApplicationManager.Current.PasswordAttemptWindow) <= utc)
                        entity.UserMembership.PasswordAnswerFailures = 1;
                    else
                        entity.UserMembership.PasswordAnswerFailures = (byte)count;
                    entity.UserMembership.PasswordAnswerFailureDate = utc;
                }

                if (entity.UserMembership.PasswordAnswerFailures >= ApplicationManager.Current.MaxPasswordFailures)
                {
                    entity.UserMembership.Disabled = true;
                    entity.UserMembership.DisabledDate = utc;
                }
            }
        }

        #endregion
    }
}
