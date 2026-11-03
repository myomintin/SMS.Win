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
    internal sealed class SchoolListViewModel : ListViewModel<SchoolViewModel, School>,
        IAddressParent
    {
        #region Properties

        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }

        #endregion

        #region Ctor, Dtor

        public SchoolListViewModel()
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
            this.AddressSelector.Clear();
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.AddAddressViewModel = AddViewModel.CreateAddAddress();
                this.AddAddressViewModel.IsExisting = true;
                this.AddressSelector = new SearchAddressViewModel();

                this.ConvertTo = (m) =>
                {
                    var vm = new SchoolViewModel(m, this);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<School>
                            (model => model.SchoolId == vm.Model.SchoolId).CanDelete;
                    };

                    return vm;
                };
                //this.IncludeFilterTypes = false;
                //this.FilterTypes = new Type[] { typeof(Country), typeof(EmailType), typeof(PhoneType) };
                //this.IncludeFilterStates = false;
                //this.FilterStates = null;
                //this.SaveMode = SaveMode.PrintConsole | SaveMode.AcceptChanges;
                this.IsLoaded = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private bool CanAddMore()
        {
            return ApplicationManager.d || this.Collection.Count < ApplicationManager.e;
        }

        private void LoadData()
        {
            try
            {
                this.Collection.AddRange(this.UoW.GetAllSchools().Include()
                    .Ordered().To(m => this.ConvertTo(m)), false);
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Collection.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        public override void Add(SchoolViewModel vm)
        {
            this.Collection.Add(vm, true);
            vm.CanSave = false;
            this.CanSave = true;
        }

        protected override void OnAdd()
        {
            try
            {
                if (!CanAddMore())
                {
                    string msg = ApplicationManager.af();

                    DialogManager.WriteToLog(msg, null);
                    DialogManager.ShowWarning("Add School", msg, ApplicationManager.ah("schools"));

                    return;
                }

                var vm = new SchoolViewModel(new School(), this);

                Setup(vm);
                if (NavigationManager.ShowSchool(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add School", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowSchool(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "school"),
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
                                SR.GetString("EditDataAsk3", "school"),
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
                DialogManager.ShowErrorAndLog("Edit School", ex);
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

                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                this.Collection.SelectedIndex = index;
                this.Refreshing = false;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Data", ex);
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
                    DialogManager.ShowError("Validate Data", SR.GetString("ValidateData1", "school"));
                }
                if (this.CanSave)
                {
                    SaveChanges();
                    if (!ApplicationManager.d &&
                        this.Collection.Count < ApplicationManager.e)
                    {
                        int count = this.UoW.Count<School>();

                        if (count > this.Collection.Count)
                        {
                            int index = this.Collection.SelectedIndex;

                            Clear();
                            this.Refreshing = true;
                            LoadData();
                            this.Collection.SelectedIndex = index;
                            this.Refreshing = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        protected override void DetectChanges()
        {
            this.Collection.ForEach(vm => vm.DetectChanges());
        }

        private void Setup(SchoolViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("School");
            if (vm.IsLoaded)
                return;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SchoolName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        string.Equals(vm.SchoolName, m.SchoolName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }
                    //if (!flag)
                    //{
                    //    var value = this.UoW.FirstOrDefaultAsNoTracking<School>(m =>
                    //        vm.SchoolName.Equals(m.SchoolName, StringComparison.InvariantCultureIgnoreCase));

                    //    if (value != null && value.SchoolId != vm.Model.SchoolId)
                    //        flag = true;
                    //}
                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "School Name", vm.SchoolName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData();
        }

        #endregion
    }
}
