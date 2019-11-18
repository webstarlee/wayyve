using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using Refractored.Controls;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class NearByAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<SuggestionsAdapterClickEventArgs> OnItemClick;
        public event EventHandler<SuggestionsAdapterClickEventArgs> OnItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<UserInfoObject> NearByList = new ObservableCollection<UserInfoObject>();

        public NearByAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                //HasStableIds = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SuggestionsView
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_SuggestionsView, parent, false);
                var vh = new SuggestionsAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is SuggestionsAdapterViewHolder holder)
                {
                    var item = NearByList[position];
                    if (item != null)
                    {
                       
                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 10);

                        GlideImageLoader.LoadImage(ActivityContext,item.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.ImageOnline.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online) ? ViewStates.Visible : ViewStates.Gone;

                        holder.Verified.Visibility = item.VerifiedFinal ? ViewStates.Visible : ViewStates.Gone;

                        if (position == 1)
                        {
                            var height = holder.MainLayout.LayoutParameters.Height;
                            if (height <= 450)
                                holder.MainLayout.LayoutParameters.Height = height + 150;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public override int ItemCount => NearByList?.Count ?? 0;

        public UserInfoObject GetItem(int position)
        {
            return NearByList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        void Click(SuggestionsAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(SuggestionsAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = NearByList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avater != "")
                {
                    d.Add(item.Avater);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class SuggestionsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }
        public ImageView Image { get; set; }
        public CircleImageView ImageOnline { get; set; }
        public TextView Name { get; set; }
        public TextView Verified { get; set; }
        public LinearLayout MainLayout { get; set; }

        #endregion

        public SuggestionsAdapterViewHolder(View itemView, Action<SuggestionsAdapterClickEventArgs> clickListener, Action<SuggestionsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                MainLayout = MainView.FindViewById<LinearLayout>(Resource.Id.mainlayout);
                Image = MainView.FindViewById<ImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                Verified = MainView.FindViewById<TextView>(Resource.Id.verified);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, Verified, IonIconsFonts.CheckmarkCircled);

                //Event
                itemView.Click += (sender, e) => clickListener(new SuggestionsAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class SuggestionsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }

    } 
}