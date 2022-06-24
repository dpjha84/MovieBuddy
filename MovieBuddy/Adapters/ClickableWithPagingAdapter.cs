using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public abstract class ClickableWithPagingAdapter<T> : AdapterBase, IClickableWithPagingAdapter<T>
    {
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> YoutubeClick;
        public virtual void LoadData(List<T> data)
        {
            AddToCollection(data);
            NotifyDataSetChanged();
        }
        public virtual void LoadVideos(List<string> data)
        {
            AddVideosToCollection(data);
            NotifyDataSetChanged();
        }

        public void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public void OnYoutubeClick(int position)
        {
            YoutubeClick?.Invoke(this, position);
        }


        protected abstract void AddToCollection(List<T> data);

        protected virtual void AddVideosToCollection(List<string> data) { }
    }
}