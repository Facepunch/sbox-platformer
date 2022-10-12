
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

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

			if ( BaseGamemode.Instance == null ) return;

			var span = TimeSpan.FromSeconds( (BaseGamemode.Instance.StateTimer * 60).Clamp( 0, float.MaxValue ) );
			Timer.Text = span.ToString( @"hh\:mm" );
			State.Text = BaseGamemode.Instance.GameState.ToString();
		}
	}
}
