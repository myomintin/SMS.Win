using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class SearchStudentViewModel : SearchOptionViewModelBDSCR<Student, StudentSearchOption>
    {
        #region Ctor, Dtor

        public SearchStudentViewModel(ISMSUnitOfWork uow = null)
            : base(uow, true, true)
        {
            this.IsLoaded = false;
            this.SearchOptionInfos.AddRange(SearchOptionHelper.GetStudentSearchOptions()
                .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
            this.SearchOptionInfos.SelectedIndex = 3;
            this.OnSearch = () => this.DoSearch();
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected override void SearchOptionInfoChanged()
        {
            if (this.SearchOptionInfos.SelectedIndex == -1)
            {
                this.CanSearch = this.ShowBooleanMatchType = this.ShowDateMatchType =
                    this.ShowStringMatchType = this.ShowCountryMatchType =
                    this.ShowReligionMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowCountryMatchType = current.Option == StudentSearchOption.BirthCountry;
                this.ShowReligionMatchType = current.Option == StudentSearchOption.Religion;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.ShowCountryMatchType)
                    CountryChanged();
                if (this.ShowReligionMatchType)
                    ReligionChanged();
            }
        }

        public IQueryable<Student> GetQuery()
        {
            if (!this.IsLoaded || !this.CanSearch)
                return null;

            var current = this.SearchOptionInfos.SelectedItem;
            IQueryable<Student> query = null;

            if (current.SearchValueType == SearchValueType.Boolean)
            {
                query = GetBooleanQuery<StudentSection>().Select(m => m.Student);
            }
            else if (current.SearchValueType == SearchValueType.DateTime)
            {
                if (current.Option == StudentSearchOption.RegisteredDate ||
                    current.Option == StudentSearchOption.ResignedDate)
                {
                    query = GetDateQuery<StudentSection>().Select(m => m.Student);
                }
                else
                {
                    query = GetDateQuery<Student>();
                }
            }
            else if (current.SearchValueType == SearchValueType.Text)
            {
                if (current.Option == StudentSearchOption.CreatedBy)
                {
                    query = GetStringQuery<Student, User, Guid?>(m => m.CreatedBy == null,
                        "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                }
                else if (current.Option == StudentSearchOption.ModifiedBy)
                {
                    query = GetStringQuery<Student, User, Guid?>(m => m.ModifiedBy == null,
                        "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                }
                else if (current.Option == StudentSearchOption.StudentFullName)
                {
                    query = GetStringQuery<Student>("StudentName", "OtherName");
                }
                else
                {
                    query = GetStringQuery<Student>();
                }
            }
            else if (current.SearchValueType == SearchValueType.Object)
            {
                if (current.Option == StudentSearchOption.BirthCountry)
                {
                    if (this.Countries.SelectedIndex == 0)
                        query = this.UoW.Where<Student>(m => m.BirthCountryId == null);
                    else
                    {
                        var id = this.Countries.SelectedItem.CountryId;
                        query = this.UoW.Where<Student>(m => m.BirthCountryId == id);
                    }
                }
                else if (current.Option == StudentSearchOption.Religion)
                {
                    if (this.Religions.SelectedIndex == 0)
                        query = this.UoW.Where<Student>(m => m.ReligionId == null);
                    else
                    {
                        var id = this.Religions.SelectedItem.ReligionId;
                        query = this.UoW.Where<Student>(m => m.ReligionId == id);
                    }
                }
            }
            else if (current.SearchValueType == SearchValueType.NoneType)
            {
                if (current.Option == StudentSearchOption.NoUsage)
                {
                    query = this.UoW.Where<Student>(m => m.StudentSections.Count == 0);
                }
            }

            return query;
        }

        public void DoSearch()
        {
            try
            {
                App.CursorWait();
                var query = this.GetQuery();

                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Ordered().AsNoTracking(), false);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog(this.ViewName, ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        #endregion
    }
}
