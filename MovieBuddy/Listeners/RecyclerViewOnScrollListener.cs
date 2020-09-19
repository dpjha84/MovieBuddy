using System;
using Android.Support.V4.Widget;
using Android.Views;

namespace MovieBuddy
{
    public class RecyclerViewOnScrollListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;
        private int currentPage = 0;

        public void OnScrollChange(NestedScrollView scrollView, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            View view = (View)scrollView.GetChildAt(scrollView.ChildCount - 1);
            int diff = (view.Bottom - (scrollView.Height + scrollView.ScrollY));
            if (diff == 0)
            {
                LoadMoreEvent(currentPage, null);
            }
        }
    }
}