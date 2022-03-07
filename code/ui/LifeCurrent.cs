using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace Sandbox.UI
{
	public class LifeCurrent : Panel
	{

		public Label Number;
		//public Label Text;
		public Image Image;

		private float _life;


		public LifeCurrent()
		{


			Image = Add.Image( "ui/hud/citizen/citizen.png", "playerimage" );
			Number = Add.Label( "", "number" );
			//Text = Add.Label( "text", "lifetext" );


		}

		public override void Tick()
		{
			//Text.SetText( "	♡" );

			var player = Local.Pawn;
			if ( player == null ) return;


			if ( Local.Pawn is not PlatformerPawn pl ) return;
			var life = pl.NumberLife;

			_life = _life.LerpTo( life, Time.Delta * 0 );

			Number.SetClass( "lifelow", life <= 1 );

			Number.Text = $"{life}";

			if (life <= 1 )
			{
				LowHealth();
			}
			if (life == 3)
			{
				HighHealth();
			}

		}

		public void LowHealth()
		{
			Image.SetTexture( "ui/hud/citizen/citizen_low.png" );
		}
		public void HighHealth()
		{
			Image.SetTexture( "ui/hud/citizen/citizen.png" );
		}

	}
}
