using CodeflexSystems.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Extensions
{
    /// <summary>
    /// Extensions for DbContext.
    /// </summary>
    public static class DbContextExtension
    {
        /// <summary>
        /// Get ObjectContext  
        /// </summary>
        /// <param name="context"></param>
        /// <returns>ObjectContext</returns>
        public static ObjectContext ObjectContext(this DbContext context)
        {
            return (context as IObjectContextAdapter).ObjectContext;
        }

        #region EntitySetBase

        public static EntitySet GetEntitySet<T>(this ObjectContext context)
            where T : class
        {
            return context.GetEntitySetBase<EntitySet>(typeof(T).Name);
        }

        public static EntitySet GetEntitySet<T>(this ObjectContext context, T entity)
            where T : class
        {
            return context.GetEntitySetBase<EntitySet>(typeof(T).Name);
        }

        /// <summary>
        /// Returns an EntityContainer object by using the specified entity container name and the data model.
        /// <see cref="System.Data.Metadata.Edm.MetadataWorkspace.GetEntityContainer"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"><see cref="System.Data.Entity.Core.Objects.ObjectContext"/></param>
        /// <returns>EntitySetBase</returns>      
        public static TResult GetEntitySetBase<T, TResult>(this ObjectContext context)
            where T : class
            where TResult : EntitySetBase
        {
            return context.GetEntitySetBase<TResult>(typeof(T).Name);
        }

        /// <summary>
        /// Returns an EntityContainer object by using the specified entity container name and the data model.
        /// <see cref="System.Data.Metadata.Edm.MetadataWorkspace.GetEntityContainer"/> 
        /// </summary>
        /// <param name="context"><see cref="System.Data.Entity.Core.Objects.ObjectContext"/></param>
        /// <param name="elementTypeName"></param>
        /// <returns>EntitySetBase</returns>
        public static T GetEntitySetBase<T>(this ObjectContext context, string elementTypeName)
            where T : EntitySetBase
        {
            return context.GetEntitySetBases<T>().FirstOrDefault(item => item.ElementType.Name.Equals(elementTypeName));
        }

        /// <summary>
        /// Returns an EntityContainer object by using the specified entity container name and the data model.
        /// <see cref="System.Data.Metadata.Edm.MetadataWorkspace.GetEntityContainer"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"><see cref="System.Data.Entity.Core.Objects.ObjectContext"/></param>
        /// <returns>EntityContainer</returns>
        public static IEnumerable<T> GetEntitySetBases<T>(this ObjectContext context)
            where T : EntitySetBase
        {
            var container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);

            return container.BaseEntitySets.OfType<T>();
        }

        #endregion

        #region EntityTypeBase

        public static EntityType GetEntityType<T>(this ObjectContext context)
            where T : class
        {
            return context.GetEntityType(typeof(T));
        }

        public static EntityType GetEntityType<T>(this ObjectContext context, T entity)
            where T : class
        {
            return context.GetEntityType(typeof(T));
        }

        public static EntityType GetEntityType(this ObjectContext context, Type type)
        {
            var name = type.FullNameWithNesting();
            var oSpaceType = context.GetEntityTypies().FirstOrDefault(t => t.FullName == name);

            if (oSpaceType == null)
                return null;

            return (EntityType)context.MetadataWorkspace.GetEdmSpaceType(oSpaceType);
        }

        public static IEnumerable<EntityType> GetEntityTypies(this ObjectContext context)
        {
            return context.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace);
        }

        /// <summary>
        /// Gets item in the specified data model.
        /// <see cref="System.Data.Metadata.Edm.MetadataWorkspace.GetItems"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"></param>
        /// <returns>EntityTypeBase</returns>
        public static TResult GetEntityTypeBase<T, TResult>(this ObjectContext context)
            where T : class
            where TResult : EntityTypeBase
        {
            var typeName = typeof(T).Name;

            return context.GetEntityTypeBases<TResult>().FirstOrDefault(em => em.Name == typeName);
        }

        /// <summary>
        /// Gets all the items in the specified data model.
        /// <see cref="System.Data.Metadata.Edm.MetadataWorkspace.GetItems"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns>ReadOnlyCollection<EntityTypeBase></EntityTypeBase><see cref=" System.Collections.ObjectModel.ReadOnlyCollection"/></returns>
        public static IEnumerable<T> GetEntityTypeBases<T>(this ObjectContext context)
            where T : EntityTypeBase
        {
            return context.MetadataWorkspace.GetItems<T>(DataSpace.CSpace);
        }

        /// <summary>
        /// Return true if EntityTypeBase is equal with given ObjectContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="context"></param>
        /// <returns>bool</returns>
        public static bool IsEntityTypeBase<T, TTarget>(this ObjectContext context)
            where T : class
            where TTarget : EntityTypeBase
        {
            var typeName = typeof(T).Name;

            return context.GetEntityTypeBases<TTarget>().Any(item => item.Name.Equals(typeName));
        }

        #endregion
    }
}
