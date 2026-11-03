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
    internal sealed class CurrencyViewModel : EntityViewModelCM<Currency>
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "Currency Name is required.")]
        [MaxLength(128, ErrorMessage = "Currency Name is too long.")]
        public string CurrencyName
        {
            get { return this.Model.CurrencyName; }
            set
            {
                if (this.Model.CurrencyName != value)
                {
                    ValidateProperty(value, "CurrencyName");
                    this.Model.CurrencyName = value;
                    RaiseDataChanged("CurrencyName");
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Currency Code is required.")]
        [MaxLength(5, ErrorMessage = "Currency Code is too long.")]
        public string CurrencyCode
        {
            get { return this.Model.CurrencyCode; }
            set
            {
                if (this.Model.CurrencyCode != value)
                {
                    ValidateProperty(value, "CurrencyCode");
                    this.Model.CurrencyCode = value;
                    RaiseDataChanged("CurrencyCode");
                }
            }
        }

        [MaxLength(5, ErrorMessage = "Currency Symbol is too long.")]
        public string CurrencySymbol
        {
            get { return this.Model.CurrencySymbol; }
            set
            {
                if (this.Model.CurrencySymbol != value)
                {
                    ValidateProperty(value, "CurrencySymbol");
                    this.Model.CurrencySymbol = value;
                    RaiseDataChanged("CurrencySymbol");
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
            get { return IsDummy(this.Model.CurrencyId); }
        }

        public bool IsReadOnly { get; set; }

        #endregion

        #region Ctor, Dtor

        public CurrencyViewModel(Currency model)
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
            if (string.IsNullOrEmpty(this.CurrencyName))
                ValidateProperty(this.CurrencyName, "CurrencyName");
            if (string.IsNullOrEmpty(this.CurrencyCode))
                ValidateProperty(this.CurrencyCode, "CurrencyCode");
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
