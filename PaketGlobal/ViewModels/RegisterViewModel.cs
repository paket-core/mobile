﻿using System;
namespace PaketGlobal
{
	public class RegisterViewModel : BaseViewModel
	{
		private string userName;
		private string fullName;
		private string phoneNumber;
        private string address;

		public string UserName {
			get { return userName; }
			set { SetProperty(ref userName, value); }
		}

		public string FullName {
			get { return fullName; }
			set { SetProperty(ref fullName, value); }
		}

		public string PhoneNumber {
			get { return phoneNumber; }
			set { SetProperty(ref phoneNumber, value); }
		}


        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

		public override void Reset()
		{
			base.Reset();

			UserName = null;
			FullName = null;
			PhoneNumber = null;
            Address = null;
		}
	}
}
