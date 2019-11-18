using System;
using System.Collections.Generic;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Blogs;
using ClipboardManager = Android.Content.ClipboardManager;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.Blogs
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ArticlesViewActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private ImageView  ImageBlog;
        private TextView  TxtTitle, TxtViews;
        private WebView TxtHtml;
        private ImageButton BtnMore;
        private ArticleDataObject ArticleData;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.ArticlesViewLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                GetDataArticles();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.action_share:
                    ShareEvent();
                    break;

                case Resource.Id.action_copy:
                    CopyLinkEvent();
                    break;

            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MenuArticleShare, menu);
            ChangeMenuIconColor(menu, Color.Black);

            return base.OnCreateOptionsMenu(menu);

        }

        private void ChangeMenuIconColor(IMenu menu, Color color)
        {
            for (int i = 0; i < menu.Size(); i++)
            {
                var drawable = menu.GetItem(i).Icon;
                if (drawable == null) continue;
                drawable.Mutate();
                drawable.SetColorFilter(color, PorterDuff.Mode.SrcAtop);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                ImageBlog = FindViewById<ImageView>(Resource.Id.imageBlog);
                TxtTitle = FindViewById<TextView>(Resource.Id.title);
                TxtHtml = FindViewById<WebView>(Resource.Id.LocalWebView);
                TxtViews = FindViewById<TextView>(Resource.Id.views);
                BtnMore = FindViewById<ImageButton>(Resource.Id.more);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = "";
                    toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    BtnMore.Click += BtnMoreOnClick;
                }
                else
                {
                    BtnMore.Click -= BtnMoreOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void BtnMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                arrayAdapter.Add(GetString(Resource.String.Lbl_CopyLink));
                arrayAdapter.Add(GetString(Resource.String.Lbl_Share));

                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Menu >> Copy Link
        private void CopyLinkEvent()
        {
            try
            {
                var clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", ArticleData.Url);
                clipboardManager.PrimaryClip = clipData;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Text_copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Menu >> Share
        private async void ShareEvent()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = ArticleData.Title,
                    Text = " ",
                    Url = ArticleData.Url
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                if (text == GetString(Resource.String.Lbl_CopyLink))
                {
                    CopyLinkEvent();
                }
                else if (text == GetString(Resource.String.Lbl_Share))
                {
                    ShareEvent();
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

        #endregion

        private void GetDataArticles()
        {
            try
            {
                ArticleData = JsonConvert.DeserializeObject<ArticleDataObject>(Intent.GetStringExtra("ArticleObject"));
                if (ArticleData != null)
                {
                    GlideImageLoader.LoadImage(this, ArticleData.Thumbnail, ImageBlog, ImageStyle.CenterCrop,ImagePlaceholders.Drawable);

                    SupportActionBar.Title = Methods.FunString.DecodeString(ArticleData.Title);
                     
                    TxtTitle.Text = Methods.FunString.DecodeString(ArticleData.Title);
                    TxtViews.Text = ArticleData.View + " " + GetText(Resource.String.Lbl_Views);
                    var content =  Html.FromHtml(ArticleData.Content, FromHtmlOptions.ModeCompact).ToString();

                    string data = "<!DOCTYPE html>";
                    data += "<head><title></title></head>";
                    data += "<body>" + content + "</body>";
                    data += "</html>";
                     
                    TxtHtml.SetWebViewClient(new WebViewClient());
                    TxtHtml.Settings.LoadsImagesAutomatically = true;
                    TxtHtml.Settings.JavaScriptEnabled = true;
                    TxtHtml.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                     
                    TxtHtml.LoadData(data, "text/html", "UTF-8");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}