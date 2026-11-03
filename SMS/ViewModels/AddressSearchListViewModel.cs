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
    internal sealed class AddressSearchListViewModel : SearchListOptionViewModelBDSC<ExtentedAddressViewModel, Address, AddressSearchOption>
    {
        #region Ctor, Dtor

        public AddressSearchListViewModel()
            : base(false)
        {
            Initialize();
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
                this.ViewName = "Search Addresses";
                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetAddressSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 1;

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
                    this.ShowStringMatchType = this.ShowCountryMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowCountryMatchType = current.Option == AddressSearchOption.Country;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.ShowCountryMatchType)
                    CountryChanged();
            }
        }

        protected override void OnSearch()
        {
            try
            {
                App.CursorWait();
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<Address> query = null;

                if (current.SearchValueType == SearchValueType.DateTime)
                {
                    query = GetDateQuery<Address>();
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == AddressSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<Address, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == AddressSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<Address, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<Address>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Object)
                {
                    if (current.Option == AddressSearchOption.Country)
                    {
                        var id = GetCountryId();
                        query = this.UoW.Where<Address>(m => m.CountryId == id);
                    }
                }
                else if (current.SearchValueType == SearchValueType.NoneType)
                {
                    if (current.Option == AddressSearchOption.NoUsage)
                    {
                        query = this.UoW.Where<Address>(m => m.GuardianAddresses.Count == 0 &&
                            m.Schools.Count == 0 && m.StudentAddresses.Count == 0);
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

        public override bool Delete(ExtentedAddressViewModel vm)
        {
            bool flag = this.Collection.Delete(vm);

            if (flag)
            {
                this.UoW.Remove(vm.Model);
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

        public override void Edit(ExtentedAddressViewModel vm)
        {
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
                if (NavigationManager.ShowExtendedAddress(vm).DialogResult == true)
                    Edit(vm);
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
