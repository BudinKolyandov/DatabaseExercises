namespace Orm
{
    using System.Linq;
    using System.Collections.Generic;
    using System;

    internal class ChangeTracker<T>
        where T : class, new()
    {
        private readonly List<T> allEntities;

        private readonly List<T> added;

        private readonly List<T> deleted;

        public ChangeTracker(IEnumerable<T> entities)
        {
            this.added = new List<T>();
            this.deleted = new List<T>();

            this.allEntities = CloneEntites(entities);
        }

        public IReadOnlyCollection<T> AllEntites => this.allEntities.AsReadOnly();

        public IReadOnlyCollection<T> Added => this.added.AsReadOnly();

        public IReadOnlyCollection<T> Deleted => this.deleted.AsReadOnly();

        public void Add(T item) => this.added.Add(item);

        public void Delete(T item) => this.deleted.Add(item);

        private List<T> CloneEntites(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            var propertiesToClone = typeof(T).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            foreach (var entity in entities)
            {
                var clonedEntity = Activator.CreateInstance<T>();

                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(clonedEntity, value);
                }
                clonedEntities.Add(clonedEntity);
            }
            return clonedEntities;
        }
    }
}
