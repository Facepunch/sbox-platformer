
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	public class LifeCurrent : Panel
	{

		public Label Number;
		public Label Icon;

		//public Image Image;

		public LifeCurrent()
		{
			//Image = Add.Image( "ui/hud/citizen/citizen.png", "playerimage" );
			Number = Add.Label( "", "number" );
			Icon = Add.Label( "favorite", "icon" );
		}

		public override void Tick()
		{
			if ( Local.Pawn is not PlatformerPawn pl ) return;

			var life = pl.NumberLife;

			Number.SetClass( "active", Tag.Current == null );
			Number.SetClass( "lifelow", life <= 1 );
			Number.Text = $"{life}";

			if ( life <= 1 )
			{
				LowHealth();
			}

			if ( life == 3 )
			{
				HighHealth();
			}
		}

		public void LowHealth()
		{
			//Image.SetTexture( "ui/hud/citizen/citizen_low.png" );
		}

		public void HighHealth()
		{
			//Image.SetTexture( "ui/hud/citizen/citizen.png" );
		}

	}
}
