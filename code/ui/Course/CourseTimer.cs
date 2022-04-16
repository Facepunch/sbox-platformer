
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI;

public class CourseTimer : Panel
{

	private Label Timer;

	public CourseTimer() => Timer = Add.Label( string.Empty, "timer" );

	public override void Tick()
	{
		base.Tick();

		var pawn = Local.Pawn as PlatformerPawn;
		if ( !pawn.IsValid() ) return;

		var time = pawn.TimeSinceStart;
		if ( pawn.TimerState != TimerState.Live )
		{
			time = 0;
		}

		Timer.Text = TimeSpan.FromSeconds( (time * 60).Clamp( 0, float.MaxValue ) ).ToString( @"hh\:mm" );
	}
}
