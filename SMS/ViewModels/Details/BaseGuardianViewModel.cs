using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
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
    internal abstract class BaseGuardianViewModel : EntityViewModelEPA<Guardian>
    {
        # region Fields

        private byte[] photo;
        private Country birthCountry;
        private Religion religion;

        # endregion

        # region Properties

        public ObservableCollection<Country> Countries { get; protected set; }
        public ObservableCollection<Religion> Religions { get; protected set; }

        [MaxLength(15, ErrorMessage = "Title is too long.")]
        public string Title
        {
            get { return this.Model.Title; }
            set
            {
                Repair(ref value);
                if (this.Model.Title != value)
                {
                    ValidateProperty(value, "Title");
                    this.Model.Title = value;
                    RaiseDataChanged("Title");
                    RaisePropertyChanged("TitleGuardianName");
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Guardian Name is required.")]
        [MaxLength(128, ErrorMessage = "Guardian Name is too long.")]
        public string GuardianName
        {
            get { return this.Model.GuardianName; }
            set
            {
                if (this.Model.GuardianName != value)
                {
                    ValidateProperty(value, "GuardianName");
                    this.Model.GuardianName = value;
                    RaiseDataChanged("GuardianName");
                    RaisePropertyChanged("TitleGuardianName");
                }
            }
        }

        public string TitleGuardianName
        {
            get { return this.Model.TitleGuardianName; }
        }

        public bool Gender
        {
            get { return this.Model.Gender; }
            set
            {
                if (this.Model.Gender != value)
                {
                    this.Model.Gender = value;
                    RaiseDataChanged("Gender");
                }
            }
        }

        public DateTime? BirthDate
        {
            get { return this.Model.BirthDate; }
            set
            {
                if (this.Model.BirthDate != value)
                {
                    this.Model.BirthDate = value;
                    RaiseDataChanged("BirthDate");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Nationality is too long.")]
        public string Nationality
        {
            get { return this.Model.Nationality; }
            set
            {
                Repair(ref value);
                if (this.Model.Nationality != value)
                {
                    ValidateProperty(value, "Nationality");
                    this.Model.Nationality = value;
                    RaiseDataChanged("Nationality");
                }
            }
        }

        [MaxLength(30, ErrorMessage = "NIRC is too long.")]
        public string NIRC
        {
            get { return this.Model.NIRC; }
            set
            {
                Repair(ref value);
                if (this.Model.NIRC != value)
                {
                    ValidateProperty(value, "NIRC");
                    this.Model.NIRC = value;
                    RaiseDataChanged("NIRC");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Education is too long.")]
        public string Education
        {
            get { return this.Model.Education; }
            set
            {
                Repair(ref value);
                if (this.Model.Education != value)
                {
                    ValidateProperty(value, "Education");
                    this.Model.Education = value;
                    RaiseDataChanged("Education");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Employment is too long.")]
        public string Employment
        {
            get { return this.Model.Employment; }
            set
            {
                Repair(ref value);
                if (this.Model.Employment != value)
                {
                    ValidateProperty(value, "Employment");
                    this.Model.Employment = value;
                    RaiseDataChanged("Employment");
                }
            }
        }

        [MaxLength(50, ErrorMessage = "Designation is too long.")]
        public string Designation
        {
            get { return this.Model.Designation; }
            set
            {
                Repair(ref value);
                if (this.Model.Designation != value)
                {
                    ValidateProperty(value, "Designation");
                    this.Model.Designation = value;
                    RaiseDataChanged("Designation");
                }
            }
        }

        [MaxLength(20, ErrorMessage = "Passport is too long.")]
        public string Passport
        {
            get { return this.Model.Passport; }
            set
            {
                Repair(ref value);
                if (this.Model.Passport != value)
                {
                    ValidateProperty(value, "Passport");
                    this.Model.Passport = value;
                    RaiseDataChanged("Passport");
                }
            }
        }

        [MaxLength(30, ErrorMessage = "Visa is too long.")]
        public string Visa
        {
            get { return this.Model.Visa; }
            set
            {
                Repair(ref value);
                if (this.Model.Visa != value)
                {
                    ValidateProperty(value, "Visa");
                    this.Model.Visa = value;
                    RaiseDataChanged("Visa");
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

        public Country BirthCountry
        {
            get { return this.birthCountry; }
            set
            {
                if (this.birthCountry != value)
                {
                    this.birthCountry = value;
                    if (this.birthCountry != null && !IsDummy(this.birthCountry.CountryId))
                        this.Model.BirthCountry = this.birthCountry;
                    else
                        this.Model.BirthCountry = null;
                    RaiseDataChanged("BirthCountry");
                }
            }
        }

        public Religion Religion
        {
            get { return this.religion; }
            set
            {
                if (this.religion != value)
                {
                    this.religion = value;
                    if (this.religion != null && !IsDummy(this.religion.ReligionId))
                        this.Model.Religion = this.religion;
                    else
                        this.Model.Religion = null;
                    RaiseDataChanged("Religion");
                }
            }
        }

        public byte[] Photo
        {
            get { return this.photo; }
            set
            {
                if (!this.photo.IsEqual(value))
                {
                    this.photo = value;
                    if (this.photo == null)
                    {
                        if (this.Model.Photo != null)
                            this.Model.Photo.Photo = null;
                    }
                    else
                    {
                        if (this.Model.Photo == null)
                            this.Model.Photo = new GuardianPhoto();
                        this.Model.Photo.Photo = this.photo;
                    }
                    RaiseDataChanged("Photo");
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
            get { return IsDummy(this.Model.GuardianId); }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.AddressViewModels.HasAnyErrors
                    || this.EmailViewModels.HasAnyErrors || this.PhoneViewModels.HasAnyErrors;
            }
        }

        # endregion

        # region Ctor, Dtor

        public BaseGuardianViewModel(Guardian model, IParent parent)
            : base(model, parent)
        { }

        # endregion

        # region Methods

        protected override EmailViewModel CreateEmailViewModel()
        {
            return new EmailViewModel(new GuardianEmail());
        }

        protected override PhoneViewModel CreatePhoneViewModel()
        {
            return new PhoneViewModel(new GuardianPhone());
        }

        protected override PersonAddressViewModel CreateAddressViewModel(Address model)
        {
            return new PersonAddressViewModel(new GuardianAddress { Address = model });
        }

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.AddressViewModels.AcceptAllChanges();
            this.EmailViewModels.AcceptAllChanges();
            this.PhoneViewModels.AcceptAllChanges();
            this.IsDetectedChanges = false;
            SetCreatedBy();
            SetModifiedBy();
            base.BaseAcceptChanges();
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.Parent == null)
                return;

            bool removeAdded = this.Parent.RemoveAdded;

            this.AddressViewModels.Refresh(removeAdded);
            if (this.OldObjectState == ObjectState.Added)
                return;

            var state = this.OverallState;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            if (this.Model.Photo != null)
            {
                if (this.Parent.UoW.Entry(this.Model.Photo).State == System.Data.Entity.EntityState.Added)
                    this.Model.Photo = null;
                else
                    this.Parent.UoW.RefreshToOriginalValues(this.Model.Photo);
            }
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.GuardianPhones, removeAdded);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.GuardianEmails, removeAdded);
            this.EmailViewModels.Refresh(removeAdded);
            this.PhoneViewModels.Refresh(removeAdded);
            //SetRelationships();
            //InitializeRelationships();
            SetRelationshipsInternal();
            this.IsDetectedChanges = false;
            base.BaseRefresh();

            bool flag = state == ObjectState.Modified && (this.AddressViewModels.HasAnyAdded() ||
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
                this.Parent.UoW.RemoveRange(this.Model.GuardianEmails);
                this.Parent.UoW.RemoveRange(this.Model.GuardianPhones);
                this.Parent.UoW.RemoveRange(this.Model.GuardianAddresses);
                this.Parent.UoW.Remove(this.Model.Photo);
                this.Parent.UoW.Remove(this.Model);
            }
            else if (this.OverallState == ObjectState.Added || this.OverallState == ObjectState.Modified)
            {
                this.EmailViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as GuardianEmail));
                this.EmailViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.GuardianEmails.AddIfNotExist(vm.Model as GuardianEmail));

                this.PhoneViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as GuardianPhone));
                this.PhoneViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.GuardianPhones.AddIfNotExist(vm.Model as GuardianPhone));

                this.AddressViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as GuardianAddress));
                this.AddressViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.GuardianAddresses.AddIfNotExist(vm.Model as GuardianAddress));

                if (this.Model.Photo != null && this.Model.Photo.Photo == null)
                    this.Parent.UoW.Remove(this.Model.Photo);
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
            this.CanSave = !this.HasAnyErrors;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.GuardianName))
                ValidateProperty(this.GuardianName, "GuardianName");
        }

        protected void SetRelationshipsInternal()
        {
            if (this.Model.Photo == null)
                this.photo = null;
            else
                this.photo = this.Model.Photo.Photo;
            this.birthCountry = this.Model.BirthCountry;
            this.religion = this.Model.Religion;
            if (this.birthCountry == null)
                this.birthCountry = this.Countries.GetDefault();
            if (this.religion == null)
                this.religion = this.Religions.GetDefault();
        }

        //public void SetRelationships()
        //{
        //    if (this.Model.Photo == null)
        //        this.Photo = null;
        //    else
        //        this.Photo = this.Model.Photo.Photo;
        //    this.BirthCountry = this.Model.BirthCountry;
        //    this.Religion = this.Model.Religion;
        //}

        //public void InitializeRelationships()
        //{
        //    if (this.birthCountry == null)
        //        this.birthCountry = this.Countries.GetDefault();
        //    if (this.religion == null)
        //        this.religion = this.Religions.GetDefault();
        //}

        //public void FixRelationships()
        //{
        //    if (this.photo == null)
        //        this.Model.Photo = null;
        //    else
        //    {
        //        if (this.Model.Photo == null)
        //            this.Model.Photo = new GuardianPhoto() { Photo = this.photo };
        //        else if (!this.photo.ArrayEquals(this.Model.Photo.Photo))
        //            this.Model.Photo.Photo = this.photo;
        //    }

        //    if (IsDummy(this.birthCountry.CountryId))
        //        this.Model.BirthCountry = null;
        //    else if (this.Model.BirthCountry != this.birthCountry)
        //        this.Model.BirthCountry = this.birthCountry;

        //    if (IsDummy(this.religion.ReligionId))
        //        this.Model.Religion = null;
        //    else if (this.Model.Religion != this.religion)
        //        this.Model.Religion = this.religion;
        //}

        # endregion
    }
}
