using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Util;

namespace XamarinCustomCalendar
{
	[Activity(Label = "Xamarin Custom Calendar", MainLauncher = true)]
	public class MainActivity : Activity
	{
		DateTime mCalendar;
		int[] mToday = new int[3];
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			GridView gridview = FindViewById<GridView>(Resource.Id.gridview);
			mCalendar = DateTime.Now;
			mToday[0] = mCalendar.Day;
			mToday[1] = mCalendar.Month + 1;
			mToday[2] = mCalendar.Year;

			// get display metrics
			DisplayMetrics metrics = new DisplayMetrics();
			WindowManager.DefaultDisplay.GetMetrics(metrics);

			// set adapter
			gridview = (GridView)FindViewById(Resource.Id.gridview);
			gridview.Adapter = new MonthAdapter(this, mToday[1], mToday[2], metrics);
		}
	}
}

