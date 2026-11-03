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
    internal sealed class GuardianSearchListViewModel : SearchListOptionViewModelBDSCR<ExtendedGuardianViewModel, Guardian, GuardianSearchOption>,
        IAddressParent
    {
        #region Properties

        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }

        #endregion

        # region Ctor, Dtor

        public GuardianSearchListViewModel()
            : base(true, true)
        {
            Initialize();
            InitializeCommands();
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

                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetGuardianSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 2;

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
                this.ShowCountryMatchType = current.Option == GuardianSearchOption.BirthCountry;
                this.ShowReligionMatchType = current.Option == GuardianSearchOption.Religion;
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

        protected override void OnSearch()
        {
            try
            {
                App.CursorWait();
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<Guardian> query = null;

                if (current.SearchValueType == SearchValueType.DateTime)
                {
                    query = GetDateQuery<Guardian>();
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == GuardianSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<Guardian, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == GuardianSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<Guardian, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<Guardian>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Object)
                {
                    if (current.Option == GuardianSearchOption.BirthCountry)
                    {
                        var id = GetCountryId();
                        query = this.UoW.Where<Guardian>(m => m.BirthCountryId == id);
                    }
                    else if (current.Option == GuardianSearchOption.Religion)
                    {
                        var id = GetReligionId();
                        query = this.UoW.Where<Guardian>(m => m.ReligionId == id);
                    }
                }
                else if (current.SearchValueType == SearchValueType.NoneType)
                {
                    if (current.Option == GuardianSearchOption.NoUsage)
                    {
                        query = this.UoW.Where<Guardian>(m => m.StudentGuardians.Count == 0);
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

        public override bool Delete(ExtendedGuardianViewModel vm)
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

        public override void Edit(ExtendedGuardianViewModel vm)
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
                if (NavigationManager.ShowExtendedGuardian(vm).DialogResult == true)
                    Edit(vm);
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

        private void Setup(ExtendedGuardianViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Guardian");
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        # endregion
    }
}
