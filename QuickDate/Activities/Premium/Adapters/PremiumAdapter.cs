using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using QuickDate.Helpers.Utils;

namespace QuickDate.Activities.Premium.Adapters
{
    public class PremiumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<PremiumAdapterClickEventArgs> ItemClick;
        public event EventHandler<PremiumAdapterClickEventArgs> ItemLongClick;

        private int Position;
        private readonly Context ActivityContext;
        private readonly ObservableCollection<PremiumClass> PremiumList = new ObservableCollection<PremiumClass>();
        private string CurrencySymbol = "$";

        public PremiumAdapter(Context context)
        {
            try
            { 
                ActivityContext = context;
                HasStableIds = true;
                GetPremium();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GetPremium()
        {
            try
            {
                var option = ListUtils.SettingsSiteList.FirstOrDefault();
                if (option != null)
                {
                    CurrencySymbol = option.CurrencySymbol ?? "$";

                    PremiumList.Add(new PremiumClass { Id = 1, Price = option.WeeklyProPlan, SecondryText = ActivityContext.GetText(Resource.String.Lbl_Normal), Type = ActivityContext.GetText(Resource.String.Lbl_Weekly) });
                    PremiumList.Add(new PremiumClass { Id = 2, Price = option.MonthlyProPlan, SecondryText = ActivityContext.GetText(Resource.String.Lbl_Save) + " 51%", Type = ActivityContext.GetText(Resource.String.Lbl_Monthly) });
                    PremiumList.Add(new PremiumClass { Id = 3, Price = option.YearlyProPlan, SecondryText = ActivityContext.GetText(Resource.String.Lbl_Save) + " 90%", Type = ActivityContext.GetText(Resource.String.Lbl_Yearly) });
                    PremiumList.Add(new PremiumClass { Id = 4, Price = option.LifetimeProPlan, SecondryText = ActivityContext.GetText(Resource.String.Lbl_PayOnesAccessForEver), Type = ActivityContext.GetText(Resource.String.Lbl_Lifetime) });
                }
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
                //Setup your layout here >> Style_HContact_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_PremiumView, parent, false);
                var vh = new PremiumViewHolder(itemView, OnClick, OnLongClick);
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
                Position = position;
                if (!(viewHolder is PremiumViewHolder holder))
                    return;

                var item = PremiumList[Position];

                if (item == null)
                    return;

                switch (item.Id)
                {
                    case 1:
                        holder.RelativeLayoutBg.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal);
                        break;
                    case 2:
                        holder.RelativeLayoutBg.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal5);
                        break;
                    case 3:
                        holder.RelativeLayoutBg.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal3);
                        break;
                    case 4:
                        holder.RelativeLayoutBg.SetBackgroundResource(Resource.Drawable.Shape_Gradient_Normal4);
                        break;
                    default:
                        break;
                }

                holder.Price.Text =  item.Price + " " + CurrencySymbol;
                holder.Title.Text = item.Type;
                holder.LastText.Text = item.SecondryText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public override int ItemCount => PremiumList?.Count ?? 0;

        public PremiumClass GetItem(int position)
        {
            return PremiumList[position];
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


        void OnClick(PremiumAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(PremiumAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class PremiumViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }

        
        public ImageView Image { get; set; }

        public TextView Title { get; set; }
        public TextView Price { get; set; }
        public TextView LastText { get; set; }
        public RelativeLayout RelativeLayoutBg { get; set; }
        #endregion

        public PremiumViewHolder(View itemView, Action<PremiumAdapterClickEventArgs> clickListener, Action<PremiumAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Title = MainView.FindViewById<TextView>(Resource.Id.headtitle);
                Price = MainView.FindViewById<TextView>(Resource.Id.price);
                LastText = MainView.FindViewById<TextView>(Resource.Id.lasttext);
                RelativeLayoutBg = MainView.FindViewById<RelativeLayout>(Resource.Id.background);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new PremiumAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new PremiumAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class PremiumAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}