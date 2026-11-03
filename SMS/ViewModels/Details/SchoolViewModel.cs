using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
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
    internal sealed class SchoolViewModel : EntityViewModelEP<School>, ILocationAddressViewModel
    {
        #region Fields

        private AddressViewModel addressViewModel;

        #endregion

        #region Properties
        
        public ICommand AddAddressCommand { get; private set; }

        public IAddressParent AddressParent
        {
            get { return this.Parent as IAddressParent; }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "School Name is required.")]
        [MaxLength(128, ErrorMessage = "School Name is too long.")]
        public string SchoolName
        {
            get { return this.Model.SchoolName; }
            set
            {
                if (this.Model.SchoolName != value)
                {
                    ValidateProperty(value, "SchoolName");
                    this.Model.SchoolName = value;
                    RaiseDataChanged("SchoolName");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Motto is too long.")]
        public string Motto
        {
            get { return this.Model.Motto; }
            set
            {
                Repair(ref value);
                if (this.Model.Motto != value)
                {
                    ValidateProperty(value, "Motto");
                    this.Model.Motto = value;
                    RaiseDataChanged("Motto");
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
            get { return IsDummy(this.Model.SchoolId); }
        }

        public AddressViewModel AddressViewModel
        {
            get { return this.addressViewModel; }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.addressViewModel.HasAnyErrors
                    || this.EmailViewModels.HasAnyErrors || this.PhoneViewModels.HasAnyErrors;
            }
        }

        #endregion

        #region Ctor, Dtor

        public SchoolViewModel(School model, IParent parent)
            : base(model, parent)
        {
            if (this.Model.Address == null)
                this.Model.Address = new Address();
            this.addressViewModel = new AddressViewModel(this.Model.Address);
        }

        #endregion

        #region Methods

        protected override EmailViewModel CreateEmailViewModel()
        {
            return new EmailViewModel(new SchoolEmail());
        }

        protected override PhoneViewModel CreatePhoneViewModel()
        {
            return new PhoneViewModel(new SchoolPhone());
        }
        
        protected override void InitializeCommands()
        {
            if (this.IsLoaded)
                return;

            base.InitializeCommands();
            this.AddAddressCommand = new RelayCommand<object>(o => OnAddAddress(o));
        }

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.EmailViewModels.AcceptAllChanges();
            this.PhoneViewModels.AcceptAllChanges();
            this.addressViewModel.AcceptChanges();
            this.IsDetectedChanges = false;
            SetCreatedBy();
            SetModifiedBy();
            base.BaseAcceptChanges();
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            var state = this.OverallState;
            bool removeAdded = this.Parent.RemoveAdded;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            if (this.addressViewModel.Model == this.Model.Address)
                this.addressViewModel.Refresh();
            else
                SetModel(this.Model.Address);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.SchoolEmails, removeAdded);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.SchoolPhones, removeAdded);
            this.EmailViewModels.Refresh(removeAdded);
            this.PhoneViewModels.Refresh(removeAdded);
            this.IsDetectedChanges = false;
            base.BaseRefresh();

            bool flag = state == ObjectState.Modified &&
                (this.addressViewModel.OldObjectState == ObjectState.Added ||
                this.EmailViewModels.HasAnyAdded() || this.PhoneViewModels.HasAnyAdded());
            if (flag)
                this.OverallState = state;
        }

        internal override void DetectChanges()
        {
            if (this.IsDetectedChanges || this.Parent == null)
                return;

            if (this.ObjectState == ObjectState.Deleted && this.OldObjectState != ObjectState.Added)
            {
                this.Parent.UoW.RemoveRange(this.Model.SchoolEmails);
                this.Parent.UoW.RemoveRange(this.Model.SchoolPhones);
                this.Parent.UoW.Remove(this.Model);
            }
            else if (this.OverallState == ObjectState.Added || this.OverallState == ObjectState.Modified)
            {
                this.EmailViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as SchoolEmail));
                this.EmailViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.SchoolEmails.AddIfNotExist(vm.Model as SchoolEmail));

                this.PhoneViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as SchoolPhone));
                this.PhoneViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.SchoolPhones.AddIfNotExist(vm.Model as SchoolPhone));

                if (this.ObjectState == ObjectState.Added)
                    this.Parent.UoW.Add(this.Model);
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

            ValidateRequiredProperties();
            this.addressViewModel.ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.SchoolName))
                ValidateProperty(this.SchoolName, "SchoolName");
        }

        public void LoadData()
        {
            if (this.IsLoaded)
                return;

            App.CursorWait();
            SetCreatedBy();
            SetModifiedBy();
            this.addressViewModel.LoadData(this.Parent);

            InitializeEmails();
            this.EmailViewModels.AddRange(this.Model.SchoolEmails
                .To(m => new EmailViewModel(m)), false);
            this.EmailViewModels.SelectedIndex = 0;

            InitializePhones();
            this.PhoneViewModels.AddRange(this.Model.SchoolPhones
                .To(m => new PhoneViewModel(m)), false);
            this.PhoneViewModels.SelectedIndex = 0;

            this.addressViewModel.DataChanged += (s, e) =>
            {
                RaiseRelatedDataChanged(null);
            };
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };

            InitializeCommands();
            this.IsLoaded = true;
            App.CursorDefault();
        }

        private void OnAddAddress(object o)
        {
            try
            {
                var avm = this.AddressParent.AddAddressViewModel;

                if (NavigationManager.ShowAdd(avm, o).DialogResult == false)
                    return;

                if (avm.IsNew)
                {
                    SetModel(new Address());
                }
                else
                {
                    var asvm = this.AddressParent.AddressSelector;

                    if (NavigationManager.ShowSearchAddress(asvm, o).DialogResult == true &&
                        asvm.Collection.SelectedIndex != -1)
                    {
                        var value = asvm.Collection.SelectedItem;
                        var model = this.Parent.UoW.FirstOrDefault<Address>(m => m.AddressId == value.AddressId);

                        if (model != null)
                        {
                            SetModel(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Address", ex);
            }
        }

        private void SetModel(Address value)
        {
            if (this.addressViewModel.Model != value)
            {
                if (this.addressViewModel.OldObjectState == ObjectState.Added)
                    this.Parent.UoW.AddedToDetached(this.addressViewModel.Model);
                else
                    this.addressViewModel.Refresh();
                this.addressViewModel = new AddressViewModel(value);
                if (this.addressViewModel.Model != this.Model.Address)
                    this.Model.Address = this.addressViewModel.Model;
                this.AddressViewModel.LoadData(this.Parent);
                this.addressViewModel.DataChanged += (s, e) =>
                {
                    RaiseRelatedDataChanged(null);
                };
                RaiseDataChanged("AddressViewModel");
            }
        }

        private void Setup(AddressViewModel vm)
        {
            if (vm == null)
                return;

            if (vm.OldObjectState == ObjectState.Added)
                vm.ViewName = "Add Address";
            else
                vm.ViewName = "Edit Address";
            if (vm.IsLoaded)
                return;

            vm.LoadData(this.Parent);
        }

        #endregion
    }
}
