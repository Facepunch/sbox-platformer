
using Platformer;
using Sandbox;
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

		[AdminCmd]
		public static void SkipStage()
		{
			if ( Current is not Platformer plg ) return;

			plg.StateTimer = 1;
		}

		private async Task WaitStateTimer()
		{
			while ( StateTimer > 0 )
			{
				await Task.DelayRealtimeSeconds( 1.0f );
			}

			// extra second for fun
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		private async Task GameLoopAsync()
		{
			GameState = GameStates.Warmup;
			StateTimer = 10;
			await WaitStateTimer();

			GameState = GameStates.Live;
			StateTimer = 20*60f;
			FreshStart();
			await WaitStateTimer();

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
		private void FreshStart()
		{
			foreach ( var cl in Client.All )
			{
				cl.SetInt( "points", 0 );
				cl.SetInt( "deaths", 0 );

				if ( cl.Pawn is not PlatformerPawn pl ) continue;
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

		private bool HasEnoughPlayers()
		{
			if ( All.OfType<Player>().Count() < 1 )
				return false;

			return true;
		}
		public enum GameStates
		{
			Warmup,
			Live,
			GameEnd,
			MapVote
		}

	}
}
