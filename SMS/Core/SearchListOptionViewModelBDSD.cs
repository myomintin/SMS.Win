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
    /// Search list for Boolean, DateTime, String, Decimal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchListOptionViewModelBDSD<T, TModel, TOption> : SearchListOptionViewModelBDS<T, TModel, TOption>
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private decimal? searchDecimal;
        private decimal? minDecimal;
        private decimal? maxDecimal;
        private bool enableSearchDecimal;
        private bool showDecimalMatchType;

        #endregion

        #region Properties

        public EntityCollection<Tuple<ValueMatchType, string>> DecimalMatchTypes { get; private set; }

        public decimal? SearchDecimal
        {
            get { return this.searchDecimal; }
            set
            {
                if (this.searchDecimal != value)
                {
                    this.searchDecimal = value;
                    RaisePropertyChanged("SearchDecimal");
                    CheckToSearch();
                }
            }
        }

        public bool EnableSearchDecimal
        {
            get { return this.enableSearchDecimal; }
            set
            {
                if (this.enableSearchDecimal != value)
                {
                    this.enableSearchDecimal = value;
                    RaisePropertyChanged("EnableSearchDecimal");
                }
            }
        }

        public bool ShowDecimalMatchType
        {
            get { return this.showDecimalMatchType; }
            set
            {
                if (this.showDecimalMatchType != value)
                {
                    this.showDecimalMatchType = value;
                    RaisePropertyChanged("ShowDecimalMatchType");
                }
            }
        }

        public decimal? MinDecimal
        {
            get { return this.minDecimal; }
            set
            {
                if (this.minDecimal != value)
                {
                    this.minDecimal = value;
                    RaisePropertyChanged("MinDecimal");
                }
            }
        }

        public decimal? MaxDecimal
        {
            get { return this.maxDecimal; }
            set
            {
                if (this.maxDecimal != value)
                {
                    this.maxDecimal = value;
                    RaisePropertyChanged("MaxDecimal");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchListOptionViewModelBDSD()
        {
            this.IsLoaded = false;
            this.DecimalMatchTypes = new EntityCollection<Tuple<ValueMatchType, string>>();

            this.DecimalMatchTypes.AddRange(SearchOptionHelper.GetValueMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.DecimalMatchTypes.SelectedIndex = 1;

            this.DecimalMatchTypes.SelectionChanged += (s, e) =>
            {
                DecimalMatchTypeChanged();
            };
            this.searchDecimal = null;
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected override void SearchOptionInfoChanged()
        {
            if (this.SearchOptionInfos.SelectedIndex == -1)
            {
                this.CanSearch = this.ShowBooleanMatchType = this.ShowDateMatchType =
                    this.ShowStringMatchType = this.ShowDecimalMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowDecimalMatchType = current.SearchValueType == SearchValueType.Decimal;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.showDecimalMatchType)
                    DecimalMatchTypeChanged();
            }
        }

        protected virtual void DecimalMatchTypeChanged()
        {
            bool flag = this.DecimalMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchDecimal = false;
            else
                this.EnableSearchDecimal = !(this.DecimalMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected decimal? GetSearchDecimalValue()
        {
            decimal? value = this.searchDecimal;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.DecimalMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = this.minDecimal.HasValue ? this.minDecimal.Value : 0;
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = this.minDecimal.HasValue ? this.minDecimal.Value : 0;
            }

            return value;
        }

        protected ValueMatchType GetDecimalMatchType()
        {
            ValueMatchType mt = this.DecimalMatchTypes.SelectedItem.Item1;

            if (mt == ValueMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ValueMatchType.Equal;

            return mt;
        }

        protected IQueryable<TResult> GetDecimalQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchDecimalValue(), GetDecimalMatchType()));
        }

        protected IQueryable<TResult> GetDecimalQuery<TResult, TValue>(Converter<decimal?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchDecimalValue();
            else
                value = converter(GetSearchDecimalValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                value, GetDecimalMatchType()));
        }

        protected IQueryable<TResult> GetDecimalQuery<TResult>(string propertyName)
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                propertyName, GetSearchDecimalValue(), GetDecimalMatchType()));
        }

        protected IQueryable<TResult> GetDecimalQuery<TResult, TValue>(string propertyName, Converter<decimal?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchDecimalValue();
            else
                value = converter(GetSearchDecimalValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                propertyName, value, GetDecimalMatchType()));
        }

        public override void ClearValues()
        {
            base.ClearValues();
            this.searchDecimal = null;
        }

        #endregion
    }
}
