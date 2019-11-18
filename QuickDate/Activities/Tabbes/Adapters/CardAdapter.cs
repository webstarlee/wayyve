using System;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Global;

namespace QuickDate.Activities.Tabbes.Adapters
{
    public class CardAdapter : BaseAdapter
    {
        public ObservableCollection<UserInfoObject> UsersDateList = new ObservableCollection<UserInfoObject>();

        private readonly Activity ActivityContext;

        public CardAdapter(Activity context)
        {
            ActivityContext = context;
        }
  
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            { 
                var view = convertView;
                UserInfoObject item = UsersDateList[position]; 
                if (item == null) return view; 
                CardAdapterViewHolder holder = null;

                if (view != null)
                    holder = view.Tag as CardAdapterViewHolder;

                if (holder == null)
                { 
                    var inflater = ActivityContext.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                    view = inflater.Inflate(Resource.Layout.Style_DatingCardview, parent, false);

                    string name = QuickDateTools.GetNameFinal(item);

                    string city = !string.IsNullOrEmpty(item.City) ? item.City : ActivityContext.GetText(Resource.String.Lbl_Unknown);

                    holder = new CardAdapterViewHolder(view)
                    { 
                        Name = { Text = name },
                        City = { Text = city }
                    };

                    Glide.With(ActivityContext).Load(item.Avater).Apply(new RequestOptions()).Into(holder.Image);

                    //GlideImageLoader.LoadImage(ActivityContext, item.Avater, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable,false);

                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.Status, IonIconsFonts.Record); 
                    holder.Status.SetTextColor(Color.Green);
 
                    holder.Status.Visibility = QuickDateTools.GetStatusOnline(item.Lastseen, item.Online) ? ViewStates.Visible : ViewStates.Gone;

                    if (!holder.MainWhiteBox.HasOnClickListeners)
                    {
                        holder.MainWhiteBox.Click += delegate
                        {
                            QuickDateTools.OpenProfile(ActivityContext, "LikeAndMoveCardMach", item, holder.Image); 
                        };
                    }
                     
                    view.Tag = holder;
                }

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;

            }
        }

        //Fill in count here, currently 0
        public override int Count
        {
            get
            {
                if (UsersDateList != null)
                {
                    return UsersDateList.Count;
                }
                else
                {
                    return 0;
                }
            }
        } 
    }

    public class CardAdapterViewHolder : Java.Lang.Object
    {
        public TextView Name;
        public TextView City;
        public TextView Status;
        public ImageView Image;
        public RelativeLayout MainWhiteBox;

        public CardAdapterViewHolder(View view)
        {
            try
            {
                MainWhiteBox = (RelativeLayout)view.FindViewById(Resource.Id.MainWhiteBox);
                Name = (TextView)view.FindViewById(Resource.Id.item_tourist_spot_card_name);
                City = (TextView)view.FindViewById(Resource.Id.item_tourist_spot_card_city);
                Image = (ImageView)view.FindViewById(Resource.Id.item_tourist_spot_card_image);
                Status = (TextView)view.FindViewById(Resource.Id.status);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        } 
    }
}