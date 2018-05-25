using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace PaketGlobal
{
	public enum ValidationError
	{
		[Description ("None")] None, [Description ("Login")] Login, [Description ("Name")] Name, [Description ("Abn")] Abn,
		[Description ("Unit Number")] UnitNumber, [Description ("Street Number")] StreetNumber, [Description ("Street Name")] StreetName, [Description ("City Suburb")] CitySuburb,
		[Description ("Postcode")] Postcode, [Description ("Email")] Email, [Description ("Password")] Password, [Description ("Login Password")] LoginPassword,
		[Description ("Password Confirmation")] PasswordConfirmation, [Description ("First Name")] FirstName, [Description ("Last Name")] LastName, [Description ("Mobile Number")] MobileNumber,
		[Description ("Service Type")] ServiceType, [Description ("Floor Wall Designation")] FloorWallDesignation, [Description ("Surface Type")] SurfaceType, [Description ("Level")] Level,
		[Description ("Inspection Type")] InspectionType
	}

	public static class ValidationHelper
	{
		public static bool ValidateNumber (string link)
		{
			if (String.IsNullOrWhiteSpace (link)) {
				return false;
			}

			//var r = new Regex ("^(\\+\\d{1,2}\\s?)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$");
			//var match = r.Match (link);
			//return match.Success;
			return !link.Contains ("_");
		}

		public static bool ValidateEmail (string email)
		{
			if (String.IsNullOrWhiteSpace (email)) {
				return false;
			}

			var r = new Regex ("^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$");
			var match = r.Match (email);

			return match.Success;
		}

		public static bool ValidatePassword (string pass1)
		{
			if (String.IsNullOrWhiteSpace (pass1)) {
				return false;
			}

			if (pass1.Length > 50 || pass1.Length < 3)
				return false;
			
			return true;
		}

		public static bool ValidateTextField (string str)
		{
			if (String.IsNullOrWhiteSpace (str)) {
				return false;
			}

			if (str.Length > 56 || str.Length < 2)
				return false;

			return true;
		}

		public static bool ValidateName (string str)
		{
			if (String.IsNullOrWhiteSpace (str)) {
				return false;
			}

			if (str.Length > 50 || str.Length < 2)
				return false;
			
			return true;
		}

		public static bool ValidateCompany (string str)
		{
			if (String.IsNullOrWhiteSpace (str)) {
				return false;
			}

			if (str.Length > 50 || str.Length < 2)
				return false;
			
			return true;
		}
	}
}
