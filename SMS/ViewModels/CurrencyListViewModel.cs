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
    internal sealed class CurrencyListViewModel : ListViewModel<CurrencyViewModel, Currency>
    {
        #region Fields

        private List<CurrencyInfo> defaultCurrencies;
        private List<CurrencyInfo> Currencies;
        private bool IsLoadCurrencies;

        #endregion

        #region Ctor, Dtor

        public CurrencyListViewModel()
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
            this.Currencies.Clear();
            this.IsLoadCurrencies = false;
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.defaultCurrencies = SR.GetCurrencyInfos();
                this.Currencies = new List<CurrencyInfo>();
                this.ConvertTo = (m) =>
                {
                    var vm = new CurrencyViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Currency>
                            (model => model.CurrencyId == vm.Model.CurrencyId).CanDelete;
                    };
                    //vm.DetectCanEdit = () =>
                    //{
                    //    return flag ? false : vm.Model != null;
                    //};
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

        private bool CanAddMore()
        {
            return ApplicationManager.b || this.Collection.Count < ApplicationManager.c;
        }

        private void LoadCurrencies()
        {
            if (!this.IsLoadCurrencies)
            {
                foreach (var c in this.defaultCurrencies)
                {
                    if (!this.Collection.Any(vm => vm.Model.CurrencyCode == c.CurrencyCode))
                        this.Currencies.Add(c);
                }
                this.IsLoadCurrencies = true;
            }
        }

        private void LoadData()
        {
            try
            {
                this.Collection.AddRange(this.UoW.GetAllCurrencies()
                    .Ordered().To(m => this.ConvertTo(m)), false);
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Collection.SelectedIndex = 0;
                LoadCurrencies();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        protected override void OnAdd()
        {
            if (this.IsLoadCurrencies && this.Currencies.Count == 0)
            {
                DialogManager.Show("Add Currency", "No more to add currency.");
                return;
            }
            try
            {
                if (!CanAddMore())
                {
                    string msg = ApplicationManager.au();

                    DialogManager.WriteToLog(msg, null);
                    DialogManager.ShowWarning("Add Currency", msg, ApplicationManager.ah("currencies"));

                    return;
                }

                if (NavigationManager.ShowAddCurrencies(this.Currencies).DialogResult != true)
                    return;

                foreach (var value in this.Currencies.Where(m => m.IsSelected))
                {
                    if (CanAddMore())
                    {
                        var m = new Currency
                        {
                            CurrencyName = value.CurrencyName,
                            CurrencyCode = value.CurrencyCode,
                            CurrencySymbol = value.CurrencySymbol
                        };
                        var vm = new CurrencyViewModel(m);

                        Setup(vm);
                        this.Collection.Add(vm, false);
                        vm.CanSave = false;
                        vm.IsReadOnly = true;
                        this.CanSave = true;
                        value.Active = true;
                        value.IsSelected = false;
                    }
                }

                var c = this.Currencies.LastOrDefault(m => m.Active);

                if (c != null)
                    this.Collection.IIfSetSelectedItem(vm => vm.Model.CurrencyCode == c.CurrencyCode, () => { });
                this.Currencies.RemoveRange(this.Currencies.Where(m => m.Active).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Currency", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowCurrency(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "currency"),
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
                                SR.GetString("EditDataAsk3", "currency"),
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
                DialogManager.ShowErrorAndLog("Edit Currency", ex);
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
                DialogManager.ShowErrorAndLog("Refresh Currency", ex);
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
                    DialogManager.ShowError("Validate Currency", SR.GetString("ValidateData1", "currency"));
                }
                if (this.CanSave)
                {
                    SaveChanges();
                    this.Currencies.Clear();
                    this.IsLoadCurrencies = false;
                    if (!ApplicationManager.b &&
                        this.Collection.Count < ApplicationManager.c)
                    {
                        int count = this.UoW.Count<Currency>();

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
                    LoadCurrencies();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Currency", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(CurrencyViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Currency");
            if (vm.IsLoaded)
                return;

            //vm.DataChanged += (s, e) =>
            //{
            //    if (e.PropertyName == "CurrencyCode")
            //    {
            //        bool flag = false;

            //        foreach (var item in this.Collection.Where(m =>
            //            vm.CurrencyCode.Equals(m.CurrencyCode, StringComparison.InvariantCultureIgnoreCase)))
            //        {
            //            if (flag = (item != vm))
            //                break;
            //        }
            //        //if (!flag)
            //        //{
            //        //    var value = this.UoW.FirstOrDefaultAsNoTracking<Currency>(m =>
            //        //        vm.CurrencyCode.Equals(m.CurrencyCode, StringComparison.InvariantCultureIgnoreCase));

            //        //    if (value != null && value.CurrencyId != vm.Model.CurrencyId)
            //        //        flag = true;
            //        //}
            //        if (flag)
            //        {
            //            vm.SetErrors(e.PropertyName, SR.Get("AlreadyExists1", "Currency Code", vm.CurrencyCode));
            //            vm.DetectErrors();
            //        }
            //    }
            //};
            vm.LoadData(this);
        }

        #endregion
    }
}
