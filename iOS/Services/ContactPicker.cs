using System;
using AddressBookUI;
using UIKit;

namespace PaketGlobal.iOS
{
    public static class ContactPicker
    {
        static Action<AddressBook.ABPerson> _callback;
        static ABPeoplePickerNavigationController picker;
        static void Init()
        {
            if (picker != null)
                return;
            picker = new ABPeoplePickerNavigationController();
            picker.Cancelled += (s, e) =>
            {
                picker.DismissModalViewController(true);
                _callback(null);
            };
            picker.SelectPerson2 += (s, e) =>
            {
                picker.DismissModalViewController(true);
                _callback(e.Person);
            };
        }
        public static void Select(UIViewController parent, Action<AddressBook.ABPerson> callback)
        {
            _callback = callback;
            Init();
            parent.PresentModalViewController(picker, true);
        }
    }
}
