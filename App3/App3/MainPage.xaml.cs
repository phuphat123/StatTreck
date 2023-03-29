using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using Xamarin.Forms.Maps;
using Npgsql;



using App3.Helpers;
using Microcharts.Forms;

using SkiaSharp;
using System.Globalization;


namespace App3
{
    public partial class MainPage : ContentPage


    {
        //Database Connection
        string connString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
        NpgsqlConnection _conn;
        int pk;
        Page1 loginPage;

        //MAP Data
        List<Pin> pin;
        Xamarin.Forms.Picker picker;
        Xamarin.Forms.Picker screenTimePicker;
        
        




        public ChartView CurrentChart { get; set; }


        public MainPage(Page1 p)
        {
            InitializeComponent();
            BindingContext = this;
            

            loginPage = p;
            //settingspage
            
            
            pk = loginPage.getPrimaryKey();


            Data settings = new Data();
            StackLayout s = new StackLayout();
            Xamarin.Forms.Switch GPS_Switch = new Xamarin.Forms.Switch();
            GPS_Switch.AutomationId = "GPS_Switch";
            Xamarin.Forms.Switch ScreenT_Switch = new Xamarin.Forms.Switch();
            Button Test_Save = new Button();
            Test_Save.AutomationId = "Save_Button";

            //chartview
            c = new ChartView();


            ScreenT_Switch.AutomationId = "Screen_Switch";
            GPS_Switch.Toggled += Toggle_Clicked;
            ScreenT_Switch.Toggled += Toggle_Clicked;
            Test_Save.Clicked += Button_Save;

            s.Children.Add(new Label { Text = "Settings!" });
            s.Children.Add(new Label { Text = "GPS Toggle" });
            s.Children.Add(GPS_Switch);
            s.Children.Add(new Label { Text = "Screen Time Toggle" });
            s.Children.Add(ScreenT_Switch);
            //s.Children.Add(Test_Save);
            settings.Content = s;
            settings.BackgroundColor = Color.FromHex("#FFF2B3");

            var main = this; // adding settings button to mainpage
            main.ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = new FontImageSource
                {
                    FontFamily = "FA2",
                    Glyph = FontAwesome2.FontAwesomeIcons2.Wrench,
                    Size = 18,
                    Color = Color.White
                },
                Command = new Command(() =>
                {
                    if (main.IsBusy == true) { return; }
                    else { Navigation.PushAsync(settings); }
                })
            });

            

            Xamarin.Forms.NavigationPage.SetHasBackButton(main, false);


            //Date Pickers
            picker = new Xamarin.Forms.Picker
            {
                Title = "Select a Day",
            };
            screenTimePicker = new Xamarin.Forms.Picker
            {
                Title = "Select",
                
                
            };

            //actionlisteners
            GPS_Tapped.Tapped += Button_Clicked;
            Motion_Tapped.Tapped += Button_Clicked;
            ScreenTime_Tapped.Tapped += Button_Clicked;
            //TestButton.Clicked += Button_Clicked;
            picker.SelectedIndexChanged += DatePicked;
            Text_Tapped.Tapped += Button_Clicked;
            screenTimePicker.SelectedIndexChanged += screenTimePicked;
            

