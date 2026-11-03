using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.DataAnnotations;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class ReceiptViewModel : EntityViewModelCM<Receipt>
    {
        #region Fields

        private string deleteReceiptDetailContent;
        private decimal taxPercentage;
        private bool addCurrency;

        #endregion

        #region Properties

        public ObservableCollection<Currency> Currencies { get; private set; }
        public EntityCollection<FeeType> FeeTypes { get; private set; }
        public EntityViewModelCollection<ReceiptDetailViewModel, ReceiptDetail> ReceiptDetailViewModels { get; private set; }
        public bool IsDetectedChanges { get; private set; }

        public ICommand AddReceiptDetailCommand { get; private set; }
        public ICommand EditReceiptDetailCommand { get; private set; }
        public ICommand DeleteReceiptDetailCommand { get; private set; }
        public ICommand RefreshReceiptDetailBindingCommand { get; set; }

        public IReceiptParent ReceiptParent
        {
            get
            {
                return base.Parent as IReceiptParent;
            }
        }

        public DateTime ReceiptDate
        {
            get { return this.Model.ReceiptDate; }
            set
            {
                if (this.Model.ReceiptDate != value)
                {
                    this.Model.ReceiptDate = value;
                    RaiseDataChanged("ReceiptDate");
                }
            }
        }

        [MaxLength(10, ErrorMessage = "Receipt Code is too long.")]
        public string ReceiptCode
        {
            get { return this.Model.ReceiptCode; }
            set
            {
                Repair(ref value);
                if (this.Model.ReceiptCode != value)
                {
                    ValidateProperty(value, "ReceiptCode");
                    this.Model.ReceiptCode = value;
                    RaiseDataChanged("ReceiptCode");
                    RaisePropertyChanged("ReceiptCodeNo");
                }
            }
        }

        public int ReceiptNo
        {
            get { return this.Model.ReceiptNo; }
            set
            {
                if (this.Model.ReceiptNo != value)
                {
                    this.Model.ReceiptNo = value;
                    RaiseDataChanged("ReceiptNo");
                    RaisePropertyChanged("ReceiptCodeNo");
                }
            }
        }

        public string ReceiptCodeNo
        {
            get { return this.Model.ReceiptCodeNo; }
        }

        public decimal TotalAmount
        {
            get { return this.Model.TotalAmount; }
            private set
            {
                RepairCurrency(ref value);
                if (this.Model.TotalAmount != value)
                {
                    this.Model.TotalAmount = value;
                    RaiseDataChanged("TotalAmount");
                    ComputeTaxAmount();
                    ComputeNetAmount();
                }
            }
        }

        [MinValue(0, ErrorMessage = "Tax Percentage must be at least 0.")]
        public decimal TaxPercentage
        {
            get { return this.taxPercentage; }
            set
            {
                if (this.taxPercentage != value)
                {
                    ValidateProperty(value, "TaxPercentage");
                    this.taxPercentage = value;
                    RaisePropertyChanged("TaxPercentage");
                    ComputeTaxAmount();
                }
            }
        }

        [MinValue(0, ErrorMessage = "Tax Amount must be at least 0.")]
        public decimal? TaxAmount
        {
            get { return this.Model.TaxAmount; }
            set
            {
                RepairCurrency(ref value);
                if (this.Model.TaxAmount != value)
                {
                    ValidateProperty(value, "TaxAmount");
                    this.Model.TaxAmount = value;
                    RaiseDataChanged("TaxAmount");
                    ComputeTaxPercentage();
                    ComputeNetAmount();
                }
            }
        }

        public decimal NetAmount
        {
            get { return this.Model.NetAmount; }
            private set
            {
                RepairCurrency(ref value);
                if (this.Model.NetAmount != value)
                {
                    this.Model.NetAmount = value;
                    RaiseDataChanged("NetAmount");
                }
            }
        }

        [MaxLength(250, ErrorMessage = "Description is too long.")]
        public string Description
        {
            get { return this.Model.Description; }
            set
            {
                Repair(ref value);
                if (this.Model.Description != value)
                {
                    ValidateProperty(value, "Description");
                    this.Model.Description = value;
                    RaiseDataChanged("Description");
                }
            }
        }

        public Currency Currency
        {
            get { return this.Model.Currency; }
            set
            {
                if (value != null && this.Model.Currency != value)
                {
                    this.Model.Currency = value;
                    RaiseDataChanged("Currency");
                }
            }
        }

        public DateTime CreatedDate
        {
            get { return this.Model.CreatedDate; }
        }

        public DateTime ModifiedDate
        {
            get { return this.Model.ModifiedDate; }
        }

        public DateTime CreatedDateLT
        {
            get { return this.Model.CreatedDate.ToLocalTime(); }
        }

        public DateTime ModifiedDateLT
        {
            get { return this.Model.ModifiedDate.ToLocalTime(); }
        }

        public override bool CanDelete
        {
            get { return this.Model.CanDelete; }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.ReceiptId); }
        }

        public string DeleteReceiptDetailContent
        {
            get { return this.deleteReceiptDetailContent; }
            private set
            {
                if (this.deleteReceiptDetailContent != value)
                {
                    this.deleteReceiptDetailContent = value;
                    RaisePropertyChanged("DeleteReceiptDetailContent");
                }
            }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.ReceiptDetailViewModels.HasAnyErrors;
            }
        }

        #endregion

        #region Ctor, Dtor

        public ReceiptViewModel(Receipt model)
            : base(model)
        {
            this.deleteReceiptDetailContent = "_Delete";
            if (!this.IsNew)
                Reset();
        }

        #endregion

        #region Methods

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.ReceiptDetailViewModels.AcceptAllChanges();
            this.IsDetectedChanges = false;
            SetCreatedBy();
            SetModifiedBy();
            BaseAcceptChanges();
            if (this.addCurrency && this.Model.Currency != null)
            {
                Guid id = this.Model.Currency.CurrencyId;
                if (this.Parent.UoW.Currencies.Any(m => m.CurrencyId == id))
                {
                    this.Currencies = this.Parent.UoW.Currencies.ToObservableCollection();
                    this.addCurrency = false;
                }
            }
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            var state = this.OverallState;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            this.ReceiptDetailViewModels.Refresh();
            Reset();
            this.IsDetectedChanges = false;
            base.BaseRefresh();

            bool flag = state == ObjectState.Modified &&
                this.ReceiptDetailViewModels.HasAnyAdded();
            if (flag)
                this.OverallState = state;
        }

        internal void DetectChanges()
        {
            if (this.IsDetectedChanges || this.Parent == null)
                return;

            if (this.ObjectState == ObjectState.Deleted && this.OldObjectState != ObjectState.Added)
            {
                this.Parent.UoW.RemoveRange(this.Model.ReceiptDetails);
                this.Parent.UoW.Remove(this.Model);
            }
            else if (this.OverallState == ObjectState.Added || this.OverallState == ObjectState.Modified)
            {
                this.ReceiptDetailViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model));
                this.ReceiptDetailViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.ReceiptDetails.AddIfNotExist(vm.Model));

                if (this.ReceiptDetailViewModels.Count(vm => vm.ObjectState != ObjectState.Deleted) > 0)
                {
                    if (this.ObjectState == ObjectState.Added)
                        this.Parent.UoW.Add(this.Model);
                }
                else
                {
                    if (this.ObjectState != ObjectState.Added)
                        this.Parent.UoW.Remove(this.Model);
                }
            }
            this.IsDetectedChanges = true;
        }

        protected override void SetCreatedBy()
        {
            if (string.IsNullOrEmpty(this.CreatedBy))
                this.CreatedBy = this.Parent.UoW.GetUserNameById(this.Model.CreatedBy);
        }

        protected override void SetModifiedBy()
        {
            this.ModifiedBy = this.Parent.UoW.GetUserNameById(this.Model.ModifiedBy);
        }

        public void DetectErrors()
        {
            if (!this.IsLoaded)
                return;

            this.CanSave = !(this.HasAnyErrors || this.ReceiptDetailViewModels.Count == 0);
        }

        public void LoadData(IReceiptParent parent)
        {
            if (this.IsLoaded)
                return;

            App.CursorWait();
            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.Currencies = this.Parent.UoW.Currencies.ToObservableCollection();
            if (this.Model.Currency == null)
                this.Model.Currency = this.Currencies.GetDefault();
            else if (!this.IsNew)
                this.addCurrency = this.Parent.UoW.AddCurrencyById(this.Currencies, this.Model.CurrencyId);
            //this.Currencies = this.Parent.UoW.Currencies;
            //if (this.Model.Currency == null)
            //    this.Model.Currency = this.Currencies.GetDefault();
            this.FeeTypes = new EntityCollection<FeeType>(this.Parent.UoW.FeeTypes);
            this.FeeTypes.IIfSetSelectedItem(m => m.FeeTypeName == "School",
                () => this.FeeTypes.SelectedIndex = 0);
            
            this.ReceiptDetailViewModels = new EntityViewModelCollection<ReceiptDetailViewModel, ReceiptDetail>();
            this.ReceiptDetailViewModels.AddRange(this.Model.ReceiptDetails.OrderBy(m => m.LineNo)
                .To(m => new ReceiptDetailViewModel(m)), false);
            this.ReceiptDetailViewModels.SelectedIndex = 0;

            this.ReceiptDetailViewModels.SelectionChanged += (s, e) =>
            {
                this.DeleteReceiptDetailContent = this.ReceiptDetailViewModels.ToDelete ? "Un_delete" : "_Delete";
            };
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.AddReceiptDetailCommand = new RelayCommand<object>(o => OnAddReceiptDetail(o));
            this.EditReceiptDetailCommand = new RelayCommand<object>(o => OnEditReceiptDetail(o), o => this.ReceiptDetailViewModels.CanEdit);
            this.DeleteReceiptDetailCommand = new RelayCommand(o => OnDeleteReceiptDetail(), o => this.ReceiptDetailViewModels.CanDelete);
            this.IsLoaded = true;
            App.CursorDefault();
        }

        public void Fix()
        {
            foreach (var vm in this.ReceiptDetailViewModels)
                vm.Fix();
            ComputeLineNumbers();
            ComputeTotalAmount();
            ComputeNetAmount();
        }

        public void Add(ReceiptDetailViewModel vm)
        {
            this.ReceiptDetailViewModels.Add(vm, true);
            ComputeLineNumbers();
            RaiseRelatedDataChanged(null);
            ComputeTotalAmount();
            if (this.ReceiptDetailViewModels.Count == 1)
                this.Model.SchoolId = vm.School.SchoolId;
            vm.CanSave = false;
        }

        private void OnAddReceiptDetail(object o)
        {
            try
            {
                if (NavigationManager.ShowSearchStudentSectionInfo(this.ReceiptParent.StudentSectionSelector).DialogResult != true ||
                    this.ReceiptParent.StudentSectionSelector.Collection.SelectedIndex == -1)
                    return;

                bool isSchool = this.FeeTypes.SelectedItem.IsSchool();
                bool isFerry = this.FeeTypes.SelectedItem.IsFerry();
                bool isAdmission = this.FeeTypes.SelectedItem.IsAdmission();
                var id = this.ReceiptParent.StudentSectionSelector.Collection.SelectedItem.StudentSection.StudentSectionId;
                var value = this.Parent.UoW.Where<StudentSection>(m => m.StudentSectionId == id)
                    .Include(isSchool, isSchool, isFerry).FirstOrDefault();

                if (value == null)
                    return;
                //if (isSchool && value.Section.CurrencyId.HasValue &&
                //    value.Section.CurrencyId.Value != this.Currency.CurrencyId)
                //{
                //    DialogManager.ShowError("Add Receipt Detail",
                //        string.Format("The payment type of {0} is {1}.", value.Section, value.Section.Currency),
                //        "Receipt must be the same currency type.");
                //    return;
                //}
                if (IsDummy(this.Model.SchoolId) == false &&
                    this.Model.SchoolId != value.Section.Class.Course.AcademicYear.SchoolId)
                {
                    DialogManager.ShowError("Add Receipt Detail",
                        string.Format("This receipt is for {0}.", value.Section.Class.Course.AcademicYear.School),
                        "Receipt must be the same school.");
                    return;
                }

                byte no = 0;
                var model = new ReceiptDetail();
                var vm = new ReceiptDetailViewModel(model);

                if (this.ReceiptDetailViewModels.Count > 0)
                    no = this.ReceiptDetailViewModels.Last().LineNo;
                model.LineNo = ++no;
                model.Quantity = isAdmission ? (byte)1 : this.ReceiptParent.Quantity;
                model.StudentSection = value;
                model.FeeType = this.FeeTypes.SelectedItem;
                if (isSchool)
                {
                    model.Amount = value.Section.GetFee();
                    model.DiscountAmount = value.StudentDiscount.GetDiscountAmountNullable();
                }
                else if (isFerry)
                {
                    model.Amount = value.StudentFerryFee.GetFee();
                }
                else if (isAdmission)
                {
                    if (this.ReceiptParent.AddAdmissionFee)
                        model.Amount = this.ReceiptParent.AdmissionFee;
                }
                Setup(vm);
                if (NavigationManager.ShowReceiptDetail(vm, o).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Receipt Detail", ex);
            }
        }

        public bool Delete(ReceiptDetailViewModel vm)
        {
            bool flag = this.ReceiptDetailViewModels.Delete(vm);

            if (flag)
            {
                ComputeLineNumbers();
                RaiseRelatedDataChanged(null);
                ComputeTotalAmount();
                vm.CanSave = false;
            }

            return flag;
        }

        private void OnDeleteReceiptDetail()
        {
            try
            {
                Delete(this.ReceiptDetailViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Receipt Detail", ex);
            }
        }

        public void Edit(ReceiptDetailViewModel vm)
        {
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
            ComputeTotalAmount();
        }

        private void OnEditReceiptDetail(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.ReceiptDetailViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowReceiptDetail(vm, o).DialogResult == true)
                {
                    Edit(vm);
                }
                else
                {
                    if (vm.ObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "receipt detail"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.ReceiptDetailViewModels.Remove(vm);
                            else
                            {
                                OnEditReceiptDetail(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                   SR.GetString("EditDataAsk3", "receipt detail"),
                                   SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditReceiptDetail(o);
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
                    OnRefreshReceiptDetailBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Receipt Detail", ex);
            }
        }

        private void OnRefreshReceiptDetailBinding()
        {
            if (this.RefreshReceiptDetailBindingCommand != null)
            {
                int index = this.ReceiptDetailViewModels.SelectedIndex;

                this.RefreshReceiptDetailBindingCommand.Execute(null);
                this.ReceiptDetailViewModels.SelectedIndex = index;
            }
        }

        private void Setup(ReceiptDetailViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.OldObjectState == ObjectState.Added)
                vm.ViewName = "Add Receipt Detail";
            else
                vm.ViewName = "Edit Receipt Detail";
            if (vm.IsLoaded)
                return;

            if (this.ReceiptParent.FromMonth == 0)
                vm.LoadData(this.ReceiptParent, this.ReceiptDate);
            else
                vm.LoadData(this.ReceiptParent, this.ReceiptDate.AddMonths(this.ReceiptParent.FromMonth));
        }

        private void Reset()
        {
            this.taxPercentage = this.Model.ComputeTaxPercentage();
        }

        private void ComputeTaxAmount()
        {
            this.TaxAmount = this.Model.ComputeTaxAmount(this.taxPercentage);
        }

        private void ComputeTaxPercentage()
        {
            this.TaxPercentage = this.Model.ComputeTaxPercentage();
        }

        private void ComputeNetAmount()
        {
            this.NetAmount = this.Model.ComputeNetAmount();
        }

        private void ComputeTotalAmount()
        {
            decimal totalAmount = 0;

            foreach (var vm in this.ReceiptDetailViewModels.Where(m => m.ObjectState != ObjectState.Deleted))
                totalAmount += vm.NetAmount;
            this.TotalAmount = totalAmount;
        }

        private void ComputeLineNumbers()
        {
            byte i = 0;

            foreach (var vm in this.ReceiptDetailViewModels.Where(m => m.ObjectState != ObjectState.Deleted))
            {
                vm.LineNo = ++i;
                vm.CanSave = false;
            }
        }

        #endregion
    }
}
