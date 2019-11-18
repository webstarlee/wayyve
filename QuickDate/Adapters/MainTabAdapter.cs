using System;
using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang; 
using Exception = Java.Lang.Exception;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;


namespace QuickDate.Adapters
{
    public class MainTabAdapter : FragmentStatePagerAdapter
    {
        private List<SupportFragment> Fragments { get; set; }
        private List<string> FragmentNames { get; set; }

        public MainTabAdapter(SupportFragmentManager sfm) : base(sfm)
        {
            try
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearFragment()
        {
            try
            {
                Fragments.Clear();
                FragmentNames.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Remove(fragment);
                FragmentNames.Remove(name);
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InsertFragment(int index, SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Insert(index, fragment);
                FragmentNames.Insert(index, name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int Count
        {
            get { return Fragments.Count; }
        }

        public override SupportFragment GetItem(int position)
        {
            try
            {
                if (Fragments[position] != null)
                {
                    return Fragments[position];
                }
                else
                {
                    return Fragments[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(FragmentNames[position]);
        }

       
    }
}