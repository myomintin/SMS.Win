using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentListViewModel : ListViewModelPaging<StudentViewModel, Student>,
        IAddressParent, IGuardianParent, IStudentParent
    {
        #region Properties

        public AddViewModel AddGuardianViewModel { get; private set; }
        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchSectionInfoViewModel EducationSelector { get; private set; }
        public SearchGuardianViewModel GuardianSelector { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }
        public int TotalStudents { get; set; }

        #endregion

        #region Ctor, Dtor

        public StudentListViewModel()
        {
            Initialize();
            LoadData();
            InitializeCommands();
        }

        #endregion

        #region Methods

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
                this.Paging.GetTotalCount = () => this.UoW.Count<Student>();
                this.Paging.GetData = () =>
                {
                    return this.UoW.SelectAll<Student>().OrderedList()
                        .Skip(this.Paging.SkippedIndex).Take(this.Paging.PageSize)
                        .To(m => this.ConvertTo(m));
                };
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
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

        private bool CanAddMore()
        {
            return ApplicationManager.f || this.TotalStudents < ApplicationManager.g;
        }

        private void LoadData()
        {
            try
            {
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Paging.Page = 1;
                UpdateTotalStudents();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        protected override void OnAdd()
        {
            try
            {
                if (!CanAddMore())
                {
                    string msg = ApplicationManager.ag();

                    DialogManager.WriteToLog(msg, null);
                    DialogManager.ShowWarning("Add Student", msg, ApplicationManager.ah("students"));

                    return;
                }

                var vm = new StudentViewModel(new Student { Gender = true }, this);

                Setup(vm);
                if (NavigationManager.ShowStudent(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Student", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowStudent(vm).DialogResult == true)
                {
                    Edit(vm);
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "student"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                vm.Refresh();
                                this.Paging.Remove(vm);
                            }
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                        else if (vm.CanSave)
                            vm.Refresh();
                    }
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
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Student", ex);
            }
        }

        protected override void OnRefresh()
        {
            try
            {
                if (this.CanSave)
                {
                    DialogResult result = DialogManager.AskConfirmation("Confirmation",
                        SR.GetString("RefreshDataAsk1"), SR.GetString("RefreshDataAsk2"), DialogButton.OKCancel);
                    if (result == DialogResult.Cancel)
                        return;
                }

                int page = this.Paging.Page;
                int index = this.Paging.SelectedIndexOfPage;

                App.CursorWait();
                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                this.Paging.RefreshPage(page);
                this.Refreshing = false;
                this.Paging.SelectedIndex = index;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Student", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        protected override void DetectChanges()
        {
            this.Paging.ForEach(vm => vm.DetectChanges());
        }

        internal override void OnSave()
        {
            try
            {
                App.CursorWait();
                if (this.HasAnyErrors)
                {
                    this.CanSave = false;
                    DialogManager.ShowError("Validate Student", SR.GetString("ValidateData1", "student"));
                }
                if (this.CanSave)
                {
                    SaveChanges();
                    UpdateTotalStudents();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Student", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(StudentViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Student");
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        #endregion
    }
}
