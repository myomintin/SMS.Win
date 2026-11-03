using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.Core
{
    /// <summary>
    /// Search for Boolean, DateTime, String.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchOptionViewModelBDS<TModel, TOption> : BaseViewModel
        where TModel : class
    {
        #region Fields

        private bool? searchBoolean;
        private DateTime? searchDate;
        private DateTime? minDate;
        private DateTime? maxDate;
        private string searchString;
        private bool enableSearchBoolean;
        private bool enableSearchDate;
        private bool enableSearchString;
        private bool showBooleanMatchType;
        private bool showDateMatchType;
        private bool showStringMatchType;
        private Action onSearch;
        private Action onLoadData;

        #endregion

        #region Properties

        public ISMSUnitOfWork UoW { get; protected set; }
        public EntityCollection<TModel> Collection { get; private set; }
        public EntityCollection<SearchOptionInfo<TOption>> SearchOptionInfos { get; private set; }
        public EntityCollection<Tuple<ExactMatchType, string>> BooleanMatchTypes { get; private set; }
        public EntityCollection<Tuple<ValueMatchType, string>> DateMatchTypes { get; private set; }
        public EntityCollection<Tuple<StringMatchType, string>> StringMatchTypes { get; private set; }
        public bool CanRefreshContext { get; private set; }
        public bool CanSearch { get; protected set; }
        public bool SearchLively { get; set; }
        public int UsageCount { get; private set; }
        public int MaxUsageCountToRefresh { get; set; }

        public ICommand SearchCommand { get; protected set; }

        public bool? SearchBoolean
        {
            get { return this.searchBoolean; }
            set
            {
                if (this.searchBoolean != value)
                {
                    this.searchBoolean = value;
                    RaisePropertyChanged("SearchBoolean");
                    CheckToSearch();
                }
            }
        }

        public DateTime? SearchDate
        {
            get { return this.searchDate; }
            set
            {
                if (this.searchDate != value)
                {
                    this.searchDate = value;
                    RaisePropertyChanged("SearchDate");
                    CheckToSearch();
                }
            }
        }

        public string SearchString
        {
            get { return this.searchString; }
            set
            {
                if (this.searchString != value)
                {
                    this.searchString = value;
                    RaisePropertyChanged("SearchString");
                    CheckToSearch();
                }
            }
        }

        public bool EnableSearchBoolean
        {
            get { return this.enableSearchBoolean; }
            set
            {
                if (this.enableSearchBoolean != value)
                {
                    this.enableSearchBoolean = value;
                    RaisePropertyChanged("EnableSearchBoolean");
                }
            }
        }

        public bool EnableSearchDate
        {
            get { return this.enableSearchDate; }
            set
            {
                if (this.enableSearchDate != value)
                {
                    this.enableSearchDate = value;
                    RaisePropertyChanged("EnableSearchDate");
                }
            }
        }

        public bool EnableSearchString
        {
            get { return this.enableSearchString; }
            set
            {
                if (this.enableSearchString != value)
                {
                    this.enableSearchString = value;
                    RaisePropertyChanged("EnableSearchString");
                }
            }
        }

        public bool ShowBooleanMatchType
        {
            get { return this.showBooleanMatchType; }
            set
            {
                if (this.showBooleanMatchType != value)
                {
                    this.showBooleanMatchType = value;
                    RaisePropertyChanged("ShowBooleanMatchType");
                }
            }
        }

        public bool ShowDateMatchType
        {
            get { return this.showDateMatchType; }
            set
            {
                if (this.showDateMatchType != value)
                {
                    this.showDateMatchType = value;
                    RaisePropertyChanged("ShowDateMatchType");
                }
            }
        }

        public bool ShowStringMatchType
        {
            get { return this.showStringMatchType; }
            set
            {
                if (this.showStringMatchType != value)
                {
                    this.showStringMatchType = value;
                    RaisePropertyChanged("ShowStringMatchType");
                }
            }
        }

        public DateTime? MinDate
        {
            get { return this.minDate; }
            set
            {
                if (this.minDate != value)
                {
                    this.minDate = value;
                    RaisePropertyChanged("MinDate");
                }
            }
        }

        public DateTime? MaxDate
        {
            get { return this.maxDate; }
            set
            {
                if (this.maxDate != value)
                {
                    this.maxDate = value;
                    RaisePropertyChanged("MaxDate");
                }
            }
        }

        public Action OnSearch
        {
            get { return this.onSearch; }
            set
            {
                if (value == null)
                    this.onSearch = null;
                else
                    this.onSearch = value.GetInvocationList().LastOrDefault() as Action;
            }
        }

        protected Action OnLoadData
        {
            get { return this.onLoadData; }
            set
            {
                if (value == null)
                    this.onLoadData = null;
                else
                    this.onLoadData = value.GetInvocationList().LastOrDefault() as Action;
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchOptionViewModelBDS(ISMSUnitOfWork uow = null)
        {
            if (uow == null)
            {
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
                this.CanRefreshContext = true;
            }
            else
            {
                this.UoW = uow;
                this.CanRefreshContext = false;
            }
            this.MaxUsageCountToRefresh = ApplicationManager.Current.MaxUsageCountToRefresh;
            this.UsageCount = 0;

            this.ViewName = string.Format("Search{0}s",
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " "));
            this.Collection = new EntityCollection<TModel>();
            this.SearchOptionInfos = new EntityCollection<SearchOptionInfo<TOption>>();
            this.BooleanMatchTypes = new EntityCollection<Tuple<ExactMatchType, string>>();
            this.DateMatchTypes = new EntityCollection<Tuple<ValueMatchType, string>>();
            this.StringMatchTypes = new EntityCollection<Tuple<StringMatchType, string>>();

            this.BooleanMatchTypes.AddRange(SearchOptionHelper.GetExactMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.BooleanMatchTypes.SelectedIndex = 1;
            this.DateMatchTypes.AddRange(SearchOptionHelper.GetValueMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.DateMatchTypes.SelectedIndex = 1;
            this.StringMatchTypes.AddRange(SearchOptionHelper.GetStringMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.StringMatchTypes.SelectedIndex = 3;

            this.Collection.SelectionChanged += (s, e) =>
            {
                this.CanSave = this.Collection.SelectedIndex != -1;
            };
            this.SearchOptionInfos.SelectionChanged += (s, e) =>
            {
                SearchOptionInfoChanged();
            };
            this.BooleanMatchTypes.SelectionChanged += (s, e) =>
            {
                BooleanMatchTypeChanged();
            };
            this.DateMatchTypes.SelectionChanged += (s, e) =>
            {
                DateMatchTypeChanged();
            };
            this.StringMatchTypes.SelectionChanged += (s, e) =>
            {
                StringMatchTypeChanged();
            };
            this.searchBoolean = null;
            this.searchDate = null;
            this.searchString = string.Empty;
            this.SearchLively = false;
            this.SearchCommand = new RelayCommand(o => CallOnSearch(), o => this.CanSearch);
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected virtual void SearchOptionInfoChanged()
        {
            if (this.SearchOptionInfos.SelectedIndex == -1)
            {
                this.CanSearch = this.ShowBooleanMatchType = this.ShowDateMatchType =
                    this.ShowStringMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                if (this.showBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.showDateMatchType)
                    DateMatchTypeChanged();
                if (this.showStringMatchType)
                    StringMatchTypeChanged();
            }
        }

        protected virtual void BooleanMatchTypeChanged()
        {
            bool flag = this.BooleanMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchBoolean = false;
            else
                this.EnableSearchBoolean = !(this.BooleanMatchTypes.SelectedItem.Item1 == ExactMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected virtual void DateMatchTypeChanged()
        {
            bool flag = this.DateMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchDate = false;
            else
                this.EnableSearchDate = this.DateMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue;
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected virtual void StringMatchTypeChanged()
        {
            bool flag = this.StringMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchString = false;
            else
                this.EnableSearchString = !(this.StringMatchTypes.SelectedItem.Item1 == StringMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected bool? GetSearchBooleanValue()
        {
            bool? value = this.searchBoolean;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.BooleanMatchTypes.SelectedItem.Item1 == ExactMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = false;
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = false;
            }

            return value;
        }

        protected ExactMatchType GetBooleanMatchType()
        {
            ExactMatchType mt = this.BooleanMatchTypes.SelectedItem.Item1;

            if (mt == ExactMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ExactMatchType.Equal;

            return mt;
        }

        protected DateTime? GetSearchDateValue()
        {
            DateTime? value = this.searchDate;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.DateMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = this.minDate.HasValue ? this.minDate.Value : new DateTime();
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = this.minDate.HasValue ? this.minDate.Value : new DateTime();
            }

            return value;
        }

        protected ValueMatchType GetDateMatchType()
        {
            ValueMatchType mt = this.DateMatchTypes.SelectedItem.Item1;

            if (mt == ValueMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ValueMatchType.Equal;

            return mt;
        }

        protected string GetSearchStringValue()
        {
            string value = this.searchString;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.StringMatchTypes.SelectedItem.Item1 == StringMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = string.Empty;
            }
            else
            {
                if (nullable == false && value == null)
                    value = string.Empty;
            }

            return value;
        }

        protected StringMatchType GetStringMatchType()
        {
            StringMatchType mt = this.StringMatchTypes.SelectedItem.Item1;

            if (mt == StringMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = StringMatchType.Equal;

            return mt;
        }

        protected IQueryable<TResult> GetBooleanQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchBooleanValue(), GetBooleanMatchType()));
        }

        protected IQueryable<TResult> GetDateQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchDateValue(), GetDateMatchType()));
        }

        protected IQueryable<TResult> GetStringQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchStringValue(), GetStringMatchType()));
        }

        protected IQueryable<TResult> GetStringQuery<TResult>(string propertyName)
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                propertyName, GetSearchStringValue(), GetStringMatchType()));
        }

        protected IQueryable<TResult> GetStringQuery<TResult>(string leftPropertyName, string rightPropertyName,
            ExpressionType type = ExpressionType.OrElse)
            where TResult : class
        {
            var text = GetSearchStringValue();
            var mt = GetStringMatchType();
            var param = ExpressionHelper.CreateParameter<TResult>();
            var left = ExpressionHelper.CreateExpression<TResult>(leftPropertyName, text, mt, param);
            var right = ExpressionHelper.CreateExpression<TResult>(rightPropertyName, text, mt, param);

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(type, left, right, param));
        }

        protected IQueryable<TResult> GetStringQuery<TResult, TInner, TKey>(
            Expression<Func<TResult, bool>> predicate, string propertyName,
            Expression<Func<TResult, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TResult, TInner, TResult>> resultSelector)
            where TResult : class
            where TInner : class
        {
            if (this.StringMatchTypes.SelectedItem.Item1 == StringMatchType.NoValue)
                return this.UoW.Where(predicate);
            else
                return this.UoW.SelectAll<TResult>().Join(GetStringQuery<TInner>(propertyName),
                    outerKeySelector, innerKeySelector, resultSelector);
        }

        protected virtual void CheckToSearch()
        {
            if (this.IsLoaded && this.CanSearch && this.SearchLively)
                CallOnSearch();
        }

        protected virtual void CallOnSearch()
        {
            if (this.onSearch != null)
            {
                if (this.CanRefreshContext && this.UsageCount > this.MaxUsageCountToRefresh)
                {
                    this.UoW.Clear();
                    this.UoW.Dispose();
                    this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
                    UsageCount = 0;
                    LoadData();
                }
                UsageCount++;
                this.onSearch();
            }
        }

        public virtual void Clear()
        {
            ClearValues();
            this.Collection.Clear();
        }

        public virtual void ClearValues()
        {
            this.searchBoolean = null;
            this.searchDate = null;
            this.searchString = string.Empty;
        }

        public void LoadData()
        {
            if (this.onLoadData != null)
                this.onLoadData();
        }

        #endregion
    }
}
