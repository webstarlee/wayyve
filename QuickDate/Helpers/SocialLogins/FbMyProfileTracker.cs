using System;
using Xamarin.Facebook;

namespace QuickDate.Helpers.SocialLogins
{
    public class FbMyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> MOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            try
            {
                MOnProfileChanged?.Invoke(this, new OnProfileChangedEventArgs(currentProfile));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile MProfile;
        public OnProfileChangedEventArgs(Profile profile)
        {
            try
            {
                MProfile = profile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //Extract or delete HTML tags based on their name or whether or not they contain some attributes or content with the HTML editor pro online program.
    }
}