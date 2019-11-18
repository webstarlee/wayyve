using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Me.Relex;
using QuickDate.Activities;
using QuickDate.Activities.Premium;
using QuickDate.Activities.Premium.Adapters;
using QuickDate.Activities.SettingsUser;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.PaymentGoogle;
using QuickDateClient;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;

namespace QuickDate.Helpers.Controller
{
    public class PopupController: Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        private readonly Activity ActivityContext;
        private string CreditType;
        private CreditAdapter CreditAdapter;
        private Dialog PremiumWindow, DialogAddCredits, AddPhoneNumberWindow;
        private PremiumAdapter PremiumAdapter;
        private EditText TxtNumber1, TxtNumber2;
        private string FullNumber, DialogButtonType;
        private CreditsClass ItemCredits;
        private PremiumClass ItemPremium;

        public PopupController(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void DisplayAddPhoneNumber()
        {
            try
            {
                AddPhoneNumberWindow = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                AddPhoneNumberWindow.SetContentView(Resource.Layout.DialogAddPhoneNumber);

                TxtNumber1 = AddPhoneNumberWindow.FindViewById<EditText>(Resource.Id.numberEdit1); //Gone
                TxtNumber2 = AddPhoneNumberWindow.FindViewById<EditText>(Resource.Id.numberEdit2);
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (!string.IsNullOrEmpty(dataUser?.PhoneNumber))
                {
                    var correctly = Methods.FunString.IsPhoneNumber(dataUser.PhoneNumber);
                    if (correctly)
                    {
                        TxtNumber2.Text = dataUser.PhoneNumber/*.TrimStart(new[] { '0' , '+' })*/; 
                    }
                }

                FullNumber = TxtNumber2.Text/*.TrimStart(new[] { '0', '+' })*/;

                var btnAddPhoneNumber = AddPhoneNumberWindow.FindViewById<Button>(Resource.Id.sentButton);
                var btnSkipAddPhoneNumber = AddPhoneNumberWindow.FindViewById<TextView>(Resource.Id.skipbutton);

                btnAddPhoneNumber.Click += BtnAddPhoneNumberOnClick;
                btnSkipAddPhoneNumber.Click += BtnSkipAddPhoneNumberOnClick;
                 
                AddPhoneNumberWindow.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void BtnSkipAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                AddPhoneNumberWindow.Hide();
                AddPhoneNumberWindow.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnAddPhoneNumberOnClick(object sender, EventArgs e)
        {
            try
            {
                FullNumber =  TxtNumber2.Text;

                if (Regex.IsMatch(FullNumber, "^\\+?(\\d[\\d-. ]+)?(\\([\\d-. ]+\\))?[\\d-. ]+\\d$") &&
                    FullNumber.Length > 10)
                {
                    if (!string.IsNullOrEmpty(FullNumber))
                    {
                        Intent intent = new Intent(ActivityContext, typeof(VerificationCodeActivity));
                        intent.PutExtra("Number", FullNumber);
                        ActivityContext.StartActivityForResult(intent, 125);

                        AddPhoneNumberWindow.Hide();
                        AddPhoneNumberWindow.Dismiss();
                    }
                }
                else
                {
                    var dialog = new MaterialDialog.Builder(ActivityContext);
                    dialog.Title(Resource.String.Lbl_Warning);
                    dialog.Content(FullNumber + " " + ActivityContext.GetText(Resource.String.Lbl_ISNotValidNumber));
                    dialog.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Ok)).OnPositive((materialDialog, action) => { dialog.Dispose(); });
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        ///////////////////////////////////////////////////////
        
        public void DisplayPremiumWindow()
        {
            if (!AppSettings.PremiumSystemEnabled)
                return;
            try
            {
                PremiumWindow = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                PremiumWindow.SetContentView(Resource.Layout.UpgradePremiumLayout);

                var recyclerView = PremiumWindow.FindViewById<RecyclerView>(Resource.Id.recyler);
                 
                var image1 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon1);
                var image2 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon2);
                var image3 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon3);
                var image4 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon4);
                var image5 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon5);
                var image6 = PremiumWindow.FindViewById<TextView>(Resource.Id.icon6);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image1, IonIconsFonts.HappyOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image2, IonIconsFonts.RibbonB);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image3, IonIconsFonts.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image4, IonIconsFonts.Flash);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image5, IonIconsFonts.StatsBars);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, image6, IonIconsFonts.Wand);

                PremiumAdapter = new PremiumAdapter(ActivityContext);
                recyclerView.SetLayoutManager(new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false));
                PremiumAdapter.ItemClick += PremiumAdapterOnItemClick;
                recyclerView.SetAdapter(PremiumAdapter);

                var btnSkipAddCredits = PremiumWindow.FindViewById<Button>(Resource.Id.skippButton);
                btnSkipAddCredits.Click += BtnSkipAddCreditsOnClick;

                PremiumWindow.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BtnSkipAddCreditsOnClick(object sender, EventArgs e)
        {
            try
            {
                PremiumWindow.Hide();
                PremiumWindow.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

      
        //Open walletFragment with Google 
        private void PremiumAdapterOnItemClick(object sender, PremiumAdapterClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    PremiumClass item = PremiumAdapter.GetItem(position);
                    if (item != null)
                    {
                        ItemPremium = item;
                        DialogButtonType = "membership";

                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(ActivityContext);

                        if (AppSettings.ShowPaypal)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Btn_Paypal));
                        
                        if (AppSettings.ShowCreditCard)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_CreditCard));

                        if (AppSettings.ShowBankTransfer)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_BankTransfer));

                        dialogList.Items(arrayAdapter);
                        dialogList.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Close)).OnNegative(this);
                        dialogList.AlwaysCallSingleChoiceCallback();
                        dialogList.ItemsCallback(this).Build().Show();
                         
                        PremiumWindow.Hide();
                        PremiumWindow.Dismiss();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        //////////////////////////////////////////////////////

        public void DisplayCreditWindow(string type)
        {
            try
            {
                CreditType = type;
                DialogAddCredits = new Dialog(ActivityContext, Resource.Style.MyDialogTheme);
                DialogAddCredits.SetContentView(Resource.Layout.DialogAddCredits);

                var recyclerView = DialogAddCredits.FindViewById<RecyclerView>(Resource.Id.recyler);

                var viewPagerView = DialogAddCredits.FindViewById<ViewPager>(Resource.Id.viewPager);
                var indicator = DialogAddCredits.FindViewById<CircleIndicator>(Resource.Id.indicator);

                var titleText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainTitelText);
                titleText.Text = ActivityContext.GetText(Resource.String.Lbl_Your) + " " + AppSettings.ApplicationName + " " + ActivityContext.GetText(Resource.String.Lbl_CreditsBalance);

                var mainText = DialogAddCredits.FindViewById<TextView>(Resource.Id.mainText);
                var data = ListUtils.MyUserInfo.FirstOrDefault();
                mainText.Text = data?.Balance.Replace(".00","") + " " + ActivityContext.GetText(Resource.String.Lbl_Credits);

                var btnSkip = DialogAddCredits.FindViewById<Button>(Resource.Id.skippButton);
                var btnTerms = DialogAddCredits.FindViewById<TextView>(Resource.Id.TermsText);

                var creditsClass = new List<CreditsFeaturesClass>
                {
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits1), ColorCircle = "#00bee7",ImageFromResource = Resource.Drawable.viewPager_rocket},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits2), ColorCircle = "#0456C4" ,ImageFromResource = Resource.Drawable.viewPager_msg},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits3), ColorCircle = "#ff7102" ,ImageFromResource = Resource.Drawable.viewPager_gift},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits4), ColorCircle = "#4caf50" ,ImageFromResource = Resource.Drawable.viewPager_target},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits5), ColorCircle = "#8c4fe6" ,ImageFromResource = Resource.Drawable.viewPager_crown},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits6), ColorCircle = "#22e271" ,ImageFromResource = Resource.Drawable.viewPager_sticker},
                    new CreditsFeaturesClass {Description = ActivityContext.GetText(Resource.String.Lbl_DescriptionCredits7), ColorCircle = "#f44336",ImageFromResource = Resource.Drawable.viewPager_heart}
                };
                 
                var imageDescViewPager = new ImageDescViewPager(ActivityContext, creditsClass);
                viewPagerView.Adapter = imageDescViewPager;
                indicator.SetViewPager(viewPagerView);

                CreditAdapter = new CreditAdapter(ActivityContext);
                recyclerView.SetLayoutManager(new LinearLayoutManager(ActivityContext, LinearLayoutManager.Horizontal, false));
                CreditAdapter.OnItemClick += CreditAdapterOnItemClick;
                recyclerView.SetAdapter(CreditAdapter);

                btnSkip.Click += BtnSkipOnClick;
                btnTerms.Click += BtnTermsOnClick;
                DialogAddCredits.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

       
        //Open walletFragment with Google
        private void CreditAdapterOnItemClick(object sender, CreditAdapterViewHolderClickEventArgs e)
        {
            try
            {
                int position = e.Position;
                if (position > -1)
                {
                    CreditsClass item = CreditAdapter.GetItem(position);
                    if (item != null)
                    {
                        ItemCredits = item;
                        DialogButtonType = CreditType;

                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(ActivityContext);

                        if (AppSettings.ShowPaypal)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Btn_Paypal));

                        if (AppSettings.ShowCreditCard)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_CreditCard));

                        if (AppSettings.ShowBankTransfer)
                            arrayAdapter.Add(ActivityContext.GetString(Resource.String.Lbl_BankTransfer));

                        dialogList.Items(arrayAdapter);
                        dialogList.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Close)).OnNegative(this);
                        dialogList.AlwaysCallSingleChoiceCallback();
                        dialogList.ItemsCallback(this).Build().Show();
                         
                        DialogAddCredits.Hide();
                        DialogAddCredits.Dismiss();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void BtnTermsOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/terms");
                intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_TermsOfUse));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnSkipOnClick(object sender, EventArgs e)
        {
            try
            {
                DialogAddCredits.Hide();
                DialogAddCredits.Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //////////////////////////////////////////////////////

        #region MaterialDialog
             
        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                if (text == ActivityContext. GetString(Resource.String.Btn_Paypal))
                {
                    if (DialogButtonType == "membership")
                    {
                        HomeActivity.GetInstance()?.BtnPaypalOnClick(ItemPremium.Price, "membership", ItemPremium.Type, ItemPremium.Id.ToString());
                    }
                    else if (DialogButtonType == CreditType)
                    {
                        HomeActivity.GetInstance()?.BtnPaypalOnClick(ItemCredits.Price, CreditType, ItemCredits.TotalCoins,"");
                    }
                }
                else if (text == ActivityContext.GetString(Resource.String.Lbl_CreditCard))
                {
                    OpenIntentCreditCard();
                }
                else if (text == ActivityContext.GetString(Resource.String.Lbl_BankTransfer))
                {
                    OpenIntentBankTransfer();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {

                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenIntentBankTransfer()
        {
            try
            {
                if (DialogButtonType == "membership")
                {
                    Intent intent = new Intent(ActivityContext, typeof(PaymentLocalActivity));
                    intent.PutExtra("Id", ItemPremium.Id.ToString());
                    intent.PutExtra("credits", ItemPremium.Type);
                    intent.PutExtra("Price", ItemPremium.Price);
                    intent.PutExtra("payType", "membership");
                    ActivityContext.StartActivity(intent);
                }
                else if (DialogButtonType == CreditType)
                {
                    Intent intent = new Intent(ActivityContext, typeof(PaymentLocalActivity));
                    intent.PutExtra("credits", ItemCredits.Description + " " + ItemCredits.TotalCoins);
                    intent.PutExtra("Price", ItemCredits.Price);
                    intent.PutExtra("payType", CreditType); // credits|membership
                    ActivityContext.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenIntentCreditCard()
        {
            try
            {
                if (DialogButtonType == "membership")
                {
                    Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                    intent.PutExtra("Id", ItemPremium.Id.ToString());
                    intent.PutExtra("credits", ItemPremium.Type);
                    intent.PutExtra("Price", ItemPremium.Price);
                    intent.PutExtra("payType", "membership");
                    ActivityContext.StartActivity(intent);
                }
                else if (DialogButtonType == CreditType)
                {
                    Intent intent = new Intent(ActivityContext, typeof(PaymentCardDetailsActivity));
                    intent.PutExtra("credits", ItemCredits.TotalCoins);
                    intent.PutExtra("Price", ItemCredits.Price);
                    intent.PutExtra("payType", CreditType);// credits|membership
                    ActivityContext.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
       

        #endregion
    }
}