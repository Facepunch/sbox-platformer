
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace Platformer.UI
{
	public partial class KeysCollected : Panel
	{

		private List<KeyPanel> KeyPanels = new();
		private static KeysCollected Current { get; set; }

		public KeysCollected()
		{

			Current = this;

		}

		public static void InitKeys()
		{
			Log.Info( Entity.All.OfType<KeyPickup>().Count() );
			
			foreach (var key in Entity.All.OfType<KeyPickup>())
			{
				Current.KeyPanels.Add( Current.Add.KeyPanel( key.KeyEmoji, "key1", key.KeyNumber ) );
				Log.Info(key.KeyNumber );
			}
		}

		public override void Tick()
		{

			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is not PlatformerPawn pl ) return;

			foreach ( var keypanel in KeyPanels )
			{
				keypanel.SetClass( "active", pl.KeysPlayerHas.Contains( keypanel.KeyNumber ) );
			}
		}
	}
}
