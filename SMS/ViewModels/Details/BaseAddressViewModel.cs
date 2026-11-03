using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Core;
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
    internal abstract class BaseAddressViewModel : EntityViewModelCM<Address>
    {
        #region Properties

        public ObservableCollection<Country> Countries { get; protected set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Line1 is required.")]
        [MaxLength(250, ErrorMessage = "Line1 is too long.")]
        public string Line1
        {
            get { return this.Model.Line1; }
            set
            {
                if (this.Model.Line1 != value)
                {
                    ValidateProperty(value, "Line1");
                    this.Model.Line1 = value;
                    RaiseDataChanged("Line1");
                }
            }
        }

        [MaxLength(250, ErrorMessage = "Line2 is too long.")]
        public string Line2
        {
            get { return this.Model.Line2; }
            set
            {
                Repair(ref value);
                if (this.Model.Line2 != value)
                {
                    ValidateProperty(value, "Line2");
                    this.Model.Line2 = value;
                    RaiseDataChanged("Line2");
                }
            }
        }

        [MaxLength(20, ErrorMessage = "Zip Code is too long.")]
        public string ZipCode
        {
            get { return this.Model.ZipCode; }
            set
            {
                Repair(ref value);
                if (this.Model.ZipCode != value)
                {
                    ValidateProperty(value, "ZipCode");
                    this.Model.ZipCode = value;
                    RaiseDataChanged("ZipCode");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "City is too long.")]
        public string City
        {
            get { return this.Model.City; }
            set
            {
                Repair(ref value);
                if (this.Model.City != value)
                {
                    ValidateProperty(value, "City");
                    this.Model.City = value;
                    RaiseDataChanged("City");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Township is too long.")]
        public string Township
        {
            get { return this.Model.Township; }
            set
            {
                Repair(ref value);
                if (this.Model.Township != value)
                {
                    ValidateProperty(value, "Township");
                    this.Model.Township = value;
                    RaiseDataChanged("Township");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "State is too long.")]
        public string State
        {
            get { return this.Model.State; }
            set
            {
                Repair(ref value);
                if (this.Model.State != value)
                {
                    ValidateProperty(value, "State");
                    this.Model.State = value;
                    RaiseDataChanged("State");
                }
            }
        }

        public Country Country
        {
            get { return this.Model.Country; }
            set
            {
                if (this.Model.Country != value)
                {
                    this.Model.Country = value;
                    RaiseDataChanged("Country");
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
                if (this.IsLoaded || this.IsNew || this.DetectCanDelete == null)
                    return this.Model.CanDelete;

                return this.DetectCanDelete();
            }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.AddressId); }
        }

        #endregion

        #region Ctor, Dtor

        public BaseAddressViewModel(Address model)
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
            if (string.IsNullOrEmpty(this.Line1))
                ValidateProperty(this.Line1, "Line1");
        }

        #endregion
    }
}
