using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.DataAnnotations;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentDiscountViewModel : EntityViewModelCM<StudentDiscount>
    {
        #region Fields

        private decimal discountPercentage;

        #endregion

        #region Properties

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
                    ComputeDiscount();
                }
            }
        }

        [MinValue(0, ErrorMessage = "Discount Amount must be at least 0.")]
        public decimal DiscountAmount
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
            get { return IsDummy(this.Model.CreatedDate); }
        }

        public StudentSection StudentSection
        {
            get { return this.Model.StudentSection; }
        }

        public Student Student
        {
            get
            {
                if (this.Model.StudentSection == null)
                    return null;

                return this.Model.StudentSection.Student;
            }
        }

        public Section Section
        {
            get
            {
                if (this.Model.StudentSection == null)
                    return null;

                return this.Model.StudentSection.Section;
            }
        }

        public Class Class
        {
            get
            {
                if (this.Section == null)
                    return null;

                return this.Section.Class;
            }
        }

        public Course Course
        {
            get
            {
                if (this.Class == null)
                    return null;

                return this.Class.Course;
            }
        }

        public AcademicYear AcademicYear
        {
            get
            {
                if (this.Course == null)
                    return null;

                return this.Course.AcademicYear;
            }
        }

        public School School
        {
            get
            {
                if (this.AcademicYear == null)
                    return null;

                return this.AcademicYear.School;
            }
        }

        #endregion

        #region Ctor, Dtor

        public StudentDiscountViewModel(StudentDiscount model)
            : base(model)
        {
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

            this.CanSave = !this.HasAnyErrors;
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        private void ComputeDiscount()
        {
            //if (this.DiscountPercentage > 0 && this.Section != null && this.Section.Fee.HasValue)
            //    this.DiscountAmount = (this.Section.Fee.Value / 100) * this.DiscountPercentage;
            //else
            //    this.DiscountAmount = 0;

            decimal? fee = this.Section.Fee;
            if (fee.HasValue)
                this.DiscountAmount = this.discountPercentage * fee.Value / 100;
            else
                this.DiscountAmount = 0;
        }

        private void ComputeDiscountPercentage()
        {
            this.DiscountPercentage = this.Model.ComputeDiscountPercentage();
        }

        private void Reset()
        {
            this.PropertyChangedEnabled = false;
            ComputeDiscountPercentage();
            this.PropertyChangedEnabled = true;
        }

        #endregion
    }
}
