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
    public class LifestyleFragment : Android.Support.V4.App.Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView RelationshipIcon, SmokeIcon, DrinkIcon;
        private EditText EdtRelationship, EdtSmoke, EdtDrink;
        private string TypeDialog;
        public int IdRelationShip, IdSmoke, IdDrink;

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
                View view = inflater.Inflate(Resource.Layout.FilterLifestyleLayout, container, false);

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
                RelationshipIcon = view.FindViewById<TextView>(Resource.Id.IconRelationship);
                EdtRelationship = view.FindViewById<EditText>(Resource.Id.RelationshipEditText);

                SmokeIcon = view.FindViewById<TextView>(Resource.Id.IconSmoke);
                EdtSmoke = view.FindViewById<EditText>(Resource.Id.SmokeEditText);

                DrinkIcon = view.FindViewById<TextView>(Resource.Id.IconDrink);
                EdtDrink = view.FindViewById<EditText>(Resource.Id.DrinkEditText);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, RelationshipIcon, FontAwesomeIcon.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, SmokeIcon, FontAwesomeIcon.Smoking);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, DrinkIcon, FontAwesomeIcon.Beer);

                Methods.SetFocusable(EdtRelationship);
                Methods.SetFocusable(EdtSmoke);
                Methods.SetFocusable(EdtDrink);

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
                    EdtRelationship.Touch += EdtRelationshipOnClick;
                    EdtSmoke.Touch += EdtSmokeOnClick;
                    EdtDrink.Touch += EdtDrinkOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //RelationShip
        private void EdtRelationshipOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Relationship";
                //string[] relationshipArray = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
                var relationshipArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Relationship;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (relationshipArray != null) arrayAdapter.AddRange(relationshipArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_ChooseRelationshipStatus));
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

        //Drink
        private void EdtDrinkOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Drink";
                //string[] drinkArray = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
                var drinkArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Drink;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (drinkArray != null) arrayAdapter.AddRange(drinkArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Drink));
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

        //Smoke
        private void EdtSmokeOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Smoke";
                //string[] smokeArray = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
                var smokeArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Smoke;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (smokeArray != null) arrayAdapter.AddRange(smokeArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Smoke));
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
                if (TypeDialog == "Relationship")
                {
                    var relationshipArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Relationship?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdRelationShip = int.Parse(relationshipArray ?? "1");
                    EdtRelationship.Text = itemString.ToString();
                }
                else if (TypeDialog == "Smoke")
                {
                    var smokeArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Smoke?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdSmoke = int.Parse(smokeArray ?? "1");
                    EdtSmoke.Text = itemString.ToString();
                }
                else if (TypeDialog == "Drink")
                {
                    var drinkArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Drink?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdDrink = int.Parse(drinkArray ?? "1");
                    EdtDrink.Text = itemString.ToString();
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