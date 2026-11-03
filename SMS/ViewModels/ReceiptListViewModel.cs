using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.Reports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class ReceiptListViewModel : ListViewModelDictionary<DateTime, ReceiptViewModel, Receipt>,
        IReceiptParent
    {
        #region Fields

        private DateTime receiptDate;
        private decimal admissionFee;
        private decimal taxPercentage;
        private int fromMonth;
        private byte quantity;
        private string lastReceiptCode;
        private int lastReceiptNo;
        private SMSDataSet ds;
        private List<string> errorList;

        #endregion

        #region Properties

        public SearchStudentSectionInfoViewModel StudentSectionSelector { get; private set; }
        public EntityCollection<Tuple<string, bool>> Printers { get; private set; }
        public ICommand PrintCommand { get; private set; }
        public bool ClearSearchCache { get; set; }
        public bool AddAdmissionFee { get; set; }
        public bool AddTaxPercentage { get; set; }
        public bool CanPrint { get; private set; }

        public DateTime ReceiptDate
        {
            get { return this.receiptDate; }
            set
            {
                if (this.receiptDate != value)
                {
                    this.receiptDate = value;
                    RaisePropertyChanged("ReceiptDate");
                    LoadData();
                }
            }
        }

        public decimal AdmissionFee
        {
            get { return this.admissionFee; }
            set
            {
                if (this.admissionFee != value)
                {
                    this.admissionFee = value;
                    RaisePropertyChanged("AdmissionFee");
                }
            }
        }

        public decimal TaxPercentage
        {
            get { return this.taxPercentage; }
            set
            {
                if (this.taxPercentage != value)
                {
                    this.taxPercentage = value;
                    RaisePropertyChanged("TaxPercentage");
                }
            }
        }

        public int FromMonth
        {
            get { return this.fromMonth; }
            set
            {
                if (this.fromMonth != value)
                {
                    this.fromMonth = value;
                    RaisePropertyChanged("FromMonth");
                }
            }
        }

        public byte Quantity
        {
            get { return this.quantity; }
            set
            {
                if (this.quantity != value)
                {
                    this.quantity = value;
                    RaisePropertyChanged("Quantity");
                }
            }
        }

        public ReceiptViewModel SelectedItem
        {
            get { return this.Collection.SelectedItem; }
        }

        #endregion

        #region Ctor, Dtor

        public ReceiptListViewModel()
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
            this.StudentSectionSelector.Clear();
            this.Printers.Clear();
            Clear();
            this.UoW.Clear();
            this.errorList.Clear();
        }
        
        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.StudentSectionSelector = new SearchStudentSectionInfoViewModel();
                this.ConvertTo = (m) => new ReceiptViewModel(m);
                this.GetData = () =>
                {
                    return this.UoW.Where<Receipt>(m => m.ReceiptDate == this.receiptDate)
                        .Include().Ordered().To(m => this.ConvertTo(m));
                };
                this.SelectionChanged += () =>
                {
                    OnSeletedItemChanged();
                };
                this.Printers = new EntityCollection<Tuple<string, bool>>();
                LoadPrinters();
                this.PrintCommand = new RelayCommand(o => OnPrint(), o => this.CanPrint);
#if DEBUG
                this.receiptDate = new DateTime(2016, 5,19);
#else
                this.receiptDate = DateTime.Today;
