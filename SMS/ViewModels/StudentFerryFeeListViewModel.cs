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
    internal sealed class StudentFerryFeeListViewModel : ListViewModelDictionary<Guid, StudentFerryFeeViewModel, StudentFerryFee>
    {
        #region Fields

        private SearchStudentSectionInfoViewModel selector;

        #endregion

        #region Properties

        public EntityCollection<School> Schools { get; private set; }
        public EntityCollection<AcademicYear> AcademicYears { get; private set; }
        public EntityCollection<Course> Courses { get; private set; }

        #endregion

        #region Ctor, Dtor

        public StudentFerryFeeListViewModel()
        {
            Initialize();
            LoadData();
            InitializeCommands();
        }

        #endregion

        #region Methods

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
            this.selector.Clear();
            this.Schools.Clear();
            this.AcademicYears.Clear();
            this.Courses.Clear();
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.selector = new SearchStudentSectionInfoViewModel();
                this.Schools = new EntityCollection<School>();
                this.AcademicYears = new EntityCollection<AcademicYear>();
                this.Courses = new EntityCollection<Course>();

                this.ConvertTo = (m) => new StudentFerryFeeViewModel(m);
                this.GetData = () =>
                {
                    return this.UoW.GetStudentFerryFees(this.Key)
                        .Include().Ordered().To(m => this.ConvertTo(m));
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
                    LoadStudentDiscounts();
                };
                this.RemoveAdded = true;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void LoadData()
        {
            try
            {
                this.Schools.AddRange(this.UoW.SchoolsAsNoTracking, false);
                if (!this.Refreshing)
                {
                    //if (this.Schools.Any(m => m.Active))
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault(m => m.Active);
                    //else
                    //    this.Schools.SelectedItem = this.Schools.FirstOrDefault();
                    this.Schools.IIfSetSelectedItem(m => m.Active, () => this.Schools.FirstOrDefault());
                }
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

        private void LoadStudentDiscounts()
        {
            try
            {
                if (this.CanAdd = this.CanRefresh = this.Courses.SelectedIndex != -1)
                {
                    if (!this.Refreshing)
                    {
                        this.Key = this.Courses.SelectedItem.CourseId;
                        if (this.Collection.SelectedIndex == -1)
                            this.Collection.SelectedIndex = 0;
                    }
                }
                else
                    LoadDummyData();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Student Discounts", ex);
            }
        }

        public override void Add(StudentFerryFeeViewModel vm)
        {
            this.Refreshing = true;
            if (this.Schools.SelectedIndex == -1 || this.Schools.SelectedItem.SchoolId != vm.School.SchoolId)
            {
                var school = this.Schools.FirstOrDefault(m => m.SchoolId == vm.School.SchoolId);
                if (school != null)
                    this.Schools.SelectedItem = school;
            }
            if (this.AcademicYears.SelectedIndex == -1 || this.AcademicYears.SelectedItem.AcademicYearId != vm.AcademicYear.AcademicYearId)
            {
                var academicYear = this.AcademicYears.FirstOrDefault(m => m.AcademicYearId == vm.AcademicYear.AcademicYearId);
                if (academicYear != null)
                    this.AcademicYears.SelectedItem = academicYear;
            }
            if (this.Courses.SelectedIndex == -1 || this.Courses.SelectedItem.CourseId != vm.Course.CourseId)
            {
                var course = this.Courses.FirstOrDefault(m => m.CourseId == vm.Course.CourseId);
                if (course != null)
                {
                    this.Courses.SelectedItem = course;
                    this.Key = this.Courses.SelectedItem.CourseId;
                }
            }
            this.Refreshing = false;
            this.UoW.Add(vm.Model);
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            this.Collection.Add(vm, true);
        }

        protected override void OnAdd()
        {
            try
            {
                if (NavigationManager.ShowSearchStudentSectionInfo(this.selector).DialogResult != true ||
                    this.selector.Collection.SelectedIndex == -1)
                    return;

                var value = this.selector.Collection.SelectedItem;

                if (this.UoW.Context.StudentFerryFees.Any(m => m.StudentSectionId == value.StudentSection.StudentSectionId))
                    DialogManager.ShowError("Add Student Ferry Fee", SR.GetString("AlreadyExists3", value.Student, value.Section, "Student ferry fee"));
                else
                {
                    var model = this.UoW.Where<StudentSection>(m => m.StudentSectionId == value.StudentSection.StudentSectionId)
                        .Include(false, false, false).FirstOrDefault();
                    if (model != null)
                    {
                        var vm = new StudentFerryFeeViewModel(new StudentFerryFee { StudentSection = model, Monthly = model.Section.Monthly });

                        Setup(vm);
                        if (NavigationManager.ShowStudentFerryFee(vm).DialogResult == true)
                            Add(vm);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Student Ferry Fee", ex);
            }
            finally
            {
                this.selector.ClearCache();
            }
        }

        public override bool Delete(StudentFerryFeeViewModel vm)
        {
            bool flag = this.Collection.Delete(vm);

            if (flag)
            {
                this.UoW.Remove(vm.Model);
                this.UoW.SaveChanges();
                this.Collection.Remove(vm);
            }

            return flag;
        }

        protected override void OnDelete()
        {
            try
            {
                App.CursorWait();
                Delete(this.Collection.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Student Ferry Fee", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(StudentFerryFeeViewModel vm)
        {
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowStudentFerryFee(vm).DialogResult == true)
                    Edit(vm);
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "student ferry fee"),
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
                                SR.GetString("EditDataAsk3", "student ferry fee"),
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
                DialogManager.ShowErrorAndLog("Edit Student Ferry Fee", ex);
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

                Guid? oid = null, yid = null, rid = null;
                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                if (this.Schools.SelectedIndex != -1)
                    oid = this.Schools.SelectedItem.SchoolId;
                if (this.AcademicYears.SelectedIndex != -1)
                    yid = this.AcademicYears.SelectedItem.AcademicYearId;
                if (this.Courses.SelectedIndex != -1)
                    rid = this.Courses.SelectedItem.CourseId;

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
                    {
                        this.Courses.SelectedItem = selectedCourse;
                        this.Key = this.Courses.SelectedItem.CourseId;
                        this.Collection.SelectedIndex = index;
                    }
                }
                this.Refreshing = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Student Ferry Fee", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        internal override void OnSave()
        { }

        private void Setup(StudentFerryFeeViewModel vm)
        {
            if (vm == null)
                return;

            vm.ViewName = "Edit Student Ferry Fee";
            if (vm.IsLoaded)
                return;

            vm.LoadData(this);
        }

        #endregion
    }
}
