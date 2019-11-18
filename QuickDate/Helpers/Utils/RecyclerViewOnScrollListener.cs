using System;
using Android.Support.V7.Widget;

namespace QuickDate.Helpers.Utils
{
    public class RecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);

        public event LoadMoreEventHandler LoadMoreEvent;

        public bool IsLoading { get; set; }
        private readonly dynamic LayoutManager;
        private readonly int SpanCount;

        public RecyclerViewOnScrollListener(dynamic layoutManager, int spanCount = 0)
        {
            try
            {
                IsLoading = false;
                LayoutManager = layoutManager;
                SpanCount = spanCount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            try
            {
                base.OnScrolled(recyclerView, dx, dy);

                var visibleItemCount = recyclerView.ChildCount;
                var totalItemCount = recyclerView.GetAdapter().ItemCount;

                dynamic pastVisibleItems;
                if (LayoutManager is LinearLayoutManager managerLinear)
                {
                    pastVisibleItems = managerLinear.FindFirstVisibleItemPosition();
                }
                else if (LayoutManager is GridLayoutManager managerGrid)
                {
                    pastVisibleItems = managerGrid.FindFirstVisibleItemPosition();
                }
                else if (LayoutManager is StaggeredGridLayoutManager managerStaggeredGrid)
                {
                    int[] firstVisibleItemPositions = new int[SpanCount];
                    pastVisibleItems = managerStaggeredGrid.FindFirstVisibleItemPositions(firstVisibleItemPositions)[0];
                }
                else
                {
                    pastVisibleItems = LayoutManager.FindFirstVisibleItemPosition();
                }

                if (visibleItemCount + pastVisibleItems + 4 < totalItemCount)
                    return;

                if (IsLoading) //&& !recyclerView.CanScrollVertically(1)
                    return;

                IsLoading = true;
                LoadMoreEvent?.Invoke(this, null);

                //if (visibleItemCount + pastVisibleItems + 8 >= totalItemCount)
                //{
                //    //Load More  from API Request
                //    if (IsLoading == false)
                //    {
                //        LoadMoreEvent?.Invoke(this, null);
                //        IsLoading = true;
                //    }
                //}
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}