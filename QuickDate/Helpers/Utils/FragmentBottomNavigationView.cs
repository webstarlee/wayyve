using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Content;
using Com.Gigamole.Navigationtabbar.Ntb;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.Tabbes.Fragment;
using QuickDate.Helpers.Ads;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Task = System.Threading.Tasks.Task;

namespace QuickDate.Helpers.Utils
{
    public class FragmentBottomNavigationView
    {
        private readonly HomeActivity Context;

        public JavaList<NavigationTabBar.Model> Models;
        public readonly List<Fragment> FragmentListTab0 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab1 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab2 = new List<Fragment>();
        public readonly List<Fragment> FragmentListTab4 = new List<Fragment>();

        private int PageNumber;

        public FragmentBottomNavigationView(Activity context)
        {
            try
            {
                Context = (HomeActivity)context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        public void SetupNavigation(NavigationTabBar navigationTabBar)
        {
            try
            {
                Models = new JavaList<NavigationTabBar.Model>
                {
                    new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_cards),Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Cards)).Build()
                };

                if (AppSettings.ShowTrending)
                    Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_trending), Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Trending)).Build());

                Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_notification), Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Notifications)).Build());
                Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_messages), Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_messages)).Build());
                Models.Add(new NavigationTabBar.Model.Builder(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_tab_profile), Color.ParseColor("#ffffff")).Title(Context.GetText(Resource.String.Lbl_Profile)).Build());
                

                var eee = NavigationTabBar.BadgeGravity.Top;
                navigationTabBar.SetBadgeGravity(eee);
                navigationTabBar.BadgeBgColor = Color.ParseColor(AppSettings.MainColor);
                navigationTabBar.BadgeTitleColor = Color.White;


                if (AppSettings.SetTabColoredTheme)
                {
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Cards)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    if (AppSettings.ShowTrending)
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Trending)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Notifications)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_messages)).Color = Color.ParseColor(AppSettings.TabColoredColor);
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor(AppSettings.TabColoredColor);

                    navigationTabBar.BgColor = Color.ParseColor(AppSettings.MainColor);
                    navigationTabBar.ActiveColor = Color.White;
                    navigationTabBar.InactiveColor = Color.White;
                }
                else if (AppSettings.SetTabDarkTheme)
                {
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Cards)).Color = Color.ParseColor("#444444");

                    if (AppSettings.ShowTrending)
                        Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Trending)).Color = Color.ParseColor("#444444");

                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Notifications)).Color = Color.ParseColor("#444444");
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_messages)).Color = Color.ParseColor("#444444");
                    Models.First(a => a.Title == Context.GetText(Resource.String.Lbl_Profile)).Color = Color.ParseColor("#444444");

                    navigationTabBar.BgColor = Color.ParseColor("#282828");
                    navigationTabBar.ActiveColor = Color.White;
                    navigationTabBar.InactiveColor = Color.White;
                }

                navigationTabBar.Models = Models;


                navigationTabBar.IsScaled = false;
                navigationTabBar.IconSizeFraction = (float)0.450;

                //navigationTabBar.SetBadgePosition(NavigationTabBar.BadgePosition.Center);
                if (AppSettings.SetTabIsTitledWithText)
                {
                    navigationTabBar.SetTitleMode(NavigationTabBar.TitleMode.All);
                    navigationTabBar.IsTitled = true;
                }

                navigationTabBar.StartTabSelected += NavigationTabBarOnStartTabSelected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void NavigationTabBarOnStartTabSelected(object sender, NavigationTabBar.StartTabSelectedEventArgs e)
        {
            try
            {
                switch (e.P1)
                {
                    case 0:
                        PageNumber = 0;
                        ShowFragment0();
                        Context?.TracksCounter?.CheckTracksCounter();
                        break;

                    case 1:
                        if (AppSettings.ShowTrending)
                        {
                            PageNumber = 1;
                            ShowFragment1();
                            Context?.TracksCounter?.CheckTracksCounter();
                            AdsGoogle.Ad_Interstitial(Context); 
                        }
                        else
                        {
                            PageNumber = 2;
                            NavigationTabBar.Model tabNotifications = Models.FirstOrDefault(a =>
                                a.Title == Context.GetText(Resource.String.Lbl_Notifications));
                            tabNotifications?.HideBadge();
                            ShowFragment2();
                            Context?.TracksCounter?.CheckTracksCounter();
                            AdsGoogle.Ad_RewardedVideo(Context);
                        }

                        break;

                    case 2:
                        PageNumber = 2;
                        if (AppSettings.ShowTrending)
                        {
                            NavigationTabBar.Model tabNotifications = Models.FirstOrDefault(a =>
                                a.Title == Context.GetText(Resource.String.Lbl_Notifications));
                            tabNotifications?.HideBadge();
                            ShowFragment2();
                            Context?.TracksCounter?.CheckTracksCounter();
                            AdsGoogle.Ad_Interstitial(Context);
                        }
                        else
                        {
                            PageNumber = 3;
                            NavigationTabBar.Model tabMessages = Models.FirstOrDefault(a =>
                                a.Title == Context.GetText(Resource.String.Lbl_messages));
                            tabMessages?.HideBadge();
                            Context.ShowChat();
                        }

                        break;

                    case 3:
                        if (AppSettings.ShowTrending)
                        {
                            PageNumber = 3;
                            NavigationTabBar.Model tabMessages = Models.FirstOrDefault(a => a.Title == Context.GetText(Resource.String.Lbl_messages));
                            tabMessages?.HideBadge();
                            Context.ShowChat();
                        }
                        else
                        {
                            PageNumber = 4;
                            ShowFragment4();
                        } 
                        break;
                    case 4:
                        PageNumber = 4;
                        ShowFragment4();
                        break;

                    default:
                        PageNumber = 0;
                        ShowFragment0();
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void HideFragmentFromList(List<Fragment> fragmentList, FragmentTransaction ft)
        {
            try
            {
                if (fragmentList.Count <= 0) return;
                foreach (var fra in fragmentList.Where(fra => fra.IsAdded && fra.IsVisible))
                {
                    ft.Hide(fra);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void DisplayFragment(Fragment newFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                HideFragmentFromList(FragmentListTab0, ft); 
                HideFragmentFromList(FragmentListTab1, ft); 
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                if (PageNumber == 0)
                    if (!FragmentListTab0.Contains(newFragment))
                        FragmentListTab0.Add(newFragment);

                if (PageNumber == 1)
                    if (!FragmentListTab1.Contains(newFragment))
                        FragmentListTab1.Add(newFragment);

                if (PageNumber == 2)
                    if (!FragmentListTab2.Contains(newFragment))
                        FragmentListTab2.Add(newFragment);

                if (PageNumber == 4)
                    if (!FragmentListTab4.Contains(newFragment))
                        FragmentListTab4.Add(newFragment);

                if (!newFragment.IsAdded)
                    ft.Add(Resource.Id.content, newFragment, PageNumber + newFragment.Id.ToString());
                else
                    ft.Show(newFragment);

                ft.CommitNow();
                Context.SupportFragmentManager.ExecutePendingTransactions();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            } 
        }

        private void RemoveFragment(Fragment oldFragment)
        {
            try
            {
                FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                if (PageNumber == 0)
                    if (FragmentListTab0.Contains(oldFragment))
                        FragmentListTab0.Remove(oldFragment);

                if (PageNumber == 1)
                    if (FragmentListTab1.Contains(oldFragment))
                        FragmentListTab1.Remove(oldFragment);

                if (PageNumber == 2)
                    if (FragmentListTab2.Contains(oldFragment))
                        FragmentListTab2.Remove(oldFragment);

                if (PageNumber == 4)
                    if (FragmentListTab4.Contains(oldFragment))
                        FragmentListTab4.Remove(oldFragment);

                HideFragmentFromList(FragmentListTab0, ft);
                HideFragmentFromList(FragmentListTab1, ft);
                HideFragmentFromList(FragmentListTab2, ft);
                HideFragmentFromList(FragmentListTab4, ft);

                if (oldFragment.IsAdded)
                    ft.Remove(oldFragment);

                if (PageNumber == 0)
                {
                    var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 1)
                {
                    var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 2)
                {
                    var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
                else if (PageNumber == 4)
                {
                    var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                    ft.Show(currentFragment).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        public int GetCountFragment()
        {
            try
            {
                switch (PageNumber)
                {
                    case 0:
                        return FragmentListTab0.Count > 1 ? FragmentListTab0.Count : 0;
                    case 1:
                        return FragmentListTab1.Count > 1 ? FragmentListTab1.Count : 0;
                    case 2:
                        return FragmentListTab2.Count > 1 ? FragmentListTab2.Count : 0;
                    case 4:
                        return FragmentListTab4.Count > 1 ? FragmentListTab4.Count : 0;
                    default:
                        return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            } 
        }

        public void BackStackClickFragment()
        {
            try
            {
                if (PageNumber == 0)
                {
                    if (FragmentListTab0.Count > 1)
                    {
                        var currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 1)
                {
                    if (FragmentListTab1.Count > 1)
                    {
                        var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 2)
                {
                    if (FragmentListTab2.Count > 1)
                    {
                        var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
                else if (PageNumber == 4)
                {
                    if (FragmentListTab4.Count > 1)
                    {
                        var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                        if (currentFragment != null)
                            RemoveFragment(currentFragment);
                    }
                    else
                    {
                        Context.Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        private void ShowFragment0()
        {
            try
            {
                if (FragmentListTab0.Count < 0) return;

                Fragment currentFragment;
                // If user presses it while still on that tab it removes all fragments from the list
                if (FragmentListTab0.Count > 1)
                {
                    FragmentTransaction ft = Context.SupportFragmentManager.BeginTransaction();

                    for (var index = FragmentListTab0.Count - 1; index > 0; index--)
                    {
                        var oldFragment = FragmentListTab0[index];
                        if (FragmentListTab0.Contains(oldFragment))
                            FragmentListTab0.Remove(oldFragment);

                        if (oldFragment.IsAdded)
                            ft.Remove(oldFragment);

                        HideFragmentFromList(FragmentListTab0, ft);
                    }

                    currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    ft.Show(currentFragment).Commit(); 
                }
                else
                {
                    currentFragment = FragmentListTab0[FragmentListTab0.Count - 1];
                    if (currentFragment != null)
                        DisplayFragment(currentFragment);
                }

                if (currentFragment is CardMachFragment fragment)
                {
                    if (fragment?.CardDateAdapter?.UsersDateList?.Count == 0)
                    {
                        Task.Run(() => { fragment.StartApiService(); }); 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment1()
        {
            try
            {
                if (FragmentListTab1.Count < 0) return;
                var currentFragment = FragmentListTab1[FragmentListTab1.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment2()
        {
            try
            {
                if (FragmentListTab2.Count < 0) return;
                var currentFragment = FragmentListTab2[FragmentListTab2.Count - 1];
                if (currentFragment != null)
                    DisplayFragment(currentFragment);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowFragment4()
        {
            try
            {
                if (FragmentListTab4.Count < 0) return;
                var currentFragment = FragmentListTab4[FragmentListTab4.Count - 1];
                if (currentFragment != null)
                {
                    DisplayFragment(currentFragment);

                    if (currentFragment is ProfileFragment fragment)
                    {
                        fragment.GetMyInfoData();
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
    }
}