
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Platformer.UI
{
	public class KeysCollected : Panel
	{

		private List<KeyPanel> KeyPanels = new();

		public KeysCollected()
		{
			KeyPanels.Add( Add.KeyPanel( "album", "key1", 1 ) );
			KeyPanels.Add( Add.KeyPanel( "save", "key2", 2 ) );
			KeyPanels.Add( Add.KeyPanel( "folder", "key3", 3 ) );
			KeyPanels.Add( Add.KeyPanel( "work", "key4", 4 ) );

			//Image
			//KeyPanels.Add( Add.KeyPanel( "ui/hud/keys/key.png", "key1", 1 ) );
			//KeyPanels.Add( Add.KeyPanel( "ui/hud/keys/key.png", "key2", 2 ) );
			//KeyPanels.Add( Add.KeyPanel( "ui/hud/keys/key.png", "key3", 3 ) );
			//KeyPanels.Add( Add.KeyPanel( "ui/hud/keys/key.png", "key4", 4 ) );
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
