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
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class CardAdapter2 : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public ObservableCollection<UserInfoObject> UsersDateList = new ObservableCollection<UserInfoObject>();

        private readonly Activity ActivityContext;

        public CardAdapter2(Activity context)
        {
            ActivityContext = context;
        }

        public UserInfoObject GetItem(int position)
        {
            return UsersDateList[position];
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is CardAdapter2ViewHolder holder)
                {
                    var item = UsersDateList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.ImgUser, ImageStyle.CenterCrop, ImagePlaceholders.Drawable); 
                        //holder.CountImage.Text = item.Mediafiles.Count.ToString();

                        if (item.Age != null)
                        {
                            holder.TxtName.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 20) + " , " + item.Age;
                        }
                        else if (!string.IsNullOrEmpty(item.Birthday) && item.Birthday != "0000-00-00" && item.Birthday != "0")
                        {
                            //1997-05-28 
                            var units = item.Birthday.Split('-');

                            var year = Convert.ToInt32(units[0]);
                            if (units[1][0] == '0')
                                units[1] = units[1][1].ToString();

                            var month = Convert.ToInt32(units[1]);

                            if (units[2][0] == '0')
                                units[2] = units[2][1].ToString();

                            var day = Convert.ToInt32(units[2]);

                            DateTime now = DateTime.Now;
                            DateTime birthday = new DateTime(year,month, day);
                            int age = now.Year - birthday.Year;
                            if (now < birthday.AddYears(age)) age--;

                            holder.TxtName.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 20) + " , " + age;
                        }
                        else
                            holder.TxtName.Text = Methods.FunString.SubStringCutOf(QuickDateTools.GetNameFinal(item), 20); 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SuggestionsView
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.match_card_style, parent, false);
                var vh = new CardAdapter2ViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        void Click(CardAdapter2ClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(CardAdapter2ClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

        public event EventHandler<CardAdapter2ClickEventArgs> OnItemClick;
        public event EventHandler<CardAdapter2ClickEventArgs> OnItemLongClick;

        public override int ItemCount => UsersDateList?.Count ?? 0;

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
            return Glide.With(ActivityContext).Load(p0.ToString()).Apply(new RequestOptions().CenterCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class CardAdapter2ViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public ImageView ImgUser { get; private set; }
        public TextView TxtName { get; private set; }
        //public TextView ImgIcon { get; set; }
        //public TextView CountImage { get; set; }

        #endregion

        public CardAdapter2ViewHolder(View itemView, Action<CardAdapter2ClickEventArgs> clickListener, Action<CardAdapter2ClickEventArgs> longClickListener) : base(itemView)
        {
            try { 
                ImgUser = itemView.FindViewById<ImageView>(Resource.Id.imgUser);
                TxtName = itemView.FindViewById<TextView>(Resource.Id.txtName);
                //ImgIcon = itemView.FindViewById<TextView>(Resource.Id.imgIcon);
                //CountImage = itemView.FindViewById<TextView>(Resource.Id.countImage);

                //FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ImgIcon, IonIconsFonts.Camera);

                //Event
                itemView.Click += (sender, e) => clickListener(new CardAdapter2ClickEventArgs { View = itemView, Position = AdapterPosition, Image = ImgUser });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class CardAdapter2ClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public ImageView Image { get; set; }

    }
}