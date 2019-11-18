using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using Exception = System.Exception;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class LooksFragment : Android.Support.V4.App.Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView BodyIcon, HeightIcon;
        private EditText  EdtBody, EdtFromHeight, EdtToHeight; 
        private string TypeDialog;
        public int IdBody;
        public string FromHeight = "139", ToHeight = "220";
          
        #endregion 

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (SearchFilterTabbedActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.FilterLooksEditLayout, container, false);

                InitComponent(view);

                AddOrRemoveEvent(true);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                BodyIcon = view.FindViewById<TextView>(Resource.Id.IconBody);
                EdtBody = view.FindViewById<EditText>(Resource.Id.BodyEditText);

                HeightIcon = view.FindViewById<TextView>(Resource.Id.IconHeight);
                EdtFromHeight = view.FindViewById<EditText>(Resource.Id.FromHeightEditText);
                EdtToHeight = view.FindViewById<EditText>(Resource.Id.ToHeightEditText);
                 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BodyIcon, FontAwesomeIcon.Male);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, HeightIcon, FontAwesomeIcon.TextHeight);

                Methods.SetFocusable(EdtBody);
                Methods.SetFocusable(EdtFromHeight);
                Methods.SetFocusable(EdtToHeight);
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
                    EdtBody.Touch += EdtBodyOnClick;
                    EdtFromHeight.Touch += EdtHeightOnClick;
                    EdtToHeight.Touch += EdtToHeightOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
      
        #region Events

        //Body
        private void EdtBodyOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Body";
                //string[] bodyArray = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
                var bodyArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Body;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (bodyArray != null) arrayAdapter.AddRange(bodyArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_BodyType));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void EdtHeightOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "FromHeight";
                //string[] heightArray = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
                var heightArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Height;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (heightArray != null) arrayAdapter.AddRange(heightArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_FromHeight));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void EdtToHeightOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "ToHeight";
                //string[] heightArray = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
                var heightArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Height;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (heightArray != null) arrayAdapter.AddRange(heightArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_ToHeight));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Body")
                {
                    var bodyArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Body?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdBody = int.Parse(bodyArray ?? "1");
                    EdtBody.Text = itemString.ToString();
                }
                else if (TypeDialog == "FromHeight")
                {
                    FromHeight = ListUtils.SettingsSiteList.FirstOrDefault()?.Height?[itemId]?.Keys.FirstOrDefault() ?? "139";
                    EdtFromHeight.Text = itemString.ToString();
                }
                else if (TypeDialog == "ToHeight")
                {
                    ToHeight = ListUtils.SettingsSiteList.FirstOrDefault()?.Height?[itemId]?.Keys.FirstOrDefault() ?? "220";
                    EdtToHeight.Text = itemString.ToString();
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

    }
}