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
    /// Search list for Boolean, DateTime, String, Decimal, Integral, Percentage.
    /// </summary>
    internal abstract class SearchListOptionViewModelBDSDIP<T, TModel, TOption> : SearchListOptionViewModelBDSDI<T, TModel, TOption>
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private decimal? searchPercentage;
        private decimal? minPercentage;
        private decimal? maxPercentage;
        private bool enableSearchPercentage;
        private bool showPercentageMatchType;

        #endregion

        #region Properties

        public EntityCollection<Tuple<ValueMatchType, string>> PercentageMatchTypes { get; private set; }

        public decimal? SearchPercentage
        {
            get { return this.searchPercentage; }
            set
            {
                if (this.searchPercentage != value)
                {
                    this.searchPercentage = value;
                    RaisePropertyChanged("SearchPercentage");
                    CheckToSearch();
                }
            }
        }

        public bool EnableSearchPercentage
        {
            get { return this.enableSearchPercentage; }
            set
            {
                if (this.enableSearchPercentage != value)
                {
                    this.enableSearchPercentage = value;
                    RaisePropertyChanged("EnableSearchPercentage");
                }
            }
        }

        public bool ShowPercentageMatchType
        {
            get { return this.showPercentageMatchType; }
            set
            {
                if (this.showPercentageMatchType != value)
                {
                    this.showPercentageMatchType = value;
                    RaisePropertyChanged("ShowPercentageMatchType");
                }
            }
        }

        public decimal? MinPercentage
        {
            get { return this.minPercentage; }
            set
            {
                if (this.minPercentage != value)
                {
                    this.minPercentage = value;
                    RaisePropertyChanged("MinPercentage");
                }
            }
        }

        public decimal? MaxPercentage
        {
            get { return this.maxPercentage; }
            set
            {
                if (this.maxPercentage != value)
                {
                    this.maxPercentage = value;
                    RaisePropertyChanged("MaxPercentage");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchListOptionViewModelBDSDIP()
        {
            this.IsLoaded = false;
            this.PercentageMatchTypes = new EntityCollection<Tuple<ValueMatchType, string>>();

            this.PercentageMatchTypes.AddRange(SearchOptionHelper.GetValueMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.PercentageMatchTypes.SelectedIndex = 1;

            this.PercentageMatchTypes.SelectionChanged += (s, e) =>
            {
                PercentageMatchTypeChanged();
            };
            this.minPercentage = 0;
            //this.maxPercentage = 100;
            this.searchPercentage = null;
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
                    this.ShowIntegralMatchType = this.ShowPercentageMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowDecimalMatchType = current.SearchValueType == SearchValueType.Decimal;
                this.ShowIntegralMatchType = current.SearchValueType == SearchValueType.Integral;
                this.ShowPercentageMatchType = current.SearchValueType == SearchValueType.Percentage;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.ShowDecimalMatchType)
                    DecimalMatchTypeChanged();
                if (this.ShowIntegralMatchType)
                    IntegralMatchTypeChanged();
                if (this.showPercentageMatchType)
                    PercentageMatchTypeChanged();
            }
        }

        protected virtual void PercentageMatchTypeChanged()
        {
            bool flag = this.PercentageMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchPercentage = false;
            else
                this.EnableSearchPercentage = !(this.PercentageMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected decimal? GetSearchPercentageValue()
        {
            decimal? value = this.searchPercentage;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.PercentageMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = this.minPercentage.HasValue ? this.minPercentage.Value : 0;
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = this.minPercentage.HasValue ? this.minPercentage.Value : 0;
            }

            return value;
        }

        protected ValueMatchType GetPercentageMatchType()
        {
            ValueMatchType mt = this.PercentageMatchTypes.SelectedItem.Item1;

            if (mt == ValueMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ValueMatchType.Equal;

            return mt;
        }

        protected IQueryable<TResult> GetPercentageQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchPercentageValue(), GetPercentageMatchType()));
        }

        protected IQueryable<TResult> GetPercentageQuery<TResult, TValue>(Converter<decimal?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchPercentageValue();
            else
                value = converter(GetSearchPercentageValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                value, GetPercentageMatchType()));
        }

        protected IQueryable<TResult> GetPercentageQuery<TResult>(string propertyName)
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                propertyName, GetSearchPercentageValue(), GetPercentageMatchType()));
        }

        protected IQueryable<TResult> GetPercentageQuery<TResult, TValue>(string propertyName, Converter<decimal?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchPercentageValue();
            else
                value = converter(GetSearchPercentageValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                propertyName, value, GetPercentageMatchType()));
        }

        public override void ClearValues()
        {
            base.ClearValues();
            this.searchPercentage = null;
        }

        #endregion

    }
}
