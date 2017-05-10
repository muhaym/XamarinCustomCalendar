using System;
using System.Collections.Generic;
using Android;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace XamarinCustomCalendar
{
	public class MonthAdapter : BaseAdapter
	{
		private DateTime mCalendar;
		private DateTime mCalendarToday;
		private Context mContext;
		private DisplayMetrics mDisplayMetrics;
		private List<String> mItems;
		private int mMonth;
		private int mYear;
		private int mDaysShown;
		private int mDaysLastMonth;
		private int mDaysNextMonth;
		private int mTitleHeight, mDayHeight;
		private string[] mDays = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
		private int[] mDaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		public MonthAdapter(Context c, int month, int year, DisplayMetrics metrics)
		{
			mContext = c;
			mMonth = month;
			mYear = year;
			mCalendar = new DateTime(mYear, mMonth, 1);
			mCalendarToday = DateTime.Now;
			mDisplayMetrics = metrics;
			populateMonth();
		}

		/**
		 * @param date - null if day title (0 - dd / 1 - mm / 2 - yy)
		 * @param position - position in item list
		 * @param item - view for date
		 */
		private int daysInMontha(int month)
		{
			int daysInMonth = mDaysInMonth[month];
			if (month == 1 && DateTime.IsLeapYear(mYear))
				daysInMonth++;
			return daysInMonth;
		}


		private void populateMonth()
		{
			mItems = new List<string>();
			foreach (string day in mDays)
			{
				mItems.Add(day);
				mDaysShown++;
			}

			int firstDay = getDay(mCalendar.DayOfWeek);
			int prevDay;
			if (mMonth == 0)
				prevDay = daysInMontha(11) - firstDay + 1;
			else
				prevDay = daysInMontha(mMonth - 1) - firstDay + 1;
			for (int i = 0; i < firstDay; i++)
			{
				mItems.Add(Convert.ToString(prevDay + i));
				mDaysLastMonth++;
				mDaysShown++;
			}

			int daysInMonth = daysInMontha(mMonth);
			for (int i = 1; i <= daysInMonth; i++)
			{
				mItems.Add(Convert.ToString(i));
				mDaysShown++;
			}

			mDaysNextMonth = 1;
			while (mDaysShown % 7 != 0)
			{
				mItems.Add(Convert.ToString(mDaysNextMonth));
				mDaysShown++;
				mDaysNextMonth++;
			}

			mTitleHeight = 40;
			int rows = (mDaysShown / 7);
			mDayHeight = (mDisplayMetrics.HeightPixels - mTitleHeight
					- (rows * 8) - getBarHeight()) / (rows - 1);
		}


		private int getBarHeight()
		{
			switch (mDisplayMetrics.DensityDpi)
			{
				case DisplayMetricsDensity.High:
					return 48;
				case DisplayMetricsDensity.Medium:
					return 32;
				case DisplayMetricsDensity.Low:
					return 24;
				default:
					return 48;
			}
		}

		private int getDay(DayOfWeek day)
		{
			switch (day)
			{
				case DayOfWeek.Monday:
					return 0;
				case DayOfWeek.Tuesday:
					return 1;
				case DayOfWeek.Wednesday:
					return 2;
				case DayOfWeek.Thursday:
					return 3;
				case DayOfWeek.Friday:
					return 4;
				case DayOfWeek.Saturday:
					return 5;
				case DayOfWeek.Sunday:
					return 6;
				default:
					return 0;
			}
		}

		private bool isToday(int day, int month, int year)
		{
			if (mCalendarToday.Month == month
					&& mCalendarToday.Year == year
					&& mCalendarToday.Day == day)
			{
				return true;
			}
			return false;
		}

		private int[] getDate(int position)
		{
			int[] date = new int[3];
			if (position <= 6)
			{
				return null; // day names
			}
			else if (position <= mDaysLastMonth + 6)
			{
				// previous month
				date[0] = int.Parse(mItems[position]);
				if (mMonth == 0)
				{
					date[1] = 11;
					date[2] = mYear - 1;
				}
				else
				{
					date[1] = mMonth - 1;
					date[2] = mYear;
				}
			}
			else if (position <= mDaysShown - mDaysNextMonth)
			{
				// current month
				date[0] = position - (mDaysLastMonth + 6);
				date[1] = mMonth;
				date[2] = mYear;
			}
			else
			{
				// next month
				date[0] = int.Parse(mItems[position]);
				if (mMonth == 11)
				{
					date[1] = 0;
					date[2] = mYear + 1;
				}
				else
				{
					date[1] = mMonth + 1;
					date[2] = mYear;
				}
			}
			return date;
		}


		void SetBackgroundColor(Color color, LinearLayout container)
		{
			if (container != null)
			{
				GradientDrawable bgShape = (GradientDrawable)container.Background;
				bgShape.SetColor(color);
				container.Background = bgShape;
			}
		}

		void SetTextColor(Color color, TextView text)
		{
			if (text != null)
			{
				text.SetTextColor(color);
			}
		}

		override public View GetView(int position, View convertView, ViewGroup parent)
		{
			View dateView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.calendar_item, parent, false);
			View headerView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.day_month, parent, false);
			TextView date = dateView.FindViewById<TextView>(Resource.Id.date);
			TextView status = dateView.FindViewById<TextView>(Resource.Id.date_icon);
			LinearLayout container = dateView.FindViewById<LinearLayout>(Resource.Id.holder);
			TextView dateHeader = headerView.FindViewById<TextView>(Resource.Id.dayTitle);
			date.Text = mItems[position];
			dateHeader.Text = mItems[position];
			SetBackgroundColor(Color.Rgb(243, 243, 243), container);
			int[] dateArray = getDate(position);
			if (dateArray != null)
			{
				date.SetHeight(mDayHeight);
				if (dateArray[1] != mMonth)
				{
					//If the date recieved is of previous or next month
					SetTextColor(Color.Rgb(189, 189, 189), date);
				}
				else
				{
					// If the date received is of current month
					SetTextColor(Color.Rgb(0, 171, 214), date);
					if (isToday(dateArray[0], dateArray[1], dateArray[2]))
					{
						SetBackgroundColor(Color.Rgb(189, 189, 189), container);
					}
				}
				return dateView;
			}
			else
			{
				var param = dateHeader.LayoutParameters;
				param.Height = mTitleHeight;
				dateHeader.LayoutParameters = param;
				return headerView;
			}

		}

		public override int Count
		{
			get
			{
				return mItems.Count;
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return mItems[position];
		}

		public override long GetItemId(int position)
		{
			return position;
		}
	}
}
