using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Locations;
using Android.Provider;
using Android.Support.CustomTabs;
using Android.Widget;
using System;
using QuickDate.PlacesAsync;
using Uri = Android.Net.Uri;

namespace QuickDate.Helpers.Controller
{
    public class IntentController
    {
        //############################# DON'T MODIFY HERE ##########################

        private readonly Activity Context;
        public enum IntentType
        {
            Register,
            Login,
            ForgetPassword,
            First,
        }
         
        public IntentController(Activity context)
        {
            try
            {
                Context = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //############################# Special for application ##########################
         
        //################################# General #################################
        /// <summary>
        /// Open intent Image Gallery when the request code of result is 500
        /// </summary>
        /// <param name="title"></param>
        /// <param name="allowMultiple"></param>
        public void OpenIntentImageGallery(string title, bool allowMultiple = true)
        {
            try
            {
                var Int = new Intent(Intent.ActionPick);
                Int.SetAction(Intent.ActionGetContent);
                Int.SetType("image/*");
                if (allowMultiple)
                    Int.PutExtra(Intent.ExtraAllowMultiple, true);

                Int.PutExtra(Intent.ExtraAllowMultiple, true);
                Context.StartActivityForResult(Intent.CreateChooser(Int, title), 500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent video Gallery when the request code of result is 501
        /// </summary>
        public void OpenIntentVideoGallery()
        {
            try
            {
                var Int = new Intent(Intent.ActionPick);
                Int.SetAction(Intent.ActionGetContent);
                Int.SetType("video/*");
                Context.StartActivityForResult(Int, 501);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Location when the request code of result is 502
        /// </summary>
        public void OpenIntentLocation()
        {
            try
            {
                Intent intent = new Intent(Context, typeof(LocationActivity));
                Context.StartActivityForResult(intent, 502);
            }
            catch (GooglePlayServicesRepairableException e)
            {
                Console.WriteLine(e);
                Toast.MakeText(Context, "Google Play Services is not available.", ToastLength.Short).Show();
            }
            catch (GooglePlayServicesNotAvailableException e)
            {
                Console.WriteLine(e);
                Toast.MakeText(Context, "Google Play Services is not available", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Toast.MakeText(Context, "Google Play Services e", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Open intent Camera when the request code of result is 503
        /// </summary>
        public void OpenIntentCamera()
        {
            try
            {
                var Int = new Intent(MediaStore.ActionImageCapture);
                Context.StartActivityForResult(Int, 503);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent File when the request code of result is 504
        /// </summary>
        /// <param name="title"></param>
        public void OpenIntentFile(string title)
        {
            try
            {
                var Int = new Intent(Intent.ActionPick);
                Int.SetAction(Intent.ActionGetContent);
                Int.SetType("*/*");
                Context.StartActivityForResult(Intent.CreateChooser(Int, title), 504);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Audio when the request code of result is 505
        /// </summary>
        public void OpenIntentAudio()
        {
            try
            {
                var Int = new Intent(Intent.ActionPick);
                Int.SetAction(Intent.ActionView);
                Int.SetType("audio/*");
                Context.StartActivityForResult(Int, 505);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Get Contact Number Phone when the request code of result is 506
        /// </summary>
        public void OpenIntentGetContactNumberPhone()
        {
            try
            {
                Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                Context.StartActivityForResult(pickcontact, 506);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Gps when the request code of result is 1050
        /// </summary>
        /// <param name="locationManager"></param>
        public void OpenIntentGps(LocationManager locationManager)
        {
            try
            {
                if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    string locationProviders = Settings.Secure.GetString(Context.ContentResolver, Settings.Secure.LocationProvidersAllowed);
                    if (locationProviders == null || locationProviders.Equals(""))
                    {
                        Context.StartActivityForResult(new Intent(Settings.ActionLocationSourceSettings), 1050);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Send Sms
        /// </summary>
        /// <param name="phoneNumber">any number </param>
        /// <param name="textMessages">Example : Hello Xamarin This is My Test SMS</param>
        /// <param name="openIntent">true or false >> If it is false the message will be sent in a hidden manner .. don't open intent </param>
        public void OpenIntentSendSms(string phoneNumber, string textMessages, bool openIntent = true)
        {
            try
            {
                if (openIntent)
                {
                    var smsUri = Uri.Parse("smsto:" + phoneNumber);
                    var Int = new Intent(Intent.ActionSendto, smsUri);
                    Int.PutExtra("sms_body", textMessages);
                    Int.AddFlags(ActivityFlags.NewTask);
                    Context.StartActivity(Int);
                }
                else
                {
                    //Or use this code
                    Android.Telephony.SmsManager.Default.SendTextMessage(phoneNumber, null, textMessages, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Save Contact Number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="detailedInformation">true or false </param>
        public void OpenIntentSaveContacts(string phoneNumber, string name, string email, bool detailedInformation = false)
        {
            try
            {
                if (detailedInformation)
                {
                    Intent Int = new Intent(ContactsContract.Intents.Insert.Action);
                    Int.SetType(ContactsContract.RawContacts.ContentType);
                    Int.PutExtra(ContactsContract.Intents.Insert.Phone, phoneNumber);
                    Int.PutExtra(ContactsContract.Intents.Insert.Name, name);
                    Int.PutExtra(ContactsContract.Intents.Insert.Email, email);
                    Context.StartActivity(Int);
                }
                else
                {
                    var contactUri = Uri.Parse("tel:" + phoneNumber);
                    Intent Int = new Intent(ContactsContract.Intents.ShowOrCreateContact, contactUri);
                    Int.PutExtra(ContactsContract.Intents.ExtraRecipientContactName, true);
                    Context.StartActivity(Int);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cc"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        public void OpenIntentSendEmail(string email, string cc = " ", string subject = " ", string text = " ")
        {
            try
            {
                var Int = new Intent(Intent.ActionSend);
                Int.PutExtra(Intent.ExtraEmail, email);
                Int.PutExtra(Intent.ExtraCc, cc);
                Int.PutExtra(Intent.ExtraSubject, subject);
                Int.PutExtra(Intent.ExtraText, text);
                Int.SetType("message/rfc822");
                Context.StartActivity(Int);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="phoneNumber"></param>
        public void OpenIntentSendPhoneCall(string phoneNumber)
        {
            try
            {
                var urlNumber = Uri.Parse("tel:" + phoneNumber);
                var Int = new Intent(Intent.ActionCall);
                Int.SetData(urlNumber);
                Int.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(Int);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Browser From Phone using Url
        /// </summary>
        /// <param name="website"></param>
        public void OpenBrowserFromPhone(string website)
        {
            try
            {
                var uri = Uri.Parse(website);
                var intent = new Intent(Intent.ActionView, uri);
                intent.AddFlags(ActivityFlags.NewTask);
                Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open intent Browser From App using Url
        /// </summary>
        /// <param name="url"></param>
        public void OpenBrowserFromApp(string url)
        {
            try
            {
                CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
                CustomTabsIntent customTabsIntent = builder.Build();
                customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                customTabsIntent.LaunchUrl(Context, Uri.Parse(url));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Open app PackageName by Google play
        /// </summary>
        /// <param name="appPackageName">from Context or Activity object</param>
        public void OpenAppOnGooglePlay(string appPackageName)
        {
            try
            { 
                try
                {
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + appPackageName)));
                }
                catch (ActivityNotFoundException exception)
                {
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
                    Console.WriteLine(exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Intent GetOpenFacebookIntent(Context context, string name)
        { 
            try
            {
                context.PackageManager.GetPackageInfo("com.facebook.katana", 0); //Checks if FB is even installed.
                //return new Intent(Intent.ActionView,Uri.Parse("fb://profile/" + name)); //Try's to make intent with FB is URI
                return new Intent(Intent.ActionView,Uri.Parse("fb://facewebmodal/f?href=https://www.facebook.com/" + name)); //Try's to make intent with FB is URI
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Intent(Intent.ActionView,Uri.Parse("https://www.facebook.com/" + name)); //catches and opens a url to the desired page
            }
        }

        public void OpenFacebookIntent(Context context, string name)
        {
            try
            {
                Intent facebookIntent = GetOpenFacebookIntent(context, name);
                Context.StartActivity(facebookIntent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void OpenTwitterIntent(string name)
        {
            try
            {
                Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("twitter://user?screen_name=" + name)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OpenBrowserFromApp("https://twitter.com/#!/" + name);
                //Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://twitter.com/#!/" + name)));
            } 
        }

        public void OpenLinkedInIntent(string name)
        {
            try
            {
                string url = "https://www.linkedin.com/in/" + name; 
                Intent linkedInAppIntent = new Intent(Intent.ActionView, Uri.Parse(url));
                linkedInAppIntent.AddFlags(ActivityFlags.ClearWhenTaskReset);
                Context.StartActivity(linkedInAppIntent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void OpenInstagramIntent(string name)
        {
            try
            {
                Intent likeIng = new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/_u/" + name) );
                likeIng.SetPackage("com.instagram.android");

                try
                {
                    Context.StartActivity(likeIng);
                }
                catch (ActivityNotFoundException e)
                {
                    Console.WriteLine(e);
                    Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("http://instagram.com/" + name)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }
    }
}