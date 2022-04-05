
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
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
		}

		public override void Tick()
		{

			var player = Local.Pawn;
			if ( player == null ) return;


			if ( Local.Pawn is not PlatformerPawn pl ) return;
			var life = pl.NumberLife;

			_life = _life.LerpTo( life, Time.Delta * 0 );

		}
		public void HighHealth()
		{
			Image.SetTexture( "ui/hud/citizen/citizen.png" );
		}

	}
}
