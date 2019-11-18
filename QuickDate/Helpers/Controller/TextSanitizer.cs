using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Support.V4.Content;
using Com.Luseen.Autolinklibrary;
using QuickDate.Activities;
using QuickDate.Helpers.Utils;

namespace QuickDate.Helpers.Controller
{
    public class TextSanitizer
    {
        public AutoLinkTextView AutoLinkTextView;
        public Activity Activity;

        public TextSanitizer(AutoLinkTextView linkTextView , Activity activity )
        {
            try
            {
                AutoLinkTextView = linkTextView;
                Activity = activity;
                AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Load(string autoLinkText, string position = "Left")
        {
            try
            {
                AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention, AutoLinkMode.ModeCustom);

                if (position == "Sent" || position == "sent")
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.right_ModeUrl_color));
                }
                else
                {
                    AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModePhone_color));
                    AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeEmail_color));
                    AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeHashtag_color));
                    AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeUrl_color));
                    AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeMention_color));
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.AutoLinkText_ModeUrl_color));
                }
                var text = autoLinkText.Split('/');
                if (text.Count() > 1)
                {
                    AutoLinkTextView.SetCustomRegex(@"\b(" + text.LastOrDefault() + @")\b");
                }

                string lastString = autoLinkText.Replace(" /", " ");
                if (!string.IsNullOrEmpty(lastString))
                    AutoLinkTextView.SetAutoLinkText(Methods.FunString.DecodeString(lastString));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AutoLinkTextViewOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                AutoLinkMode matchedText = autoLinkOnClickEventArgs.P0;
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email" || matchedText == AutoLinkMode.ModeEmail)
                {
                    Methods.App.SendEmail(Application.Context, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website" || matchedText == AutoLinkMode.ModeUrl)
                {
                    String url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }

                    var intent = new Intent(Application.Context, typeof(LocalWebViewActivity));
                    intent.PutExtra("URL", url);
                    intent.PutExtra("Type", url);
                    Activity.StartActivity(intent);
                }
                else if (typetext == "Hashtag" || matchedText == AutoLinkMode.ModeHashtag)
                {
                   
                }
                else if (typetext == "Mention" || matchedText == AutoLinkMode.ModeMention)
                {
                    
                }
                else if (typetext == "Number" || matchedText == AutoLinkMode.ModePhone)
                {
                    Methods.App.SaveContacts(Activity, autoLinkOnClickEventArgs.P1, "", "2");
                }
                else
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}