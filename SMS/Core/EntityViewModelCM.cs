using CodeflexSystems.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    /// <summary>
    /// The implementation of the generic view model with created by and modified by for entity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal abstract class EntityViewModelCM<T> : EntityViewModel<T>
        where T : class
    {
        #region Properties

        public IParent Parent { get; protected set; }
        public string CreatedBy { get; protected set; }
        public string ModifiedBy { get; protected set; }

        #endregion

        #region Ctor, Dtor

        public EntityViewModelCM(T model)
            : base(model)
        { }

        public EntityViewModelCM(T model, IParent parent)
            : base(model)
        {
            this.Parent = parent;
        }

        #endregion

        #region Methods

        protected abstract void SetCreatedBy();
        protected abstract void SetModifiedBy();

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            SetCreatedBy();
            SetModifiedBy();
            BaseAcceptChanges();
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            BaseRefresh();
        }

        #endregion
    }
}
