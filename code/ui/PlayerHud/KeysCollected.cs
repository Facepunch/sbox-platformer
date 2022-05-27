
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace Platformer.UI
{
	public partial class KeysCollected : Panel
	{

		private List<KeyPanel> KeyPanels = new();
		private bool Built;

		public override void Tick()
		{
			// build this once on first tick, theoretically all entities exist by now
			if ( !Built )
			{
				Built = true;

				foreach ( var key in Entity.All.OfType<KeyPickup>() )
				{
					KeyPanels.Add( Add.KeyPanel( $"{key.KeyIcon}", "key1", key.KeyNumber ) );
				}
			}

			if ( Platformer.CurrentGameMode == Platformer.GameModes.Competitive )
			{
				var player = Local.Pawn;
				if ( player == null ) return;

				if ( Local.Pawn is not PlatformerPawn pl ) return;

				foreach ( var keypanel in KeyPanels )
				{
					keypanel.SetClass( "active", pl.KeysPlayerHas.Contains( keypanel.KeyNumber ) );

				}
			}
			if ( Platformer.CurrentGameMode == Platformer.GameModes.Coop )
			{
				var player = Local.Pawn;
				if ( player == null ) return;

				foreach ( var keypanel in KeyPanels )
				{
					keypanel.SetClass( "active", Platformer.Current.KeysAllPlayerHas.Contains( keypanel.KeyNumber ) );
				}
			}
		}
	}
}
