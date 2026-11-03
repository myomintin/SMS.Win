using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal abstract class ListViewModelDictionary<TKey, T, TModel> : ListBaseViewModel
        where T : EntityViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private TKey key;
        private TKey dummyKey;
        private EntityViewModelCollection<T, TModel> collection;
        private EntityViewModelCollection<T, TModel> dummyCollection;
        private Func<TModel, T> convertTo;
        private Func<IEnumerable<T>> getData;

        #endregion

        #region Properties

        public Dictionary<TKey, EntityViewModelCollection<T, TModel>> Dictionary { get; private set; }
        public Action SelectionChanged { get; set; }

        public Func<TModel, T> ConvertTo
        {
            get { return this.convertTo; }
            set
            {
                if (value == null)
                    this.convertTo = null;
                else
                    this.convertTo = value.GetInvocationList().LastOrDefault() as Func<TModel, T>;
            }
        }

        public Func<IEnumerable<T>> GetData
        {
            get { return this.getData; }
            set
            {
                if (value == null)
                    this.getData = value;
                else
                    this.getData = value.GetInvocationList().LastOrDefault() as Func<IEnumerable<T>>;
            }
        }

        public TKey Key
        {
            get { return this.key; }
            set
            {
                Check();
                if (!object.Equals(this.key, value))
                {
                    this.key = value;
                    RaisePropertyChanged("Key");
                    SetKey();
                }
            }
        }

        public EntityViewModelCollection<T, TModel> Collection
        {
            get { return this.collection; }
            protected set
            {
                if (this.collection != value)
                {
                    this.collection = value;
                    RaisePropertyChanged("Collection");
                    OnRefreshBinding();
                }
            }
        }

        public bool HasAnyErrors
        {
            get { return this.Dictionary.Any(m => m.Value.HasAnyErrors); }
        }

        #endregion

        #region Ctor, Dtor

        public ListViewModelDictionary()
        {
            this.ViewName = string.Format("{0} List",
                System.Text.RegularExpressions.Regex.Replace(typeof(TModel).Name, "(?=[A-Z])", " ").TrimStart());
            this.Dictionary = new Dictionary<TKey, EntityViewModelCollection<T, TModel>>();
            this.dummyKey = default(TKey);
            this.dummyCollection = new EntityViewModelCollection<T, TModel>();
            LoadDummyData(false);
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            try
            {
                this.AddCommand = new RelayCommand(o => OnAdd(), o => this.CanAdd);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Collection.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnDelete(), o => this.Collection.CanDelete);
                this.RefreshCommand = new RelayCommand(o => OnRefresh(), o => this.CanRefresh);
                this.SaveCommand = new RelayCommand(o => OnSave(), o => this.CanSave);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize Commands", ex);
            }
        }

        public virtual void Clear()
        {
            foreach (var value in this.Dictionary.Values)
                value.Clear();
            this.Dictionary.Clear();
            LoadDummyData();
            this.CanAdd = this.CanRefresh = this.CanSave = false;
        }

        public virtual void Add(T vm)
        {
            this.Collection.Add(vm, true);
            vm.CanSave = false;
            this.CanSave = true;
        }

        public virtual bool Delete(T vm)
        {
            bool flag = this.Collection.Delete(vm);

            if (flag)
            {
                vm.CanSave = false;
                this.CanSave = true;
            }

            return flag;
        }

        protected override void OnDelete()
        {
            try
            {
                Delete(this.Collection.SelectedItem);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Delete Data", ex);
            }
        }

        public virtual void Edit(T vm)
        {
            vm.CanSave = false;
            this.CanSave = true;
        }

        protected virtual void DetectChanges()
        {
            foreach (var source in this.Dictionary.Values)
            {
                source.Where(vm => vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.UoW.Remove(vm.Model));
                source.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.UoW.Add(vm.Model));
            }
        }

        public virtual void SaveChanges()
        {
            DetectChanges();
#if DEBUG
            if ((this.SaveMode & SaveMode.PrintConsole) == SaveMode.PrintConsole)
                PrintChanges();
            if ((this.SaveMode & SaveMode.PrintUI) == SaveMode.PrintUI)
                ShowChanges();
            if ((this.SaveMode & SaveMode.SaveChanges) == SaveMode.SaveChanges)
                this.UoW.SaveChanges();
            if ((this.SaveMode & SaveMode.AcceptChanges) == SaveMode.AcceptChanges)
            {
                foreach (var source in this.Dictionary.Values)
                    source.AcceptAllChanges();
            }
#else
            this.UoW.SaveChanges();
            foreach (var source in this.Dictionary.Values)
                    source.AcceptAllChanges();
#endif
            this.CanSave = false;
            OnRefreshBinding();
        }

        public void LoadDummyData(bool withPropertyChanged = true)
        {
            this.key = this.dummyKey;
            this.collection = this.dummyCollection;
            if (withPropertyChanged)
            {
                RaisePropertyChanged("Key");
                RaisePropertyChanged("Collection");
                //OnRefreshBinding();
            }
        }

        public void SetKeyAndValue(TKey key, IEnumerable<T> list)
        {
            if (list == null)
                throw new NullReferenceException(SR.GetString("CannotBeNull", "list"));
            if (this.key.Equals(key))
                return;

            this.key = key;
            RaisePropertyChanged("Key");

            if (!this.Dictionary.ContainsKey(this.key))
                this.Dictionary.Add(this.key, CreateCollection(list));
            this.Collection = this.Dictionary[this.key];
        }

        public void SetKeyAndValue(TKey key, IEnumerable<TModel> list)
        {
            if (list == null)
                throw new NullReferenceException(SR.GetString("CannotBeNull", "list"));
            if (this.convertTo == null)
                throw new NullReferenceException(SR.GetString("CannotBeNull", "ConvertTo"));
            if (this.key.Equals(key))
                return;

            this.key = key;
            RaisePropertyChanged("Key");

            if (!this.Dictionary.ContainsKey(this.key))
                this.Dictionary.Add(this.key, CreateCollection(list.To(m => this.convertTo(m))));
            this.Collection = this.Dictionary[this.key];
        }

        private void Check()
        {
            if (this.getData == null)
                throw new NullReferenceException(SR.GetString("CannotBeNull", "GetData"));
        }

        private void SetKey()
        {
            if (!this.Dictionary.ContainsKey(this.key))
                this.Dictionary.Add(this.key, CreateCollection(this.getData()));
            this.Collection = this.Dictionary[this.key];
        }

        private EntityViewModelCollection<T, TModel> CreateCollection(IEnumerable<T> list)
        {
            var value = new EntityViewModelCollection<T, TModel>(list);

            value.SelectionChanged += (s, e) =>
            {
                this.DeleteContent = value.ToDelete ? "Un_delete" : "_Delete";
                if (this.SelectionChanged != null)
                    this.SelectionChanged();
            };

            return value;
        }

        #endregion
    }
}
