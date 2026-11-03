using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class MainViewModel : ListViewModelDictionary<Guid, StudentViewModel, Student>,
        IAddressParent, IGuardianParent, IStudentParent
    {
        # region Properties

        public AddViewModel AddStudentViewModel { get; private set; }
        public AddViewModel AddGuardianViewModel { get; private set; }
        public AddViewModel AddAddressViewModel { get; private set; }
        public SearchStudentViewModel StudentSelector { get; private set; }
        public SearchSectionInfoViewModel EducationSelector { get; private set; }
        public SearchGuardianViewModel GuardianSelector { get; private set; }
        public SearchAddressViewModel AddressSelector { get; private set; }
        public EntityCollection<School> Schools { get; private set; }
        public EntityCollection<AcademicYear> AcademicYears { get; private set; }
        public EntityCollection<Course> Courses { get; private set; }
        public EntityCollection<Class> Classes { get; private set; }
        public EntityCollection<Section> Sections { get; private set; }
        public int TotalStudents { get; set; }

        # endregion
        
        # region Ctor, Dtor

        public MainViewModel()
        {
            Initialize();
            LoadData();
            InitializeCommands();
        }

        # endregion

        # region Methods

        protected override void OnDispose()
        {
            base.OnDispose();
            DisposeUoW();
        }

        private void DisposeUoW()
        {
            if (this.UoW != null)
                this.UoW.Dispose();
        }

        internal void ClearAllCollections()
        {
            this.StudentSelector.Clear();
            this.GuardianSelector.Clear();
            this.AddressSelector.Clear();
            this.Schools.Clear();
            this.AcademicYears.Clear();
            this.Courses.Clear();
            this.Classes.Clear();
            Clear();
            this.UoW.Clear();
            this.TotalStudents = 0;
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.AddStudentViewModel = AddViewModel.CreateAddStudent();
                this.AddStudentViewModel.IsExisting = true;
                this.AddGuardianViewModel = AddViewModel.CreateAddGuardian();
                this.AddGuardianViewModel.IsExisting = true;
                this.AddAddressViewModel = AddViewModel.CreateAddAddress();
                this.AddAddressViewModel.IsExisting = true;

                this.StudentSelector = new SearchStudentViewModel();
                this.EducationSelector = new SearchSectionInfoViewModel();
                this.GuardianSelector = new SearchGuardianViewModel();
                this.AddressSelector = new SearchAddressViewModel();

                this.Schools = new EntityCollection<School>();
                this.AcademicYears = new EntityCollection<AcademicYear>();
                this.Courses = new EntityCollection<Course>();
                this.Classes = new EntityCollection<Class>();
                this.Sections = new EntityCollection<Section>();

                this.ConvertTo = (m) =>
                {
                    var vm = new StudentViewModel(m, this);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Student>
                            (model => model.StudentId == vm.Model.StudentId).CanDelete;
                    };

                    return vm;
                };
                this.GetData = () =>
                {
                    return this.UoW.Where<StudentSection>(m => m.SectionId == this.Key)
                        .Select(m => m.Student).Ordered().To(m => this.ConvertTo(m));
                };

                this.Schools.SelectionChanged += (s, e) =>
                {
                    LoadAcademicYears();
                };
                this.AcademicYears.SelectionChanged += (s, e) =>
                {
                    LoadCourses();
                };
                this.Courses.SelectionChanged += (s, e) =>
                {
                    LoadClasses();
                };
                this.Classes.SelectionChanged += (s, e) =>
                {
                    LoadSections();
                };
                this.Sections.SelectionChanged += (s, e) =>
                {
                    LoadStudents();
                };
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void UpdateTotalStudents()
        {
            if (!ApplicationManager.f)
            {
                var q = from y in this.UoW.Context.AcademicYears.Where(m => m.Active)
                        from r in y.Courses
                        from c in r.Classes
                        from s in c.Sections
                        from x in s.StudentSections.Where(m => m.Active)
                        select x.StudentId;
                this.TotalStudents = q.Distinct().Count();
            }
            else
                this.TotalStudents = 0;
        }

        private bool CanAddMore()
        {
            return ApplicationManager.f || this.TotalStudents < ApplicationManager.g;
        }

        private void LoadData()
        {
            try
            {
                this.Schools.AddRange(this.UoW.GetSchools().Ordered(), false);
                if (!this.Refreshing)
                {
                    //if (this.Schools.Any(m => m.Active))
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault(m => m.Active);
                    //else
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault();
                    this.Schools.IIfSetSelectedItem(m => m.Active, () => this.Schools.FirstOrDefault());
                }
                UpdateTotalStudents();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        private void LoadAcademicYears()
        {
            try
            {
                this.AcademicYears.Clear();
                if (this.Schools.SelectedIndex != -1)
                {
                    this.AcademicYears.AddRange(this.Schools.SelectedItem.AcademicYears
                        .OrderBy(m => m.AcademicYearName), false);
                    if (!this.Refreshing)
                    {
                        //if (this.AcademicYears.Any(m => m.Active))
                        //    this.AcademicYears.SelectedItem = this.AcademicYears.FirstOrDefault(m => m.Active);
                        //else
                        //    this.AcademicYears.SelectedItem = this.AcademicYears.FirstOrDefault();
                        this.AcademicYears.IIfSetSelectedItem(m => m.Active, () => this.AcademicYears.FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Academic Years", ex);
            }
        }

        private void LoadCourses()
        {
            try
            {
                this.Courses.Clear();
                if (this.AcademicYears.SelectedIndex != -1)
                {
                    this.Courses.AddRange(this.AcademicYears.SelectedItem.Courses
                        .OrderBy(m => m.CourseName), false);
                    if (!this.Refreshing)
                    {
                        this.Courses.SelectedItem = this.Courses.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Courses", ex);
            }
        }

        private void LoadClasses()
        {
            try
            {
                this.Classes.Clear();
                if (this.Courses.SelectedIndex != -1)
                {
                    this.Classes.AddRange(this.Courses.SelectedItem.Classes
                        .OrderBy(m => m.ClassName), false);
                    if (!this.Refreshing)
                    {
                        this.Classes.SelectedItem = this.Classes.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Classes", ex);
            }
        }
                
        private void LoadSections()
        {
            try
            {
                this.Sections.Clear();
                if (this.Classes.SelectedIndex != -1)
                {
                    this.Sections.AddRange(this.Classes.SelectedItem.Sections
                        .OrderBy(m => m.SectionName), false);
                    if (!this.Refreshing)
                    {
                        this.Sections.SelectedItem = this.Sections.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Sections", ex);
            }
        }

        private void LoadStudents()
        {
            try
            {
                if (this.CanAdd = this.CanRefresh = this.Sections.SelectedIndex != -1)
                {
                    if (!this.Refreshing)
                    {
                        this.Key = this.Sections.SelectedItem.SectionId;
                        if (this.Collection.SelectedIndex == -1)
                            this.Collection.SelectedIndex = 0;
                    }
                }
                else
                    LoadDummyData();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Students", ex);
            }
        }

        private void AddEducation(StudentViewModel vm)
        {
            Guid id = this.Sections.SelectedItem.SectionId;
            if (!vm.EducationViewModels.Any(m => m.Section.SectionId == id))
            {
                vm.Add(new StudentEducationViewModel(new StudentSection
                {
                    Student = vm.Model,
                    Section = this.Sections.SelectedItem,
                    RegisteredDate = DateTime.Today,
                    Active = true
                }));
            }
        }

        private void FixDictionary(StudentViewModel vm)
        {
            Guid id = vm.Model.StudentId;
            foreach (var list in this.Dictionary)
            {
                if (list.Value.Any(m => m.Model.StudentId == id))
                {
                    if (!vm.EducationViewModels.Any(m => m.Section.SectionId == list.Key))
                        list.Value.Remove(vm);
                }
            }
            foreach (var evm in vm.EducationViewModels)
            {
                Guid sid = evm.Section.SectionId;
                if (this.Dictionary.ContainsKey(sid))
                {
                    var list = this.Dictionary[sid];
                    if (!list.Any(m => m.Model.StudentId == id))
                        list.Add(vm, true);
                }
            }
        }

        public override void Add(StudentViewModel vm)
        {
            if (!vm.IsNew)
            {
                Guid id = vm.Model.StudentId;
                if (this.Collection.Any(m => id == m.Model.StudentId))
                    throw new AlreadyExistsException(SR.GetString("AlreadyExists2", vm.Model, "Student"));
            }

            AddEducation(vm);
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            FixDictionary(vm);
            UpdateTotalStudents();
        }

        public override void Edit(StudentViewModel vm)
        {
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            FixDictionary(vm);
            UpdateTotalStudents();
        }

        protected override void OnAdd()
        {
            try
            {
                if (!CanAddMore())
                {
                    string msg = ApplicationManager.ag();

                    DialogManager.WriteToLog(msg, null);
                    DialogManager.ShowWarning("Add Student", msg, ApplicationManager.ah("students"));

                    return;
                }

                //this.AddStudentViewModel.EnableNew = CanAddStudent();
                if (NavigationManager.ShowAdd(this.AddStudentViewModel, null).DialogResult == false)
                    return;

                if (this.AddStudentViewModel.IsNew)
                {
                    var vm = new StudentViewModel(new Student { Gender = true }, this);

                    Setup(vm);
                    AddEducation(vm);
                    if (NavigationManager.ShowStudent(vm).DialogResult == true)
                    {
                        Add(vm);
                    }
                }
                else
                {
                    if (NavigationManager.ShowSearchStudent(this.StudentSelector, null).DialogResult == true &&
                        this.StudentSelector.Collection.SelectedIndex != -1)
                    {
                        var value = this.StudentSelector.Collection.SelectedItem;
                        var model = this.UoW.FirstOrDefault<Student>(m => m.StudentId == value.StudentId);

                        if (model != null)
                        {
                            var vm = new StudentViewModel(model, this);

                            Setup(vm);
                            AddEducation(vm);
                            if (NavigationManager.ShowStudent(vm).DialogResult == true)
                            {
                                Add(vm);
                            }
                        }
                    }
                }
            }
            catch (AlreadyExistsException ex)
            {
                DialogManager.ShowError("Add Student", ex);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Student", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowStudent(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "student"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.Collection.Remove(vm);
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk3", "student"),
                                SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                flag = true;
                            else
                            {
                                OnEdit();
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
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Student", ex);
            }
        }

        protected override void OnRefresh()
        {
            try
            {
                if (this.CanSave)
                {
                    DialogResult result = DialogManager.AskConfirmation("Confirmation",
                        SR.GetString("RefreshDataAsk1"), SR.GetString("RefreshDataAsk2"), DialogButton.OKCancel);
                    if (result == DialogResult.Cancel)
                        return;
                }

                Guid? oid = null, yid = null, rid = null, cid = null, sid = null;
                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                if (this.Schools.SelectedIndex != -1)
                    oid = this.Schools.SelectedItem.SchoolId;
                if (this.AcademicYears.SelectedIndex != -1)
                    yid = this.AcademicYears.SelectedItem.AcademicYearId;
                if (this.Courses.SelectedIndex != -1)
                    rid = this.Courses.SelectedItem.CourseId;
                if (this.Classes.SelectedIndex != -1)
                    cid = this.Classes.SelectedItem.ClassId;
                if (this.Sections.SelectedIndex != -1)
                    sid = this.Sections.SelectedItem.SectionId;

                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                if (oid.HasValue)
                {
                    var selectedSchool = this.Schools.FirstOrDefault(m => m.SchoolId == oid.Value);
                    if (selectedSchool != null)
                        this.Schools.SelectedItem = selectedSchool;
                }
                if (yid.HasValue)
                {
                    var selectedAcademicYear = this.AcademicYears.FirstOrDefault(m => m.AcademicYearId == yid.Value);
                    if (selectedAcademicYear != null)
                        this.AcademicYears.SelectedItem = selectedAcademicYear;
                }
                if (rid.HasValue)
                {
                    var selectedCourse = this.Courses.FirstOrDefault(m => m.CourseId == rid.Value);
                    if (selectedCourse != null)
                        this.Courses.SelectedItem = selectedCourse;
                }
                if (cid.HasValue)
                {
                    var selectedClass = this.Classes.FirstOrDefault(m => m.ClassId == cid.Value);
                    if (selectedClass != null)
                        this.Classes.SelectedItem = selectedClass;
                }
                if (sid.HasValue)
                {
                    var selectedSection = this.Sections.FirstOrDefault(m => m.SectionId == sid.Value);
                    if (selectedSection != null)
                    {
                        this.Sections.SelectedItem = selectedSection;
                        this.Key = this.Sections.SelectedItem.SectionId;
                        this.Collection.SelectedIndex = index;
                    }
                }
                //OnRefreshBinding();
                this.Refreshing = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Student", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        protected override void DetectChanges()
        {
            this.Dictionary.ForEach(source => source.Value.ForEach(vm => vm.DetectChanges()));
        }

        internal override void OnSave()
        {
            try
            {
                if (this.HasAnyErrors)
                {
                    this.CanSave = false;
                    DialogManager.ShowError("Validate Data", SR.GetString("ValidateData1", "student"));
                }
                if (this.CanSave)
                {
                    SaveChanges();
                    UpdateTotalStudents();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Student", ex);
            }
        }

        //protected override void DetectChanges()
        //{
        //    foreach (var item in this.Collection.Where(vm => vm.OverallState == ObjectState.Added || vm.OverallState == ObjectState.Modified))
        //    {
        //        item.EmailViewModels.Where(vm => vm.ObjectState == ObjectState.Deleted)
        //            .ForEach(vm => item.Model.StudentEmails.Remove(vm.Model as StudentEmail));
        //        item.EmailViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
        //            .ForEach(vm => item.Model.StudentEmails.AddIfNotExist(vm.Model as StudentEmail));

        //        item.PhoneViewModels.Where(vm => vm.ObjectState == ObjectState.Deleted)
        //            .ForEach(vm => item.Model.StudentPhones.Remove(vm.Model as StudentPhone));
        //        item.PhoneViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
        //            .ForEach(vm => item.Model.StudentPhones.AddIfNotExist(vm.Model as StudentPhone));

        //        item.AddressViewModels.Where(vm => vm.ObjectState == ObjectState.Deleted)
        //            .ForEach(vm => item.Model.StudentAddresses.Remove(vm.Model as StudentAddress));
        //        item.AddressViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
        //            .ForEach(vm => item.Model.StudentAddresses.AddIfNotExist(vm.Model as StudentAddress));

        //        //item.GuardianViewModels.Where(vm => vm.ObjectState == ObjectState.Deleted)
        //        //    .ForEach(vm => item.Model.StudentGuardians.Remove(vm.Model as StudentGuardian));
        //        //item.GuardianViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
        //        //    .ForEach(vm => item.Model.StudentGuardians.AddIfNotExist(vm.Model as StudentGuardian));

        //        item.EducationViewModels.Where(vm => vm.ObjectState == ObjectState.Deleted)
        //            .ForEach(vm => item.Model.StudentSections.Remove(vm.Model));
        //        item.EducationViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
        //            .ForEach(vm => item.Model.StudentSections.AddIfNotExist(vm.Model));
        //    }
        //    foreach (var item in this.Collection.Where(vm => vm.ObjectState == ObjectState.Deleted))
        //    {
        //        this.UoW.Context.StudentEmails.RemoveRange(item.Model.StudentEmails);
        //        this.UoW.Context.StudentPhones.RemoveRange(item.Model.StudentPhones);
        //        this.UoW.Context.StudentAddresses.RemoveRange(item.Model.StudentAddresses);
        //        this.UoW.Context.StudentGuardians.RemoveRange(item.Model.StudentGuardians);
        //        if (item.Model.Photo != null)
        //            this.UoW.Remove(item.Model.Photo);
        //    }
        //    base.DetectChanges();
        //}

        private void Setup(StudentViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Student");
            if (vm.IsLoaded)
                return;

            vm.LoadData();
        }

        # endregion
    }
}
