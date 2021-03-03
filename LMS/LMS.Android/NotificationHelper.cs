using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using LMS.Droid;
using LMS.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationHelper))]
namespace LMS.Droid
{
    public class NotificationHelper : INotification
    {
        private Context context;
        private NotificationManager notificationManager;
        private NotificationCompat.Builder builder;
        public static string NOTIFICATION_CHANNEL_ID = "10023";

        public NotificationHelper()
        {
            context = global::Android.App.Application.Context;
        }

        [Obsolete]
        public void CreateNotification(String title, String message)
        {
            try
            {
                var intent = new Intent(context, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra(title, message);
                var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);


                builder = new NotificationCompat.Builder(context);
                builder.SetSmallIcon(Resource.Drawable.profile);
                builder.SetContentTitle(title)
                    
                        .SetAutoCancel(true)
                        .SetContentTitle(title)
                        .SetContentText(message)
                        .SetChannelId(NOTIFICATION_CHANNEL_ID)
                        .SetPriority((int)NotificationPriority.High)
                        .SetVibrate(new long[0])
                        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                        .SetVisibility((int)NotificationVisibility.Public)
                        .SetSmallIcon(Resource.Drawable.profile)
                        .SetContentIntent(pendingIntent);

                NotificationManager notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;

                if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.O)
                {
                    NotificationImportance importance = global::Android.App.NotificationImportance.High;

                    NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, title, importance);
                    notificationChannel.EnableLights(true);
                    notificationChannel.EnableVibration(true);
                    notificationChannel.SetShowBadge(true);
                    notificationChannel.Importance = NotificationImportance.High;
                    notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                    if (notificationManager != null)
                    {
                        builder.SetChannelId(NOTIFICATION_CHANNEL_ID);
                        notificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }

                notificationManager.Notify(0, builder.Build());
            }
            catch (Exception ex)
            {
                //
            }
        }
    }

    
}