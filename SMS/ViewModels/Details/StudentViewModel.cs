using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentViewModel : EntityViewModelEPAG<Student>
    {
        #region Fields

        private string deleteEducationContent;
        private byte[] photo;
        private Country birthCountry;
        private Religion religion;

        #endregion

        #region Properties

        public ObservableCollection<Country> Countries { get; private set; }
        public ObservableCollection<Religion> Religions { get; private set; }
        public EntityViewModelCollection<StudentEducationViewModel, StudentSection> EducationViewModels { get; private set; }
        public Action<StudentEducationViewModel> EducationPropertiesChanged { get; set; }

        public ICommand AddEducationCommand { get; private set; }
        public ICommand EditEducationCommand { get; private set; }
        public ICommand DeleteEducationCommand { get; private set; }
        public ICommand RefreshEducationBindingCommand { get; set; }

        public IStudentParent StudentParent
        {
            get
            {
                return base.Parent as IStudentParent;
            }
        }

        [MaxLength(10, ErrorMessage = "Id is too long.")]
        public string Id
        {
            get { return this.Model.Id; }
            set
            {
                Repair(ref value);
                if (this.Model.Id != value)
                {
                    ValidateProperty(value, "Id");
                    this.Model.Id = value;
                    RaiseDataChanged("Id");
                }
            }
        }

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
                }
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Student Name is required.")]
        [MaxLength(128, ErrorMessage = "Student Name is too long.")]
        public string StudentName
        {
            get { return this.Model.StudentName; }
            set
            {
                if (this.Model.StudentName != value)
                {
                    ValidateProperty(value, "StudentName");
                    this.Model.StudentName = value;
                    RaiseDataChanged("StudentName");
                    RaisePropertyChanged("StudentFullName");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Other Name is too long.")]
        public string OtherName
        {
            get { return this.Model.OtherName; }
            set
            {
                Repair(ref value);
                if (this.Model.OtherName != value)
                {
                    ValidateProperty(value, "OtherName");
                    this.Model.OtherName = value;
                    RaiseDataChanged("OtherName");
                    RaisePropertyChanged("StudentFullName");
                }
            }
        }

        public string StudentFullName
        {
            get { return this.Model.StudentFullName; }
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
                    if (this.birthCountry != null && IsDummy(this.birthCountry.CountryId) == false)
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
                    //if (this.religion == null)
                    //    this.Model.Religion = null;
                    //else
                    //{
                    //    if (IsDummy(this.religion.ReligionId))
                    //        this.Model.Religion = null;
                    //    else
                    //        this.Model.Religion = this.religion;
                    //}
                    if (this.religion != null && IsDummy(this.religion.ReligionId) == false)
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
                        this.Model.Photo = null;
                    else
                    {
                        if (this.Model.Photo == null)
                            this.Model.Photo = new StudentPhoto() { Photo = this.photo };
                        else
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
            get { return IsDummy(this.Model.StudentId); }
        }

        public override bool HasAnyErrors
        {
            get
            {
                if (!this.IsLoaded)
                    return this.HasErrors;

                return this.HasErrors || this.EducationViewModels.HasAnyErrors
                    || this.GuardianViewModels.HasAnyErrors || this.AddressViewModels.HasAnyErrors
                    || this.EmailViewModels.HasAnyErrors || this.PhoneViewModels.HasAnyErrors;
            }
        }

        public string DeleteEducationContent
        {
            get { return this.deleteEducationContent; }
            private set
            {
                if (this.deleteEducationContent != value)
                {
                    this.deleteEducationContent = value;
                    RaisePropertyChanged("DeleteEducationContent");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public StudentViewModel(Student model, IStudentParent parent)
            : base(model, parent)
        {
            this.deleteEducationContent = "_Delete";
            //SetRelationships();
        }

        #endregion

        #region Methods

        protected override EmailViewModel CreateEmailViewModel()
        {
            return new EmailViewModel(new StudentEmail());
        }

        protected override PhoneViewModel CreatePhoneViewModel()
        {
            return new PhoneViewModel(new StudentPhone());
        }

        protected override PersonAddressViewModel CreateAddressViewModel(Address model)
        {
            return new PersonAddressViewModel(new StudentAddress { Address = model });
        }

        protected override PersonGuardianViewModel CreateGuardianViewModel(Guardian model)
        {
            return new PersonGuardianViewModel(new StudentGuardian() { Guardian = model }, this.Parent);
        }

        protected override void InitializeCommands()
        {
            if (this.IsLoaded)
                return;

            base.InitializeCommands();
            this.AddEducationCommand = new RelayCommand<object>(o => OnAddEducation(o));
            this.EditEducationCommand = new RelayCommand<object>(o => OnEditEducation(o), o => this.EducationViewModels.CanEdit);
            this.DeleteEducationCommand = new RelayCommand(o => OnDeleteEducation(), o => this.EducationViewModels.CanDelete);
        }

        private void InitializeEducations()
        {
            if (this.EducationViewModels == null)
            {
                this.EducationViewModels = new EntityViewModelCollection<StudentEducationViewModel, StudentSection>();
                this.EducationViewModels.SelectionChanged += (s, e) =>
                {
                    this.DeleteEducationContent = this.EducationViewModels.ToDelete ? "Un_delete" : "_Delete";
                };
            }
            else
                this.EducationViewModels.Clear();
        }

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.EducationViewModels.AcceptAllChanges();
            this.GuardianViewModels.AcceptAllChanges();
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

            this.GuardianViewModels.Refresh(removeAdded);
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
            this.EducationViewModels.Refresh(removeAdded);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.StudentEmails, removeAdded);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.StudentPhones, removeAdded);
            this.EmailViewModels.Refresh(removeAdded);
            this.PhoneViewModels.Refresh(removeAdded);
            //SetRelationships();
            //InitializeRelationships();
            SetRelationshipsInternal();
            this.IsDetectedChanges = false;
            base.BaseRefresh();

            bool flag = state == ObjectState.Modified && (this.EducationViewModels.HasAnyAdded() ||
                this.GuardianViewModels.HasAnyAdded() || this.AddressViewModels.HasAnyAdded() ||
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
                this.Parent.UoW.RemoveRange(this.Model.StudentEmails);
                this.Parent.UoW.RemoveRange(this.Model.StudentPhones);
                this.Parent.UoW.RemoveRange(this.Model.StudentAddresses);
                this.Parent.UoW.RemoveRange(this.Model.StudentGuardians);
                this.Parent.UoW.RemoveRange(this.Model.StudentSections);
                this.Parent.UoW.Remove(this.Model.Photo);
                this.Parent.UoW.Remove(this.Model);
            }
            else if (this.OverallState == ObjectState.Added || this.OverallState == ObjectState.Modified)
            {
                this.EmailViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as StudentEmail));
                this.EmailViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.StudentEmails.AddIfNotExist(vm.Model as StudentEmail));

                this.PhoneViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as StudentPhone));
                this.PhoneViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.StudentPhones.AddIfNotExist(vm.Model as StudentPhone));

                this.AddressViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as StudentAddress));
                this.AddressViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.StudentAddresses.AddIfNotExist(vm.Model as StudentAddress));

                this.GuardianViewModels.ForEach(vm => vm.GuardianViewModel.DetectChanges());
                this.GuardianViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model as StudentGuardian));
                this.GuardianViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.StudentGuardians.AddIfNotExist(vm.Model as StudentGuardian));

                this.EducationViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model));
                this.EducationViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.StudentSections.AddIfNotExist(vm.Model));

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
            if (string.IsNullOrEmpty(this.StudentName))
                ValidateProperty(this.StudentName, "StudentName");
        }

        public void LoadData()
        {
            if (this.IsLoaded)
                return;

            App.CursorWait();
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //Console.WriteLine("Student View Model - Load Data : Start");
            SetCreatedBy();
            SetModifiedBy();
            this.Countries = this.Parent.UoW.Countries.WithDummy();
            this.Religions = this.Parent.UoW.Religions.WithDummy();
            //SetRelationships();
            //InitializeRelationships();
            SetRelationshipsInternal();

            InitializeEducations();
            this.EducationViewModels.AddRange(this.Model.StudentSections
                .OrderByDescending(m => m.CreatedDate)
                .To(m =>
                {
                    var vm = new StudentEducationViewModel(m);
                    vm.DetectCanDelete = () =>
                    {
                        return this.Parent.UoW.FirstAsNoTracking<StudentSection>
                            (model => model.StudentSectionId == vm.Model.StudentSectionId).CanDelete;
                    };

                    return vm;
                }), false);
            this.EducationViewModels.SelectedIndex = 0;

            InitializeGuardians();
            this.GuardianViewModels.AddRange(this.Model.StudentGuardians
                .To(m => new PersonGuardianViewModel(m, this.Parent)), false);
            this.GuardianViewModels.SelectedIndex = 0;

            InitializeAddresses();
            this.AddressViewModels.AddRange(this.Model.StudentAddresses
                .To(m => new PersonAddressViewModel(m)), false);
            this.AddressViewModels.SelectedIndex = 0;

            InitializeEmails();
            this.EmailViewModels.AddRange(this.Model.StudentEmails
                .To(m => new EmailViewModel(m)), false);
            this.EmailViewModels.SelectedIndex = 0;

            InitializePhones();
            this.PhoneViewModels.AddRange(this.Model.StudentPhones
                .To(m => new PhoneViewModel(m)), false);
            this.PhoneViewModels.SelectedIndex = 0;

            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };

            InitializeCommands();
            this.IsLoaded = true;
            //sw.Stop();
            //Console.WriteLine("Student View Model - Load Data : End : {0}", sw.Elapsed);
            App.CursorDefault();
        }

        private void SetRelationshipsInternal()
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

        public void Add(StudentEducationViewModel vm)
        {
            if (this.EducationViewModels.Any(m => vm.Section.ClassId == m.Section.ClassId))
                throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Class, "Class"));
            if (!ApplicationManager.f)
            {
                if (vm.AcademicYear.Active)
                {
                    if (this.StudentParent.TotalStudents < ApplicationManager.g)
                    {
                        ++this.StudentParent.TotalStudents;
                        Debug.WriteLine(this.StudentParent.TotalStudents);
                    }
                    else
                        ApplicationManager.at();
                }
            }
            
            this.EducationViewModels.Insert(0, vm, true);
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        private bool CanAddMore()
        {
            return ApplicationManager.f ||
                this.StudentParent.TotalStudents < ApplicationManager.g;
        }

        private void OnAddEducation(object o)
        {
            try
            {
                if (!CanAddMore())
                {
                    string msg = ApplicationManager.ag();

                    DialogManager.WriteToLog(msg, null);
                    DialogManager.ShowWarning("Add Education", msg, ApplicationManager.ah("students"));

                    return;
                }

                var esvm = this.StudentParent.EducationSelector;

                if (NavigationManager.ShowSearchSectionInfo(esvm, o).DialogResult == true &&
                    esvm.Collection.SelectedIndex != -1)
                {
                    var value = esvm.Collection.SelectedItem;
                    var section = this.StudentParent.UoW.FirstOrDefault<Section>(m => m.SectionId == value.Section.SectionId);

                    if (section != null)
                    {
                        Add(new StudentEducationViewModel(new StudentSection
                        {
                            Student = this.Model,
                            Section = section,
                            RegisteredDate = this.StudentParent.EducationSelector.RegisteredDate,
                            Active = true
                        }));
                    }
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Education", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Education", ex);
            }
        }

        public bool Delete(StudentEducationViewModel vm)
        {
            bool flag = this.EducationViewModels.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                RaiseRelatedDataChanged(null);
            }

            return flag;
        }

        private void OnDeleteEducation()
        {
            try
            {
                Delete(this.EducationViewModels.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Education", ex);
            }
        }

        public void Edit(StudentEducationViewModel vm)
        {
            vm.CanSave = false;
            RaiseRelatedDataChanged(null);
        }

        private void OnEditEducation(object o)
        {
            try
            {
                bool flag = false;
                var vm = this.EducationViewModels.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowStudentEducation(vm, o).DialogResult == true)
                {
                    Edit(vm);
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "education"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.EducationViewModels.Remove(vm);
                            else
                            {
                                OnEditEducation(o);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "education"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEditEducation(o);
                                return;
                            }
                        }
                        else if (vm.CanSave)
                            flag = true;
                    }
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshEducationBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Education", ex);
            }
        }

        private void OnRefreshEducationBinding()
        {
            if (this.RefreshEducationBindingCommand != null)
            {
                int index = this.EducationViewModels.SelectedIndex;

                this.RefreshEducationBindingCommand.Execute(null);
                this.EducationViewModels.SelectedIndex = index;
            }
        }

        private void CallPropertiesChanged(StudentEducationViewModel vm)
        {
            if (this.EducationPropertiesChanged == null)
                OnRefreshEducationBinding();
            else
                this.EducationPropertiesChanged(vm);
        }

        private void Setup(StudentEducationViewModel vm)
        {
            if (vm == null || vm.IsLoaded)
                return;

            vm.ViewName = "Edit Student's Education";
            vm.LoadData(this.Parent);
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
        //            this.Model.Photo = new StudentPhoto() { Photo = this.photo };
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

        #endregion
    }
}
