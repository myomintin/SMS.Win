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
    internal sealed class GuardianListViewModel : ListViewModelPaging<ExtendedGuardianViewModel, Guardian>,
        IAddressParent
    {
        #region Properties

        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }

        #endregion

        #region Ctor, Dtor

        public GuardianListViewModel()
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
                    var vm = new ExtendedGuardianViewModel(m, this);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Guardian>
                            (model => model.GuardianId == vm.Model.GuardianId).CanDelete;
                    };

                    return vm;
                };
                this.Paging.GetTotalCount = () => this.UoW.Count<Guardian>();
                this.Paging.GetData = () =>
                {
                    return this.UoW.SelectAll<Guardian>().OrderedList()
                        .Skip(this.Paging.SkippedIndex).Take(this.Paging.PageSize)
                        .To(m => this.ConvertTo(m));
                };
                //this.SaveMode = SaveMode.PrintConsole | SaveMode.AcceptChanges;
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
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Paging.Page = 1;
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
                var vm = new ExtendedGuardianViewModel(new Guardian() { Gender = true }, this);

                Setup(vm);
                if (NavigationManager.ShowExtendedGuardian(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Guardian", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowExtendedGuardian(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "guardian"),
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
                                SR.GetString("EditDataAsk3", "guardian"),
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
                DialogManager.ShowErrorAndLog("Edit Guardian", ex);
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
                this.Paging.SelectedIndex = index;
                this.Refreshing = false;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Guardian", ex);
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
                    DialogManager.ShowError("Validate Guardian", SR.GetString("ValidateData1", "guardian"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Guardian", ex);
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

        private void Setup(ExtendedGuardianViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Guardian");
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        #endregion
    }
}
