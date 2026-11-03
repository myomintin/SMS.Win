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
    internal sealed class CourseListViewModel : ListViewModelDictionary<Guid, CourseViewModel, Course>
    {
        #region Properties

        public EntityCollection<School> Schools { get; private set; }
        public EntityCollection<AcademicYear> AcademicYears { get; private set; }

        #endregion

        #region Ctor, Dtor

        public CourseListViewModel()
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
                this.Schools.SelectionChanged += (s, e) =>
                {
                    LoadAcademicYears();
                };
                this.AcademicYears.SelectionChanged += (s, e) =>
                {
                    LoadCourses();
                };

                this.ConvertTo = (m) =>
                {
                    var vm = new CourseViewModel(m);

                    vm.DetectCanDelete = () =>
                    {
                        return this.UoW.FirstAsNoTracking<Course>
                            (model => model.CourseId == vm.Model.CourseId).CanDelete;
                    };

                    return vm;
                };
                this.GetData = () =>
                {
                    return this.UoW.Where<Course>(m => m.AcademicYearId == this.Key)
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
                if (this.CanAdd = this.CanRefresh = this.AcademicYears.SelectedIndex != -1)
                {
                    if (!this.Refreshing)
                    {
                        this.Key = this.AcademicYears.SelectedItem.AcademicYearId;
                        if (this.Collection.SelectedIndex == -1)
                            this.Collection.SelectedIndex = 0;
                    }
                }
                else
                    LoadDummyData();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Courses", ex);
            }
        }

        protected override void OnAdd()
        {
            try
            {
                var vm = new CourseViewModel(new Course() { AcademicYear = this.AcademicYears.SelectedItem });

                Setup(vm);
                if (NavigationManager.ShowCourse(vm).DialogResult == true)
                {
                    Add(vm);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add Course", ex);
            }
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowCourse(vm).DialogResult == true)
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
                                SR.GetString("EditDataAsk1", "course"),
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
                                SR.GetString("EditDataAsk3", "course"),
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
                DialogManager.ShowErrorAndLog("Edit Course", ex);
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

                Guid? oid = null, yid = null;
                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                if (this.Schools.SelectedIndex != -1)
                    oid = this.Schools.SelectedItem.SchoolId;
                if (this.AcademicYears.SelectedIndex != -1)
                    yid = this.AcademicYears.SelectedItem.AcademicYearId;

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
                    {
                        this.AcademicYears.SelectedItem = selectedAcademicYear;
                        this.Key = this.AcademicYears.SelectedItem.AcademicYearId;
                        this.Collection.SelectedIndex = index;
                    }
                }
                this.Refreshing = false;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Course", ex);
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
                    DialogManager.ShowError("Validate Course", SR.GetString("ValidateData1", "course"));
                }
                if (this.CanSave)
                    SaveChanges();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Course", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void Setup(CourseViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("Course");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "CourseName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        string.Equals(vm.CourseName, m.CourseName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }

                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "Course Name", vm.CourseName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData(this);
        }

        #endregion
    }
}
