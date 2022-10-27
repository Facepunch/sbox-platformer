
using Platformer.UI;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Platformer.Gamemodes;

internal partial class Coop : BaseGamemode
{

	public static Coop Current => Instance as Coop;

	[Net]
	public IList<int> KeysAllPlayerHas { get; set; }
	[Net]
	public float NumberOfKeys { get; set; }
	[Net]
	public TimerState CoopTimerState { get; set; }
	[Net]
	public TimeSince TimeCoopStart { get; set; } = 0f;

	public override Platformer.GameModes Mode => Platformer.GameModes.Coop;

	protected override void OnGameLive()
	{
		base.OnGameLive();

		TimeCoopStart = 0;

		foreach ( var cl in Client.All )
		{
			if ( cl.Pawn is not PlatformerPawn pl ) continue;
			pl.StartCourse();
		}
	}

	public override void DoPlayerKilled( PlatformerPawn player )
	{
		base.DoPlayerKilled( player );

		if ( GameState == GameStates.Warmup ) return;
		if ( !player.Client.IsValid() ) return;

		var deathpawn = new PlatformerDeadPawn( player.Client );
		player.Client.Pawn = deathpawn;
		player.Client.Pawn.Transform = player.Transform;
	}

	public override void DoClientJoined( Client cl )
	{
		base.DoClientJoined( cl );

		if ( GameState == GameStates.Live )
		{
			var deathpawn = new PlatformerDeadPawn( cl );
			cl.Pawn = deathpawn;

			var allplayers = All.OfType<PlatformerPawn>();
			var randomplayer = allplayers.OrderBy( x => Rand.Int( 99999 ) ).FirstOrDefault();
			deathpawn.Position = randomplayer.Position + Vector3.Up * 32;

			PlatformerChatBox.AddChatEntry( To.Everyone, cl.Name, "has joined the game", cl.PlayerId, null, false );
		}

		if ( GameState != GameStates.Live )
		{
			var pawn = new PlatformerPawn();
			cl.Pawn = pawn;
			pawn.Respawn();

			var spawnpoints = All.OfType<SpawnPoint>();
			var randomSpawnPoint = spawnpoints.OrderBy( x => Rand.Int( 99999 ) ).FirstOrDefault();

			if ( randomSpawnPoint != null )
			{
				var tx = randomSpawnPoint.Transform;
				tx.Position = tx.Position + Vector3.Up * 50.0f;
				pawn.Transform = tx;
			}

			pawn.NumberLife = 1;

			PlatformerChatBox.AddChatEntry( To.Everyone, cl.Name, "has joined the game", cl.PlayerId, null, false );
		}
	}

	public override void DoPlayerRespawn( PlatformerPawn player )
	{
		base.DoPlayerRespawn( player );

		if ( player.NumberLife == 0 )
		{
			player.ResetCollectibles<LifePickup>();
			player.ResetCollectibles<HealthPickup>();
			player.NumberLife = 1;
		}
	}

	public override PlatformerPawn CreatePlayerInstance( Client cl ) => new CompetitivePlayer( cl );

	public void RespawnAsAlive( Entity toucher )
	{
		foreach ( var client in Client.All.Where( c => c.Pawn is PlatformerDeadPawn ) )
		{
			client.Pawn.Delete();

			var pawn = CreatePlayerInstance( client ) as CompetitivePlayer;
			client.Pawn = pawn;
			pawn.Respawn();

			if ( toucher is not CompetitivePlayer pl ) 
				return;

			pawn.Checkpoints = pl.Checkpoints;
			pawn.GotoBestCheckpoint();
		}
	}

}
