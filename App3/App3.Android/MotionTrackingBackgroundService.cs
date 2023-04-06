using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;

using Android.OS;
using Android.Util;
using Android.Widget;
using Npgsql;
using Xamarin.Essentials;
using System.Threading;

namespace App3.Droid
{
    /*
     * This class runs the background service to obtain data for the accelerometer.
     */
    [Service]
    public class MotionTrackingBackgroundService : Service
    {
        private static readonly string TAG = typeof(MotionTrackingBackgroundService).FullName;

        private MotionTrackingBackgroundService _accelerometerService;
        private Task _task;
        private NotificationManager _notificationManager;
        private NotificationChannel _channel;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        int primaryKey;
        CancellationTokenSource _cancellationTokenSource;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            primaryKey = intent.GetIntExtra("PrimaryKey", -1);
            System.Diagnostics.Debug.WriteLine("Primary Key Retrieved M.T.S : " + primaryKey);

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _channel = new NotificationChannel("app", "app", NotificationImportance.High);
            _notificationManager.CreateNotificationChannel(_channel);

            var notification = new Notification.Builder(this, "app")
                .SetContentTitle("Motion Tracking")
                .SetContentText("Motion Tracking currently on")
                .SetSmallIcon(Resource.Drawable.ic_stat_360)
                .Build();
            StartForeground(9, notification);

            _cancellationTokenSource = new CancellationTokenSource();
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Accelerometer.Start(SensorSpeed.UI);

            return StartCommandResult.Sticky;
        }

        public void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            _task = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var accelerationX = e.Reading.Acceleration.X;
                    var accelerationY = e.Reading.Acceleration.Y;
                    var accelerationZ = e.Reading.Acceleration.Z;
                    System.Diagnostics.Debug.WriteLine("X: " + accelerationX + "Y: " + accelerationY + "Z: " + accelerationZ);
                    SaveData(accelerationX, accelerationY, accelerationZ);
                    await Task.Delay(5000, _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token);
        }


        string connectionString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
        
        //This function saves the accelerometer values to the database
        public void SaveData(double accelerationX, double accelerationY, double accelerationZ)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO motion (id,acceleration_x, acceleration_y, acceleration_z) VALUES (@id,@acceleration_x, @acceleration_y, @acceleration_z)";
                    cmd.Parameters.AddWithValue("id", primaryKey);
                    cmd.Parameters.AddWithValue("acceleration_x", accelerationX);
                    cmd.Parameters.AddWithValue("acceleration_y", accelerationY);
                    cmd.Parameters.AddWithValue("acceleration_z", accelerationZ);
                    cmd.ExecuteNonQuery();

                }
            }
        }

        //This kills the accelerometer service
        public override void OnDestroy()
        {
            base.OnDestroy();

            Log.Debug(TAG, "OnDestroy called");
            Accelerometer.Stop();
            Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;

            _cancellationTokenSource.Cancel(); // Cancel the running task
            _accelerometerService = null;
        }
    }
}