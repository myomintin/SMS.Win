using CodeflexSystems.Library.DataAnnotations;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentFerryFeeViewModel : EntityViewModelCM<StudentFerryFee>
    {
        #region Properties

        [MinValue(0, ErrorMessage = "Fee must be at least 0.")]
        public decimal Fee
        {
            get { return this.Model.Fee; }
            set
            {
                RepairCurrency(ref value);
                if (this.Model.Fee != value)
                {
                    ValidateProperty(value, "Fee");
                    this.Model.Fee = value;
                    RaiseDataChanged("Fee");
                }
            }
        }

        public bool Monthly
        {
            get { return this.Model.Monthly; }
            set
            {
                if (this.Model.Monthly != value)
                {
                    this.Model.Monthly = value;
                    RaiseDataChanged("Monthly");
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

        public StudentFerryFeeViewModel(StudentFerryFee model)
            : base(model)
        { }

        #endregion

        #region Methods

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

        #endregion
    }
}
