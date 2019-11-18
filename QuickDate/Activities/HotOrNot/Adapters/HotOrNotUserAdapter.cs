using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.HotOrNot.Adapters
{
    public class HotOrNotUserAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        public ObservableCollection<UserInfoObject> UsersDateList = new ObservableCollection<UserInfoObject>();
        public event EventHandler<HotOrNotUserAdapterClickEventArgs> HotItemClick;
        public event EventHandler<HotOrNotUserAdapterClickEventArgs> NotItemClick;
        public event EventHandler<HotOrNotUserAdapterClickEventArgs> OnItemClick;
        public event EventHandler<HotOrNotUserAdapterClickEventArgs> OnItemLongClick;

        #endregion

        public HotOrNotUserAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                GetFavorites();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => UsersDateList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HotOrNotView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HotOrNotView, parent, false);
                var vh = new HotOrNotUserAdapterViewHolder(itemView, NotClick, HotClick, Click, LongClick);
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
                if (viewHolder is HotOrNotUserAdapterViewHolder holder)
                {
                    var item = UsersDateList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Image, ImageStyle.FitCenter, ImagePlaceholders.Drawable);

                        holder.Name.Text = QuickDateTools.GetNameFinal(item);

                        if (!holder.Image.HasOnClickListeners) 
                        {
                            holder.Image.Click += (sender, args) => { QuickDateTools.OpenProfile(ActivityContext, "Close", item,holder.Image); };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void GetFavorites()
        {
            try
            {
                UsersDateList = ListUtils.FavoriteUserList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public UserInfoObject GetItem(int position)
        {
            return UsersDateList[position];
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

        public void HotClick(HotOrNotUserAdapterClickEventArgs args)
        {
            HotItemClick?.Invoke(this, args);
        }
        public void NotClick(HotOrNotUserAdapterClickEventArgs args)
        {
            NotItemClick?.Invoke(this, args);
        }
        public void Click(HotOrNotUserAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public void LongClick(HotOrNotUserAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UsersDateList[p0];

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

    public class HotOrNotUserAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; }
        public ImageView Image { get; private set; } 
        public TextView Name { get; private set; }
        public CircleButton CloseButton { get; private set; }
        public CircleButton LikeButton { get; private set; }

        #endregion

        public HotOrNotUserAdapterViewHolder(View itemView, Action<HotOrNotUserAdapterClickEventArgs> NotClickListener,Action<HotOrNotUserAdapterClickEventArgs> HotClickListener, Action<HotOrNotUserAdapterClickEventArgs> clickListener, Action<HotOrNotUserAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.imgUser); 
                Name = MainView.FindViewById<TextView>(Resource.Id.txtName); 
                CloseButton = MainView.FindViewById<CircleButton>(Resource.Id.closebutton1);
                LikeButton = MainView.FindViewById<CircleButton>(Resource.Id.likebutton2);

                //Dont Remove this code #####
                FontUtils.SetFont(Name, Fonts.SfRegular);
                //#####

                LikeButton.Click += (sender, e) => HotClickListener(new HotOrNotUserAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
                CloseButton.Click += (sender, e) => NotClickListener(new HotOrNotUserAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
                itemView.Click += (sender, e) => clickListener(new HotOrNotUserAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
                itemView.LongClick += (sender, e) => longClickListener(new HotOrNotUserAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class HotOrNotUserAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }
    }

    public class HotOrNotUsersClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public UserInfoObject UserClass { get; set; }
        public HotOrNotUserAdapterViewHolder Holder { get; set; }
    }
}