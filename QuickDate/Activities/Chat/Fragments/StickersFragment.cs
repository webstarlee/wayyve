using System;
using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View.Animation;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.Chat.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Chat;

namespace QuickDate.Activities.Chat.Fragments
{
    public class StickersFragment : Fragment
    {
 
        #region Variables Basic

        private RecyclerView StickerRecyclerView;
        private StickerAdapter StickerAdapter;
        private LinearLayout LayoutPremium;
        private TextView TxtCountCart;
        private Button BtnGetPremium, BtnBuyCredits;
        private string UserId;
        private MessagesBoxActivity ChatWindow;

        #endregion
         
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            UserId = Arguments.GetString("userid") ?? MessagesBoxActivity.Userid.ToString();
            ChatWindow = (MessagesBoxActivity)Activity;
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
                StickerRecyclerView = contentView.FindViewById<RecyclerView>(Resource.Id.rvGift);
                LayoutPremium = contentView.FindViewById<LinearLayout>(Resource.Id.LnyPremium);
                TxtCountCart = contentView.FindViewById<TextView>(Resource.Id.countCartTextView);
                BtnGetPremium = contentView.FindViewById<Button>(Resource.Id.GetPremiumButton);
                BtnBuyCredits = contentView.FindViewById<Button>(Resource.Id.BuyCreditsButton);

                var credit = ListUtils.SettingsSiteList.FirstOrDefault()?.NotProChatStickersCredit ?? "25";
                TxtCountCart.Text = GetText(Resource.String.Lbl_countCartGift) + credit + "02" + GetText(Resource.String.Lbl_Credits);

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
                var gridLayoutManager = new GridLayoutManager(Activity, 3);
                StickerRecyclerView.SetLayoutManager(gridLayoutManager);
                StickerRecyclerView.NestedScrollingEnabled = false;
                StickerAdapter = new StickerAdapter(Activity);
                StickerAdapter.OnItemClick += StickerAdapterOnItemClick;
                StickerRecyclerView.SetAdapter(StickerAdapter);
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
          
        private void StickerAdapterOnItemClick(object sender, StickerAdapterClickEventArgs e)
        {
            try
            {
                var stickerUrl = StickerAdapter.GetItem(e.Position);

                int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var time2 = unixTimestamp.ToString();
                string timeNow = DateTime.Now.ToString("hh:mm");

                if (Methods.CheckConnectivity())
                {
                    GetChatConversationsObject.Messages message = new GetChatConversationsObject.Messages
                    {
                        Id = unixTimestamp,
                        FromName = UserDetails.FullName,
                        FromAvater = UserDetails.Avatar,
                        ToName = ChatWindow?.UserInfoData?.Fullname ?? "",
                        ToAvater = ChatWindow?.UserInfoData?.Avater ?? "",
                        From = UserDetails.UserId,
                        To = Convert.ToInt32(UserId),
                        Text = "",
                        Media = "",
                        FromDelete = 0,
                        ToDelete = 0,
                        Sticker = stickerUrl?.File,
                        CreatedAt = timeNow,
                        Seen = 0,
                        Type = "Sent",
                        MessageType = "sticker"
                    };

                    int index = MessagesBoxActivity.MAdapter.MessageList.IndexOf(MessagesBoxActivity.MAdapter.MessageList.Last());
                    if (index > -1)
                    {
                        MessagesBoxActivity.MAdapter.MessageList.Add(message);
                        MessagesBoxActivity.MAdapter.NotifyItemInserted(index);

                        //Scroll Down >> 
                        ChatWindow?.ChatBoxRecyclerView.ScrollToPosition(index);
                    }

                    MessageController.SendMessageTask(Activity , MessagesBoxActivity.Userid, "", stickerUrl?.Id.ToString(), "",time2, ChatWindow?.UserInfoData).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short);
                }

                try
                {
                    if (ChatWindow != null)
                    {
                        var interpolator = new FastOutSlowInInterpolator();
                        ChatWindow.StickerButton.Tag = "Closed";

                        ChatWindow.ResetButtonTags();
                        ChatWindow.StickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                        ChatWindow.TopFragmentHolder.Animate().SetInterpolator(interpolator).TranslationY(1200).SetDuration(300);
                        ChatWindow.SupportFragmentManager.BeginTransaction().Remove(ChatWindow.ChatStickersFragment).Commit();
                    } 
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}