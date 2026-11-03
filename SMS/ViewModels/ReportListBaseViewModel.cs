using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.Reports;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal abstract class ReportListBaseViewModel : BaseViewModel
    {
        #region Fields

        private EntityCollection<KeyValues<AcademicYear, bool?>> academicYearCollection;
        private List<KeyValues<Class, bool?>> classCollection;
        private List<KeyValues<Section, bool?>> sectionCollection;
        private List<KeyValues<StudentSection, bool?>> studentSectionCollection;
        private SMSDataSet ds;
        private Lazy<DueDatePopupViewModel> dueDatePopupViewModel;
        private Lazy<ReceiptPopupViewModel> receiptPopupViewModel;
        private Lazy<ReminderPopupViewModel> reminderPopupViewModel;
        private Lazy<StudentInfoPopupViewModel> studentInfoPopupViewModel;

        #endregion

        #region Properties

        public ISMSUnitOfWork UoW { get; private set; }
        public EntityCollection<ReportInfo> Collection { get; private set; }
        public EntityCollection<KeyValues<School, bool?>> Schools { get; private set; }
        public Dictionary<Guid, EntityCollection<KeyValues<AcademicYear, bool?>>> AcademicYears { get; private set; }
        public Dictionary<Guid, List<KeyValues<Course, bool?>>> Courses { get; private set; }
        public Dictionary<Guid, List<KeyValues<Class, bool?>>> Classes { get; private set; }
        public Dictionary<Guid, List<KeyValues<Section, bool?>>> Sections { get; private set; }
        public Dictionary<Guid, List<KeyValues<StudentSection, bool?>>> StudentSections { get; private set; }
        public EntityCollection<string> ActiveCollection { get; private set; }
        public EntityCollection<string> SectionNameFormatCollection { get; private set; }
        public EntityCollection<string> StudentNameFormatCollection { get; private set; }
        public EntityCollection<string> AddressFormatCollection { get; private set; }
        public ICommand PreviewCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public bool CanRefresh { get; private set; }
        public Action AcademicYearChanged { get; set; }

        public EntityCollection<KeyValues<AcademicYear, bool?>> AcademicYearCollection
        {
            get { return this.academicYearCollection; }
            private set
            {
                if (this.academicYearCollection != value)
                {
                    this.academicYearCollection = value;
                    RaisePropertyChanged("AcademicYearCollection");
                    if (this.AcademicYearChanged != null)
                        this.AcademicYearChanged();
                }
            }
        }

        public ReportInfo SelectedItem
        {
            get { return this.Collection.SelectedItem; }
        }

        private List<KeyValues<Course, bool?>> CourseCollection
        {
            get { return this.Courses[this.academicYearCollection.SelectedItem.Key.AcademicYearId]; }
        }

        private DueDatePopupViewModel DueDatePopup
        {
            get { return this.dueDatePopupViewModel.Value; }
        }

        private ReceiptPopupViewModel ReceiptPopup
        {
            get { return this.receiptPopupViewModel.Value; }
        }

        private ReminderPopupViewModel ReminderPopup
        {
            get { return this.reminderPopupViewModel.Value; }
        }

        public StudentInfoPopupViewModel StudentInfoPopup
        {
            get { return this.studentInfoPopupViewModel.Value; }
        }

        #endregion

        #region Ctor, Dtor

        public ReportListBaseViewModel()
        {
            this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
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
            ClearCollections();
            this.Collection.Clear();
            this.Schools.Clear();
            this.AcademicYears.ForEach(m => m.Value.Clear());
            this.AcademicYears.Clear();
            this.academicYearCollection.Clear();
            this.Courses.ForEach(m => m.Value.Clear());
            this.Courses.Clear();
            this.Classes.ForEach(m => m.Value.Clear());
            this.Classes.Clear();
            this.Sections.ForEach(m => m.Value.Clear());
            this.Sections.Clear();
            this.StudentSections.ForEach(m => m.Value.Clear());
            this.StudentSections.Clear();
            this.ActiveCollection.Clear();
            this.SectionNameFormatCollection.Clear();
            this.StudentNameFormatCollection.Clear();
            this.AddressFormatCollection.Clear();
            if (this.receiptPopupViewModel.IsValueCreated)
                this.ReceiptPopup.Clear();
            if (this.studentInfoPopupViewModel.IsValueCreated)
                this.StudentInfoPopup.Clear();
            this.UoW.Clear();
            this.IsLoaded = false;
        }

        private void ClearCollections()
        {
            this.classCollection.Clear();
            this.sectionCollection.Clear();
            this.studentSectionCollection.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.ds = ReportHelper.CreateDataSet();
                this.Collection = new EntityCollection<ReportInfo>();
                this.Schools = new EntityCollection<KeyValues<School, bool?>>();
                this.AcademicYears = new Dictionary<Guid, EntityCollection<KeyValues<AcademicYear, bool?>>>();
                this.Courses = new Dictionary<Guid, List<KeyValues<Course, bool?>>>();
                this.Classes = new Dictionary<Guid, List<KeyValues<Class, bool?>>>();
                this.Sections = new Dictionary<Guid, List<KeyValues<Section, bool?>>>();
                this.StudentSections = new Dictionary<Guid, List<KeyValues<StudentSection, bool?>>>();

                this.academicYearCollection = new EntityCollection<KeyValues<AcademicYear, bool?>>();
                this.classCollection = new List<KeyValues<Class, bool?>>();
                this.sectionCollection = new List<KeyValues<Section, bool?>>();
                this.studentSectionCollection = new List<KeyValues<StudentSection, bool?>>();
                this.ActiveCollection = new EntityCollection<string>();
                this.SectionNameFormatCollection = new EntityCollection<string>();
                this.StudentNameFormatCollection = new EntityCollection<string>();
                this.AddressFormatCollection = new EntityCollection<string>();
                this.Collection.SelectionChanged += (s, e) =>
                {
                    this.CanSave = this.CanRefresh = this.Collection.SelectedIndex != -1;
                };

                this.dueDatePopupViewModel = new Lazy<DueDatePopupViewModel>(() =>
                    new DueDatePopupViewModel());
                this.receiptPopupViewModel = new Lazy<ReceiptPopupViewModel>(() =>
                {
                    var vm = new ReceiptPopupViewModel();
                    vm.LoadData(this.UoW);

                    return vm;
                });
                this.reminderPopupViewModel = new Lazy<ReminderPopupViewModel>(() =>
                    new ReminderPopupViewModel());
                this.studentInfoPopupViewModel = new Lazy<StudentInfoPopupViewModel>(() =>
                {
                    var vm = new StudentInfoPopupViewModel();
                    vm.LoadData(this.UoW);

                    return vm;
                });
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        private void InitializeCommands()
        {
            try
            {
                this.PreviewCommand = new RelayCommand(o => OnPreview(), o => this.CanSave);
                this.RefreshCommand = new RelayCommand(o => OnRefresh(), o => this.CanRefresh);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize Commands", ex);
            }
        }

        protected virtual void LoadData()
        {
            try
            {
                this.ActiveCollection.AddRange("Active", "Inactive", "All");
                this.ActiveCollection.SelectedIndex = 0;
                this.SectionNameFormatCollection.AddRange(
                    "Section Name", "Section Name [ OD ]", "Section Name [ ST ~ ET ]", "Section Name [ OD, ST ~ ET ]",
                    "Section Name [ Room ]", "Section Name [ OD, Room ]", "Section Name [ ST ~ ET, Room ]",
                    "Section Name [ OD, ST ~ ET, Room ]", "Section Name [ CMD ~ CPD ]",
                    "Section Name [ Room, CMD ~ CPD ]", "Section Name [ OD, ST ~ ET, CMD ~ CPD ]",
                    "Section Name [ OD, ST ~ ET, Room, CMD ~ CPD ]");
                this.SectionNameFormatCollection.SelectedIndex = 0;
                this.StudentNameFormatCollection.AddRange(
                    "Student Name", "Other Name", "Student Name @ Other Name",
                     "Title Student Name", "Title Other Name", "Title Student Name @ Other Name");
                this.StudentNameFormatCollection.SelectedIndex = 2;
                this.AddressFormatCollection.AddRange(
                    "Line1", "Line1, City", "Line1, Township", "Line1, City, Township",
                    "Line1, City, Township, Country Name");
                this.AddressFormatCollection.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        public void Load()
        {
            try
            {
                if (this.IsLoaded || this.SelectedItem.DisplayLevel < ReportLevel.School)
                    return;

                this.Schools.AddRange(this.UoW.SchoolsAsNoTracking
                    .To(m => KeyValues.Create<School, bool?>(m, false)), false);
                //if (this.Schools.Any(m => m.Key.Active))
                //    this.Schools.SelectedItem = this.Schools.FirstOrDefault(m => m.Key.Active);
                //else
                //    this.Schools.SelectedItem = this.Schools.FirstOrDefault();
                this.Schools.IIfSetSelectedItem(m => m.Key.Active, () => this.Schools.FirstOrDefault());
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Schools", ex);
            }
        }

        public void Load(School model)
        {
            try
            {
                if (this.AcademicYears.ContainsKey(model.SchoolId))
                {
                    this.AcademicYearCollection = this.AcademicYears[model.SchoolId];
                    return;
                }

                var list = new EntityCollection<KeyValues<AcademicYear, bool?>>(
                    this.UoW.WhereAsNoTracking<AcademicYear>(m => m.SchoolId == model.SchoolId)
                    .OrderBy(m => m.AcademicYearName)
                    .To(m => KeyValues.Create<AcademicYear, bool?>(m, false)));

                this.AcademicYears.Add(model.SchoolId, list);
                if (list.Any(m => m.Key.Active))
                    list.SelectedItem = list.FirstOrDefault(m => m.Key.Active);
                else
                    list.SelectedItem = list.LastOrDefault();
                this.AcademicYearCollection = list;
                list.SelectionChanged += (s, e) =>
                {
                    if (this.AcademicYearChanged != null)
                        this.AcademicYearChanged();
                };
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Academic Years", ex);
            }
        }

        public void Load(AcademicYear model)
        {
            try
            {
                if (this.Courses.ContainsKey(model.AcademicYearId))
                    return;

                this.Courses.Add(model.AcademicYearId,
                    this.UoW.WhereAsNoTracking<Course>(m => m.AcademicYearId == model.AcademicYearId)
                    .OrderBy(m => m.CourseName)
                    .To(m =>
                    {
                        var entity = KeyValues.Create<Course, bool?>(m, false);
                        entity.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Value1")
                            {
                                //if (entity.Value1 == true)
                                //    Load(entity.Key);
                                SetCheck(entity.Key, entity.Value1);
                            }
                        };
                        return entity;
                    }).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Courses", ex);
            }
        }

        public void Load(Course model)
        {
            try
            {
                if (this.Classes.ContainsKey(model.CourseId))
                    return;

                this.Classes.Add(model.CourseId,
                    this.UoW.WhereAsNoTracking<Class>(m => m.CourseId == model.CourseId)
                    .OrderBy(m => m.ClassName)
                    .To(m =>
                    {
                        var entity = KeyValues.Create<Class, bool?>(m, false);
                        entity.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Value1")
                            {
                                //if (entity.Value1 == true)
                                //    Load(entity.Key);
                                SetCheck(entity.Key, entity.Value1);
                            }
                        };
                        return entity;
                    }).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Classes", ex);
            }
        }

        public void Load(Class model)
        {
            try
            {
                if (this.Sections.ContainsKey(model.ClassId))
                    return;

                this.Sections.Add(model.ClassId,
                    this.UoW.WhereAsNoTracking<Section>(m => m.ClassId == model.ClassId)
                    .OrderBy(m => m.SectionName)
                    .To(m =>
                    {
                        var entity = KeyValues.Create<Section, bool?>(m, false);
                        entity.PropertyChanged += (s, e) =>
                        {
                            if (e.PropertyName == "Value1")
                            {
                                //if (entity.Value1 == true)
                                //    Load(entity.Key);
                                SetCheck(entity.Key, entity.Value1);
                            }
                        };
                        return entity;
                    }).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Sections", ex);
            }
        }

        public void Load(Section model)
        {
            try
            {
                if (this.StudentSections.ContainsKey(model.SectionId))
                    return;

                this.StudentSections.Add(model.SectionId,
                    this.UoW.WhereAsNoTracking<StudentSection>(m => m.SectionId == model.SectionId)
                    .Include(m => m.Student).OrderBy(m => m.Student.StudentName)
                    .To(m => KeyValues.Create<StudentSection, bool?>(m, false)).ToList());
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Students", ex);
            }
        }

        public void SetCheck(School model, bool? flag)
        {
            foreach (var list in this.AcademicYears.Where(m => m.Key == model.SchoolId))
                foreach (var value in list.Value)
                    value.Value1 = flag;
        }

        public void SetCheck(AcademicYear model, bool? flag)
        {
            foreach (var list in this.Courses.Where(m => m.Key == model.AcademicYearId))
                foreach (var value in list.Value)
                    value.Value1 = flag;
        }

        public void SetCheck(Course model, bool? flag)
        {
            foreach (var list in this.Classes.Where(m => m.Key == model.CourseId))
                foreach (var value in list.Value)
                    value.Value1 = flag;
        }

        public void SetCheck(Class model, bool? flag)
        {
            foreach (var list in this.Sections.Where(m => m.Key == model.ClassId))
                foreach (var value in list.Value)
                    value.Value1 = flag;
        }

        public void SetCheck(Section model, bool? flag)
        {
            foreach (var list in this.StudentSections.Where(m => m.Key == model.SectionId))
                foreach (var value in list.Value)
                    value.Value1 = flag;
        }

        private void LoadAndCheckClasses()
        {
            this.CourseCollection.ForEach(m =>
            {
                if (m.Value1 == true)
                {
                    Load(m.Key);
                    SetCheck(m.Key, true);
                }
                if (this.Classes.ContainsKey(m.Key.CourseId))
                    this.classCollection.AddRange(this.Classes[m.Key.CourseId]);
            });
        }

        private void LoadAndCheckSections()
        {
            this.classCollection.ForEach(m =>
            {
                if (m.Value1 == true)
                {
                    Load(m.Key);
                    SetCheck(m.Key, true);
                }
                if (this.Sections.ContainsKey(m.Key.ClassId))
                    this.sectionCollection.AddRange(this.Sections[m.Key.ClassId]);
            });
        }

        private void LoadAndCheckStudentSections()
        {
            this.sectionCollection.ForEach(m =>
            {
                if (m.Value1 == true)
                {
                    Load(m.Key);
                    SetCheck(m.Key, true);
                }
                if (this.StudentSections.ContainsKey(m.Key.SectionId))
                    this.studentSectionCollection.AddRange(this.StudentSections[m.Key.SectionId]);
            });
        }

        private void LoadAndCheckChildren()
        {
            ClearCollections();
            if (this.SelectedItem.DisplayLevel > ReportLevel.Course)
            {
                LoadAndCheckClasses();
                if (this.SelectedItem.DisplayLevel > ReportLevel.Class)
                {
                    LoadAndCheckSections();
                    if (this.SelectedItem.DisplayLevel > ReportLevel.Section)
                        LoadAndCheckStudentSections();
                }
            }
        }

        private bool ComfirmChecked()
        {
            bool flag = false;

            if (this.SelectedItem.DisplayLevel == ReportLevel.None)
                flag = true;
            else if (this.SelectedItem.DisplayLevel == ReportLevel.School)
                flag = this.Schools.Any(m => m.Value1 == true);
            else if (this.SelectedItem.DisplayLevel == ReportLevel.AcademicYear)
                flag = this.academicYearCollection.Any(m => m.Value1 == true);
            else if (this.SelectedItem.DisplayLevel == ReportLevel.Course)
                flag = this.CourseCollection.Any(m => m.Value1 == true);
            else if (this.SelectedItem.DisplayLevel == ReportLevel.Class)
                flag = this.classCollection.Any(m => m.Value1 == true);
            else if (this.SelectedItem.DisplayLevel == ReportLevel.Section)
                flag = this.sectionCollection.Any(m => m.Value1 == true);
            else if (this.SelectedItem.DisplayLevel == ReportLevel.StudentSection)
                flag = this.studentSectionCollection.Any(m => m.Value1 == true);

            return flag;
        }

        private void OnPreview()
        {
            if (this.Collection.SelectedIndex == -1)
                return;

            var type = this.SelectedItem.ReportType;
            if (type == ReportType.StudentListAge)
            {
                if (NavigationManager.ShowDueDatePopup(this.DueDatePopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.StudentListInfo)
            {
                this.StudentInfoPopup.SetShow(true, true, true, true);
                if (NavigationManager.ShowStudentInfoPopup(this.StudentInfoPopup).DialogResult != true)
                    return;
                if (this.StudentInfoPopup.CheckAnyGuardianTypes == false)
                {
                    DialogManager.ShowInfo("View Report", SR.GetString("CheckAnyToViewReport", "guardian type"));
                    return;
                }
            }
            else if (type == ReportType.StudentListAddress || type == ReportType.StudentListAddress2)
            {
                this.StudentInfoPopup.SetShow(true, false, false, false);
                if (NavigationManager.ShowStudentInfoPopup(this.StudentInfoPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.StudentListEmail || type == ReportType.StudentListEmail2)
            {
                this.StudentInfoPopup.SetShow(false, true, false, false);
                if (NavigationManager.ShowStudentInfoPopup(this.StudentInfoPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.StudentListPhone || type == ReportType.StudentListPhone2)
            {
                this.StudentInfoPopup.SetShow(false, false, true, false);
                if (NavigationManager.ShowStudentInfoPopup(this.StudentInfoPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.StudentListGuardian || type == ReportType.StudentListGuardian2)
            {
                this.StudentInfoPopup.SetShow(false, false, false, true);
                if (NavigationManager.ShowStudentInfoPopup(this.StudentInfoPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.Reminder || type == ReportType.Reminder2)
            {
                if (NavigationManager.ShowReminderPopup(this.ReminderPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.DailyIncome || type == ReportType.DailyIncomeDetails ||
                type == ReportType.DailyIncomeDetails2)
            {
                this.ReceiptPopup.Setup(0);
                if (NavigationManager.ShowReceiptPopup(this.ReceiptPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.MonthlyIncome)
            {
                this.ReceiptPopup.Setup(1);
                if (NavigationManager.ShowReceiptPopup(this.ReceiptPopup).DialogResult != true)
                    return;
            }
            else if (type == ReportType.YearlyIncome)
            {
                this.ReceiptPopup.Setup(2);
                if (NavigationManager.ShowReceiptPopup(this.ReceiptPopup).DialogResult != true)
                    return;
            }

            var win = NavigationManager.ShowBusyPopup();
            try
            {
                App.CursorWait();
                LoadAndCheckChildren();
                if (!ComfirmChecked())
                {
                    DialogManager.ShowInfo("View Report", SR.GetString("CheckAnyToViewReport",
                        ReportHelper.ToReportLevelString(this.SelectedItem.DisplayLevel)));
                    return;
                }

                var report = GetReport();
                NavigationManager.ShowReportViewer(report);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("View Report", ex);
            }
            finally
            {
                win.DoClose();
                App.CursorDefault();
            }
        }

        private void OnRefresh()
        {
            try
            {
                int index = this.Collection.SelectedIndex;

                App.CursorWait();
                ClearAllCollections();
                //this.Collection.RaiseSelectionChanged();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                LoadData();
                if (this.receiptPopupViewModel.IsValueCreated)
                    this.ReceiptPopup.LoadData(this.UoW);
                if (this.studentInfoPopupViewModel.IsValueCreated)
                    this.StudentInfoPopup.LoadData(this.UoW);
                this.Collection.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private object GetReport()
        {
            var type = this.SelectedItem.ReportType;

            if (type == ReportType.StudentList)
            {
                LoadAndCheckStudentSections();

                var rpt = new CRStudentList();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentList(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List ( All )");

                return rpt;
            }
            else if (type == ReportType.StudentListDetails)
            {
                LoadAndCheckStudentSections();

                var rpt = new CRStudentListDetails();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListDetails(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List Details ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List Details ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List Details ( All )");

                return rpt;
            }
            else if (type == ReportType.StudentListAge)
            {
                LoadAndCheckStudentSections();

                var rpt = new CRStudentListAge();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListAge(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex,
                    this.DueDatePopup.DueDate);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Age ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Age ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Age ( All )");
                rpt.SetParameterValue("DueDate", this.DueDatePopup.DueDate.ToDateString());

                return rpt;
            }
            else if (type == ReportType.StudentListInfo)
            {
                var rpt = new CRStudentListInfo();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListInfo(checkedStudentSections, this.StudentInfoPopup.AllAddresses,
                    this.StudentInfoPopup.AllEmails, this.StudentInfoPopup.AllPhones,
                    this.StudentInfoPopup.GuardianTypeCollection.Select(m => m.Key.GuardianTypeId).ToList(),
                    this.StudentNameFormatCollection.SelectedIndex, this.AddressFormatCollection.SelectedIndex, 1);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Info ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Info ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Info ( All )");

                return rpt;
            }
            else if (type == ReportType.StudentListInRoom)
            {
                LoadAndCheckStudentSections();

                var rpt = new CRStudentListInRoom();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true && m.Key.Active);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));

                this.ds.AddStudentList(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.StudentListAddress || type == ReportType.StudentListAddress2)
            {
                LoadAndCheckStudentSections();

                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (type == ReportType.StudentListAddress)
                    rpt = new CRStudentList2Sheet();
                else
                    rpt = new CRStudentList2SheetLandscape();
                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListAddress(checkedStudentSections, this.StudentInfoPopup.AllAddresses,
                    this.StudentNameFormatCollection.SelectedIndex, this.AddressFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Address ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Address ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Address ( All )");
                rpt.SetParameterValue("ValueTitle", "Addresses");

                return rpt;
            }
            else if (type == ReportType.StudentListEmail || type == ReportType.StudentListEmail2)
            {
                LoadAndCheckStudentSections();

                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (type == ReportType.StudentListEmail)
                    rpt = new CRStudentList2Sheet();
                else
                    rpt = new CRStudentList2SheetLandscape();
                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListEmail(checkedStudentSections, this.StudentInfoPopup.AllEmails,
                    this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Email ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Email ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Email ( All )");
                rpt.SetParameterValue("ValueTitle", "Emails");

                return rpt;
            }
            else if (type == ReportType.StudentListPhone || type == ReportType.StudentListPhone2)
            {
                LoadAndCheckStudentSections();

                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (type == ReportType.StudentListPhone)
                    rpt = new CRStudentList2Sheet();
                else
                    rpt = new CRStudentList2SheetLandscape();
                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListPhone(checkedStudentSections, this.StudentInfoPopup.AllPhones,
                    this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Phone ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Phone ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Phone ( All )");
                rpt.SetParameterValue("ValueTitle", "Phones");

                return rpt;
            }
            else if (type == ReportType.StudentListGuardian || type == ReportType.StudentListGuardian2)
            {
                LoadAndCheckStudentSections();

                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (type == ReportType.StudentListGuardian)
                    rpt = new CRStudentList2Sheet();
                else
                    rpt = new CRStudentList2SheetLandscape();
                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddStudentListGuardian(checkedStudentSections,
                    this.StudentInfoPopup.GuardianTypeCollection.Select(m => m.Key.GuardianTypeId).ToList(),
                    this.StudentNameFormatCollection.SelectedIndex, 1);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                if (this.ActiveCollection.SelectedIndex == 0)
                    rpt.SetParameterValue("HeaderText", "Student List - Guardian ( Active )");
                else if (this.ActiveCollection.SelectedIndex == 1)
                    rpt.SetParameterValue("HeaderText", "Student List - Guardian ( Inactive )");
                else
                    rpt.SetParameterValue("HeaderText", "Student List - Guardian ( All )");
                rpt.SetParameterValue("ValueTitle", "Guardians");

                return rpt;
            }
            else if (type == ReportType.StudentCard)
            {
                var rpt = new CRStudentCard();
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));

                this.ds.AddStudentCard(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("SchoolName", this.Schools.SelectedItem.Key.SchoolName);
                rpt.SetParameterValue("SchoolAddress", ModelExtension.GetAddress(this.AddressFormatCollection.SelectedIndex)
                    .Invoke(this.Schools.SelectedItem.Key.Address));

                return rpt;
            }
            else if (type == ReportType.StudentDiscount)
            {
                var rpt = new CRStudentDiscount();
                var list = new List<Guid>();

                this.ds.StudentDiscount.Clear();
                foreach (var value in this.CourseCollection.Where(m => m.Value1 == true))
                {
                    var query = this.UoW.GetStudentDiscounts(value.Key.CourseId)
                                .Include().Ordered().AsNoTracking();

                    this.ds.AddStudentDiscount(query, value.Key.CourseId, this.StudentNameFormatCollection.SelectedIndex,
                        this.SectionNameFormatCollection.SelectedIndex, false);
                    list.Add(value.Key.CourseId);
                }
                this.ds.Add(GetCourses(list));
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.StudentFerryFee)
            {
                var rpt = new CRStudentFerryFee();
                var list = new List<Guid>();

                this.ds.StudentFerryFee.Clear();
                foreach (var value in this.CourseCollection.Where(m => m.Value1 == true))
                {
                    var query = this.UoW.GetStudentFerryFees(value.Key.CourseId)
                                .Include().Ordered().AsNoTracking();

                    this.ds.AddStudentFerryFee(query, value.Key.CourseId, this.StudentNameFormatCollection.SelectedIndex,
                        this.SectionNameFormatCollection.SelectedIndex, false);
                    list.Add(value.Key.CourseId);
                }
                this.ds.Add(GetCourses(list));
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Reminder || type == ReportType.Reminder2)
            {
                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

                if (type == ReportType.Reminder)
                    rpt = new CRReminder();
                else
                    rpt = new CRReminderColumn();
                if (this.ActiveCollection.SelectedIndex == 0)
                    filter = filter.Where(m => m.Key.Active);
                else if (this.ActiveCollection.SelectedIndex == 1)
                    filter = filter.Where(m => m.Key.Active == false);

                var checkedStudentSections = filter.Select(m => m.Key);
                var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));

                this.ds.AddReminder(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
                this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
                rpt.SetParameterValue("SchoolName", this.Schools.SelectedItem.Key.SchoolName);
                rpt.SetParameterValue("SchoolMotto", this.Schools.SelectedItem.Key.Motto);
                rpt.SetParameterValue("SchoolAddress", ModelExtension.GetAddress(this.AddressFormatCollection.SelectedIndex)
                    .Invoke(this.Schools.SelectedItem.Key.Address));
                rpt.SetParameterValue("SchoolPhones", this.Schools.SelectedItem.Key.SchoolPhones.ToPhoneString());
                rpt.SetParameterValue("DueDate", this.ReminderPopup.DueDate.ToDateString());
                rpt.SetParameterValue("PaymentMonth", this.ReminderPopup.PaymentMonth.ToString("MMMM yyyy"));

                return rpt;
            }
            //else if (type == ReportType.Reminder2)
            //{
            //    var rpt = new CRReminderColumn();
            //    var filter = this.studentSectionCollection.Where(m => m.Value1 == true);

            //    if (this.ActiveCollection.SelectedIndex == 0)
            //        filter = filter.Where(m => m.Key.Active);
            //    else if (this.ActiveCollection.SelectedIndex == 1)
            //        filter = filter.Where(m => m.Key.Active == false);

            //    var checkedStudentSections = filter.Select(m => m.Key);
            //    var checkedSections = GetSections(checkedStudentSections.Select(m => m.SectionId));

            //    this.ds.AddReminder(checkedStudentSections, this.StudentNameFormatCollection.SelectedIndex);
            //    this.ds.Add(checkedSections, this.SectionNameFormatCollection.SelectedIndex);

            //    rpt.SetDataSource(this.ds);
            //    rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
            //    rpt.SetParameterValue("SchoolName", this.Schools.SelectedItem.Key.SchoolName);
            //    rpt.SetParameterValue("SchoolMotto", this.Schools.SelectedItem.Key.Motto);
            //    rpt.SetParameterValue("SchoolAddress", ModelExtension.GetAddress(this.AddressFormatCollection.SelectedIndex)
            //        .Invoke(this.Schools.SelectedItem.Key.Address));
            //    rpt.SetParameterValue("SchoolPhones", this.Schools.SelectedItem.Key.SchoolPhones.ToPhoneString());
            //    rpt.SetParameterValue("DueDate", this.ReminderPopup.DueDate.ToDateString());
            //    rpt.SetParameterValue("PaymentMonth", this.ReminderPopup.PaymentMonth.ToString("MMMM yyyy"));

            //    return rpt;
            //}
            else if (type == ReportType.DailyIncome)
            {
                var rpt = new CRDailyIncome();
                var date = this.ReceiptPopup.ReceiptDate;
                var query = this.UoW.Where<Receipt>(m => m.ReceiptDate == date);

                if (this.ReceiptPopup.Currencies.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Currencies.SelectedItem.CurrencyId;
                    query = query.Where(m => m.CurrencyId == id);
                }
                if (this.ReceiptPopup.Schools.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Schools.SelectedItem.SchoolId;
                    query = query.Where(m => m.SchoolId == id);
                }

                var list = query.Ordered().AsNoTracking().ToList();

                this.ds.AddDailyIncome(list);
                this.ds.Add(this.UoW.GetCurrenciesAsNoTracking(list.Select(m => m.CurrencyId)));
                this.ds.Add(this.UoW.GetSchoolsAsNoTracking(list.Select(m => m.SchoolId)));

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
                rpt.SetParameterValue("Date", string.Format("Receipt Date : {0}",
                    this.ReceiptPopup.ReceiptDate.ToDateString()));

                return rpt;
            }
            else if (type == ReportType.DailyIncomeDetails || type == ReportType.DailyIncomeDetails2)
            {
                CrystalDecisions.CrystalReports.Engine.ReportClass rpt;
                var date = this.ReceiptPopup.ReceiptDate;
                var query = this.UoW.Where<Receipt>(m => m.ReceiptDate == date);

                if (type == ReportType.DailyIncomeDetails)
                    rpt = new CRDailyIncomeDetails();
                else
                    rpt = new CRDailyIncomeDetails2();
                if (this.ReceiptPopup.Currencies.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Currencies.SelectedItem.CurrencyId;
                    query = query.Where(m => m.CurrencyId == id);
                }
                if (this.ReceiptPopup.Schools.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Schools.SelectedItem.SchoolId;
                    query = query.Where(m => m.SchoolId == id);
                }

                var list = query.Include(m => m.ReceiptDetails)
                    .Include(m => m.ReceiptDetails)
                    .Include(m => m.ReceiptDetails.Select(n => n.StudentSection.Student))
                    .Include(m => m.ReceiptDetails.Select(n => n.StudentSection.Section))
                    .Ordered().AsNoTracking().ToList();

                this.ds.AddDailyIncomeDetails(list, this.StudentNameFormatCollection.SelectedIndex,
                    this.SectionNameFormatCollection.SelectedIndex);
                this.ds.Add(this.UoW.GetCurrenciesAsNoTracking(list.Select(m => m.CurrencyId)));
                this.ds.Add(this.UoW.GetSchoolsAsNoTracking(list.Select(m => m.SchoolId)));
                this.ds.Add(this.UoW.SelectAllAsNoTracking<FeeType>());

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
                rpt.SetParameterValue("Date", string.Format("Receipt Date : {0}",
                    this.ReceiptPopup.ReceiptDate.ToDateString()));

                return rpt;
            }
            else if (type == ReportType.MonthlyIncome)
            {
                var rpt = new CRMonthlyIncome();
                int month = this.ReceiptPopup.ReceiptDate.Month;
                int year = this.ReceiptPopup.ReceiptDate.Year;
                var query = this.UoW.Where<Receipt>(m => m.ReceiptDate.Month == month
                    && m.ReceiptDate.Year == year);

                if (this.ReceiptPopup.Currencies.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Currencies.SelectedItem.CurrencyId;
                    query = query.Where(m => m.CurrencyId == id);
                }
                if (this.ReceiptPopup.Schools.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Schools.SelectedItem.SchoolId;
                    query = query.Where(m => m.SchoolId == id);
                }

                var list = query.Ordered().AsNoTracking().ToList();
                var clist = new List<Guid>();
                var slist = new List<Guid>();

                this.ds.AddMonthlyIncome(list, clist, slist);
                this.ds.Add(this.UoW.GetCurrenciesAsNoTracking(clist));
                this.ds.Add(this.UoW.GetSchoolsAsNoTracking(slist));

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
                rpt.SetParameterValue("Date", string.Format("Month For : {0}",
                    this.ReceiptPopup.ReceiptDate.ToString(this.ReceiptPopup.FormatString)));

                return rpt;
            }

            else if (type == ReportType.YearlyIncome)
            {
                var rpt = new CRYearlyIncome();
                var sd = this.ReceiptPopup.ReceiptDate;
                var ed = this.ReceiptPopup.ToDate.AddMonths(1);

                this.RepairDay(ref sd, 1);
                this.RepairDay(ref ed, 1);

                var query = this.UoW.Where<Receipt>(m => m.ReceiptDate >= sd && m.ReceiptDate < ed);
                if (this.ReceiptPopup.Currencies.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Currencies.SelectedItem.CurrencyId;
                    query = query.Where(m => m.CurrencyId == id);
                }
                if (this.ReceiptPopup.Schools.SelectedIndex > 0)
                {
                    var id = this.ReceiptPopup.Schools.SelectedItem.SchoolId;
                    query = query.Where(m => m.SchoolId == id);
                }

                var list = query.Ordered().AsNoTracking().ToList();
                var clist = new List<Guid>();
                var slist = new List<Guid>();

                this.ds.AddYearlyIncome(list, clist, slist);
                this.ds.Add(this.UoW.GetCurrenciesAsNoTracking(clist));
                this.ds.Add(this.UoW.GetSchoolsAsNoTracking(slist));

                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());
                rpt.SetParameterValue("Date", string.Format("{0} to {1}",
                    this.ReceiptPopup.ReceiptDate.ToString(this.ReceiptPopup.FormatString),
                    this.ReceiptPopup.ToDate.ToString(this.ReceiptPopup.FormatString)));

                return rpt;
            }
            else if (type == ReportType.School)
            {
                var rpt = new CRSchool();

                this.ds.Add(this.Schools.Where(m => m.Value1 == true).Select(m => m.Key));
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.AcademicYear)
            {
                var rpt = new CRAcademicYear();

                this.ds.Add(this.academicYearCollection.Where(m => m.Value1 == true).Select(m => m.Key));
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Course)
            {
                var rpt = new CRCourse();

                this.ds.Add(this.CourseCollection.Where(m => m.Value1 == true).Select(m => m.Key));
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Class)
            {
                var rpt = new CRClass();
                var checkedClasses = this.classCollection.Where(m => m.Value1 == true).Select(m => m.Key);
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Section)
            {
                var rpt = new CRSection();
                var checkedSections = this.sectionCollection.Where(m => m.Value1 == true).Select(m => m.Key);
                var checkedClasses = GetClasses(checkedSections.Select(m => m.ClassId));
                var checkedCourses = GetCourses(checkedClasses.Select(m => m.CourseId));

                this.ds.AddSectionDetails(checkedSections);
                this.ds.Add(checkedClasses);
                this.ds.Add(checkedCourses);
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.AddressType)
            {
                var rpt = new CRAddressType();

                this.ds.Add(this.UoW.SelectAllAsNoTracking<AddressType>());
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.EmailType)
            {
                var rpt = new CREmailType();

                this.ds.Add(this.UoW.SelectAllAsNoTracking<EmailType>());
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.PhoneType)
            {
                var rpt = new CRPhoneType();

                this.ds.Add(this.UoW.SelectAllAsNoTracking<PhoneType>());
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.GuardianType)
            {
                var rpt = new CRGuardianType();

                this.ds.Add(this.UoW.SelectAllAsNoTracking<GuardianType>());
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Country)
            {
                var rpt = new CRCountry();

                this.ds.Add(this.UoW.CountriesAsNoTracking);
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Religion)
            {
                var rpt = new CRReligion();

                this.ds.Add(this.UoW.ReligionsAsNoTracking);
                SetDataSource(rpt);

                return rpt;
            }
            else if (type == ReportType.Currency)
            {
                var rpt = new CRCurrency();
                var query = this.UoW.GetCurrencies();

                this.ds.Add(query.Ordered().AsNoTracking());
                SetDataSource(rpt);
                rpt.SetParameterValue("HeaderText", "Currency List");

                return rpt;
            }
            else if (type == ReportType.FeeType)
            {
                var rpt = new CRFeeType();

                this.ds.Add(this.UoW.SelectAllAsNoTracking<FeeType>());
                SetDataSource(rpt);

                return rpt;
            }

            return null;
        }

        private void SetDataSource(CrystalDecisions.CrystalReports.Engine.ReportClass rpt)
        {
            if (rpt != null)
            {
                rpt.SetDataSource(this.ds);
                rpt.SetParameterValue("PrintDate", DateTime.Now.ToDateString());

                if (this.SelectedItem.DisplayLevel == ReportLevel.AcademicYear)
                    rpt.SetParameterValue("FooterText", this.Schools.SelectedItem.Key.SchoolName);
                else if (this.SelectedItem.DisplayLevel > ReportLevel.AcademicYear)
                    rpt.SetParameterValue("FooterText", string.Format("{0} - {1}",
                        this.Schools.SelectedItem.Key.SchoolName, this.academicYearCollection.SelectedItem.Key.AcademicYearName));
            }
        }

        //private IEnumerable<School> GetSchools(IEnumerable<Guid> ids)
        //{
        //    foreach (var id in ids.Distinct())
        //    {
        //        var value = this.UoW.SchoolsNoTracking.FirstOrDefault(m => m.SchoolId == id);
        //        if (value != null)
        //            yield return value;
        //    }
        //    yield break;
        //}

        private IEnumerable<AcademicYear> GetAcademicYears(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.academicYearCollection.FirstOrDefault(m => m.Key.AcademicYearId == id);
                if (value != null)
                    yield return value.Key;
            }
            yield break;
        }

        private IEnumerable<Course> GetCourses(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.CourseCollection.FirstOrDefault(m => m.Key.CourseId == id);
                if (value != null)
                    yield return value.Key;
            }
            yield break;
        }

        private IEnumerable<Class> GetClasses(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.classCollection.FirstOrDefault(m => m.Key.ClassId == id);
                if (value != null)
                    yield return value.Key;
            }
            yield break;
        }

        private IEnumerable<Section> GetSections(IEnumerable<Guid> ids)
        {
            foreach (var id in ids.Distinct())
            {
                var value = this.sectionCollection.FirstOrDefault(m => m.Key.SectionId == id);
                if (value != null)
                    yield return value.Key;
            }
            yield break;
        }

        //private IEnumerable<Currency> GetCurrencies(IEnumerable<Guid> ids)
        //{
        //    foreach (var id in ids.Distinct())
        //    {
        //        var value = this.UoW.CurrenciesNoTracking.FirstOrDefault(m => m.CurrencyId == id);
        //        if (value != null)
        //            yield return value;
        //    }
        //    yield break;
        //}

        //private IEnumerable<FeeType> GetFeeTypes(IEnumerable<Guid> ids)
        //{
        //    foreach (var id in ids.Distinct())
        //    {
        //        var value = this.UoW.FirstAsNoTracking<FeeType>(m => m.FeeTypeId == id);
        //        if (value != null)
        //            yield return value;
        //    }
        //    yield break;
        //}

        #endregion
    }
}
