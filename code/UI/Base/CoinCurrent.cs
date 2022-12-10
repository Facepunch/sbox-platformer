
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	public class CoinCurrent : Panel
	{

		//public Image Image;
		public Label Icon;
		public Label Number;


		public CoinCurrent()
		{

			//Image = Add.Image( "ui/hud/coin.png", "coinimage" );
			Icon = Add.Label( "paid", "icon" );
			Number = Add.Label( "", "coinnumber" );

		}

		public override void Tick()
		{


			var player = Game.LocalPawn;
			if ( player == null ) return;

			if ( Game.LocalPawn is not PlatformerPawn pl ) return;
			var Coin = pl.Coin;

			if(Platformer.Mode != Platformer.GameModes.Tag)
			{
				SetClass( "active", true );
			}

			Number.Text = $"{Coin}";
		}
	}
}
