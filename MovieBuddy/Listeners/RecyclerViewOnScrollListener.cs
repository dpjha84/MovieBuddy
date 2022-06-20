using Android.Support.V4.Widget;
using Android.Views;
using System;

namespace MovieBuddy
{
    public class RecyclerViewOnScrollListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;
        private readonly int currentPage = 0;

        public void OnScrollChange(NestedScrollView scrollView, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            View view = scrollView.GetChildAt(scrollView.ChildCount - 1);
            int diff = (view.Bottom - (scrollView.Height + scrollView.ScrollY));
            if (diff == 0)
            {
                LoadMoreEvent(currentPage, null);
            }
        }
    }
}