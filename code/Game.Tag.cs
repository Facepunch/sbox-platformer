
using Platformer;
using Platformer.UI;
using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer
{
	public partial class Platformer : Game
	{

		[Net]
		public int RoundNumber { get; set; } = 1;

		[Net]
		public bool RoundFinish { get; set; }

		private async Task TagRoundLoopAsync()
		{
			RoundFinish = false;

			ClearTags();
			Alerts( To.Everyone, ("Get Ready!") );
			GameState = GameStates.Runaway;
			StateTimer = 1 * 30f;
			StartTag();
			FreshStart();
			await WaitStateTimer();
			if ( GameIsEnded ) return;

			Alerts( To.Everyone, ("Don't Get Tagged!") );
			GameState = GameStates.Live;
			StateTimer = 1 * 60f;
			MoveTagPlayer();
			await WaitStateTimer();
			if ( GameIsEnded ) return;

			if(RoundNumber == 5 )
			{
			 _ = EndGame();
			}

		}

		private void ClearTags()
		{

			foreach ( var cl in Client.All )
			{
				if ( cl.Pawn is not PlatformerPawn pl ) continue;
				pl.ResetTagged();
			}

		}

		private async void RoundFinished()
		{
			RoundFinish = true;

			GameState = GameStates.GameEnd;
			StateTimer = 10;
			await WaitStateTimer();

			_ = TagRoundLoopAsync();
			RoundNumber++;
		}
	}
}
