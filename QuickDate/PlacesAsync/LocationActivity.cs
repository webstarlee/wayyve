using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Google.Places;
using Java.Lang;
using Plugin.Geolocator;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.PlacesAsync.Adapters;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using SearchView = Android.Support.V7.Widget.SearchView;
using Task = System.Threading.Tasks.Task;

namespace QuickDate.PlacesAsync
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class LocationActivity : Android.Support.V4.App.FragmentActivity, IOnMapReadyCallback, IOnSuccessListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private double Lat, Lng;
        private LocationManager LocationManager;
        private GoogleMap Map;
        private string Provider, DeviceAddress, SearchText;
        private SearchView SearchView;
        private GoogleApiClient GoogleApiClient;
        private TextView MapIcon, ListIcon;
        private FloatingActionButton BtnSelect;
        private LinearLayout ListButton, MapButton;
        private PlacesAdapter MAdapter;
        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.MapLayout);

                GoogleApiClient = new GoogleApiClient.Builder(this,this,this)
                    .EnableAutoManage(this, 0, this)
                    .AddApi(Android.Gms.Location.Places.PlacesClass.GEO_DATA_API)
                    .AddApi(Android.Gms.Location.Places.PlacesClass.PLACE_DETECTION_API)
                    .Build();

                InitializeLocationManager();
                 
                if (!PlacesApi.IsInitialized)
                    PlacesApi.Initialize(this, GetString(Resource.String.google_key));
                 
                //Get Value And Set Toolbar
                InitComponent();
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

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
                GoogleApiClient.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                GoogleApiClient.Disconnect();
                base.OnStop();
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

        #region Functions

        private async void InitComponent()
        {
            try
            {
                MapIcon = FindViewById<TextView>(Resource.Id.map_icon);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, MapIcon, IonIconsFonts.AndroidLocate);

                ListIcon = FindViewById<TextView>(Resource.Id.list_icon);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, ListIcon, IonIconsFonts.AndroidList);

                MapButton = FindViewById<LinearLayout>(Resource.Id.map_button);
                ListButton = FindViewById<LinearLayout>(Resource.Id.list_button);
               
                SearchView = FindViewById<SearchView>(Resource.Id.searchView);
                SearchView.SetQuery("", false);
                SearchView.SetIconifiedByDefault(false);
                SearchView.OnActionViewExpanded();
                SearchView.Iconified = false;
                SearchView.ClearFocus();

                //Change text colors
                var editText = (EditText)SearchView.FindViewById(Resource.Id.search_src_text);
                editText.SetHintTextColor(Color.ParseColor("#444444"));
                editText.SetTextColor(AppSettings.TitleTextColor);
                editText.Hint = GetText(Resource.String.Lbl_SearchForPlace);

                //Change Color Icon Search
                ImageView searchViewIcon = (ImageView)SearchView.FindViewById(Resource.Id.search_mag_icon); 
                searchViewIcon.SetColorFilter(AppSettings.TitleTextColor);
                 
                BtnSelect = FindViewById<FloatingActionButton>(Resource.Id.add_button);


                MAdapter = new PlacesAdapter(this);
                MAdapter.ItemClick += MAdapterOnItemClick;

                var mapFrag = SupportMapFragment.NewInstance();
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.map, mapFrag, mapFrag.Tag).Commit();
                mapFrag.GetMapAsync(this);
                  
                if (!string.IsNullOrEmpty(UserDetails.Lat) || !string.IsNullOrEmpty(UserDetails.Lng))
                {
                    Lat = Convert.ToDouble(UserDetails.Lat);
                    Lng = Convert.ToDouble(UserDetails.Lng); 
                     
                    OnLocationChanged();
                }
                else
                {
                    await GetPosition();
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
                    MapButton.Click += IconMyLocationOnClick;
                    ListButton.Click += ListButtonOnClick;
                    BtnSelect.Click += BtnSelectOnClick;
                    SearchView.QueryTextChange += SearchViewOnQueryTextChange;
                    SearchView.QueryTextSubmit += SearchViewOnQueryTextSubmit; 
                }
                else
                {
                    MapButton.Click -= IconMyLocationOnClick;
                    ListButton.Click -= ListButtonOnClick;
                    BtnSelect.Click -= BtnSelectOnClick;
                    SearchView.QueryTextChange -= SearchViewOnQueryTextChange;
                    SearchView.QueryTextSubmit -= SearchViewOnQueryTextSubmit;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Search View

        private async void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                SearchText = e.NewText;

                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText))
                    return;

                SearchView.ClearFocus();

                //Show a progress
                RunOnUiThread(() => { AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading)); });

                var latLng = await GetLocationFromAddress(SearchText.Replace(" ", ""));
                if (latLng != null)
                {
                    RunOnUiThread(() => { AndHUD.Shared.Dismiss(this); });

                    DeviceAddress = SearchText;

                    Lat = latLng.Latitude;
                    Lng = latLng.Longitude;

                    // Creating a marker
                    MarkerOptions markerOptions = new MarkerOptions();

                    // Setting the position for the marker
                    markerOptions.SetPosition(latLng);

                    var addresses = await ReverseGeocodeCurrentLocation(latLng);
                    if (addresses != null)
                    {
                        DeviceAddress = addresses.GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                        //string city = addresses.Locality;
                        //string state = addresses.AdminArea;
                        //string country = addresses.CountryName;
                        //string postalCode = addresses.PostalCode;
                        //string knownName = addresses.FeatureName; // Only if available else return NULL 

                        // Setting the title for the marker.
                        // This will be displayed on taping the marker
                        markerOptions.SetTitle(DeviceAddress);
                    }

                    // Clears the previously touched position
                    Map.Clear();

                    // Animating to the touched position
                    Map.AnimateCamera(CameraUpdateFactory.NewLatLng(latLng));

                    // Placing a marker on the touched position
                    Map.AddMarker(markerOptions);

                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(latLng);
                    builder.Zoom(18);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    Map.MoveCamera(cameraUpdate);
                }
                else
                {
                    RunOnUiThread(() => { AndHUD.Shared.Dismiss(this); });


                    //Error Message  
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                RunOnUiThread(() => { AndHUD.Shared.Dismiss(this); });
                Console.WriteLine(exception);
            }
        }

        private void SearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                SearchText = e.NewText;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Events
         
        private void ListButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                GetNearbyPlaces(); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void BtnSelectOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent();
                intent.PutExtra("Address", DeviceAddress);
                intent.PutExtra("latLng", Lat  + "," + Lng);
                SetResult(Result.Ok, intent);
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void IconMyLocationOnClick(object sender, EventArgs e)
        {
            try
            {
                await GetPosition();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Permissions 

        //Permissions
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        await GetPosition();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Location

        private void InitializeLocationManager()
        {
            try
            {
                LocationManager = (LocationManager)GetSystemService(LocationService);
                var criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                var acceptableLocationProviders = LocationManager.GetProviders(criteriaForLocationService, true);
                Provider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
                Console.WriteLine(Provider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Position GPS Current Location
        private async Task GetPosition()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    await CheckAndGetLocation();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                    {
                        await CheckAndGetLocation();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(105);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task CheckAndGetLocation()
        {
            try
            {
                if (!LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
                {

                }
                else
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 50;
                    var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
                    Console.WriteLine("Position Status: {0}", position.Timestamp);
                    Console.WriteLine("Position Latitude: {0}", position.Latitude);
                    Console.WriteLine("Position Longitude: {0}", position.Longitude);

                    Lat = position.Latitude;
                    Lng = position.Longitude;

                    var dd = locator.StopListeningAsync();
                    Console.WriteLine(dd);

                    OnLocationChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation(LatLng latLng)
        {
            try
            {
                var locale = Resources.Configuration.Locale;
                Geocoder geocode = new Geocoder(this, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                if (addresses.Count > 0)
                {
                    //string address = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                    //string city = addresses[0].Locality;
                    //string state = addresses[0].AdminArea;
                    //string country = addresses[0].CountryName;
                    //string postalCode = addresses[0].PostalCode;
                    //string knownName = addresses[0].FeatureName; // Only if available else return NULL 
                }
                else
                {
                    //Error Message  
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short).Show();
                }

                return addresses.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private async Task<LatLng> GetLocationFromAddress(string strAddress)
        {
            var locale = Resources.Configuration.Locale;
            Geocoder coder = new Geocoder(this, locale);

            try
            {
                var address = await coder.GetFromLocationNameAsync(strAddress, 2);
                if (address == null)
                    return null;

                Address location = address[0];
                Lat = location.Latitude;
                Lng = location.Longitude;

                LatLng p1 = new LatLng(Lat, Lng);
                return p1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                Map = googleMap;

                var makerOptions = new MarkerOptions();
                makerOptions.SetPosition(new LatLng(Lat, Lng));
                makerOptions.SetTitle(GetText(Resource.String.Lbl_Location));

                Map.AddMarker(makerOptions);
                Map.MapType = GoogleMap.MapTypeNormal;

                //Optional
                googleMap.UiSettings.ZoomControlsEnabled = true;
                googleMap.UiSettings.CompassEnabled = true;

                OnLocationChanged();

                googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());

                LatLng location = new LatLng(Lat, Lng);

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);

                CameraPosition cameraPosition = builder.Build();

                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                googleMap.MoveCamera(cameraUpdate);

                googleMap.MapClick += async (sender, e) =>
                {
                    try
                    {
                        LatLng latLng = e.Point;
                        var tapTextView = "Tapped: Point=" + e.Point;
                        Console.WriteLine(tapTextView);

                        Lat = latLng.Latitude;
                        Lng = latLng.Longitude;

                        // Creating a marker
                        MarkerOptions markerOptions = new MarkerOptions();

                        // Setting the position for the marker
                        markerOptions.SetPosition(e.Point);

                        var addresses = await ReverseGeocodeCurrentLocation(latLng);
                        if (addresses != null)
                        {
                            DeviceAddress = addresses.GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                            //string city = addresses.Locality;
                            //string state = addresses.AdminArea;
                            //string country = addresses.CountryName;
                            //string postalCode = addresses.PostalCode;
                            //string knownName = addresses.FeatureName; // Only if available else return NULL 

                            // Setting the title for the marker.
                            // This will be displayed on taping the marker
                            markerOptions.SetTitle(DeviceAddress);
                        }

                        // Clears the previously touched position
                        googleMap.Clear();

                        // Animating to the touched position
                        googleMap.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Point));

                        // Placing a marker on the touched position
                        googleMap.AddMarker(markerOptions);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };

                googleMap.MapLongClick += (sender, e) =>
                {
                    try
                    {
                        var tapTextView = "Long Pressed: Point=" + e.Point;
                        Console.WriteLine(tapTextView);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };

                googleMap.CameraChange += (sender, e) =>
                {
                    try
                    {
                        var cameraTextView = e.Position.ToString();
                        Console.WriteLine(cameraTextView);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void OnLocationChanged()
        {
            try
            { 
                // Creating a marker
                MarkerOptions markerOptions = new MarkerOptions();

                LatLng latLng = new LatLng(Lat, Lng);
                // Setting the position for the marker
                markerOptions.SetPosition(latLng);
                markerOptions.SetTitle(GetText(Resource.String.Lbl_Title_Location));

                var locale = Resources.Configuration.Locale;
                Geocoder geocode = new Geocoder(this, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                if (addresses.Count > 0)
                {
                    DeviceAddress = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                    //string city = addresses[0].Locality;
                    //string state = addresses[0].AdminArea;
                    //string country = addresses[0].CountryName;
                    //string postalCode = addresses[0].PostalCode;
                    //string knownName = addresses[0].FeatureName; // Only if available else return NULL 
                    
                    // Setting the title for the marker.
                    // This will be displayed on taping the marker
                    markerOptions.SetSnippet(DeviceAddress); 
                }
                 
                if (Map != null)
                {
                    Map.Clear();

                    Map.AddMarker(markerOptions);
                    //Map.SetOnInfoWindowClickListener(this); // Add event click on marker icon
                    Map.MapType = GoogleMap.MapTypeNormal;

                    var builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(Lat, Lng));
                    var cameraPosition = builder.Zoom(17).Target(new LatLng(Lat, Lng)).Build();
                    cameraPosition.Zoom = 18;

                    var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    Map.MoveCamera(cameraUpdate);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public void OnConnectionFailed(ConnectionResult result)
        {
            try
            {
                Toast.MakeText(this, "Failed to Connect to Google Services", ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void OnConnected(Bundle connectionHint)
        {
             
        }

        public void OnConnectionSuspended(int cause)
        {

        }

        #region Nearby Places

        private void GetNearbyPlaces()
        {
            try
            {
                var placesClient = PlacesApi.CreateClient(this);
                List<Place.Field> placeFields = new List<Place.Field> { Place.Field.Name, Place.Field.Address };

                FindCurrentPlaceRequest currentPlaceRequest = FindCurrentPlaceRequest.NewInstance(placeFields);
                var currentPlaceTask = placesClient.FindCurrentPlace(currentPlaceRequest);
                currentPlaceTask.AddOnSuccessListener(this, this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                var findCurrentPlaceResponse = (FindCurrentPlaceResponse)result;
                if (findCurrentPlaceResponse == null) return;

                MAdapter.PlacesList = new ObservableCollection<MyPlace>();
                foreach (var placeLikelihood in findCurrentPlaceResponse.PlaceLikelihoods)
                { 
                    MAdapter.PlacesList.Add(new MyPlace()
                    {
                        Address = placeLikelihood.Place.Address,
                        AddressComponents = placeLikelihood.Place.AddressComponents,
                        Attributions = placeLikelihood.Place.Attributions,
                        Id = placeLikelihood.Place.Id,
                        LatLng = placeLikelihood.Place.LatLng,
                        Name = placeLikelihood.Place.Name,
                        OpeningHours = placeLikelihood.Place.OpeningHours,
                        PhoneNumber = placeLikelihood.Place.PhoneNumber,
                        PhotoMetadatas = placeLikelihood.Place.PhotoMetadatas,
                        PlusCode = placeLikelihood.Place.PlusCode,
                        PriceLevel = placeLikelihood.Place.PriceLevel,
                        Rating = placeLikelihood.Place.Rating,
                        UserRatingsTotal = placeLikelihood.Place.UserRatingsTotal,
                        Viewport = placeLikelihood.Place.Viewport,
                        WebsiteUri = placeLikelihood.Place.WebsiteUri,
                    });
                }
                MAdapter.NotifyDataSetChanged();

                var dialogList = new MaterialDialog.Builder(this);
                dialogList.Title(Resource.String.Lbl_NearBy); 
                dialogList.Adapter(MAdapter,new LinearLayoutManager(this));
                dialogList.AutoDismiss(true);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.Build().Show(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MAdapterOnItemClick(object sender, PlacesAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    Intent intent = new Intent();
                    intent.PutExtra("Address", item.Address);
                    intent.PutExtra("latLng", item.LatLng.Latitude + "," + item.LatLng.Longitude);
                    SetResult(Result.Ok, intent);
                    Finish();
                }
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