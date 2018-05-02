using System;

namespace PaketGlobal
{
	public class MenuItem
	{
		//public Graphics.Elements.GetIconDelegate IconName { get; set; }
		public string IconName { get; set; }
		public string Text { get; set; }
		public Action OnItemTouched { get; set; }
		public bool HasSeparator { get; set; }
	}
}

