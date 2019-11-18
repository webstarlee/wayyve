using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Liaoinstan.SpringViewLib.Containers;

namespace QuickDate.Helpers.PullSwipeStyles
{
    public class DefaultFooter : BaseFooter
    {
        private readonly Context MainContext;
        private readonly AnimationDrawable AnimationLoading;

        private readonly int[] LoadingAnimSrcs = new[] { Resource.Drawable.mt_loading01, Resource.Drawable.mt_loading02 };

        private TextView Title;
        //public ProgressBar ProgressBarView;
        private ImageView ImageIcon;

        public DefaultFooter(Activity context)
        {
            MainContext = context;
            AnimationLoading = new AnimationDrawable();
            foreach (var src in LoadingAnimSrcs)
            {
                AnimationLoading.AddFrame(ContextCompat.GetDrawable(context, src), 150);
                AnimationLoading.OneShot = (false);
            }
        }

        public override View GetView(LayoutInflater inflater, ViewGroup viewGroup)
        {
            View view = inflater.Inflate(Resource.Layout.PullRefreshFooter, viewGroup, true);
            //ProgressBarView = view.FindViewById<ProgressBar>(Resource.Id.default_header_progressbar);
            Title = view.FindViewById<TextView>(Resource.Id.default_header_title);
            ImageIcon = view.FindViewById<ImageView>(Resource.Id.default_header_arrow);
            if (AnimationLoading != null)
            {
                ImageIcon.SetImageDrawable(AnimationLoading);
            }

            return view;
        }

        public override void OnDropAnim(View rootView, int dy)
        {

        }

        public override void OnLimitDes(View rootView, bool upORdown)
        {
            if (!upORdown)
            {
                Title.Text = MainContext.GetText(Resource.String.Lbl_PullToRefresh);
            }
            else
            {
                Title.Text = MainContext.GetText(Resource.String.Lbl_RefreshMyNewsFeed);
            }
        }

        public override void OnPreDrag(View rootView)
        {
            AnimationLoading.Stop();
            if (AnimationLoading != null && AnimationLoading.NumberOfFrames > 0)
            {
                ImageIcon.SetImageDrawable(AnimationLoading.GetFrame(0));
            }
        }

        public override void OnStartAnim()
        {
            Title.Text = MainContext.GetText(Resource.String.Lbl_Refreshing);
            if (AnimationLoading != null)
            {
                ImageIcon.SetImageDrawable(AnimationLoading);
                AnimationLoading.Start();
            }
        }

        public override void OnFinishAnim()
        {
            AnimationLoading.Stop();
            if (AnimationLoading != null && AnimationLoading.NumberOfFrames > 0)
            {
                ImageIcon.SetImageDrawable(AnimationLoading.GetFrame(0));
            }
        }
    }
} 