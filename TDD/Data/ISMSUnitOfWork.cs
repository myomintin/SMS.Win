using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Security;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data
{
    internal interface ISMSUnitOfWork : IUnitOfWork, IMembershipProvider<User>
    {
        ObservableCollection<AddressType> AddressTypes { get; }
        ObservableCollection<Country> Countries { get; }
        ObservableCollection<Country> CountriesNoTracking { get; }
        ObservableCollection<Currency> Currencies { get; }
        ObservableCollection<Currency> CurrenciesNoTracking { get; }
        ObservableCollection<EmailType> EmailTypes { get; }
        ObservableCollection<FeeType> FeeTypes { get; }
        ObservableCollection<GuardianType> GuardianTypes { get; }
        ObservableCollection<GuardianType> GuardianTypesNoTracking { get; }
        ObservableCollection<PhoneType> PhoneTypes { get; }
        ObservableCollection<Religion> Religions { get; }
        ObservableCollection<Religion> ReligionsNoTracking { get; }
        ObservableCollection<School> SchoolsNoTracking { get; }

        void Clear();

        UserMembership CreateMemebership(string password, string passwordQuestion, string passwordAnswer);
        bool ChangePassword(User entity, string oldPassword, string newPassword, bool saveChanges = false);
        bool ChangePasswordQuestionAndAnswer(User entity, string password, string newPasswordQuestion, string newPasswordAnswer, bool saveChanges = false);
        User Delete(User entity, bool saveChanges = false);
        IEnumerable<User> DeleteRange(Expression<Func<User, bool>> filterExpression, bool saveChanges = false);
        User GetUser(Guid userId);
        User GetUser(object userId);
        User GetUserByUserName(string userName, StringMatchType matchType = StringMatchType.Equal, bool asNoTracking = false);
        IQueryable<User> GetUsersByUserName(string userName, StringMatchType matchType = StringMatchType.Equal);
        IQueryable<User> GetUsersByEmail(string email, StringMatchType matchType = StringMatchType.Equal);
        string GetUserNameById(Guid? id);
        bool Logon(string userName, string password, bool persistCookie = false);
        void Logout();
        bool ResetPassword(User entity, string newPassword, bool saveChanges = false);
        void SetMemebership(UserMembership membership, string password, string passwordQuestion, string passwordAnswer);
        bool UnlockUser(User user, bool saveChanges = false);
        bool ValidateUser(User entity, string password);

        IQueryable<Currency> GetAllCurrencies();
        IQueryable<Currency> GetCurrencies();
        IQueryable<School> GetAllSchools();
        IQueryable<School> GetSchools();
        IQueryable<School> GetSchools(Expression<Func<School, bool>> predicate);
        IQueryable<StudentSection> GetStudentSections(Expression<Func<StudentSection, bool>> predicate, Guid? schoolId, bool? activeAcademicYear);
        IQueryable<StudentSection> GetStudentSections(Guid? schoolId, bool? activeAcademicYear);
        IQueryable<StudentSectionInfo> GetStudentSectionInfos(Expression<Func<StudentSection, bool>> predicate);
        IQueryable<StudentSectionInfo> GetStudentSectionInfosById(string id, Guid? schoolId, bool? activeAcademicYear);
        IQueryable<StudentSectionInfo> GetStudentSectionInfosByName(string name, Guid? schoolId, bool? activeAcademicYear);
        IQueryable<StudentDiscount> GetStudentDiscounts(Guid? courseId);
        IQueryable<StudentFerryFee> GetStudentFerryFees(Guid? courseId);
        IEnumerable<Currency> GetCurrencies(IEnumerable<Guid> ids);
        IEnumerable<Currency> GetCurrencies(params Guid[] ids);
        IEnumerable<FeeType> GetFeeTypes(IEnumerable<Guid> ids);
        IEnumerable<FeeType> GetFeeTypes(params Guid[] ids);
        IEnumerable<School> GetSchools(params Guid[] ids);
        IEnumerable<School> GetSchools(IEnumerable<Guid> ids);
    }
}
