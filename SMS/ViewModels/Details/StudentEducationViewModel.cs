using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentEducationViewModel : EntityViewModelCM<StudentSection>
    {
        #region Properties

        public Section Section
        {
            get { return this.Model.Section; }
        }

        public DateTime? RegisteredDate
        {
            get { return this.Model.RegisteredDate; }
            set
            {
                if (this.Model.RegisteredDate != value)
                {
                    this.Model.RegisteredDate = value;
                    RaiseDataChanged("RegisteredDate");
                }
            }
        }

        public DateTime? ResignedDate
        {
            get { return this.Model.ResignedDate; }
            set
            {
                if (this.Model.ResignedDate != value)
                {
                    this.Model.ResignedDate = value;
                    RaiseDataChanged("ResignedDate");
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
                    if (this.Model.Active)
                        this.ResignedDate = null;
                    else
                        this.ResignedDate = DateTime.Today;
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
            get { return IsDummy(this.Model.StudentSectionId); }
        }

        public Class Class
        {
            get { return this.Section.Class; }
        }

        public Course Course
        {
            get { return this.Class.Course; }
        }

        public AcademicYear AcademicYear
        {
            get { return this.Course.AcademicYear; }
        }

        public School School
        {
            get { return this.AcademicYear.School; }
        }

        public bool Current
        {
            get { return this.AcademicYear.Active; }
        }

        #endregion

        #region Ctor, Dtor

        public StudentEducationViewModel(StudentSection model)
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
