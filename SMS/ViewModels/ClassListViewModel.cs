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
    internal sealed class ClassListViewModel : ListViewModelDictionary<Guid, ClassViewModel, Class>
    {
        #region Properties

        public EntityCollection<School> Schools { get; private set; }
        public EntityCollection<AcademicYear> AcademicYears { get; private set; }
        public EntityCollection<Course> Courses { get; private set; }

        #endregion

        #region Ctor, Dtor

        public ClassListViewModel()
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
                this.Schools = new EntityCollection<School>();
                this.AcademicYears = new EntityCollection<AcademicYear>();
                this.Courses = new EntityCollection<Course>();
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

                this.ConvertTo = (m) =>
                {
                    var vm = new ClassViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Class>
                            (model => model.ClassId == vm.Model.ClassId).CanDelete;
                    };

                    return vm;
                };
                this.GetData = () =>
                {
                    return this.UoW.Where<Class>(m => m.CourseId == this.Key)
                        .Ordered().To(m => this.ConvertTo(m));
                };
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
                this.Schools.AddRange(this.UoW.Schools, false);
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
                        this.Courses.SelectedItem = this.Courses.FirstOrDefault();
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
                DialogManager.ShowErrorAndLog("Load Classes", ex);
            }
        }

        protected override void OnAdd()
        {
            try
            {
                var vm = new ClassViewModel(new Class() { Course = this.Courses.SelectedItem });

                Setup(vm);
                if (NavigationManager.ShowClass(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Class", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowClass(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "class"), 
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
                                SR.GetString("EditDataAsk3", "class"),
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
                DialogManager.ShowErrorAndLog("Edit Class", ex);
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
                this.Refreshing = false;
                if (rid.HasValue)
                {
                    var selectedCourse = this.Courses.FirstOrDefault(m => m.CourseId == rid.Value);
                    if (selectedCourse != null)
                    //{
                        this.Courses.SelectedItem = selectedCourse;
                    //    this.Key = this.Courses.SelectedItem.CourseId;
                    //    this.Collection.SelectedIndex = index;
                    //}
                }
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Class", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        internal override void OnSave()
        {
            try
            {
                App.CursorWait();
                if (this.HasAnyErrors)
                {
                    this.CanSave = false;
                    DialogManager.ShowError("Validate Class", SR.GetString("ValidateData1", "class"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Class", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(ClassViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Class");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "ClassName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        string.Equals(vm.ClassName, m.ClassName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Class Name", vm.ClassName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this);
        }

        #endregion
    }
}
