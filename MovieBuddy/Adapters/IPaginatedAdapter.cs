using System.Collections.Generic;

namespace MovieBuddy
{
    public interface IPaginatedAdapter<T>
    {
        void LoadData(List<T> data);
    }
}