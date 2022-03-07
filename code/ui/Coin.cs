using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace Sandbox.UI
{
	public class Coin : Panel
	{

		public Label Number;
		//public Label Text;
		public Image Image;


		public Coin()
		{


			Image = Add.Image( "ui/hud/coin.png", "coinimage" );
			Number = Add.Label( "12", "coinnumber" );
			//Text = Add.Label( "text", "lifetext" );


		}

		public override void Tick()
		{
			//Text.SetText( "	♡" );

			var player = Local.Pawn;
			if ( player == null ) return;


			if ( Local.Pawn is not PlatformerPawn pl ) return;

		}
	}
}
