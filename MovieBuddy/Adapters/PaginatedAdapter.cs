using System.Collections.Generic;

namespace MovieBuddy
{
    public abstract class PaginatedAdapter<T> : AdapterBase, IPaginatedAdapter<T>
    {
        public void LoadData(List<T> data)
        {
            AddToCollection(data);
            NotifyDataSetChanged();
        }

        protected virtual void AddToCollection(List<T> data) { }
    }
}