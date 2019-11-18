using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using Refractored.Controls;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Favorite.Adapters
{
    public class FavoriteUsersAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        #region Variables Basic

        private readonly Activity ActivityContext;
        private readonly HomeActivity HomeActivity;
        public ObservableCollection<UserInfoObject> FavoritesList = new ObservableCollection<UserInfoObject>();
        public event EventHandler<FavoriteUsersAdapterClickEventArgs> OnItemClick;
        public event EventHandler<FavoriteUsersAdapterClickEventArgs> OnItemLongClick;

        #endregion

        public FavoriteUsersAdapter(Activity context, HomeActivity homeActivity)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                HomeActivity = homeActivity;
                GetFavorites();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => FavoritesList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_FavoriteView
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_FavoriteView, parent, false);
                var vh = new FavoriteUsersAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is FavoriteUsersAdapterViewHolder holder)
                {
                    var item = FavoritesList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext,item.Avater, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.ImageOnline.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online)? ViewStates.Visible: ViewStates.Gone;

                        holder.Name.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 14);

                        if (!holder.Button.HasOnClickListeners)
                            holder.Button.Click += (sender, e) => FavoriteButtonClick(new FavoriteUsersClickEventArgs { View = holder.ItemView, UserClass = item, Position = position, ButtonFollow = holder.Button });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FavoriteButtonClick(FavoriteUsersClickEventArgs e)
        {
            try
            {
                if (e.UserClass != null)
                { 
                    var index = FavoritesList.IndexOf(FavoritesList.FirstOrDefault(a => a.Id == e.UserClass.Id));
                    if (index != -1)
                    {
                        FavoritesList.Remove(e.UserClass);
                        NotifyItemRemoved(index);
                        NotifyItemRangeRemoved(0, ItemCount);
                    }

                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.Remove_Favorite(e.UserClass);
                    sqlEntity.Dispose();

                  var countList =  HomeActivity?.ProfileFragment?.FavoriteFragment?.MAdapter?.ItemCount;
                  if (countList == 0)
                  {
                      HomeActivity?.ProfileFragment?.FavoriteFragment?.ShowEmptyPage();
                  }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        public void GetFavorites()
        {
            try
            {
                FavoritesList = ListUtils.FavoriteUserList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public UserInfoObject GetItem(int position)
        {
            return FavoritesList[position];
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

        public void Click(FavoriteUsersAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public void LongClick(FavoriteUsersAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = FavoritesList[p0];

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

    public class FavoriteUsersAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; }
        public ImageView Image { get; private set; }
        public CircleImageView ImageOnline { get; private set; }

        public TextView Name { get; private set; }
        public TextView LastTimeOnline { get; private set; }
        public Button Button { get; private set; }
        
        #endregion

        public FavoriteUsersAdapterViewHolder(View itemView, Action<FavoriteUsersAdapterClickEventArgs> clickListener, Action<FavoriteUsersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<Button>(Resource.Id.btn_UnFavorite);
                 
                //Dont Remove this code #####
                FontUtils.SetFont(Name, Fonts.SfRegular);
                FontUtils.SetFont(LastTimeOnline, Fonts.SfMedium);
                FontUtils.SetFont(Button, Fonts.SfRegular);
                //#####

                itemView.Click += (sender, e) => clickListener(new FavoriteUsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
                itemView.LongClick += (sender, e) => longClickListener(new FavoriteUsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Image = Image });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class FavoriteUsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; } 
        public ImageView Image { get; set; } 
    }

    public class FavoriteUsersClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public UserInfoObject UserClass { get; set; }
        public Button ButtonFollow { get; set; }
    }
}