#endif
                this.ds = ReportHelper.CreateDataSet();
                this.errorList = new List<string>();
                this.ClearSearchCache = true;
                this.AddAdmissionFee = false;
                this.AddTaxPercentage = false;
                this.admissionFee = 0;
                this.taxPercentage = 0;
                this.fromMonth = 1;
                this.quantity = 1;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void LoadPrinters()
        {
            CodeflexSystems.Library.Printing.PrinterHelper.GetInstalledPrinters(this.Printers);
            this.Printers.IIfSetSelectedItem(m => m.Item2, () => this.Printers.SelectedIndex = 0);
        }

        private void OnSeletedItemChanged()
        {
            Setup(this.SelectedItem);
            //RaisePropertyChanged("SelectedItem");
            this.CanPrint = this.Collection.SelectedIndex != -1;
        }

        private void LoadData()
        {
            try
            {
                if (this.StudentSectionSelector.Schools.Count > 0)
                {
                    this.CanAdd = this.CanRefresh = true;
                    if (!this.Refreshing)
                    {
                        this.Key = this.receiptDate;
                        if (this.Collection.SelectedIndex == -1)
                            this.Collection.SelectedIndex = 0;
                        OnSeletedItemChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        [DebuggerStepThrough]
        private void Check(ReceiptViewModel vm)
        {
            this.errorList.Clear();
            if (vm.TotalAmount != vm.Model.ComputeTotalAmount())
                this.errorList.Add("Total Amount");
            if (vm.NetAmount != vm.Model.ComputeNetAmount())
                this.errorList.Add("Net Amount");
            foreach (var dvm in vm.ReceiptDetailViewModels)
            {
                if (dvm.NetAmount != dvm.Model.ComputeNetAmount())
                    this.errorList.Add("Detail Net Amount");
            }
            if (this.errorList.Count > 0)
            {
#if DEBUG
                DialogManager.ShowErrorAndLog("Error",
                    string.Format("Receipt {0} has error.", vm.ReceiptCodeNo),
                    string.Format("Invalid {0}.", ", ".Combine(this.errorList)));
#else
                DialogManager.WriteToLog(string.Format("Receipt {0} has error.", vm.ReceiptCodeNo),
                    string.Format("Invalid {0}.", ", ".Combine(this.errorList)));
#endif
            }
        }

        public override void Add(ReceiptViewModel vm)
        {
            vm.Fix();
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            if (vm.ObjectState == ObjectState.Unchanged)
            {
                // if same dates, just add it
                if (this.receiptDate == vm.ReceiptDate)
                    this.Collection.Add(vm, true);
                else
                {
                    // if key is already loaded, change receipt date and add it
                    // if not loaded yet, just change receipt date
                    if (this.Dictionary.ContainsKey(vm.ReceiptDate))
                    {
                        this.ReceiptDate = vm.ReceiptDate;
                        this.Collection.Add(vm, true);
                    }
                    else
                        this.ReceiptDate = vm.ReceiptDate;
                }
            }
            //SetLastReceiptCodeNo(vm);
        }

        protected override void OnAdd()
        {
            try
            {
                var last = this.UoW.OrderByDescending<Receipt, int>(m => m.ReceiptNo)
                    .AsNoTracking().FirstOrDefault();

                if (last != null)
                {
                    this.lastReceiptCode = last.ReceiptCode;
                    this.lastReceiptNo = last.ReceiptNo;
                }
                else
                {
                    this.lastReceiptNo = 0;
                    this.lastReceiptCode = null;
                }

                var vm = new ReceiptViewModel(new Receipt()
                {
                    ReceiptDate = this.receiptDate,
                    ReceiptCode = this.lastReceiptCode,
                    ReceiptNo = this.lastReceiptNo + 1,
                    SchoolId = Guid.Empty
                });

                if (this.AddTaxPercentage && this.taxPercentage > 0)
                    vm.TaxPercentage = this.taxPercentage;
                Setup(vm);
                if (NavigationManager.ShowReceipt(vm).DialogResult == true)
                {
                    Check(vm);
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Receipt", ex);
            }
            finally
            {
                OnClearSearchCache();
            }
        }

        public override bool Delete(ReceiptViewModel vm)
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
                DialogManager.ShowErrorAndLog("Delete Receipt", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(ReceiptViewModel vm)
        {
            vm.Fix();
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            //// if there is no any detail after edit, just delete it
            //if (vm.ReceiptDetailViewModels.Count == 0)
            //    Delete(vm);
            //else
            //{
            // if same dates
            if (this.receiptDate == vm.ReceiptDate)
            {
                OnRefreshBinding();
            }
            else
            {
                // first, remove it from collection 
                this.Collection.Remove(vm);
                // if key is already loaded, change receipt date and add it
                // if not loaded yet, just change receipt date
                if (this.Dictionary.ContainsKey(vm.ReceiptDate))
                {
                    this.ReceiptDate = vm.ReceiptDate;
                    this.Collection.Add(vm, true);
                }
                else
                    this.ReceiptDate = vm.ReceiptDate;
            }
            //}
            //SetLastReceiptCodeNo(vm);
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                if (NavigationManager.ShowReceipt(vm).DialogResult == true)
                {
                    if (vm.ReceiptDetailViewModels.Count(rdvm => rdvm.ObjectState != ObjectState.Deleted) == 0)
                    {
                        DialogResult result = DialogManager.AskConfirmation("Confirmation",
                            SR.GetString("EditDataAsk5", "receipt", "has no receipt detail"),
                            SR.GetString("EditDataAsk6"), DialogButton.YesNo);
                        if (result == DialogResult.Yes)
                            Delete(vm);
                        else
                        {
                            OnEdit();
                            return;
                        }
                    }
                    else
                    {
                        Check(vm);
                        Edit(vm);
                    }
                }
                else
                {
                    if (vm.ObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "receipt"),
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
                                SR.GetString("EditDataAsk3", "receipt"),
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
                DialogManager.ShowErrorAndLog("Edit Receipt", ex);
            }
            finally
            {
                OnClearSearchCache();
            }
        }

        protected override void OnRefreshBinding()
        {
            //if (this.RefreshBindingCommand != null)
            //    this.RefreshBindingCommand.Execute(null);
            ////OnSeletedItemChanged();
            //RaisePropertyChanged("SelectedItem");
            if (this.RefreshBindingCommand != null)
            {
                int index = this.Collection.SelectedIndex;

                this.RefreshBindingCommand.Execute(null);
                this.Collection.SelectedIndex = index;
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
                this.StudentSectionSelector.LoadData();
                LoadPrinters();
                LoadData();
                this.Key = this.receiptDate;
                this.Collection.SelectedIndex = index;
                //OnSeletedItemChanged();
                this.Refreshing = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Receipt", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        internal override void OnSave()
        { }

        private void OnPrint()
        {
            try
            {
                var model = this.SelectedItem.Model;

                App.CursorWait();
                this.ds.AddReceipts(model, 2, 0);
                this.ds.Add(this.UoW.GetCurrenciesAsNoTracking(model.CurrencyId));
                this.ds.AddSchools(this.UoW.GetSchoolsAsNoTracking(model.SchoolId), 1);
                this.ds.Add(this.UoW.GetFeeTypesAsNoTracking(model.ReceiptDetails.Select(m => m.FeeTypeId)));
                if (this.Printers.SelectedIndex == -1)
                    NavigationManager.ShowReportViewer(ReportHelper.GetReceiptReport(this.ds));
                else
                    ReportHelper.PrintReceipt(this.ds, this.Printers.SelectedItem.Item1);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Print Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(ReceiptViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Receipt");
            if (vm.IsLoaded)
                return;

            vm.LoadData(this);
        }

        //private void SetLastReceiptCodeNo(ReceiptViewModel vm)
        //{
        //    int no = vm.ReceiptNo + 1;

        //    if (no > this.lastReceiptNo)
        //        this.lastReceiptNo = no;
        //    this.lastReceiptCode = vm.ReceiptCode;
        //}

        private void OnClearSearchCache()
        {
            if (this.ClearSearchCache)
                this.StudentSectionSelector.ClearCache();
        }

        #endregion
    }
}
