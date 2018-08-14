using System.Collections;
using ArrayList = Java.Util.ArrayList;

namespace PaketGlobal.Droid
{
    public static class CollectionExtensions
    {
        public static ArrayList ToArrayList(this ICollection input)
        {
            return input.Count == 0 ? new ArrayList() : new ArrayList(input);
        }
    }
}