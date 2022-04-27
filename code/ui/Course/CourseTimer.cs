
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI;

public class CourseTimer : Panel
{

	private Label Timer;

	private TimeSince time;

	public CourseTimer() => Timer = Add.Label( string.Empty, "timer" );

	public override void Tick()
	{
		base.Tick();

		var pawn = Local.Pawn as PlatformerPawn;
		if ( !pawn.IsValid() ) return;

		if ( Platformer.CurrentGameMode == Platformer.GameModes.Competitive )
		{
			time = pawn.TimeSinceStart;
			if ( pawn.TimerState != TimerState.Live )
			{
				time = 0;
			}
			Timer.Text = TimeSpan.FromSeconds( (time * 60).Clamp( 0, float.MaxValue ) ).ToString( @"hh\:mm\:ss" );
		}

		if ( Platformer.CurrentGameMode == Platformer.GameModes.Coop )
		{
			//time = Platformer.TimeCoopStart;
			//if ( Platformer.CurrentState == Platformer.GameStates.Warmup )
			//{
			//	time = 0;
			//	Log.Info( time );
			//}
			var game = Game.Current as Platformer;
			Timer.Text = TimeSpan.FromSeconds( (game.TimeCoopStart * 60).Clamp( 0, float.MaxValue ) ).ToString( @"hh\:mm\:ss" );
		}


		if ( Platformer.CurrentGameMode != Platformer.GameModes.Tag )
		{
			SetClass( "active", true );
		}

		
	}
}
