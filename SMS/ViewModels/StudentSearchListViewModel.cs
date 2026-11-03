using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentSearchListViewModel : SearchListOptionViewModelBDSCR<StudentViewModel, Student, StudentSearchOption>,
        IAddressParent, IGuardianParent, IStudentParent
    {
        # region Properties

        public AddViewModel AddGuardianViewModel { get; private set; }
        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchSectionInfoViewModel EducationSelector { get; private set; }
        public SearchGuardianViewModel GuardianSelector { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }
        public int TotalStudents { get; set; }

        # endregion

        # region Ctor, Dtor

        public StudentSearchListViewModel()
            : base(true, true)
        {
            Initialize();
            InitializeCommands();
            UpdateTotalStudents();
        }

        # endregion

        # region Methods

        protected override void OnDispose()
        {
            base.OnDispose();
            DisposeUoW();
        }

        private void DisposeUoW()
        {
            if (this.UoW != null)
                this.UoW.Dispose();
        }

        internal void ClearAllCollections()
        {
            this.GuardianSelector.Clear();
            this.AddressSelector.Clear();
            Clear();
            this.UoW.Clear();
            this.TotalStudents = 0;
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.AddGuardianViewModel = AddViewModel.CreateAddGuardian();
                this.AddGuardianViewModel.IsExisting = true;
                this.AddAddressViewModel = AddViewModel.CreateAddAddress();
                this.AddAddressViewModel.IsExisting = true;

                this.EducationSelector = new SearchSectionInfoViewModel();
                this.GuardianSelector = new SearchGuardianViewModel();
                this.AddressSelector = new SearchAddressViewModel();

                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetStudentSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 3;

                this.ConvertTo = (m) =>
                {
                    var vm = new StudentViewModel(m, this);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Student>
                            (model => model.StudentId == vm.Model.StudentId).CanDelete;
                    };

                    return vm;
                };
                this.RemoveAdded = true;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

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

        private void UpdateTotalStudents()
        {
            if (!ApplicationManager.f)
            {
                var q = from y in this.UoW.Context.AcademicYears.Where(m => m.Active)
                        from r in y.Courses
                        from c in r.Classes
                        from s in c.Sections
                        from x in s.StudentSections.Where(m => m.Active)
                        select x.StudentId;
                this.TotalStudents = q.Distinct().Count();
            }
            else
                this.TotalStudents = 0;
        }

        protected override void OnSearch()
        {
            try
            {
                App.CursorWait();
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
                        var id = GetCountryId();
                        query = this.UoW.Where<Student>(m => m.BirthCountryId == id);
                    }
                    else if (current.Option == StudentSearchOption.Religion)
                    {
                        var id = GetReligionId();
                        query = this.UoW.Where<Student>(m => m.ReligionId == id);
                    }
                }
                else if (current.SearchValueType == SearchValueType.NoneType)
                {
                    if (current.Option == StudentSearchOption.NoUsage)
                    {
                        query = this.UoW.Where<Student>(m => m.StudentSections.Count == 0);
                    }
                }

                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Ordered().To(m => this.ConvertTo(m)), false);
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

        public override bool Delete(StudentViewModel vm)
        {
            bool flag = this.Collection.Delete(vm);

            if (flag)
            {
                vm.DetectChanges();
                this.UoW.SaveChanges();
                this.Collection.Remove(vm);
            }

            return flag;
        }

        protected override void OnDelete()
        {
            try
            {
                App.CursorWait();
                Delete(this.Collection.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(StudentViewModel vm)
        {
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowStudent(vm).DialogResult == true)
                    Edit(vm);
                else
                {
                    if (vm.HasAnyErrors)
                    {
                        DialogResult result = DialogManager.AskConfirmation("Confirmation",
                            SR.GetString("EditDataAsk3", "student"),
                            SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                        if (result == DialogResult.Yes)
                            flag = true;
                        else
                        {
                            OnEdit();
                            return;
                        }
                    }
                    else if (vm.CanSave)
                        flag = true;
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Data", ex);
            }
        }

        internal override void OnSave()
        { }

        private void Setup(StudentViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Student");
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        # endregion
    }
}
