using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace CodeflexSystems.SMS.Data
{
    internal interface IUnitOfWork : IDisposable
    {
        DbContextConfiguration Configuration { get; }
        SMSContext Context { get; }
        ObjectContext ObjectContext { get; }
        DbSaveOption DbSaveOption { get; set; }
        bool RememberDbSaveOption { get; set; }
        string CreatedDatePropertyName { get; set; }
        string CreatedByPropertyName { get; set; }
        string ModifiedDatePropertyName { get; set; }
        string ModifiedByPropertyName { get; set; }

        int SaveChanges();
        T Add<T>(T entity, bool saveChanges = false) where T : class;
        IEnumerable<T> AddRange<T>(IEnumerable<T> entities, bool saveChanges = false) where T : class;
        T Attach<T>(T entity) where T : class;
        T Modify<T>(T entity, bool saveChanges = false) where T : class;
        T Remove<T>(T entity, bool saveChanges = false) where T : class;
        IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities, bool saveChanges = false) where T : class;
        void Refresh<T>(RefreshMode refreshMode, T entity) where T : class;
        void Refresh<T>(T entity) where T : class;
        void RefreshRange<T>(ICollection<T> collection, bool removeAdded = false) where T : class;
        void RefreshRange<T>(RefreshMode refreshMode, ICollection<T> collection, bool removeAdded = false) where T : class;
        void RefreshRangeToOriginalValues<T>(ICollection<T> collection, bool removeAdded = false) where T : class;
        void RefreshToOriginalValues<T>(T entity) where T : class;
        void AddedToDetached<T>(T entity) where T : class;
 
        bool Any<T>() where T : class;
        bool Any<T>(Expression<Func<T, bool>> predicate) where T : class;
        T First<T>() where T : class;
        T First<T>(Expression<Func<T, bool>> predicate) where T : class;
        T FirstAsNoTracking<T>() where T : class;
        T FirstAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
        T FirstOrDefault<T>() where T : class;
        T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        T FirstOrDefaultAsNoTracking<T>() where T : class;
        T FirstOrDefaultAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
        IOrderedQueryable<T> OrderBy<T, TKey>(Expression<Func<T, TKey>> keySelector) where T : class;
        IOrderedQueryable<T> OrderBy<T, TKey>(Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer) where T : class;
        IOrderedQueryable<T> OrderByDescending<T, TKey>(Expression<Func<T, TKey>> keySelector) where T : class;
        IOrderedQueryable<T> OrderByDescending<T, TKey>(Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer) where T : class;
        DbSet<T> SelectAll<T>() where T : class;
        DbQuery<T> SelectAllAsNoTracking<T>() where T : class;
        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> Where<T>(Expression<Func<T, int, bool>> predicate) where T : class;
        IQueryable<T> WhereAsNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> WhereAsNoTracking<T>(Expression<Func<T, int, bool>> predicate) where T : class;

        int Count<T>() where T : class;
        int Count<T>(Expression<Func<T, bool>> predicate) where T : class;
        long LongCount<T>() where T : class;
        long LongCount<T>(Expression<Func<T, bool>> predicate) where T : class;
        DbEntityEntry Entry(object entity);
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        IEnumerable<T> FilterEntitiesByEntryStates<T>(IEnumerable<T> collection, params EntityState[] param) where T : class;
        IEnumerable FilterEntitiesByEntryStates(IEnumerable collection, params EntityState[] param);
        IEnumerable<DbEntityEntry<T>> FilterEntriesByEntryStates<T>(IEnumerable<T> collection, params EntityState[] param) where T : class;
        IEnumerable<DbEntityEntry> FilterEntriesByEntryStates(IEnumerable collection, params EntityState[] param);
        DbEntityValidationResult GetValidationError<T>(T entity) where T : class;
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        T GetById<TId, T>(params TId[] ids) where T : class;
        T GetByObject<TInput, T>(TInput entity) where TInput : class where T : class;
        string GetStringFromChangeTracker(bool includeFilterTypes, Type[] filterTypes, bool includeFilterStates, EntityState[] filterStates);
    }
}
