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
    internal sealed class CountryListViewModel : ListViewModel<CountryViewModel, Country>
    {
        #region Fields

        private List<CountryInfo> defaultCountries;
        private List<CountryInfo> Countries;
        private bool IsLoadCountries;

        #endregion

        #region Ctor, Dtor

        public CountryListViewModel()
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
            this.Countries.Clear();
            this.IsLoadCountries = false;
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.defaultCountries = SR.GetCountryInfos();
                this.Countries = new List<CountryInfo>();
                this.ConvertTo = (m) =>
                {
                    var vm = new CountryViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Country>
                            (model => model.CountryId == vm.Model.CountryId).CanDelete;
                    };
                    vm.IsReadOnly = true;

                    return vm;
                };
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void LoadCountries()
        {
            if (!this.IsLoadCountries)
            {
                foreach (var c in this.defaultCountries)
                {
                    if (!this.Collection.Any(vm => vm.Model.CountryCode == c.CountryCode))
                        this.Countries.Add(c);
                }
                this.IsLoadCountries = true;
            }
        }

        private void LoadData()
        {
            try
            {
                this.Collection.AddRange(this.UoW.SelectAll<Country>()
                    .Ordered().To(m => this.ConvertTo(m)), false);
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Collection.SelectedIndex = 0;
                LoadCountries();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        protected override void OnAdd()
        {
            if (this.IsLoadCountries && this.Countries.Count == 0)
            {
                DialogManager.Show("Add Country", "No more to add country.");
                return;
            }
            try
            {
                if (NavigationManager.ShowAddCountries(this.Countries).DialogResult != true)
                    return;

                foreach (var value in this.Countries.Where(m => m.IsSelected))
                {
                    if (this.CanAdd)
                    {
                        var m = new Country
                        {
                            CountryName = value.CountryName,
                            CountryCode = value.CountryCode,
                            Code = value.Code
                        };
                        var vm = new CountryViewModel(m);

                        Setup(vm);
                        this.Collection.Add(vm, false);
                        vm.CanSave = false;
                        vm.IsReadOnly = true;
                        this.CanSave = true;
                        value.Active = true;
                        value.IsSelected = false;
                    }
                }

                var c = this.Countries.LastOrDefault(m => m.Active);

                if (c != null)
                    this.Collection.IIfSetSelectedItem(vm => vm.Model.CountryCode == c.CountryCode, () => { });
                this.Countries.RemoveRange(this.Countries.Where(m => m.Active).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Country", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowCountry(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "country"),
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
                                SR.GetString("EditDataAsk3", "country"),
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
                DialogManager.ShowErrorAndLog("Edit Country", ex);
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
                DialogManager.ShowErrorAndLog("Refresh Country", ex);
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
                    DialogManager.ShowError("Validate Country", SR.GetString("ValidateData1", "country"));
                }
                if (this.CanSave)
                {
                    SaveChanges();
                    this.Countries.Clear();
                    this.IsLoadCountries = false;
                    LoadCountries();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Country", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(CountryViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Country");
            if (vm.IsLoaded)
                return;

            //vm.DataChanged += (s, e) =>
            //{
            //    if (e.PropertyName == "CountryName")
            //    {
            //        bool flag = false;

            //        foreach (var item in this.Paging.Where(m =>
            //            string.Equals(vm.CountryName, m.CountryName, StringComparison.InvariantCultureIgnoreCase)))
            //        {
            //            if (flag = (item != vm))
            //                break;
            //        }
            //        if (!flag)
            //        {
            //            var value = this.UoW.FirstOrDefaultAsNoTracking<Country>(m =>
            //                vm.CountryName.Equals(m.CountryName, StringComparison.InvariantCultureIgnoreCase));

            //            if (value != null && value.CountryId == vm.Model.CountryId)
            //                flag = true;
            //        }
            //        if (flag)
            //        {
            //            vm.SetErrors(e.PropertyName, SR.Get("AlreadyExists1", "Country Name", vm.CountryName));
            //            vm.DetectErrors();
            //        }
            //    }
            //};
            vm.LoadData(this);
        }

        #endregion
    }
}
