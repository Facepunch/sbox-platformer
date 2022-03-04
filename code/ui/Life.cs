using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sandbox.UI
{
	public class Life : Panel
	{
		public Label Label;
		private float _life;


		public Life()
		{
			Label = Add.Label();
		}

		public override void Tick()
		{

			var player = Local.Pawn;
			if ( player == null ) return;


			if ( Local.Pawn is not PlatformerPawn pl ) return;
			var life = pl.NumberLife;

			_life = _life.LerpTo( life, Time.Delta * 0 );

			Label.SetClass( "lifelow", life <= 1 );

			Label.Text = $"{life}";
		}

	}
}
