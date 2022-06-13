using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Platformer.UI
{
	public class EnergyCurrent : Panel
	{

		public Label Number;
		public Label Text;
		public Label Bg;
		

		public EnergyCurrent()
		{
			Text = Add.Label( "Energy", "energynm" );
			Number = Add.Label( "", "energy" );
			Bg = Add.Label( "", "energybg" );
		}

		public override void Tick()
		{
			base.Tick();
			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is not PlatformerPawn pl ) return;
		
			SetClass( "active", pl.PlayerHasGlider );

			Number.Style.Width = Length.Fraction( Math.Max( pl.GliderEnergy / 120, 0 ) );
			Number.Style.Dirty();
		}
	}
}
