﻿using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaketGlobal
{
	public static class Utils
	{
		public static async Task<bool> CheckPermissions(Permission permission)
		{
			var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
			
            bool request = false;

			if (permissionStatus == PermissionStatus.Denied)
			{
				request = true;
			}

			if (request || permissionStatus != PermissionStatus.Granted)
			{
				var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(permission);
				
                if (newStatus.ContainsKey(permission) && newStatus[permission] != PermissionStatus.Granted)
				{
                    //TODO: show alert?
					//var title = $"{permission} Permission";
					//var question = $"To use the plugin the {permission} permission is required.";
					//var positive = "Settings";
					//var negative = "Maybe Later";
					//var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
					//if (task == null)
					//	return false;

					//var result = await task;
					//if (result)
					//{
					//	CrossPermissions.Current.OpenAppSettings();
					//}

                    return false;
				}
			}

			return true;
		}
	}
}
