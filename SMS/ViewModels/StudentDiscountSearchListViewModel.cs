using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class StudentDiscountSearchListViewModel : SearchListOptionViewModelBDSDP<StudentDiscountViewModel, StudentDiscount, StudentDiscountSearchOption>
    {
        #region Fields

        private SearchStudentViewModel searcher;

        #endregion

        #region Ctor, Dtor

        public StudentDiscountSearchListViewModel()
        {
            Initialize();
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
            this.searcher.Clear();
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.searcher = new SearchStudentViewModel();
                this.searcher.OnSearch = () => OnSearchStudents();
                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetStudentDiscountSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 1;

                this.ConvertTo = (m) => new StudentDiscountViewModel(m);
                this.RemoveAdded = true;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        protected override void SearchOptionInfoChanged()
        {
            base.SearchOptionInfoChanged();
            if (this.SearchOptionInfos.SelectedIndex != -1 &&
                this.SearchOptionInfos.SelectedItem.Option == StudentDiscountSearchOption.Student)
            {
                this.CanSearch = true;
                CheckToSearch();
            }
        }

        protected override void OnSearch()
        {
            try
            {
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<StudentDiscount> query = null;

                if (current.SearchValueType == SearchValueType.DateTime)
                {
                    query = GetDateQuery<StudentDiscount>();
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == StudentDiscountSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<StudentDiscount, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == StudentDiscountSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<StudentDiscount, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<StudentDiscount>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Decimal)
                {
                    query = GetDecimalQuery<StudentDiscount>();
                }
                else if (current.SearchValueType == SearchValueType.Object)
                {
                    if (current.Option == StudentDiscountSearchOption.Student &&
                        NavigationManager.ShowSearchStudent(this.searcher, null).DialogResult == true)
                    {
                        var value = this.searcher.Collection.SelectedItem.StudentId;
                        query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.StudentId == value);
                    }
                }
                else if (current.SearchValueType == SearchValueType.Percentage)
                {
                    var value = GetSearchPercentageValue();
                    var mt = GetPercentageMatchType();

                    if (current.Option == StudentDiscountSearchOption.DiscountPercentage)
                    {
                        if (mt == ValueMatchType.Equal)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) == value);
                        }
                        else if (mt == ValueMatchType.GreaterThan)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) > value);
                        }
                        else if (mt == ValueMatchType.GreaterThanOrEqual)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) >= value);
                        }
                        else if (mt == ValueMatchType.LessThan)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) < value);
                        }
                        else if (mt == ValueMatchType.LessThanOrEqual)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) <= value);
                        }
                        else if (mt == ValueMatchType.NotEqual)
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.StudentSection.Section.Fee.HasValue &&
                                (m.DiscountAmount * 100 / m.StudentSection.Section.Fee.Value) != value);
                        }
                        else
                        {
                            query = this.UoW.Where<StudentDiscount>(m => m.DiscountAmount == 0);
                        }
                    }
                }

                App.CursorWait();
                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Ordered().To(m => this.ConvertTo(m)), false);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog(this.ViewName, ex);
            }
            finally
            {
                this.searcher.Clear();
                App.CursorDefault();
            }
        }

        private void OnSearchStudents()
        {
            try
            {
                App.CursorWait();
                var query = this.searcher.GetQuery();

                this.searcher.Collection.Clear();
                if (query != null)
                {
                    var query2 = from t in query.SelectMany(m => m.StudentSections)
                                 where t.StudentDiscount != null
                                 select t.StudentId;
                    var query3 = this.searcher.UoW.SelectAll<Student>().Join(query2.Distinct(),
                        m => m.StudentId, u => u, (m, u) => m);
                    this.searcher.Collection.AddRange(query3.Ordered().AsNoTracking(), false);
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Search Students", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override bool Delete(StudentDiscountViewModel vm)
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
                DialogManager.ShowErrorAndLog("Delete Data", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(StudentDiscountViewModel vm)
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
                if (NavigationManager.ShowStudentDiscount(vm).DialogResult == true)
                    Edit(vm);
                else
                {
                    if (vm.HasAnyErrors)
                    {
                        DialogResult result = DialogManager.AskConfirmation("Confirmation",
                            SR.GetString("EditDataAsk3", "student discount"),
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

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Data", ex);
            }
        }

        internal override void OnSave()
        { }

        private void Setup(StudentDiscountViewModel vm)
        {
            if (vm == null)
                return;

            vm.ViewName = "Edit Student Discount";
            if (vm.IsLoaded)
                return;

            vm.LoadData(this);
        }

        #endregion
    }
}
