using System;
using System.Linq;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using QuickDate.Activities.Gift.Adapters;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Classes.Common;
using QuickDateClient.Requests;


namespace QuickDate.Activities.Gift
{
    public class GiftDialogFragment : BottomSheetDialogFragment
    {
        #region Variables Basic

        private RecyclerView GiftRecyclerView;
        private GiftAdapter GiftAdapter;
        private LinearLayout LayoutPremium;
        private TextView TxtCountCart;
        private Button BtnGetPremium, BtnBuyCredits;
        private string UserId;

        #endregion

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                UserId = Arguments.GetString("UserId");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.ButtomSheetGift, container, false);
                 
                InitComponent(view);
                SetRecyclerViewAdapters();

                BtnGetPremium.Click += BtnGetPremiumOnClick;
                BtnBuyCredits.Click += BtnBuyCreditsOnClick;

                
                
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            } 
        }
         
        #region Functions

        private void InitComponent(View contentView)
        {
            try
            {
                GiftRecyclerView = contentView.FindViewById<RecyclerView>(Resource.Id.rvGift);
                LayoutPremium = contentView.FindViewById<LinearLayout>(Resource.Id.LnyPremium);
                TxtCountCart = contentView.FindViewById<TextView>(Resource.Id.countCartTextView);
                BtnGetPremium = contentView.FindViewById<Button>(Resource.Id.GetPremiumButton);
                BtnBuyCredits = contentView.FindViewById<Button>(Resource.Id.BuyCreditsButton);

                var costPerGift = ListUtils.SettingsSiteList.FirstOrDefault()?.CostPerGift ?? "50";
                TxtCountCart.Text = GetText(Resource.String.Lbl_countCartGift) + " " + costPerGift + " " + GetText(Resource.String.Lbl_Credits);

                LayoutPremium.Visibility = AppSettings.EnableAppFree ? ViewStates.Gone : UserDetails.IsPro == "0" ? ViewStates.Visible : ViewStates.Gone;

                BtnGetPremium.Visibility = !AppSettings.PremiumSystemEnabled ? ViewStates.Gone : ViewStates.Visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
  
        private void SetRecyclerViewAdapters()
        {
            try
            { 
                GiftRecyclerView.NestedScrollingEnabled = false;
                GiftAdapter = new GiftAdapter(Activity,"Normal");
                GiftAdapter.OnItemClick += GiftAdapterOnItemClick;
                
                var gridLayoutManager = new GridLayoutManager(Activity, 3);
                GiftRecyclerView.SetLayoutManager(gridLayoutManager);
                GiftRecyclerView.SetItemViewCacheSize(20);
                GiftRecyclerView.HasFixedSize = true;
                GiftRecyclerView.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<DataFile>(Activity, GiftAdapter, sizeProvider, 10);
                GiftRecyclerView.AddOnScrollListener(preLoader);
                GiftRecyclerView.SetAdapter(GiftAdapter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void BtnBuyCreditsOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayCreditWindow("credits");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnGetPremiumOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayPremiumWindow();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private async void GiftAdapterOnItemClick(object sender, GiftAdapterClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    var item = GiftAdapter.GetItem(position);
                    if (item != null)
                    {
                        Toast.MakeText(Context, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();

                        (int apiStatus, var respond) = await RequestsAsync.Users.SendGiftAsync(UserId, item.Id.ToString()).ConfigureAwait(false);
                        if (apiStatus == 200)
                        {
                            if (respond is AmountObject result)
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    { 
                                        if (HomeActivity.GetInstance().ProfileFragment?.WalletNumber != null)
                                            HomeActivity.GetInstance().ProfileFragment.WalletNumber.Text = result.CreditAmount.ToString(); 
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                });

                                //Close Fragment 
                                Dismiss();
                            }
                        }
                        else Methods.DisplayReportResult(Activity, respond);
                    }
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
    }
}