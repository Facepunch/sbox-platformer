
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
		public RealTimeSince StateTimer { get; set; } = 0f;

		[Net]
		public RealTimeUntil StateTimerDown { get; set; } = 0f;

		[Net]
		public GameStates GameState { get; set; } = GameStates.Warmup;
		[Net]
		public string NextMap { get; set; } = "facepunch.datacore";

		[AdminCmd]
		public static void SkipStage()
		{
			if ( Current is not Platformer plg ) return;

			plg.StateTimer = 1;
		}

		private async Task WaitStateTimer()
		{
			if ( CurrentState == GameStates.Warmup )
			{
				if ( StateTimer <= 45 )
				{
					GameState = GameStates.Live;
				}
			}
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
			StateTimer = 1;
			await WaitStateTimer();

			GameState = GameStates.GameEnd;
			StateTimer = 10.0f;
			await WaitStateTimer();

			//GameState = GameStates.MapVote;
			//StateTimer = 10.0f;
			//await WaitStateTimer();

			Global.ChangeLevel( NextMap );
		}
		private void FreshStart()
		{
			foreach ( var cl in Client.All )
			{
				cl.SetInt( "points", 0 );
				cl.SetInt( "deaths", 0 );
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
