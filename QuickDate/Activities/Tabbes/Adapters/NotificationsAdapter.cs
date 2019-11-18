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
using QuickDateClient.Classes.Common;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class NotificationsAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<NotificationsAdapterClickEventArgs> OnItemClick;
        public event EventHandler<NotificationsAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        public ObservableCollection<GetNotificationsObject.Datum> NotificationsList = new ObservableCollection<GetNotificationsObject.Datum>();

        public NotificationsAdapter(Activity context)
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
                //Setup your layout here >> Notifications_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_NotificationsView, parent, false);
                var vh = new NotificationsAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is NotificationsAdapterViewHolder holder)
                {
                    var item = NotificationsList[position];
                    if (item != null)
                    {
                        holder.UserNameNoitfy.Text = QuickDateTools.GetNameFinal(item.Notifier);  
                         
                        GlideImageLoader.LoadImage(ActivityContext,item.Notifier.Avater, holder.ImageUser, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        if (item.Type == "got_new_match")
                        {
                            holder.CircleIcon.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn3);
                            if (holder.IconNotify.Text != IonIconsFonts.IosHeart)
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconNotify, IonIconsFonts.IosHeart);
                        }
                        else if (item.Type == "like")
                        {
                            holder.CircleIcon.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn2);
                            if (holder.IconNotify.Text != IonIconsFonts.HeartBroken)
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconNotify, IonIconsFonts.HeartBroken);
                        }
                        else if (item.Type == "visit")
                        {
                            holder.CircleIcon.SetBackgroundResource(Resource.Drawable.Shape_Radius_Gradient_Btn);
                            if (holder.IconNotify.Text != IonIconsFonts.IosEye)
                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.IconNotify, IonIconsFonts.IosEye);
                        }
                         
                        holder.Description.Text = QuickDateTools.GetNotificationsText(item.Type);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => NotificationsList?.Count ?? 0;

        public GetNotificationsObject.Datum GetItem(int position)
        {
            return NotificationsList[position];
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

        void Click(NotificationsAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(NotificationsAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = NotificationsList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Notifier.Avater != "")
                {
                    d.Add(item.Notifier.Avater);
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

    public class NotificationsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public ImageView ImageUser { get; set; }
        public View CircleIcon { get; set; }
        public TextView IconNotify { get; set; }
        public TextView UserNameNoitfy { get; set; }
        public TextView Description { get; set; }

        #endregion

        public NotificationsAdapterViewHolder(View itemView, Action<NotificationsAdapterClickEventArgs> clickListener, Action<NotificationsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                ImageUser = (ImageView)MainView.FindViewById(Resource.Id.ImageUser);
                CircleIcon = MainView.FindViewById<View>(Resource.Id.CircleIcon);
                IconNotify = (TextView)MainView.FindViewById(Resource.Id.IconNotifications);
                UserNameNoitfy = (TextView)MainView.FindViewById(Resource.Id.NotificationsName);
                Description = (TextView)MainView.FindViewById(Resource.Id.NotificationsText);

                FontUtils.SetFont(UserNameNoitfy, Fonts.SfRegular);
                FontUtils.SetFont(Description, Fonts.SfMedium);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = AdapterPosition , Image = ImageUser });
                itemView.LongClick += (sender, e) => longClickListener(new NotificationsAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = ImageUser });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class NotificationsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }
}