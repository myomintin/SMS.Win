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
    internal sealed class AcademicYearListViewModel : ListViewModelDictionary<Guid, AcademicYearViewModel, AcademicYear>
    {
        #region Properties

        public EntityCollection<School> Schools { get; private set; }

        #endregion

        #region Ctor, Dtor

        public AcademicYearListViewModel()
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
            this.Schools.Clear();
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.Schools = new EntityCollection<School>();
                this.Schools.SelectionChanged += (s, e) =>
                {
                    LoadAcademicYears();
                };

                this.ConvertTo = (m) =>
                {
                    var vm = new AcademicYearViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<AcademicYear>
                            (model => model.AcademicYearId == vm.Model.AcademicYearId).CanDelete;
                    };

                    return vm;
                };
                this.GetData = () =>
                {
                    return this.UoW.Where<AcademicYear>(m => m.SchoolId == this.Key)
                        .Ordered().To(m => this.ConvertTo(m));
                };
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void LoadData()
        {
            try
            {
                this.Schools.AddRange(this.UoW.Schools, false);
                if (!this.Refreshing)
                {
                    //if (this.Schools.Any(m => m.Active))
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault(m => m.Active);
                    //else
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault();
                    this.Schools.IIfSetSelectedItem(m => m.Active, () => this.Schools.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        private void LoadAcademicYears()
        {
            try
            {
                if (this.CanAdd = this.CanRefresh = this.Schools.SelectedIndex != -1)
                {
                    if (!this.Refreshing)
                    {
                        this.Key = this.Schools.SelectedItem.SchoolId;
                        if (this.Collection.SelectedIndex == -1)
                            this.Collection.SelectedIndex = 0;
                    }
                }
                else
                    LoadDummyData();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Academic Years", ex);
            }
        }

        protected override void OnAdd()
        {
            try
            {
                var vm = new AcademicYearViewModel(new AcademicYear() { School = this.Schools.SelectedItem });

                Setup(vm);
                if (NavigationManager.ShowAcademicYear(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Academic Year", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowAcademicYear(vm).DialogResult == true)
                {
                    this.CanSave = true;
                    vm.CanSave = false;
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "academic year"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.Collection.Remove(vm);
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "academic year"),
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
                DialogManager.ShowErrorAndLog("Edit Academic Year", ex);
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

                Guid? oid = null;
                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                if (this.Schools.SelectedIndex != -1)
                    oid = this.Schools.SelectedItem.SchoolId;

                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                if (oid.HasValue)
                {
                    var selectedSchool = this.Schools.FirstOrDefault(m => m.SchoolId == oid.Value);
                    if (selectedSchool != null)
                    {
                        this.Schools.SelectedItem = selectedSchool;
                        this.Key = this.Schools.SelectedItem.SchoolId;
                        this.Collection.SelectedIndex = index;
                    }
                }
                this.Refreshing = false;
                OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Academic Year", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        internal override void OnSave()
        {
            try
            {
                App.CursorWait();
                if (this.HasAnyErrors)
                {
                    this.CanSave = false;
                    DialogManager.ShowError("Validate Address", SR.GetString("ValidateData1", "academic year"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Academic Year", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(AcademicYearViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Academic Year");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "AcademicYearName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        string.Equals(vm.AcademicYearName, m.AcademicYearName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Academic Year Name", vm.AcademicYearName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this);
        }

        #endregion
    }
}
