using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI.Construct;

namespace Sandbox.UI
{
	public class KeyPanel : Label
	{
		public int KeyNumber { get; set; }
	}

	public static class KeyPanelConstructor
	{
		public static KeyPanel KeyPanel( this PanelCreator self, string image = null, string classname = null, int index = 0 )
		{
			KeyPanel keypanel = self.panel.AddChild<KeyPanel>();
			if ( image != null )
			{
				keypanel.Text=( image );
			}

			if ( classname != null )
			{
				keypanel.AddClass( classname );
			}

			keypanel.KeyNumber = index;

			return keypanel;
		}
	}
}
