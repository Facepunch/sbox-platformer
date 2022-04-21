
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Platformer.UI
{
	public class RoundTimer : Panel
	{

		public Label Timer;
		public Image image;
		public Label State;

		public RoundTimer()
		{
			Timer = Add.Label( string.Empty, "game-timer" );
			image = Add.Image( "ui/hud/clock.png", "game-icon" );
			State = Add.Label( string.Empty, "game-state" );
		}

		public override void Tick()
		{
			base.Tick();

			var game = Game.Current as Platformer;
			if ( !game.IsValid() ) return;

			var span = TimeSpan.FromSeconds( (game.StateTimer * 60).Clamp( 0, float.MaxValue ) );

			Timer.Text = span.ToString( @"hh\:mm" );
			State.Text = game.GameState.ToString();
		}
	}
}
