using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CodeflexSystems.SMS.Data
{
    [DataObject]
    internal class UnitOfWork : IUnitOfWork
    {
        #region Properties

        public DbContextConfiguration Configuration
        {
            get { return this.Context.Configuration; }
        }

        public SMSContext Context { get; private set; }
        public ObjectContext ObjectContext { get; private set; }
        public DbSaveOption DbSaveOption { get; set; }
        public bool RememberDbSaveOption { get; set; }
        public string CreatedDatePropertyName { get; set; }
        public string CreatedByPropertyName { get; set; }
        public string ModifiedDatePropertyName { get; set; }
        public string ModifiedByPropertyName { get; set; }

        #endregion

        #region Ctor, Dtor

        public UnitOfWork(SMSContext context = null)
        {
            this.DbSaveOption = DbSaveOption.ClientWins;
            this.CreatedDatePropertyName = "CreatedDate";
            this.CreatedByPropertyName = "CreatedBy";
            this.ModifiedDatePropertyName = "ModifiedDate";
            this.ModifiedByPropertyName = "ModifiedBy";
            if (context == null)
                this.Context = ApplicationManager.Resolve<SMSContext>();
            else
                this.Context = context;
            this.ObjectContext = (this.Context as IObjectContextAdapter).ObjectContext;
        }

        ~UnitOfWork()
        {
            Dispose();
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Saves all the changes in the context.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public virtual int SaveChanges()
        {
            var errors = this.Context.GetValidationErrors();
            if (errors.Count() > 0)
                throw new DbEntityValidationException(SR.GetString("EntityValidationErrors"), errors);

            int result = 0;
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    SetValues();
                    result = InternalSaveChanges();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds a entity to the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to add.</param>
        /// <param name="saveChanges">True to save changes instantly; otherwise, false.</param>
        /// <returns>Returns the entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public virtual T Add<T>(T entity, bool saveChanges = false)
            where T : class
        {
            if (entity == null)
                return null;

            var result = this.Context.Set<T>().Add(entity);
            
            if (saveChanges)
                SaveChanges();

            return result;
        }

        /// <summary>
        /// Adds the collection of entities to the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="saveChanges">True to save changes instantly; otherwise, false.</param>
        /// <returns>Returns the collection of entities.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public virtual IEnumerable<T> AddRange<T>(IEnumerable<T> entities, bool saveChanges = false)
            where T : class
        {
            var result = this.Context.Set<T>().AddRange(entities);

            if (saveChanges)
                SaveChanges();

            return result;
        }

        /// <summary>
        /// Attachs the entity to the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to attach.</param>
        /// <returns>Returns the entity.</returns>
        public virtual T Attach<T>(T entity)
            where T : class
        {
            if (entity == null)
                return null;

            var entry = Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.Context.Set<T>().Attach(entity);
                if (entry.GetDatabaseValues() == null)
                {
                    entry.State = EntityState.Detached;

                    return null;
                }
            }

            return entity;
        }

        /// <summary>
        /// Sets the state of the entity to modify.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to set modified.</param>
        /// <param name="saveChanges">True to save changes instantly; otherwise, false.</param>
        /// <returns>Returns the entity.</returns>
        public virtual T Modify<T>(T entity, bool saveChanges = false)
            where T : class
        {
            if (entity == null)
                return null;

            var entry = Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.Context.Set<T>().Attach(entity);
                if (entry.GetDatabaseValues() == null)
                {
                    entry.State = EntityState.Detached;

                    return null;
                }
            }
            entry.State = EntityState.Modified;

            if (saveChanges)
                SaveChanges();

            return entity;
        }

        /// <summary>
        /// Deletes the entity from the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to remove.</param>
        /// <param name="saveChanges">True to save changes instantly; otherwise, false.</param>
        /// <returns>Returns the entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Delete)]
        public virtual T Remove<T>(T entity, bool saveChanges = false)
            where T : class
        {
            if (entity == null)
                return null;

            var result = this.Context.Set<T>().Remove(entity);

            if (saveChanges)
                SaveChanges();

            return result;
        }

        /// <summary>
        /// Deletes the collection of entities from the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="saveChanges">True to save changes instantly; otherwise, false.</param>
        /// <returns>Returns the collection of entities.</returns>
        [DataObjectMethod(DataObjectMethodType.Delete)]
        public virtual IEnumerable<T> RemoveRange<T>(IEnumerable<T> entities, bool saveChanges = false)
            where T : class
        {
            if (entities == null)
                return null;

            var result = this.Context.Set<T>().RemoveRange(entities);
            
            if (saveChanges)
                SaveChanges();

            return result;
        }

        /// <summary>
        /// Updates an entity in the object context with data from the data source.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="refreshMode">One of the RefreshMode values.</param>
        /// <param name="entity">The entity to be refreshed.</param>
        public virtual void Refresh<T>(RefreshMode refreshMode, T entity)
            where T : class
        {
            if (entity == null)
                return;

            var entry = Entry(entity);
            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                this.ObjectContext.Refresh(refreshMode, entity);
        }

        /// <summary>
        /// Updates an entity in the object context with data from the data source.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to be refreshed.</param>
        public virtual void Refresh<T>(T entity)
            where T : class
        {
            Refresh(RefreshMode.StoreWins, entity);
        }

        /// <summary>
        /// Updates a collection of entities in the object context with data from the data source.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="collection">A collection of entities to be refreshed.</param>
        /// <param name="removeAdded">True to remove the entities that are Added; otherwise, false.</param>
        public virtual void RefreshRange<T>(ICollection<T> collection, bool removeAdded = false)
            where T : class
        {
            RefreshRange(RefreshMode.StoreWins, collection, removeAdded);
        }

        /// <summary>
        /// Updates a collection of entities in the object context with data from the data source.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="refreshMode">One of the RefreshMode values.</param>
        /// <param name="collection">A collection of entities to be refreshed.</param>
        /// <param name="removeAdded">True to remove the entities that are Added; otherwise, false.</param>
        public virtual void RefreshRange<T>(RefreshMode refreshMode, ICollection<T> collection, bool removeAdded = false)
            where T : class
        {
            if (collection == null)
                return;

            if (removeAdded)
            {
                var list = FilterEntriesByEntryStates(collection, EntityState.Added).ToList();
                foreach (var entry in list)
                {
                    entry.State = EntityState.Detached;
                    if (entry.State == EntityState.Detached)
                        collection.Remove(entry.Entity);
                }
            }
            this.ObjectContext.Refresh(refreshMode, FilterEntitiesByEntryStates(collection, EntityState.Modified, EntityState.Deleted));
        }

        /// <summary>
        /// Updates a collection of entities in the object context with data from the original values as they were when last queried from the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="collection">A collection of entities to be refreshed.</param>
        /// <param name="removeAdded">True to remove the entities that are Added; otherwise, false.</param>
        public virtual void RefreshRangeToOriginalValues<T>(ICollection<T> collection, bool removeAdded = false)
            where T : class
        {
            if (collection == null)
                return;

            if (removeAdded)
            {
                var list = FilterEntriesByEntryStates(collection, EntityState.Added).ToList();
                foreach (var entry in list)
                {
                    entry.State = EntityState.Detached;
                    if (entry.State == EntityState.Detached)
                        collection.Remove(entry.Entity);
                }
            }
            foreach (var entry in FilterEntriesByEntryStates(collection, EntityState.Modified, EntityState.Deleted))
                entry.State = EntityState.Unchanged;
        }

        /// <summary>
        /// Updates an entity in the object context with data from the original values as they were when last queried from the database.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to be refreshed.</param>
        public virtual void RefreshToOriginalValues<T>(T entity)
            where T : class
        {
            if (entity == null)
                return;

            var entry = Entry(entity);
            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                entry.State = EntityState.Unchanged;
        }

        /// <summary>
        /// Sets the state of the entity to detach if the state of entity is added.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to set detached.</param>
        public virtual void AddedToDetached<T>(T entity)
            where T : class
        {
            if (entity == null)
                return;

            var entry = Entry(entity);
            if (entry.State == EntityState.Added)
                entry.State = EntityState.Detached;
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>True if the source sequence contains any elements; otherwise, false.</returns>
        public virtual bool Any<T>()
            where T : class
        {
            return this.Context.Set<T>().Any();
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
        public virtual bool Any<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.Context.Set<T>().Any(predicate);
        }

        /// <summary>
        /// Returns the first element of the entities.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T First<T>()
            where T : class
        {
            return this.Context.Set<T>().First();
        }

        /// <summary>
        /// Returns the first element of the entities that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>The first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T First<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().First(predicate);
        }

        /// <summary>
        /// Returns the first element of the entities without caching.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstAsNoTracking<T>()
            where T : class
        {
            return this.Context.Set<T>().AsNoTracking().First();
        }

        /// <summary>
        /// Returns the first element of the entities that satisfies a specified condition without caching.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>The first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstAsNoTracking<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().AsNoTracking().First(predicate);
        }

        /// <summary>
        /// Returns the first element of the entities, or a default value if no element is found.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>default(T) if source is empty; otherwise, the first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstOrDefault<T>()
            where T : class
        {
            return this.Context.Set<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first element of the entities that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>default(T) if source is empty; otherwise, the first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstOrDefault<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Returns the first element of the entities, or a default value if no element is found without caching.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>default(T) if source is empty; otherwise, the first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstOrDefaultAsNoTracking<T>()
            where T : class
        {
            return this.Context.Set<T>().AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first element of the entities that satisfies a specified condition or a default value if no such element is found without caching.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>default(T) if source is empty; otherwise, the first entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T FirstOrDefaultAsNoTracking<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().AsNoTracking().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IOrderedQueryable<T> OrderBy<T, TKey>(Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            return this.Context.Set<T>().OrderBy(keySelector);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by using a specified comparer.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IOrderedQueryable<T> OrderBy<T, TKey>(Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer)
            where T : class
        {
            return this.Context.Set<T>().OrderBy(keySelector, comparer);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IOrderedQueryable<T> OrderByDescending<T, TKey>(Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            return this.Context.Set<T>().OrderByDescending(keySelector);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order by using a specified comparer.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IOrderedQueryable<T> OrderByDescending<T, TKey>(Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer)
            where T : class
        {
            return this.Context.Set<T>().OrderByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Returns a DbSet for the specified type, this allows CRUD operations to be performed for the given entity in the context.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A DbSet instance for the given entity type.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual DbSet<T> SelectAll<T>()
            where T : class
        {
            return this.Context.Set<T>();
        }

        /// <summary>
        /// Returns a new query where the entities returned will not be cached in the DbContext.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A new query with NoTracking applied.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual DbQuery<T> SelectAllAsNoTracking<T>()
            where T : class
        {
            return this.Context.Set<T>().AsNoTracking();
        }
        
        /// <summary>
        /// Filters a sequence of entities based on a predicate.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>A new query that satisfy the condition specified by predicate.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Filters a sequence of entities based on a predicate. Each entity's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each each for a condition; the second parameter of the function represents the index of the entity in the source sequence.</param>
        /// <returns>A new query that satisfy the condition specified by predicate.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IQueryable<T> Where<T>(Expression<Func<T, int, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Filters a sequence of entities based on a predicate without caching.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>A new query with NoTracking applied that satisfy the condition specified by predicate.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IQueryable<T> WhereAsNoTracking<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().AsNoTracking().Where(predicate);
        }

        /// <summary>
        /// Filters a sequence of entities based on a predicate without caching. Each entity's index is used in the logic of the predicate function.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each each for a condition; the second parameter of the function represents the index of the entity in the source sequence.</param>
        /// <returns>A new query with NoTracking applied that satisfy the condition specified by predicate.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual IQueryable<T> WhereAsNoTracking<T>(Expression<Func<T, int, bool>> predicate)
            where T : class
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return this.Context.Set<T>().AsNoTracking().Where(predicate);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose()
        {
            if (this.Context != null)
                this.Context.Dispose();
        }

        /// <summary>
        /// Returns the number of the entites.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The number of entities.</returns>
        public virtual int Count<T>()
            where T : class
        {
            return this.Context.Set<T>().Count();
            //return SelectAllAsNoTracking<T>().Count();
        }

        /// <summary>
        /// Returns the number of the entites based on a predicate.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>The number of entities.</returns>
        public virtual int Count<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.Context.Set<T>().Count(predicate);
            //return WhereAsNoTracking<T>(predicate).Count();
        }

        /// <summary>
        /// Returns the number of the entites.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The number of entities.</returns>
        public virtual long LongCount<T>()
            where T : class
        {
            return this.Context.Set<T>().LongCount();
            //return SelectAllAsNoTracking<T>().LongCount();
        }

        /// <summary>
        /// Returns the number of the entites based on a predicate.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>The number of entities.</returns>
        public virtual long LongCount<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return this.Context.Set<T>().LongCount(predicate);
            //return WhereAsNoTracking(predicate).LongCount();
        }

        /// <summary>
        /// Gets a DbEntityEntry from the object entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An entry for the entity.</returns>
        public virtual DbEntityEntry Entry(object entity)
        {
            return this.Context.Entry(entity);
        }

        /// <summary>
        /// Gets a DbEntityEntry from the entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>An entry for the entity.</returns>
        public virtual DbEntityEntry<T> Entry<T>(T entity)
            where T : class
        {
            return this.Context.Entry(entity);
        }

        /// <summary>
        /// Filters a collection of entities based on EntityStates.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="collection">A collection of entities to filter.</param>
        /// <param name="param">The array of EntityStates.</param>
        /// <returns>>A collection of entities that filtered based on EntityStates.</returns>
        public virtual IEnumerable<T> FilterEntitiesByEntryStates<T>(IEnumerable<T> collection, params EntityState[] param)
            where T : class
        {
            foreach (var entity in collection)
            {
                if (param.Contains(Entry(entity).State))
                    yield return entity;
            }
            yield break;
        }

        /// <summary>
        /// Filters a collection of objects based on EntityStates.
        /// </summary>
        /// <param name="collection">A collection of objects to filter.</param>
        /// <param name="param">The array of EntityStates.</param>
        /// <returns>A collection of objects that filtered based on EntityStates.</returns>
        public virtual IEnumerable FilterEntitiesByEntryStates(IEnumerable collection, params EntityState[] param)
        {
            foreach (var entity in collection)
            {
                if (param.Contains(Entry(entity).State))
                    yield return entity;
            }
            yield break;
        }

        /// <summary>
        /// Filters a collection of entities based on EntityStates.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="collection">A collection of entities to filter.</param>
        /// <param name="param">The array of EntityStates.</param>
        /// <returns>A collection of DbEntityEntry<T> that filtered based on EntityStates.</returns>
        public virtual IEnumerable<DbEntityEntry<T>> FilterEntriesByEntryStates<T>(IEnumerable<T> collection, params EntityState[] param)
            where T : class
        {
            foreach (var entity in collection)
            {
                var entry = Entry(entity);
                if (param.Contains(entry.State))
                    yield return entry;
            }
            yield break;
        }

        /// <summary>
        /// Filters a collection of objects based on EntityStates.
        /// </summary>
        /// <param name="collection">A collection of objects to filter.</param>
        /// <param name="param">The array of EntityStates.</param>
        /// <returns>A collection of DbEntityEntry that filtered based on EntityStates.</returns>
        public virtual IEnumerable<DbEntityEntry> FilterEntriesByEntryStates(IEnumerable collection, params EntityState[] param)
        {
            foreach (var entity in collection)
            {
                var entry = Entry(entity);
                if (param.Contains(entry.State))
                    yield return entry;
            }
            yield break;
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>An entity validation result.</returns>
        public virtual DbEntityValidationResult GetValidationError<T>(T entity)
            where T : class
        {
            if (entity == null)
                return null;

            return this.Context.Entry(entity).GetValidationResult();
        }

        /// <summary>
        /// Gets the validation results of the context.
        /// </summary>
        /// <returns>A collection of validation results for invalid entities.</returns>
        public virtual IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            return this.Context.GetValidationErrors();
        }

        /// <summary>
        /// Gets the entity by the ids.
        /// </summary>
        /// <typeparam name="TId">The ids' types.</typeparam>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="ids">The ids of entity.</param>
        /// <returns>Returns the entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T GetById<TId, T>(params TId[] ids)
            where T : class
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            var type = typeof(T);
            var et = this.ObjectContext.GetEntityType<T>();
            var es = this.ObjectContext.GetEntitySetBase<T, EntitySet>();

            if (et == null || es == null)
                throw new InvalidCastException(SR.GetString("NotEntityOfDbContext", type.FullName));

            if (et.KeyMembers.Count != ids.Length)
                throw new ArgumentException(SR.GetString("Invalid", "primary key"));

            if (!et.KeyMembers.Select(km => km.TypeUsage.EdmType.Name)
                .SequenceEqual(ids.Select(id => id.GetType().Name)))
                throw new ArgumentException(SR.GetString("Invalid", "primary key type"));

            var ekm = new List<EntityKeyMember>();
            for (int i = 0; i < et.KeyMembers.Count; i++)
                ekm.Add(new EntityKeyMember(et.KeyMembers[i].Name, ids[i]));

            var ek = new EntityKey();
            ek.EntityContainerName = this.ObjectContext.DefaultContainerName;
            ek.EntitySetName = es.Name;
            ek.EntityKeyValues = ekm.ToArray();

            object result;
            if (this.ObjectContext.TryGetObjectByKey(ek, out result))
                return (T)result;

            return null;
        }

        /// <summary>
        /// Gets the entity by the object.
        /// </summary>
        /// <typeparam name="TInput">The object type.</typeparam>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The object.</param>
        /// <returns>Returns the entity.</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public virtual T GetByObject<TInput, T>(TInput entity)
            where TInput : class
            where T : class
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var type = entity.GetType();
            var et = this.ObjectContext.GetEntityType<TInput>();
            var es = this.ObjectContext.GetEntitySetBase<TInput, EntitySet>();

            if (et == null || es == null)
                throw new InvalidCastException(SR.GetString("NotEntityOfDbContext", type.FullName));

            if (type.Name != et.Name)
                throw new ArgumentException(SR.GetString("Invalid", "argument type"));

            var ek = this.ObjectContext.CreateEntityKey(es.Name, entity);
            object result;
            if (this.ObjectContext.TryGetObjectByKey(ek, out result))
                return (T)result;

            return null;
        }

        /// <summary>
        /// Gets the string of the DbContext's change tracker based on filter types and filter states.
        /// </summary>
        /// <param name="includeFilterTypes">True to include the filter types; otherwise, false.</param>
        /// <param name="filterTypes">The types of the entities to filter.</param>
        /// <param name="includeFilterStates">True to include the filter states; otherwise, false.</param>
        /// <param name="filterStates">The entity states to filter.</param>
        /// <returns>The string of the DbContext's change tracker.</returns>
        public string GetStringFromChangeTracker(bool includeFilterTypes, Type[] filterTypes, bool includeFilterStates, EntityState[] filterStates)
        {
            StringBuilder sb = new StringBuilder();
            var entities = this.Context.ChangeTracker.Entries();

            sb.AppendFormat("Total entries count : {0}", entities.Count());

            if (filterTypes != null && filterTypes.Length > 0)
            {
                if (includeFilterTypes)
                    entities = entities.Where(m => filterTypes.Contains(ObjectContext.GetObjectType(m.Entity.GetType())));
                else
                    entities = entities.Where(m => !filterTypes.Contains(ObjectContext.GetObjectType(m.Entity.GetType())));
            }

            if (filterStates != null && filterStates.Length > 0)
            {
                if (includeFilterStates)
                    entities = entities.Where(m => filterStates.Contains(m.State));
                else
                    entities = entities.Where(m => !filterStates.Contains(m.State));
            }

            foreach (var entity in entities)
            {
                sb.AppendLine();
                sb.AppendFormat("{0}, {1}, {2}", ObjectContext.GetObjectType(entity.Entity.GetType()),
                    entity.State, entity.Entity.ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region Internal Operations

        private int InternalSaveChanges()
        {
            try
            {
                return this.Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (RememberDbSaveOption)
                {
                    if (this.DbSaveOption == DbSaveOption.ClientWins)
                    {
                        foreach (var entry in ex.Entries)
                            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                    else if (this.DbSaveOption == DbSaveOption.StoreWins)
                    {
                        foreach (var entry in ex.Entries)
                            entry.Reload();
                    }
                    else if (this.DbSaveOption == DbSaveOption.Merges)
                    {
                        foreach (var entry in ex.Entries)
                        {
                            var databaseValues = entry.GetDatabaseValues();
                            var mergedValues = MergeValues(entry.OriginalValues, entry.CurrentValues, databaseValues);

                            entry.OriginalValues.SetValues(databaseValues);
                            entry.CurrentValues.SetValues(mergedValues);
                        }
                    }
                    else
                        throw new Exception(SR.GetString("Invalid", "DbSaveOption value."));

                    return InternalSaveChanges();
                }
                else
                    throw ex;
            }
        }

        private DbPropertyValues MergeValues(DbPropertyValues original, DbPropertyValues current, DbPropertyValues database)
        {
            var result = original.Clone();

            foreach (var propertyName in original.PropertyNames)
            {
                if (original[propertyName] is DbPropertyValues)
                {
                    var mergedComplexValues = MergeValues(
                        (DbPropertyValues)original[propertyName],
                        (DbPropertyValues)current[propertyName],
                        (DbPropertyValues)database[propertyName]);

                    ((DbPropertyValues)result[propertyName]).SetValues(mergedComplexValues);
                }
                else
                {
                    if (!object.Equals(current[propertyName], original[propertyName]))
                        result[propertyName] = current[propertyName];
                    else if (!object.Equals(database[propertyName], original[propertyName]))
                        result[propertyName] = database[propertyName];
                }
            }

            return result;
        }

        private void SetValues()
        {
            foreach (var entry in this.Context.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added))
                SetAddedValues(entry);

            foreach (var entry in this.Context.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Modified))
                SetModifiedValues(entry);
        }

        private void SetAddedValues(DbEntityEntry entry)
        {
            if (entry == null || entry.State != EntityState.Added)
                return;

            var userId = ApplicationManager.Current.UserId;
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                if (entry.CurrentValues[propertyName] is DbPropertyValues)
                    SetAddedValues(entry.ComplexProperty(propertyName).EntityEntry);
                else
                {
                    if (propertyName == this.CreatedDatePropertyName ||
                        propertyName == this.ModifiedDatePropertyName)
                        entry.CurrentValues[propertyName] = DateTime.UtcNow;
                    else if (propertyName == this.CreatedByPropertyName ||
                        propertyName == this.ModifiedByPropertyName)
                        entry.CurrentValues[propertyName] = userId;
                }
            }
        }

        private void SetModifiedValues(DbEntityEntry entry)
        {
            if (entry == null || entry.State != EntityState.Modified)
                return;

            var userId = ApplicationManager.Current.UserId;
            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                if (entry.CurrentValues[propertyName] is DbPropertyValues)
                    SetModifiedValues(entry.ComplexProperty(propertyName).EntityEntry);
                else
                {
                    if (propertyName == this.ModifiedDatePropertyName)
                        entry.CurrentValues[propertyName] = DateTime.UtcNow;
                    else if (propertyName == this.ModifiedByPropertyName)
                        entry.CurrentValues[propertyName] = userId;
                }
            }
        }

        #endregion
    }
}
