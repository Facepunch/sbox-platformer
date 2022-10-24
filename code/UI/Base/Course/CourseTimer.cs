
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI
{
	[UseTemplate( "/UI/Base/Course/StatusCard.html" )]
	public class CourseTimer : StatusCard
	{
		private TimeSince time;

		public CourseTimer()
		{
			Icon = "history_toggle_off ";
			Header = "TIMER";
			ReverseColor = true;
		}

		public override void Tick()
		{
			base.Tick();

			var pawn = Local.Pawn as PlatformerPawn;
			if ( !pawn.IsValid() ) return;

			if ( Competitive.Current != null )
			{
				time = pawn.TimeSinceStart;
				if ( pawn.TimerState != TimerState.Live )
				{
					time = 0;
				}
				Message = TimeSpan.FromSeconds( (time * 60).Clamp( 0, float.MaxValue ) ).ToString( @"hh\:mm\:ss" );
			}

			if ( Coop.Current != null )
			{
				Message = TimeSpan.FromSeconds( (Coop.Current.TimeCoopStart * 60).Clamp( 0, float.MaxValue ) ).ToString( @"hh\:mm\:ss" );
			}

			if ( Tag.Current != null )
			{
				SetClass( "active", true );
			}
		}
	}
}
