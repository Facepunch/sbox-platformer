
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer
{
	public partial class Platformer : Game
	{
		[Net]
		public IList<int> KeysAllPlayerHas { get; set; } = new List<int>();

		[Net]
		public float NumberOfKeys { get; set; }

		[Net, Predicted]
		public TimerState CoopTimerState { get; set; }

		[Net]
		public TimeSince TimeCoopStart { get; set; } = 0f;

		public enum TimerState
		{
			InStartZone,
			Live,
			Finished
		}

		public void StartCoopTimer()
		{
			if ( CurrentState == GameStates.Live )
			{
				TimeCoopStart = 0;
			}
			foreach ( var cl in Client.All )
			{
				if ( cl.Pawn is not PlatformerPawn pl ) continue;
				pl.StartCourse();
			}
		}

		public async Task GameLoopCoopEndAsync()
		{
			GameState = GameStates.GameEnd;
			StateTimer = 10;
			await WaitStateTimer();

			GameState = GameStates.MapVote;
			var mapVote = new MapVoteEntity();
			mapVote.VoteTimeLeft = 15f;
			StateTimer = mapVote.VoteTimeLeft;
			await WaitStateTimer();

			Global.ChangeLevel( mapVote.WinningMap );
		}
	}
}
