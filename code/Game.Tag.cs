
using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer
{
	public partial class Platformer : Game
	{

		[Net]
		public int RoundNumber { get; set; }

		private async Task TagRoundLoopAsync()
		{
			RoundNumber = 1;

			while( RoundNumber < 5 )
			{
				GameState = GameStates.Runaway;
				StateTimer = 1 * 30f;

				Alerts( To.Everyone, "Get Ready!" );
				FreshStart();
				ClearTags();
				StartTag();
				await WaitStateTimer();

				GameState = GameStates.Live;
				StateTimer = 3 * 60f;

				Alerts( To.Everyone, "Don't get tagged!" );
				MoveTagPlayer();
				await WaitStateTimer();

				if( RoundNumber < 4 )
				{
					GameState = GameStates.GameEnd;
					StateTimer = 1 * 10f;
					await WaitStateTimer();
				}

				RoundNumber++;
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

		private void StartTag()
		{
			var allplayers = Entity.All.OfType<PlatformerPawn>();
			var randomplayer = allplayers.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
			var tagspawnpoint = Entity.All.OfType<TaggerSpawn>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( tagspawnpoint == null ) return;

			TaggerPlayer = randomplayer;
			randomplayer.Position = tagspawnpoint.Position;
			randomplayer.Tagged = true;
		}

		public void MoveTagPlayer()
		{
			var pawn = TaggerPlayer;
			pawn.Respawn();

			var spawnpoints = Entity.All.OfType<SpawnPoint>();
			var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( randomSpawnPoint != null )
			{
				var tx = randomSpawnPoint.Transform;
				tx.Position += Vector3.Up * 50.0f;
				pawn.Transform = tx;
			}
		}

	}
}
