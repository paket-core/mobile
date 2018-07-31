//------------------------------------------------------------------------------
// HACK: not auto-generated
//
// in a Shared Project with Xamarin Studio, this file does NOT
// get auto-generated like you'd expect. You MUST keep this file
// manually in-sync wit the AppResources.resx XML.
//------------------------------------------------------------------------------


namespace PaketGlobal
{
    using System;
    using System.Reflection;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AppResources
    {

        private static System.Resources.ResourceManager resourceMan;

        private static System.Globalization.CultureInfo resourceCulture;

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources()
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.Equals(null, resourceMan))
                {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("PaketGlobal.Resources", typeof(AppResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string Copied
        {
            get
            {
                return ResourceManager.GetString("Copied", resourceCulture);
            }
        }

        internal static string ActivationEth
        {
            get
            {
                return ResourceManager.GetString("ActivationEth", resourceCulture);
            }
        }

        internal static string AddInfo
        {
            get
            {
                return ResourceManager.GetString("AddInfo", resourceCulture);
            }
        }

        internal static string CompleteRegistration
        {
            get
            {
                return ResourceManager.GetString("CompleteRegistration", resourceCulture);
            }
        }

        internal static string InvalidPubKey
        {
            get
            {
                return ResourceManager.GetString("InvalidPubKey", resourceCulture);
            }
        }

        internal static string UserNotFound
        {
            get
            {
                return ResourceManager.GetString("UserNotFound", resourceCulture);
            }
        }

        internal static string SelectDeadlineDate
        {
            get
            {
                return ResourceManager.GetString("SelectDeadlineDate", resourceCulture);
            }
        }

        internal static string UserNotRegistered
        {
            get
            {
                return ResourceManager.GetString("UserNotRegistered", resourceCulture);
            }
        }

        internal static string UserNotTrusted
        {
            get
            {
                return ResourceManager.GetString("UserNotTrusted", resourceCulture);
            }
        }

    }
}
