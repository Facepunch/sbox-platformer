
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
		public static GameStates CurrentState => (Current as Platformer)?.GameState ?? GameStates.Warmup;

		[Net]
		public RealTimeUntil StateTimer { get; set; } = 0f;

		[Net]
		public GameStates GameState { get; set; } = GameStates.Warmup;
		[Net]
		public string NextMap { get; set; } = "facepunch.tup_block";
		public bool GameIsLive { get; private set; }
		public bool GameIsEnded { get; set; }
		internal PlatformerPawn TaggerPlayer { get; private set; }

		[AdminCmd]
		public static void SkipStage()
		{
			if ( Current is not Platformer plg ) return;

			plg.StateTimer = 1;
		}

		private bool CanBreakState()
		{
			if( GameMode == GameModes.Tag )
			{
				var alltagged = All.OfType<PlatformerPawn>().All( x => x is PlatformerPawn p && p.Tagged );
				if ( GameState == GameStates.Live && alltagged )
					return true;
			}

			return false;
		}

		private async Task WaitStateTimer()
		{
			while ( StateTimer > 0 )
			{
				if ( CanBreakState() )
				{
					break;
				}
				await Task.DelayRealtimeSeconds( 1.0f );
			}

			// extra second for fun
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		private bool GameLoopExecuting;
		private async Task GameLoopAsync()
		{
			Assert.False( GameLoopExecuting );

			GameLoopExecuting = true;

			while ( !HasEnoughPlayers() )
			{
				Alerts( To.Everyone, "Waiting For Players" );
				await Task.Delay( 1000 );
			}

			GameState = GameStates.Warmup;
			StateTimer = 30;
			await WaitStateTimer();

			GameState = GameStates.Live;
			StateTimer = 15 * 60;
			FreshStart();
			StartCoopTimer();

			if ( GameMode == GameModes.Tag )
			{
				await TagRoundLoopAsync();
			}
			else
			{
				await WaitStateTimer();
			}

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

		[ClientRpc]
		public static void Alerts( string Title )
		{
			NewMajorArea.ShowLandmark( Title );
		}

		private void FreshStart()
		{
			foreach ( var cl in Client.All )
			{
				cl.SetInt( "points", 0 );
				cl.SetInt( "deaths", 0 );

				if ( cl.Pawn is not PlatformerPawn pl ) continue;
				if ( pl.Tagged == true ) continue;
				pl.ResetTimer();
				pl.ResetBestTime();
				pl.GotoBestCheckpoint();
				pl.KeysPlayerHas.Clear();
				pl.NumberOfKeys = 0;
			}

			All.OfType<Player>().ToList().ForEach( x =>
			{
				x.Respawn();
			} );
		}
		public static void StartCoopTimer()
		{
			foreach ( var cl in Client.All )
			{
				if ( cl.Pawn is not PlatformerPawn pl ) continue;
				pl.StartCourse();
			}
		}

		private bool HasEnoughPlayers()
		{
			var playerCount = All.OfType<PlatformerPawn>().Count();

			if ( GameMode == GameModes.Tag && playerCount < 2 )
				return false;

			if ( GameMode == GameModes.Competitive && playerCount < 1 )
				return false;

			return true;
		}

		public enum GameStates
		{
			Warmup,
			Runaway,
			Live,
			GameEnd,
			MapVote
		}

	}
}
