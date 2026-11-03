using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    /// <summary>
    /// Search for Boolean, DateTime, String, Country.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchOptionViewModelBDSC<TModel, TOption> : SearchOptionViewModelBDS<TModel, TOption>
        where TModel : class
    {
        #region Fields

        private bool showCountryMatchType;
        protected bool countriesWithDummy;

        #endregion

        #region Properties

        public EntityCollection<Country> Countries { get; private set; }

        public bool ShowCountryMatchType
        {
            get { return this.showCountryMatchType; }
            set
            {
                if (this.showCountryMatchType != value)
                {
                    this.showCountryMatchType = value;
                    RaisePropertyChanged("ShowCountryMatchType");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchOptionViewModelBDSC(ISMSUnitOfWork uow = null, bool countriesWithDummy = true)
            : base(uow)
        {
            this.IsLoaded = false;
            this.countriesWithDummy = countriesWithDummy;
            if (this.countriesWithDummy)
                this.Countries = new EntityCollection<Country>(this.UoW.CountriesAsNoTracking.WithDummy());
            else
                this.Countries = new EntityCollection<Country>(this.UoW.CountriesAsNoTracking);
            this.Countries.SelectedItem = this.Countries.GetDefault();
            this.Countries.SelectionChanged += (s, e) =>
            {
                CountryChanged();
            };
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected virtual void CountryChanged()
        {
            this.CanSearch = this.Countries.SelectedIndex != -1;
            CheckToSearch();
        }

        public override void Clear()
        {
            base.Clear();
            this.Countries.Clear();
        }

        #endregion
    }
}
