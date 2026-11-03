using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class PersonAddressViewModel : EntityViewModelCM<PersonAddress>
    {
        #region Fields

        private AddressViewModel addressViewModel;

        #endregion

        #region Properties

        public ObservableCollection<AddressType> AddressTypes { get; private set; }

        public bool IsDefault
        {
            get { return this.Model.IsDefault; }
            set
            {
                if (this.Model.IsDefault != value)
                {
                    this.Model.IsDefault = value;
                    RaiseDataChanged("IsDefault");
                }
            }
        }

        public AddressViewModel AddressViewModel
        {
            get { return this.addressViewModel; }
            //internal set
            //{
            //    if (this.addressViewModel != value)
            //    {
            //        this.addressViewModel = value;
            //        if (this.addressViewModel.Model != this.Model.Address)
            //            this.Model.Address = this.addressViewModel.Model;
            //        RaiseDataChanged("AddressViewModel");
            //    }
            //}
        }

        public AddressType AddressType
        {
            get { return this.Model.AddressType; }
            set
            {
                if (this.Model.AddressType != value)
                {
                    this.Model.AddressType = value;
                    RaiseDataChanged("AddressType");
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
            get { return IsDummy(this.Model.CreatedDate); }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.addressViewModel.HasAnyErrors;
            }
        }

        #endregion

        #region Ctor, Dtor

        public PersonAddressViewModel(PersonAddress model)
            : base(model)
        {
            if (this.Model.Address == null)
                this.Model.Address = new Address();
            this.addressViewModel = new AddressViewModel(this.Model.Address);
        }

        #endregion

        #region Methods

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.addressViewModel.AcceptChanges();
            SetCreatedBy();
            SetModifiedBy();
            base.BaseAcceptChanges();
        }

        public override void Refresh()
        {
            this.addressViewModel.Refresh();
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            base.BaseRefresh();
            this.OverallState = this.addressViewModel.OverallState;
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

            this.addressViewModel.ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.addressViewModel.LoadData(this.Parent);
            this.AddressTypes = this.Parent.UoW.AddressTypes;
            if (this.Model.AddressType == null)
                this.Model.AddressType = this.AddressTypes.GetDefault();

            this.addressViewModel.DataChanged += (s, e) =>
            {
                RaiseRelatedDataChanged(null);
            };
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        #endregion
    }
}
