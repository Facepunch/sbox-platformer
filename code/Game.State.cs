
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

		[Net]
		public bool EnoughPlayersToStart { get; private set; }

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

		//public void SetGameMode( GameModes mode )
		//{
		//	GameMode = mode;

		//	Log.Info( GameMode );

		//}

		private async Task GameLoopAsync()
		{
			if ( GameMode == GameModes.Competitive )
			{
				GameState = GameStates.Warmup;
				StateTimer = 30;
				await WaitStateTimer();

				GameState = GameStates.Live;
				StateTimer = 20 * 60f;
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
			if ( HasEnoughPlayers() == false ) return;
			if (GameMode == GameModes.Tag )
			{
				EnoughPlayersToStart = true;
				
				Alerts( To.Everyone,( "Waiting For Players" ) );
				GameState = GameStates.Warmup;
				StateTimer = 30;
				await WaitStateTimer();

				TagRoundLoopAsync();
			}
		}

		[ClientRpc]
		public static void Alerts( string Title )
		{
			NewMajorArea.ShowLandmark( Title );
		}

		public async Task EndGame()
		{
			GameIsEnded = true;
			RoundFinish = true;

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
				if ( pl.Tagged == true ) return;
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

		public void StartTag()
		{
			var allplayers = Entity.All.OfType<PlatformerPawn>();

			var randomplayer = allplayers.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			var tagspawnpoint = Entity.All.OfType<TaggerSpawn>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( tagspawnpoint == null ) return;
			TaggerPlayer = randomplayer;
			randomplayer.Position = tagspawnpoint.Position;
			randomplayer.Tagged = true;
			randomplayer.PlayerTagArrow();

		}

		public void MoveTagPlayer()
		{

			var pawn = TaggerPlayer;
			pawn.Respawn();

			// Get all of the spawnpoints
			var spawnpoints = Entity.All.OfType<SpawnPoint>();

			// chose a random one
			var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			// if it exists, place the pawn there
			if ( randomSpawnPoint != null )
			{
				var tx = randomSpawnPoint.Transform;
				tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
				pawn.Transform = tx;
			}
		}

		private bool HasEnoughPlayers()
		{
			if ( All.OfType<PlatformerPawn>().Count() < 2 )
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
