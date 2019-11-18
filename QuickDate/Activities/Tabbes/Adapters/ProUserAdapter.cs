using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using Qintong.Library;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class ProUserAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<ProUserAdapterClickEventArgs> OnItemClick;
        public event EventHandler<ProUserAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<UserInfoObject> ProUserList = new ObservableCollection<UserInfoObject>();

        public ProUserAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
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
                //Setup your layout here >> Style_StoryView
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_StoryView, parent, false);
                var vh = new ProUserAdapterViewHolder(itemView, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            { 
                if (viewHolder is ProUserAdapterViewHolder holder)
                {
                    var item = ProUserList[position];
                    if (item != null)
                    {
                        holder.Circleindicator.Status = InsLoadingView.Statuses.Loading;
                        holder.Circleindicator.SetStartColor(Color.ParseColor(AppSettings.StartColor));
                        holder.Circleindicator.SetEndColor(Color.ParseColor(AppSettings.EndColor));

                        GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Circleindicator, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 20);

                        if (item.Type != "Your")
                        {
                            holder.Circleindicator.Status = InsLoadingView.Statuses.Unclicked;
                            holder.IconCircle.Visibility = ViewStates.Gone;
                            holder.IconStory.Visibility = ViewStates.Gone;
                        }

                        if (!holder.Circleindicator.HasOnClickListeners)
                            holder.Circleindicator.Click += (sender, e) => Click(new ProUserAdapterClickEventArgs { View = holder.MainView, Position = position, Image = holder.Circleindicator });

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        public override int ItemCount => ProUserList?.Count ?? 0;
         

        public UserInfoObject GetItem(int position)
        {
            return ProUserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public void Click(ProUserAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);

        void LongClick(ProUserAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = ProUserList[p0];

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

    public class ProUserAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public TextView Name { get; set; }
        public InsLoadingView Circleindicator { get; set; }
        public View IconCircle { get; set; }
        public TextView IconStory { get; set; }

        #endregion

        public ProUserAdapterViewHolder(View itemView, Action<ProUserAdapterClickEventArgs> longClickListener) :base(itemView)
        {
            try
            {
                MainView = itemView;

                Name = MainView.FindViewById<TextView>(Resource.Id.Txt_Username);
                Circleindicator = MainView.FindViewById<InsLoadingView>(Resource.Id.profile_indicator);
                IconCircle = MainView.FindViewById<View>(Resource.Id.IconCircle);
                IconStory = MainView.FindViewById<TextView>(Resource.Id.IconStory);

                //Dont Remove this code #####
                FontUtils.SetFont(Name, Fonts.SfRegular);
                //#####


                //Event
                //itemView.Click += (sender, e) => clickListener(new ProUserAdapterClickEventArgs { View = itemView, Position = AdapterPosition }); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class ProUserAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }
}