            pin = new List<Pin>();


        }
        private bool _isBusy;
        public new bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                ((BoxView)FindByName("Overlay")).IsVisible = _isBusy;
                ((StackLayout)FindByName("BusyIndicator")).IsVisible = _isBusy;
                ((ActivityIndicator)((StackLayout)FindByName("BusyIndicator")).Children[0]).IsRunning = _isBusy;
            }
        }


        //Google Maps for Map Feature
        Xamarin.Forms.Maps.Map map;
        private Xamarin.Forms.Maps.Polyline _polyline;
        int time;

        int items;
        int width = 1500;
        private async void screenTimePicked(object sender, EventArgs args)
        {
            if (screenTimePicker.SelectedItem == null) return; // Add this line to check for null

            // Get the selected date from the picker
            string selectedDate = screenTimePicker.SelectedItem.ToString();

            // Convert the selected date to a DateTime object
            DateTime date = DateTime.ParseExact(selectedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            // Fetch the data for the selected date from the database
            Dictionary<string, double> appUsageData = new Dictionary<string, double>();
            _conn = new NpgsqlConnection(connString);

            using (_conn)
            {
                _conn.Open();
                string query = "SELECT package_name, usage_duration FROM app_usage WHERE id = @id AND usage_date = @day";
                using (var command = new NpgsqlCommand(query, _conn))
                {
                    command.Parameters.AddWithValue("@id", pk);
                    command.Parameters.AddWithValue("@day", date);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string appName = reader.GetString(0);
                        double usageTime = reader.GetDouble(1);
                        appUsageData[appName] = usageTime;
                    }
                }
            }

            // Update the chart with the new data
            var chart = new Microcharts.BarChart()
            {
                Entries = appUsageData.Select(pair => new Microcharts.ChartEntry((float)pair.Value) { Label = pair.Key, ValueLabel = $"{pair.Value:F2} mins", Color = SKColor.Parse("#FF1493") }).ToList(),
                BackgroundColor = SKColor.Parse("#FFF2B3"),
                
            };
            items = appUsageData.Count;
            c.Chart = chart;
            width = items * 60;
            c.WidthRequest = width;
        }


        //Datepicker for GPS
        private void DatePicked(object sender, EventArgs args)
        {
            if (picker.SelectedItem == null) return; // Add this line to check for null

            Debug.WriteLine("DatePicked called!");
            var pins = map.Pins;
            
            for (int i = pins.Count-1; i >= 0; i--)
            {
                map.Pins.RemoveAt(i);
                
            }
            map.MapElements.Remove(_polyline);
            
            plotLocations(availableDaysGPS[picker.SelectedIndex]);
            Position tempPos = map.Pins[0].Position;
            var mapSpan = MapSpan.FromCenterAndRadius(tempPos, Distance.FromMeters(500));
            map.MoveToRegion(mapSpan);



        }
        Data GPSpage;

        //Test Button for saving AppUsageData
        private async void Button_Save(object s, EventArgs e) {
            IsBusy = true;
            pk = loginPage.getPrimaryKey();

            // Get the app usage time and store it in a database or file
            IAppUsageTracker appUsageTracker = DependencyService.Get<IAppUsageTracker>();

            Dictionary<string, double> appUsageTime = appUsageTracker.GetAppUsageTime();


            if (appUsageTracker.HasUsageAccessGranted())
            {
                // Use a background thread to fetch the app usage data
                await Task.Run(() =>
                {
                    
                    var appUsageData = appUsageTime.ToDictionary(pair => pair.Key, pair => (pair.Value));

                    try
                    {
                        _conn = new NpgsqlConnection(connString);

                        _conn.Open();
                        foreach (var appUsage in appUsageData)
                        {
                            var cmd = new NpgsqlCommand();
                            cmd.Connection = _conn;
                            cmd.CommandText = "INSERT INTO app_usage(id,package_name, usage_duration, usage_date) VALUES (@id,@package_name, @usage_duration, to_timestamp(@usage_date, 'YYYY-MM-DD HH24:MI:SS'))";
                            cmd.Parameters.AddWithValue("@id", pk);
                            cmd.Parameters.AddWithValue("@package_name", appUsage.Key);
                            cmd.Parameters.AddWithValue("@usage_duration", appUsage.Value);
                            cmd.Parameters.AddWithValue("@usage_date", DateTime.Today.ToString("yyyy-MM-dd") + " 00:00:00");
                            cmd.ExecuteNonQuery();
                        }
                        Debug.WriteLine("Successfully saved data");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {

                        _conn.Close();
                    }

                });
            }
            IsBusy = false;
        }
        //EventHandler for features

        List<DateTime> availableDaysST;
        List<DateTime> availableDaysGPS;

        ChartView c;
        private async void Button_Clicked(object sender, EventArgs e) 
        {
            try
            {
                //clear datepicker

                Debug.WriteLine("Button Clicked!");

                var button = sender;
                Data emptyPage = new Data();
                emptyPage.BackgroundColor = Color.FromHex("#FFF2B3");




                if (button == GPS)          //GPS Button
                {
                    if (GPSpage == null)
                    {
                        IsBusy = true; //loading screen

                        picker.Items.Clear();



                        emptyPage.Padding = new Thickness(5, 5, 5, 20);
                        availableDaysGPS = new List<DateTime>();
                        Debug.WriteLine("GPS Clicked");
                        StackLayout s = new StackLayout();
                        s.Children.Add(new Label { Text = "Location Tracking", HorizontalTextAlignment = TextAlignment.Center, FontFamily = "BUB2", FontSize = 20 });
                        s.Children.Add(picker);
                        map = new Xamarin.Forms.Maps.Map();

                        map.HeightRequest = 100;
                        map.WidthRequest = 200;
                        map.MapType = MapType.Street;
                        map.IsShowingUser = false;
                        _conn = new NpgsqlConnection(connString);
                        using (_conn)   //Gathering all the dates avaliable in the database.
                        {
                            _conn.Open();
                            string query = "SELECT DISTINCT date::date FROM coordinates WHERE id = @id";
                            using (var command = new NpgsqlCommand(query, _conn))
                            {
                                command.Parameters.AddWithValue("@id", pk);
                                var reader = command.ExecuteReader();

                                while (reader.Read())
                                {
                                    availableDaysGPS.Add(Convert.ToDateTime(reader["date"]));
                                }
                            }
                        }

                        foreach (DateTime day in availableDaysGPS) //Displaying all dates in a Picker object
                        {
                            picker.Items.Add(day.ToString("MM/dd/yyyy"));
                        }

                        s.Children.Add(map);
                        emptyPage.Content = s;
                        await Navigation.PushAsync(emptyPage);
                        GPSpage = emptyPage;
                        emptyPage = null;
                        IsBusy = false;

                    }
                    else
                    {
                        await Navigation.PushAsync(GPSpage);
                        IsBusy = false;
                    }
                }
                else if (button == Motion) //Motion Button
                {
                    IsBusy = true;
                    Debug.WriteLine("Motion Clicked");
                    StackLayout s = new StackLayout();

                    s.Children.Add(new Label { Text = "You have clicked motion" });

                    var motionPage = new Motion2();
                    motionPage.Shaken += (Shaken, args) =>
                    {
                        s.Children.Add(new Label { Text = "Shake detected" });
                        emptyPage.Content = s;
                    };
                    motionPage.ToggleAccelerometer();
                    await Navigation.PushAsync(emptyPage);
                    IsBusy = false;
                }
                else if (button == ScreenTime)
                {
                    

                    screenTimePicker.Items.Clear();
                    
                    
                    
                    availableDaysST = new List<DateTime>();
                    Debug.WriteLine("ScreenTime Clicked");
                    IsBusy = true;
                    StackLayout s = new StackLayout();
                    
                    s.Children.Add(screenTimePicker);
                    _conn = new NpgsqlConnection(connString);
                    using (_conn)   //Gathering all the dates avaliable in the database.
                    {
                        _conn.Open();
                        string query = "SELECT DISTINCT usage_date::date FROM app_usage WHERE id = @id";
                        using (var command = new NpgsqlCommand(query, _conn))
                        {
                            command.Parameters.AddWithValue("@id", pk);
                            var reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                availableDaysST.Add(Convert.ToDateTime(reader["usage_date"]));
                            }
                        }
                    }

                    foreach (DateTime day in availableDaysST) //Displaying all dates in a Picker object
                    {
                        screenTimePicker.Items.Add(day.ToString("MM/dd/yyyy"));
                    }

                    Xamarin.Forms.ScrollView scrollview = new Xamarin.Forms.ScrollView();
                    
                    s.Children.Add(scrollview);




                    var chart = new Microcharts.BarChart()
                    ;
                    
                    
                                 

                                chart.LabelTextSize = 20;
                    chart.AnimationDuration = TimeSpan.FromSeconds(10);
                                c.Chart = chart;
                                c.HeightRequest = 1500;
                                c.WidthRequest = 1500;

                    
                    

                    
                                
                    

                    scrollview.Content = c;
                                scrollview.Orientation = ScrollOrientation.Horizontal;
                    scrollview.WidthRequest = width;
                                emptyPage.Content = s;


                            

                    
                    await Navigation.PushAsync(emptyPage);
                    IsBusy = false;

                }
                else if (button == Text) { }

            }
            catch(Exception ex) { Debug.WriteLine("Error 5: "+ ex.Message); }
        }

        private void plotLocations(DateTime d)
        {
            try
            {
                string query = "SELECT latitude, longitude, date FROM coordinates WHERE id = @id AND date::date = @day ORDER BY date::time ASC";
                _conn = new NpgsqlConnection(connString);
                var cmd = new NpgsqlCommand(query, _conn);
                cmd.Parameters.AddWithValue("@id", pk);
                cmd.Parameters.AddWithValue("@day", d);

                using (_conn)
                {
                    _conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var positions = new List<Position>();
                        while (reader.Read())
                        {
                            double latitude = reader.GetDouble(0);
                            double longitude = reader.GetDouble(1);
                            DateTime date = reader.GetDateTime(2);
                            var position = new Position(latitude, longitude);
                            map.Pins.Add(new Pin { Type = PinType.Generic,Position = position, Label = date.Day + " , " + date.TimeOfDay });
                            
                            positions.Add(position);
                        }

                        if (positions.Count > 1)
                        {
                            var polyline = new Xamarin.Forms.Maps.Polyline
                            {
                                StrokeColor = Color.Blue,
                                StrokeWidth = 9,
                                //Geopath = { positions }
                            };

                            foreach (var pos in positions)
                            {
                                polyline.Geopath.Add(pos);
                            }
                            _polyline = polyline;
                            map.MapElements.Add(polyline);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Error 3: " + ex.Message); }
        }


        private async void Toggle_Clicked(object sender, ToggledEventArgs e)
        {
            bool isToggled = e.Value;
            Xamarin.Forms.Switch s = sender as Xamarin.Forms.Switch;

            if (isToggled && s.AutomationId == "GPS_Switch")
            {

                //GPS Permission Requesting
                System.Diagnostics.Debug.WriteLine("GPS_Switch Toggle On");
                var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("No Permission yet");

                    return;

                }
                if (status == PermissionStatus.Granted)
                {
                    DependencyService.Get<IStartService>().StartService("LocationService", pk);
                }
            }
            if (!isToggled && s.AutomationId == "GPS_Switch")
            {
                System.Diagnostics.Debug.WriteLine("GPS_Switch Toggle Off");
                DependencyService.Get<IStopService>().StopService("LocationService");
            }

            //AppTime Service
            if (isToggled && s.AutomationId == "Screen_Switch")
            {

                Debug.WriteLine("Screen_Time Toggled On");
                DependencyService.Get<IStartService>().StartService("ScreenTime", pk);
            }
            if (!isToggled && s.AutomationId == "Screen_Switch")
            {
                Debug.WriteLine("Screen_Time Toggled Off");
                DependencyService.Get<IStopService>().StopService("ScreenTime");
            }
        }


        public partial class Motion2 : ContentPage
        {
            SensorSpeed speed = SensorSpeed.Game;

            public event EventHandler Shaken;



            public void ToggleAccelerometer()
            {
                try
                {
                    if (Accelerometer.IsMonitoring)
                        Accelerometer.Stop();
                    else
                        Accelerometer.Start(speed);
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    Console.WriteLine("Feature not supported" + fnsEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error" + ex);
                }

                Shaken?.Invoke(this, EventArgs.Empty);
            }
        }




    }
}
