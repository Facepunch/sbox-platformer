
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Platformer.UI
{
	public class TagRoundHud : Panel
	{

		public Label Text;
		public Image image;
		public Label State;

		public TagRoundHud()
		{
			Text = Add.Label( string.Empty, "round-number" );
			//image = Add.Image( "ui/hud/tag/not-tagged.png", "round-icon" );
			State = Add.Label( string.Empty, "round-text" );
		}

		public override void Tick()
		{
			base.Tick();

			var game = Game.Current as Platformer;
			if ( !game.IsValid() ) return;

			SetClass( "active", Platformer.CurrentGameMode == Platformer.GameModes.Tag );

			Text.Text = $"{game.RoundNumber}";
			State.Text = "Round";

		}
	}
}
