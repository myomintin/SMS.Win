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
    internal sealed class AcademicYearViewModel : EntityViewModelCM<AcademicYear>
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "Academic Year Name is required.")]
        [MaxLength(128, ErrorMessage = "Academic Year Name is too long.")]
        public string AcademicYearName
        {
            get { return this.Model.AcademicYearName; }
            set
            {
                if (this.Model.AcademicYearName != value)
                {
                    ValidateProperty(value, "AcademicYearName");
                    this.Model.AcademicYearName = value;
                    RaiseDataChanged("AcademicYearName");
                }
            }
        }

        public bool Active
        {
            get { return this.Model.Active; }
            set
            {
                if (this.Model.Active != value)
                {
                    this.Model.Active = value;
                    RaiseDataChanged("Active");
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
            get
            {
                if (this.IsNew || this.DetectCanDelete == null)
                    return this.Model.CanDelete;

                return this.DetectCanDelete();
            }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.AcademicYearId); }
        }

        public School School
        {
            get { return this.Model.School; }
            set
            {
                if (this.Model.School != value)
                {
                    this.Model.School = value;
                    RaiseDataChanged("School");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public AcademicYearViewModel(AcademicYear model)
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

            ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.AcademicYearName))
                ValidateProperty(this.AcademicYearName, "AcademicYearName");
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
