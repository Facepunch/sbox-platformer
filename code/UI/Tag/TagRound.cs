
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	[UseTemplate( "/UI/Base/Course/StatusCard.html" )]
	public class TagRound : StatusCard
	{
		public int CurrentRound { get; set; }
		public int TotalRound { get; set; }

		public TagRound()
		{
			Icon = "history_toggle_off ";
			Header = "ROUND";
			ReverseColor = true;
		}

		public override void Tick()
		{
			base.Tick();

			if ( !Tag.Current.IsValid() )
				return;

			Message = $"{Tag.Current.RoundNumber}/{Platformer.NumTagRounds}";
		}
	}
}
