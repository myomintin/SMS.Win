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
    /// Search list for Boolean, DateTime, String, Decimal, Integral.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchListOptionViewModelBDSDI<T, TModel, TOption> : SearchListOptionViewModelBDSD<T, TModel, TOption>
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private long? searchIntegral;
        private long? minIntegral;
        private long? maxIntegral;
        private bool enableSearchIntegral;
        private bool showIntegralMatchType;

        #endregion

        #region Properties

        public EntityCollection<Tuple<ValueMatchType, string>> IntegralMatchTypes { get; private set; }

        public long? SearchIntegral
        {
            get { return this.searchIntegral; }
            set
            {
                if (this.searchIntegral != value)
                {
                    this.searchIntegral = value;
                    RaisePropertyChanged("SearchIntegral");
                    CheckToSearch();
                }
            }
        }

        public bool EnableSearchIntegral
        {
            get { return this.enableSearchIntegral; }
            set
            {
                if (this.enableSearchIntegral != value)
                {
                    this.enableSearchIntegral = value;
                    RaisePropertyChanged("EnableSearchIntegral");
                }
            }
        }

        public bool ShowIntegralMatchType
        {
            get { return this.showIntegralMatchType; }
            set
            {
                if (this.showIntegralMatchType != value)
                {
                    this.showIntegralMatchType = value;
                    RaisePropertyChanged("ShowIntegralMatchType");
                }
            }
        }

        public long? MinIntegral
        {
            get { return this.minIntegral; }
            set
            {
                if (this.minIntegral != value)
                {
                    this.minIntegral = value;
                    RaisePropertyChanged("MinIntegral");
                }
            }
        }

        public long? MaxIntegral
        {
            get { return this.maxIntegral; }
            set
            {
                if (this.maxIntegral != value)
                {
                    this.maxIntegral = value;
                    RaisePropertyChanged("MaxIntegral");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchListOptionViewModelBDSDI()
        {
            this.IsLoaded = false;
            this.IntegralMatchTypes = new EntityCollection<Tuple<ValueMatchType, string>>();

            this.IntegralMatchTypes.AddRange(SearchOptionHelper.GetValueMatchTypes()
                .To(m => SearchOptionHelper.GetMatchTypeInfo(m)), false);
            this.IntegralMatchTypes.SelectedIndex = 1;

            this.IntegralMatchTypes.SelectionChanged += (s, e) =>
            {
                IntegralMatchTypeChanged();
            };
            this.searchIntegral = null;
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
                    this.ShowIntegralMatchType = false;
            }
            else
            {
                var current = this.SearchOptionInfos.SelectedItem;

                this.ShowBooleanMatchType = current.SearchValueType == SearchValueType.Boolean;
                this.ShowDateMatchType = current.SearchValueType == SearchValueType.DateTime;
                this.ShowStringMatchType = current.SearchValueType == SearchValueType.Text;
                this.ShowDecimalMatchType = current.SearchValueType == SearchValueType.Decimal;
                this.ShowIntegralMatchType = current.SearchValueType == SearchValueType.Integral;
                if (this.ShowBooleanMatchType)
                    BooleanMatchTypeChanged();
                if (this.ShowDateMatchType)
                    DateMatchTypeChanged();
                if (this.ShowStringMatchType)
                    StringMatchTypeChanged();
                if (this.ShowDecimalMatchType)
                    DecimalMatchTypeChanged();
                if (this.showIntegralMatchType)
                    IntegralMatchTypeChanged();
            }
        }

        protected virtual void IntegralMatchTypeChanged()
        {
            bool flag = this.IntegralMatchTypes.SelectedIndex == -1;

            if (flag)
                this.EnableSearchIntegral = false;
            else
                this.EnableSearchIntegral = !(this.IntegralMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue);
            this.CanSearch = !(this.SearchOptionInfos.SelectedIndex == -1 || flag);
            CheckToSearch();
        }

        protected long? GetSearchIntegralValue()
        {
            long? value = this.searchIntegral;
            bool nullable = this.SearchOptionInfos.SelectedItem.Nullable;

            if (this.IntegralMatchTypes.SelectedItem.Item1 == ValueMatchType.NoValue)
            {
                if (nullable)
                    value = null;
                else
                    value = this.minIntegral.HasValue ? this.minIntegral.Value : 0;
            }
            else
            {
                if (nullable == false && value.HasValue == false)
                    value = this.minIntegral.HasValue ? this.minIntegral.Value : 0;
            }

            return value;
        }

        protected ValueMatchType GetIntegralMatchType()
        {
            ValueMatchType mt = this.IntegralMatchTypes.SelectedItem.Item1;

            if (mt == ValueMatchType.NoValue &&
                this.SearchOptionInfos.SelectedItem.Nullable == false)
                mt = ValueMatchType.Equal;

            return mt;
        }

        protected IQueryable<TResult> GetIntegralQuery<TResult>()
            where TResult : class
        {
            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                GetSearchIntegralValue(), GetIntegralMatchType()));
        }

        protected IQueryable<TResult> GetIntegralQuery<TResult, TValue>(Converter<long?, TValue> converter)
            where TResult : class
        {
            object value;

            if (converter == null)
                value = GetSearchIntegralValue();
            else
                value = converter(GetSearchIntegralValue());

            return this.UoW.Where(ExpressionHelper.CreatePredicate<TResult>(
                this.SearchOptionInfos.SelectedItem.Option.ToString(),
                value, GetIntegralMatchType()));
        }

        public override void ClearValues()
        {
            base.ClearValues();
            this.searchIntegral = null;
        }

        #endregion
    }
}
