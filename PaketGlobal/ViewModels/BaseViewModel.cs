using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using PaketGlobal.ClientService;

namespace PaketGlobal
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		public ServiceClient ServiceClient {
			get { return GetInstance<ServiceClient>(); }
		}

		public Workspace Workspace {
			get { return GetInstance<Workspace>(); }
		}

		public Profile Profile {
			get { return GetInstance<Profile>(); }
		}

		protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null, Action<T> onChanging = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			onChanging?.Invoke(value);

			OnPropertyChanging(propertyName);

			backingStore = value;

			onChanged?.Invoke();

			OnPropertyChanged(propertyName);
		}

		public void OnPropertyChanging(string propertyName)
		{
			PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
		}

		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public T GetInstance<T>()
		{
			return GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<T>();
		}

		public virtual void Reset()
		{

		}
	}
}
