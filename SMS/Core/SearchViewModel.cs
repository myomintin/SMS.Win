using CodeflexSystems.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal class SearchViewModel<TModel, TOption> : BaseViewModel
        where TModel : class
    {
        #region Fields

        private string searchString;

        #endregion

        #region Properties

        public EntityCollection<TModel> Collection { get; private set; }
        public EntityCollection<TOption> SearchOptions { get; private set; }
        public Action OnSearching { get; set; }
        public bool CanSearch { get; internal set; }

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

        #endregion

        #region Ctor, Dtor

        public SearchViewModel()
        {
            this.ViewName = string.Format("Search{0}s", 
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " "));
            this.Collection = new EntityCollection<TModel>();
            this.SearchOptions = new EntityCollection<TOption>();

            this.Collection.SelectionChanged += (s, e) =>
            {
                this.CanSave = this.Collection.SelectedIndex != -1;
            };
            this.SearchOptions.SelectionChanged += (s, e) =>
            {
                CheckToSearch();
            };
        }

        #endregion

        #region Methods

        private void CheckToSearch()
        {
            this.CanSearch = !(this.SearchOptions.SelectedIndex == -1 ||
                string.IsNullOrEmpty(this.searchString) || this.OnSearching == null ||
                this.searchString.Length < ApplicationManager.Current.MinRequiredSearchWordLength);
            if (this.CanSearch)
                this.OnSearching();
        }

        public virtual void Clear()
        {
            this.Collection.Clear();
            this.SearchString = string.Empty;
        }

        #endregion
    }
}
