
using Sandbox.UI;
using Sandbox;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	public class NewArea : Panel
	{
		public static NewArea Instance;
		public float timesince = 0;
		public Label usetext;
		public Label Destination;

		public string CArea;

		public NewArea()
		{
			Destination = Add.Label( "", "destext" );

			StyleSheet.Load( "UI/Base/Area/NewArea.scss" );
		}

		public override void Tick()
		{

			var player = Game.LocalPawn;
			if ( player == null ) return;

			if ( Game.LocalPawn is not PlatformerPawn pl ) return;
			CArea = pl.CurrentArea;

			Destination.Text = $"{CArea}";

			base.Tick();

		}
	}
}
