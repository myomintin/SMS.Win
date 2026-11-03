using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.DataAnnotations;
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

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class ReceiptDetailViewModel : EntityViewModel<ReceiptDetail>
    {
        #region Fields

        private decimal totalAmount;
        private decimal discountPercentage;

        #endregion

        #region Properties

        public IReceiptParent Parent { get; private set; }
        public bool CanSelectMonth { get; private set; }

        public byte LineNo
        {
            get { return this.Model.LineNo; }
            set
            {
                if (this.Model.LineNo != value)
                {
                    this.Model.LineNo = value;
                    RaiseDataChanged("LineNo");
                }
            }
        }

        public DateTime? FromMonth
        {
            get { return this.Model.FromMonth; }
            set
            {
                RepairDay(ref value, 1);
                if (this.Model.FromMonth != value)
                {
                    this.Model.FromMonth = value;
                    RaiseDataChanged("FromMonth");
                    RaisePropertyChanged("ToMonth");
                }
            }
        }

        public byte Quantity
        {
            get { return this.Model.Quantity; }
            private set
            {
                if (this.Model.Quantity != value)
                {
                    this.Model.Quantity = value;
                    RaiseDataChanged("Quantity");
                    RaisePropertyChanged("ToMonth");
                    ComputeTotalAmount();
                }
            }
        }

        [MinValue(0, ErrorMessage = "Amount must be at least 0.")]
        public decimal Amount
        {
            get { return this.Model.Amount; }
            set
            {
                RepairCurrency(ref value);
                if (this.Model.Amount != value)
                {
                    ValidateProperty(value, "Amount");
                    this.Model.Amount = value;
                    RaiseDataChanged("Amount");
                    ComputeTotalAmount();
                }
            }
        }

        public decimal TotalAmount
        {
            get { return this.totalAmount; }
            private set
            {
                RepairCurrency(ref value);
                if (this.totalAmount != value)
                {
                    this.totalAmount = value;
                    RaisePropertyChanged("TotalAmount");
                    ComputeDiscountAmount();
                    ComputeNetAmount();
                }
            }
        }

        [Range(0D, 100D, ErrorMessage = "Discount Percentage must be between 0 to 100.")]
        public decimal DiscountPercentage
        {
            get { return this.discountPercentage; }
            set
            {
                if (this.discountPercentage != value)
                {
                    ValidateProperty(value, "DiscountPercentage");
                    this.discountPercentage = value;
                    RaisePropertyChanged("DiscountPercentage");
                    ComputeDiscountAmount();
                }
            }
        }

        [MinValue(0, ErrorMessage = "Discount Amount must be at least 0.")]
        public decimal? DiscountAmount
        {
            get { return this.Model.DiscountAmount; }
            set
            {
                RepairCurrency(ref value);
                if (this.Model.DiscountAmount != value)
                {
                    ValidateProperty(value, "DiscountAmount");
                    this.Model.DiscountAmount = value;
                    RaiseDataChanged("DiscountAmount");
                    ComputeDiscountPercentage();
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

        public override bool CanDelete
        {
            get { return this.Model.CanDelete; }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.ReceiptDetailId); }
        }

        public FeeType FeeType
        {
            get { return this.Model.FeeType; }
        }

        public StudentSection StudentSection
        {
            get { return this.Model.StudentSection; }
        }

        public Student Student
        {
            get
            {
                var value = this.Model.StudentSection;

                if (value == null)
                    return null;
                return value.Student;
            }
        }

        public Section Section
        {
            get
            {
                var value = this.Model.StudentSection;

                if (value == null)
                    return null;
                return value.Section;
            }
        }

        public Class Class
        {
            get
            {
                var value = this.Section;

                if (value == null)
                    return null;
                return value.Class;
            }
        }

        public Course Course
        {
            get
            {
                var value = this.Class;

                if (value == null)
                    return null;
                return value.Course;
            }
        }

        public AcademicYear AcademicYear
        {
            get
            {
                var value = this.Course;

                if (value == null)
                    return null;
                return value.AcademicYear;
            }
        }

        public School School
        {
            get
            {
                var value = this.AcademicYear;

                if (value == null)
                    return null;
                return value.School;
            }
        }

        public string ToMonth
        {
            get
            {
                if (this.Model.FromMonth.HasValue == false)
                    return null;

                var value = this.Model.FromMonth.Value;

                if (this.Model.Quantity == 1)
                    return value.ToString("MMMM yyyy");
                return value.AddMonths(this.Model.Quantity - 1).ToString("MMMM yyyy");
            }
        }

        #endregion

        #region Ctor, Dtor

        public ReceiptDetailViewModel(ReceiptDetail model)
            : base(model)
        {
            if (!this.IsNew)
                Reset();
        }

        #endregion

        #region Methods

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            Reset();
            base.BaseRefresh();
        }

        public void DetectErrors()
        {
            if (!this.IsLoaded)
                return;

            this.CanSave = !(this.HasAnyErrors || this.StudentSection == null);
        }

        public void LoadData(IReceiptParent parent, DateTime? date)
        {
            if (this.IsLoaded)
                return;

            App.CursorWait();
            this.Parent = parent;

            if (this.IsNew)
            {
                if (date.HasValue)
                {
                    RepairDay(ref date, 1);
                    this.Model.FromMonth = date;
                }
                this.totalAmount = this.Model.ComputeTotalAmount();
                this.discountPercentage = this.Model.ComputeDiscountPercentage(withQuantity: false);
            }
            this.CanSelectMonth = IsMonthly();

            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
            if (this.IsNew)
            {
                ComputeNetAmount();
                DetectErrors();
            }
            App.CursorDefault();
        }

        public void Fix()
        {
            ComputeNetAmount();
        }

        private void Reset()
        {
            this.totalAmount = this.Model.ComputeTotalAmount();
            this.discountPercentage = this.Model.ComputeDiscountPercentage();
        }

        private void ComputeDiscountAmount()
        {
            this.DiscountAmount = this.Model.ComputeDiscountAmount(this.discountPercentage);
        }

        private void ComputeDiscountPercentage()
        {
            this.DiscountPercentage = this.Model.ComputeDiscountPercentage();
        }

        private void ComputeNetAmount()
        {
            this.NetAmount = this.Model.ComputeNetAmount();
        }

        private void ComputeTotalAmount()
        {
            this.TotalAmount = this.Model.ComputeTotalAmount();
        }

        private bool IsMonthly()
        {
            bool flag = true;

            if (this.FeeType != null && this.FeeType.FeeTypeName == "School" && this.Section != null)
                flag = this.Section.Monthly;

            return flag;
        }

        #endregion
    }
}
