using System;
using System.Globalization;

using Xamarin.Forms;

namespace PaketGlobal
{

    public class PackageIconStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                      object parameter, CultureInfo culture)
        {
            var status = (string)value;

            if (status == "waiting pickup")
            {
                return "waiting_status_icon.png";
            }
            else if (status == "delivered")
            {
                return "delivered_status_icon.png";
            }

            return "in_transit_status.png";
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PackageProgressIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                      object parameter, CultureInfo culture)
        {
            var status = (string)value;

            if (status == "delivered")
            {
                return "point_0";
            }

            return "point_1";
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PackageProgressStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                      object parameter, CultureInfo culture)
        {
            var status = (string)value;

            if (status == "waiting pickup")
            {
                return 0.1f;
            }
            else if (status == "delivered")
            {
                return 1.0f;
            }

            return 0.5f;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

	public class DeliveryStatusToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			var status = (DeliveryStatus)value;

			switch (status) {
				case DeliveryStatus.InTransit:
					return "In Transit";
				case DeliveryStatus.Delivered:
					return "Delivered";
				case DeliveryStatus.DeadlineExpired:
					return "Deadline Expired";
				case DeliveryStatus.Closed:
					return "Closed";
				default:
					return "Unknown";
			}
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class DeliveryStatusToRefundVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			var status = (DeliveryStatus)value;
			return status == DeliveryStatus.DeadlineExpired;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RefundStatusToReclaimVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			var status = (DeliveryStatus)value;
			return status == DeliveryStatus.Delivered;
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class PaketRoleToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
							  object parameter, CultureInfo culture)
		{
			var role = (PaketRole)value;

			switch (role) {
				case PaketRole.Launcher:
					return "Launcher";
				case PaketRole.Courier:
					return "Courier";
				case PaketRole.Recipient:
					return "Recipient";
				default:
					return "Unknown";
			}
		}

		public object ConvertBack(object value, Type targetType,
								  object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
