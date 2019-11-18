using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Utils;

namespace QuickDate.Activities.Premium.Adapters
{
    public class CreditAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CreditAdapterViewHolderClickEventArgs> OnItemClick;
        public event EventHandler<CreditAdapterViewHolderClickEventArgs> OnItemLongClick;

        private readonly Context ActivityContext;
        private readonly ObservableCollection<CreditsClass> CreditList = new ObservableCollection<CreditsClass>();
        private string CurrencySymbol = "$";

        public CreditAdapter(Context context)
        {
            try
            { 
                ActivityContext = context;
                HasStableIds = true;
                GetCredits(); 
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
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_CreditView, parent, false);
                var vh = new CreditAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is CreditAdapterViewHolder holder)
                {
                    var item = CreditList[position];

                    if (item == null)
                        return;

                    holder.Price.Text = item.Price + " " + CurrencySymbol;
                    holder.Title.Text = item.Description;
                    holder.CoinCount.Text = item.TotalCoins;
                    holder.CreditImage.SetImageResource(item.ImageFromResource);
                    holder.Title.SetTextColor(Color.ParseColor(item.Color));
                    holder.Price.SetTextColor(Color.ParseColor(item.Color));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GetCredits()
        {
            try
            { 
                var option = ListUtils.SettingsSiteList.FirstOrDefault();
                if (option != null)
                {
                    CurrencySymbol = option.CurrencySymbol ?? "$";

                    CreditList.Add(new CreditsClass { Id = 1, TotalCoins = option.BagOfCreditsAmount, Price =  option.BagOfCreditsPrice, Description = ActivityContext.GetString(Resource.String.Lbl_BagOfCredits), ImageFromResource = Resource.Drawable.Image_coin, Color = "#4caf50" });
                    CreditList.Add(new CreditsClass { Id = 2, TotalCoins = option.BoxOfCreditsAmount , Price = option.BoxOfCreditsPrice , Description = ActivityContext.GetString(Resource.String.Lbl_BoxOfCredits), ImageFromResource = Resource.Drawable.Image_coins, Color = "#0976f3" });
                    CreditList.Add(new CreditsClass { Id = 3, TotalCoins = option.ChestOfCreditsAmount, Price = option.ChestOfCreditsPrice, Description = ActivityContext.GetString(Resource.String.Lbl_ChestOfCredits), ImageFromResource = Resource.Drawable.Image_coinbag, Color = "#9135fa" });
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public CreditsClass GetItem(int position)
        {
            return CreditList[position];
        }

       public override int ItemCount => CreditList?.Count ?? 0;
         
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


        void Click(CreditAdapterViewHolderClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(CreditAdapterViewHolderClickEventArgs args) => OnItemLongClick?.Invoke(this, args);

    }

    public class CreditAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

       
        public View MainView { get; set; }
        public TextView Title { get; set; }
        public ImageView CreditImage { get; set; }
        public TextView Price { get; set; }
        public TextView CoinCount { get; set; }
        
        public RelativeLayout RelativeLayoutBg { get; set; }

        #endregion

        public CreditAdapterViewHolder(View itemView, Action<CreditAdapterViewHolderClickEventArgs> clickListener,  Action<CreditAdapterViewHolderClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Title = MainView.FindViewById<TextView>(Resource.Id.headtitle);
                CreditImage = MainView.FindViewById<ImageView>(Resource.Id.creditImage);
                Price = MainView.FindViewById<TextView>(Resource.Id.lasttext);
                CoinCount = MainView.FindViewById<TextView>(Resource.Id.coincount);


                RelativeLayoutBg = MainView.FindViewById<RelativeLayout>(Resource.Id.background);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new CreditAdapterViewHolderClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new CreditAdapterViewHolderClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class CreditAdapterViewHolderClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}