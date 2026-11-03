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
    /// Search for Boolean, DateTime, String, Country, Religion.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchOptionViewModelBDSCR<TModel, TOption> : SearchOptionViewModelBDSC<TModel, TOption>
        where TModel : class
    {
        #region Fields

        private bool showReligionMatchType;
        protected bool religionsWithDummy;

        #endregion

        #region Properties

        public EntityCollection<Religion> Religions { get; private set; }

        public bool ShowReligionMatchType
        {
            get { return this.showReligionMatchType; }
            set
            {
                if (this.showReligionMatchType != value)
                {
                    this.showReligionMatchType = value;
                    RaisePropertyChanged("ShowReligionMatchType");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public SearchOptionViewModelBDSCR(ISMSUnitOfWork uow = null, bool countriesWithDummy = true, bool religionsWithDummy = true)
            : base(uow, countriesWithDummy)
        {
            this.IsLoaded = false;
            this.religionsWithDummy = religionsWithDummy;
            if (this.religionsWithDummy)
                this.Religions = new EntityCollection<Religion>(this.UoW.ReligionsAsNoTracking.WithDummy());
            else
                this.Religions = new EntityCollection<Religion>(this.UoW.ReligionsAsNoTracking);
            this.Religions.SelectionChanged += (s, e) =>
            {
                ReligionChanged();
            };
            this.IsLoaded = true;
        }

        #endregion

        #region Methods

        protected virtual void ReligionChanged()
        {
            this.CanSearch = this.Religions.SelectedIndex != -1;
            CheckToSearch();
        }

        public override void Clear()
        {
            base.Clear();
            this.Religions.Clear();
        }

        #endregion
    }
}
