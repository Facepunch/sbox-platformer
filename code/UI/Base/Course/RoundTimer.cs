
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI
{
	[UseTemplate( "/UI/Base/Course/StatusCard.html" )]
	public class RoundTimer : StatusCard
	{
		public RoundTimer()
		{
			Icon = "schedule";
		}

		public override void Tick()
		{
			if ( !BaseGamemode.Instance.IsValid() ) 
				return;

			var span = TimeSpan.FromSeconds( (BaseGamemode.Instance.StateTimer * 60).Clamp( 0, float.MaxValue ) );

			Message = span.ToString( @"hh\:mm" );
			Header = BaseGamemode.Instance.GameState.ToString().ToUpperInvariant();
		}
	}
}
