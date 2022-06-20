using System;

namespace MovieBuddy
{
    public abstract class ClickableAdapter : AdapterBase, IClickableAdapter
    {
        public event EventHandler<int> ItemClick;

        public void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}