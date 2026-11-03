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
    internal sealed class AddressListViewModel : ListViewModelPaging<ExtentedAddressViewModel, Address>
    {
        #region Ctor, Dtor

        public AddressListViewModel()
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
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.ConvertTo = (m) =>
                {
                    var vm = new ExtentedAddressViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Address>
                            (model => model.AddressId == vm.Model.AddressId).CanDelete;
                    };

                    return vm;
                };
                this.Paging.GetTotalCount = () => this.UoW.Count<Address>();
                this.Paging.GetData = () =>
                {
                    return this.UoW.SelectAll<Address>().OrderedList()
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
                var vm = new ExtentedAddressViewModel(new Address());

                Setup(vm);
                if (NavigationManager.ShowExtendedAddress(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Address", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowExtendedAddress(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "address"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.Paging.Remove(vm);
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
                                SR.GetString("EditDataAsk3", "address"),
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
                DialogManager.ShowErrorAndLog("Edit Address", ex);
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
                DialogManager.ShowErrorAndLog("Refresh Address", ex);
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
                    DialogManager.ShowError("Validate Address", SR.GetString("ValidateData1", "address"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Address", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(ExtentedAddressViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Address");
            if (vm.IsLoaded)
                return;

            vm.LoadData(this);
        }

        #endregion
    }
}
