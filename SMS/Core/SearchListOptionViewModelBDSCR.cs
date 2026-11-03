using CodeflexSystems.Library.Core;
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
    /// Search list for Boolean, DateTime, String, Country, Religion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    internal abstract class SearchListOptionViewModelBDSCR<T, TModel, TOption> : SearchListOptionViewModelBDSC<T, TModel, TOption>
        where T : EntityViewModel<TModel>
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

        public SearchListOptionViewModelBDSCR(bool countriesWithDummy = true, bool religionsWithDummy = true)
            : base(countriesWithDummy)
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

        protected Guid? GetReligionId()
        {
            Guid id = this.Religions.SelectedItem.ReligionId;

            if (IsDummy(id) && this.SearchOptionInfos.SelectedItem.Nullable)
                return null;
            else
                return id;
        }

        public override void Clear()
        {
            base.Clear();
            this.Religions.Clear();
        }

        #endregion
    }
}
