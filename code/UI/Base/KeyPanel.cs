
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
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
				keypanel.SetText ( image );
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
