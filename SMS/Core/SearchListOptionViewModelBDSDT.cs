using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    /// <summary>
    /// Search list for Boolean, DateTime, String, Decimal, Time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchListOptionViewModelBDSDT<T, TModel, TOption> : SearchListOptionViewModelBDSD<T, TModel, TOption>
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private DateTime? searchTime;
        private DateTime? minTime;
        private DateTime? maxTime;
        private bool enableSearchTime;
        private bool showTimeMatchType;

        #endregion

        #region Properties

        public EntityCollection<Tuple<ValueMatchType, string>> TimeMatchTypes { get; private set; }

        public DateTime? SearchTime
        {
            get { return this.searchTime; }
            set
            {
                if (this.searchTime != value)
                {
                    this.searchTime = value;
                    RaisePropertyChanged("SearchTime");
                    CheckToSearch();
                }
            }
        }

        public bool EnableSearchTime
        {
            get { return this.enableSearchTime; }
            set
            {
                if (this.enableSearchTime != value)
                {
                    this.enableSearchTime = value;
                    RaisePropertyChanged("EnableSearchTime");
                }
            }
        }

        public bool ShowTimeMatchType
        {
            get { return this.showTimeMatchType; }
            set
            {
                if (this.showTimeMatchType != value)
                {
                    this.showTimeMatchType = value;
                    RaisePropertyChanged("ShowTimeMatchType");
                }
            }
        }

        public DateTime? MinTime
        {
            get { return this.minTime; }
            set
            {
                if (this.minTime != value)
                {
                    this.minTime = value;
                    RaisePropertyChanged("MinTime");
                }
            }
        }

        public DateTime? MaxTime
        {
            get { return this.maxTime; }
            set
            {
                if (this.maxTime != value)
                {
                    this.maxTime = value;
                    RaisePropertyChanged("MaxTime");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchListOptionViewModelBDSDT()
        {
            this.IsLoaded = false;
            this.TimeMatchTypes = new EntityCollection<Tuple<ValueMatchType, string>>();

            this.TimeMatchTypes.AddRange(SearchOptionHelper.GetValueMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.TimeMatchTypes.SelectedIndex = 1;

            this.TimeMatchTypes.SelectionChanged += (s, e) =>
            {
                TimeMatchTypeChanged();
            };
            this.searchTime = null;
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected override void SearchOptionInfoChanged()
        {
            if (this.SearchOptionInfos.SelectedIndex == -1)
            {
                this.CanSearch = this.ShowBooleanMatchType = this.ShowDateMatchType =
                    this.ShowStringMatchType = this.ShowDecimalMatchType =
                    this.ShowTimeMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowDecimalMatchType = current.SearchValueType == SearchValueType.Decimal;
                this.ShowTimeMatchType = current.SearchValueType == SearchValueType.Time;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.ShowDecimalMatchType)
                    DecimalMatchTypeChanged();
                if (this.showTimeMatchType)
                    TimeMatchTypeChanged();
            }
        }

        protected virtual void TimeMatchTypeChanged()
        {
            bool flag = this.TimeMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchTime = false;
            else
                this.EnableSearchTime = !(this.TimeMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        public DateTime? GetSearchTimeValue()
        {
            DateTime? value = this.searchTime;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.TimeMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = this.minTime.HasValue ? this.minTime.Value : new DateTime();
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = this.minTime.HasValue ? this.minTime.Value : new DateTime();
            }

            return value;
        }

        protected ValueMatchType GetTimeMatchType()
        {
            ValueMatchType mt = this.TimeMatchTypes.SelectedItem.Item1;

            if (mt == ValueMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ValueMatchType.Equal;

            return mt;
        }

        protected IQueryable<TResult> GetTimeQuery<TResult>(bool convertToTimeSpan = false)
            where TResult : class
        {
            if (convertToTimeSpan)
                return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                    this.SearchOptionInfos.SelectedItem.Option.ToString(),
                    GetTimeSpan(GetSearchTimeValue()), GetTimeMatchType()));
            else
                return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                    this.SearchOptionInfos.SelectedItem.Option.ToString(),
                    GetSearchTimeValue(), GetTimeMatchType()));
        }

        protected IQueryable<TResult> GetTimeQuery<TResult, TValue>(Converter<DateTime?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchTimeValue();
            else
                value = converter(GetSearchTimeValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                value, GetTimeMatchType()));
        }

        public override void ClearValues()
        {
            base.ClearValues();
            this.searchTime = null;
        }

        #endregion
    }
}
