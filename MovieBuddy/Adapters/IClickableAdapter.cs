using System;

namespace MovieBuddy
{
    public interface IClickableAdapter
    {
        event EventHandler<int> ItemClick;

        void OnClick(int position);
    }
}