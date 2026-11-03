using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
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
    internal sealed class ReceiptSearchListViewModel : SearchListOptionViewModelBDSDIP<ReceiptViewModel, Receipt, ReceiptSearchOption>,
        IReceiptParent
    {
        #region Fields

        private SearchStudentViewModel searcher;
        private decimal admissionFee;
        private decimal taxPercentage;
        private int fromMonth;
        private byte quantity;
        private SMSDataSet ds;
        private List<string> errorList;

        #endregion

        # region Properties

        public SearchStudentSectionInfoViewModel StudentSectionSelector { get; private set; }
        public EntityCollection<Tuple<string, bool>> Printers { get; private set; }
        public ICommand PrintCommand { get; private set; }
        public bool ClearSearchCache { get; set; }
        public bool AddAdmissionFee { get; set; }
        public bool AddTaxPercentage { get; set; }
        public bool CanPrint { get; private set; }

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

        # endregion

        # region Ctor, Dtor

        public ReceiptSearchListViewModel()
        {
            Initialize();
            InitializeCommands();
        }

        # endregion

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
            this.searcher.Clear();
            Clear();
            this.UoW.Clear();
            this.errorList.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.searcher = new SearchStudentViewModel();
                this.searcher.OnSearch = () => OnSearchStudents();
                this.StudentSectionSelector = new SearchStudentSectionInfoViewModel();
                this.ConvertTo = (m) => new ReceiptViewModel(m);
                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetReceiptSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 3;
                this.MinIntegral = 0;
                this.Collection.SelectionChanged += (s, e) =>
                {
                    this.CanPrint = this.Collection.SelectedIndex != -1;
                };
                this.Printers = new EntityCollection<Tuple<string, bool>>();
                LoadPrinters();
                this.PrintCommand = new RelayCommand(o => OnPrint(), o => this.CanPrint);

                this.ds = ReportHelper.CreateDataSet();
                this.errorList = new List<string>();
                this.RemoveAdded = true;
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

        protected override void SearchOptionInfoChanged()
        {
            base.SearchOptionInfoChanged();
            if (this.SearchOptionInfos.SelectedIndex != -1 &&
                this.SearchOptionInfos.SelectedItem.Option == ReceiptSearchOption.Student)
            {
                this.CanSearch = true;
                CheckToSearch();
            }
        }
        
        protected override void OnSearch()
        {
            try
            {
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<Receipt> query = null;

                if (current.SearchValueType == SearchValueType.DateTime)
                {
                    query = GetDateQuery<Receipt>();
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == ReceiptSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<Receipt, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == ReceiptSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<Receipt, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<Receipt>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Decimal)
                {
                    if (current.Option == ReceiptSearchOption.DetailAmount ||
                        current.Option == ReceiptSearchOption.DetailDiscountAmount ||
                        current.Option == ReceiptSearchOption.DetailNetAmount)
                    {
                        //query = GetDecimalQuery<ReceiptDetail>
                        //    (current.Option.ToString().Replace("Detail", "")).Select(m => m.Receipt);
                        var query1 = GetDecimalQuery<ReceiptDetail>(current.Option.ToString().Replace("Detail", ""))
                            .Select(m => m.ReceiptId).Distinct();
                        query = this.UoW.SelectAll<Receipt>().Join(query1, m => m.ReceiptId, u => u, (m, u) => m);
                    }
                    else
                    {
                        query = GetDecimalQuery<Receipt>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Integral)
                {
                    query = GetIntegralQuery<Receipt, int>(LongToInt32);
                }
                else if (current.SearchValueType == SearchValueType.Percentage)
                {
                    var value = GetSearchPercentageValue();
                    var mt = GetPercentageMatchType();

                    if (current.Option == ReceiptSearchOption.TaxPercentage)
                    {
                        if (mt == ValueMatchType.Equal)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) == value);
                        }
                        else if (mt == ValueMatchType.GreaterThan)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) > value);
                        }
                        else if (mt == ValueMatchType.GreaterThanOrEqual)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) >= value);
                        }
                        else if (mt == ValueMatchType.LessThan)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) < value);
                        }
                        else if (mt == ValueMatchType.LessThanOrEqual)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) <= value);
                        }
                        else if (mt == ValueMatchType.NotEqual)
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount.HasValue &&
                                (m.TaxAmount.Value * 100 / m.TotalAmount) != value);
                        }
                        else
                        {
                            query = this.UoW.Where<Receipt>(m => m.TaxAmount == null);
                        }
                    }
                }
                else if (current.SearchValueType == SearchValueType.Object)
                {
                    if (current.Option == ReceiptSearchOption.Student &&
                        NavigationManager.ShowSearchStudent(this.searcher, null).DialogResult == true)
                    {
                        var value = this.searcher.Collection.SelectedItem.StudentId;
                        //query = from ss in this.UoW.Where<StudentSection>(m => m.StudentId == value)
                        //        from rd in ss.ReceiptDetails
                        //        select rd.Receipt;
                        var query1 = this.UoW.Where<StudentSection>(m => m.StudentId == value)
                            .SelectMany(m => m.ReceiptDetails, (m, n) => n.ReceiptId).Distinct();
                        query = this.UoW.SelectAll<Receipt>().Join(query1, m => m.ReceiptId, u => u, (m, u) => m);
                    }
                }
                else if (current.SearchValueType == SearchValueType.NoneType)
                {
                    if (current.Option == ReceiptSearchOption.NoUsage)
                    {
                        query = this.UoW.Where<Receipt>(m => m.ReceiptDetails.Count == 0);
                    }
                }

                App.CursorWait();
                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Include().Ordered().To(m => this.ConvertTo(m)), false);
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

        private void OnSearchStudents()
        {
            try
            {
                App.CursorWait();
                var query = this.searcher.GetQuery();

                this.searcher.Collection.Clear();
                if (query != null)
                {
                    this.searcher.Collection.AddRange(query.Ordered().AsNoTracking(), false);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Search Students", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        [Conditional("DEBUG")]
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

        public override bool Delete(ReceiptViewModel vm)
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

        public override void Edit(ReceiptViewModel vm)
        {
            vm.Fix();
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            // if there is no any detail after edit, just delete it
            if (vm.ReceiptDetailViewModels.Count == 0)
                Delete(vm);
            else
                OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
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
            finally
            {
                OnClearSearchCache();
            }
        }

        internal override void OnSave()
        { }

        private void OnPrint()
        {
            try
            {
                var model = this.Collection.SelectedItem.Model;

                App.CursorWait();
                Setup(this.Collection.SelectedItem);
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

        private int LongToInt32(long? input)
        {
            if (input.HasValue)
                return Convert.ToInt32(input.Value);

            return -1;
            //return CodeflexSystems.Library.Converters.ConverterHelper.As<long?, int>(input, -1);
        }

        private void OnClearSearchCache()
        {
            if (this.ClearSearchCache)
                this.StudentSectionSelector.ClearCache();
        }

        #endregion
    }
}
