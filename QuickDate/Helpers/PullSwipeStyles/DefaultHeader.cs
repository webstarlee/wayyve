using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Liaoinstan.SpringViewLib.Containers;

namespace QuickDate.Helpers.PullSwipeStyles
{
    public class DefaultHeader : BaseHeader
    {
        public Context MainContext;
        public RotateAnimation RotateUpAnim;
        public RotateAnimation RotateDownAnim;

        public TextView Title;
        public ProgressBar ProgressBarView;
        public ImageView ImageIcon;

        public DefaultHeader(Activity context)
        {
            MainContext = context;
            RotateUpAnim = new RotateAnimation(0.0f, -180.0f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f) { Duration = 180, FillAfter = true };
            RotateDownAnim = new RotateAnimation(-180.0f, 0.0f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f) { Duration = 180, FillAfter = true };
        }

        public override View GetView(LayoutInflater inflater, ViewGroup viewGroup)
        {
            View view = inflater.Inflate(Resource.Layout.PullRefreshHeader, viewGroup, true);
            ProgressBarView = view.FindViewById<ProgressBar>(Resource.Id.default_header_progressbar);
            Title = view.FindViewById<TextView>(Resource.Id.default_header_title);
            ImageIcon = view.FindViewById<ImageView>(Resource.Id.default_header_arrow);

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
                if (ImageIcon.Visibility == ViewStates.Visible)
                    ImageIcon.StartAnimation(RotateUpAnim);
            }
            else
            {

                Title.Text = MainContext.GetText(Resource.String.Lbl_RefreshMyNewsFeed);
                if (ImageIcon.Visibility == ViewStates.Visible)
                    ImageIcon.StartAnimation(RotateDownAnim);
            }
        }

        public override void OnPreDrag(View rootView)
        {

        }

        public override void OnStartAnim()
        {
            Title.Text = MainContext.GetText(Resource.String.Lbl_Refreshing);
            ImageIcon.Visibility = ViewStates.Invisible;
            ImageIcon.ClearAnimation();
            ProgressBarView.Visibility = ViewStates.Visible;
        }

        public override void OnFinishAnim()
        {
            ImageIcon.Visibility = ViewStates.Visible;
            ProgressBarView.Visibility = ViewStates.Invisible;
        }
    }
}