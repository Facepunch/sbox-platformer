
using Sandbox;
using System;
using System.Linq;
using Platformer.UI;
using System.Collections.Generic;

namespace Platformer
{
	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// </summary>
	public partial class Platformer : Sandbox.Game
	{
		public static GameModes CurrentGameMode => (Current as Platformer)?.GameMode ?? GameModes.Competitive;

		[Net]
		public GameModes GameMode { get; set; } = GameModes.Competitive;

		[Net]
		public static float NumberOfCollectables { get; set; }

		[ConVar.Replicated( "plat_coop" )]
		public static bool CoopMode { get; set; } = false;

		[ConVar.Replicated( "plat_debug" )]
		public static bool PlatDebug { get; set; } = true;

		StandardPostProcess postProcess;

		private List<string> killMessages = new()
		{
			"{0} Died",
			"{0} Couldn't stand"
		};

		public enum GameModes
		{
			Competitive,
			Coop,
			Tag
		}

		public Platformer()
		{

			if ( IsServer )
			{
				var hud = new PlatformerHud();
				hud.Parent = this;
				

			}

			if ( IsClient )
			{
				postProcess = new StandardPostProcess();
				PostProcess.Add( postProcess );

				foreach ( var Keys in Entity.All.OfType<KeyPickup>() )
				{
					var goal = Entity.All.OfType<KeyPickup>();
					Keys.ToString();
				}
			}

		}

		[Event.Entity.PostSpawn]
		private void PostEntitySpawn()
		{
			if ( Host.IsClient )
			{
				KeysCollected.InitKeys();

			}

			if ( !Host.IsServer ) return;

			var GetGameMode = Entity.All.FirstOrDefault( x => x is GameModeSelect ) as GameModeSelect;
			if ( GetGameMode.IsValid() )
			{
				if ( GetGameMode.ModeTypeList == GameModeSelect.ModeType.Competitive )
				{
					GameMode = GameModes.Competitive;
				}
				if ( GetGameMode.ModeTypeList == GameModeSelect.ModeType.Coop )
				{
					GameMode = GameModes.Coop;
				}
				if ( GetGameMode.ModeTypeList == GameModeSelect.ModeType.Tag )
				{
					GameMode = GameModes.Tag;
				}
				_ = GameLoopAsync();
			}

			// temp thing til we do our own path entity for rails
			All.OfType<GenericPathEntity>()
				.ToList()
				.ForEach( x => x.Transmit = TransmitType.Always );
		}

		public override void PostLevelLoaded()
		{
			base.PostLevelLoaded();

			NumberOfCollectables = All.OfType<KeyPickup>().Count();

		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			if ( CurrentGameMode == GameModes.Coop )
			{
				if ( CurrentState == GameStates.Live )
				{
					var deathpawn = new PlatformerDeadPawn( client );
					client.Pawn = deathpawn;

					var allplayers = Entity.All.OfType<PlatformerPawn>();
					var randomplayer = allplayers.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

					deathpawn.Position = randomplayer.Position + Vector3.Up * 32;
				}
			}return;
			// Create a pawn for this client to play with
			var pawn = new PlatformerPawn( client);
			pawn.Respawn();
			client.Pawn = pawn;

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

			if ( CurrentGameMode == GameModes.Coop )
			{
				pawn.NumberLife = 1;
			}

			PlatformerChatBox.AddInformation( To.Everyone, $"{client.Name} has joined the game", $"avatar:{client.PlayerId}" );
		}

		public static void RespawnAsAlive( Entity toucher )
		{
			foreach ( var client in Client.All.Where( c => c.Pawn is PlatformerDeadPawn ) )
			{ 
				client.Pawn.Delete();

				var pawn = new PlatformerPawn( client );
				pawn.Respawn();
				client.Pawn = pawn;

				if ( toucher is not PlatformerPawn pl ) return;
				pawn.Checkpoints = pl.Checkpoints;
				pawn.GotoBestCheckpoint();
			}
		}
		public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( client, reason );

			PlatformerChatBox.AddInformation( To.Everyone, $"{client.Name} has left the game", $"avatar:{client.PlayerId}" );
		}

		public override void OnKilled( Client client, Entity pawn )
		{
			base.OnKilled( client, pawn );

			PlatformerKillfeed.AddEntryOnClient( To.Everyone, GetRandomFallMessage( client.Name ), client.NetworkIdent );

			if ( CurrentState == GameStates.Warmup ) return;
			if ( CurrentGameMode == GameModes.Coop )
			{
				var deathpawn = new PlatformerDeadPawn( client );
				client.Pawn = deathpawn;
				client.Pawn.Transform = pawn.Transform;
			}
		}

		private int lastFallMessage;
		private string GetRandomFallMessage( string playerName )
		{
			var idx = Rand.Int( 0, killMessages.Count - 1 );
			while ( idx == lastFallMessage )
				idx = Rand.Int( 0, killMessages.Count - 1 );

			lastFallMessage = idx;
			return string.Format( killMessages[idx], playerName );
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );

			postProcess.Sharpen.Enabled = false;

			postProcess.FilmGrain.Enabled = false;
			postProcess.FilmGrain.Intensity = 0.2f;
			postProcess.FilmGrain.Response = 1;

			postProcess.Vignette.Enabled = true;
			postProcess.Vignette.Intensity = 1.0f;
			postProcess.Vignette.Roundness = 1.5f;
			postProcess.Vignette.Smoothness = 0.5f;
			postProcess.Vignette.Color = Color.Black;

			postProcess.Saturate.Enabled = true;
			postProcess.Saturate.Amount = 1;

			postProcess.Blur.Enabled = false;

			if ( CurrentState == GameStates.Warmup )
			{
				postProcess.FilmGrain.Intensity = 0.4f;
				postProcess.FilmGrain.Response = 0.5f;

				postProcess.Saturate.Amount = 0.5f;
			}
		}

		[ClientRpc]
		public static void PropCarryBreak(Vector3 pos, string particle, string sound )
		{
			Particles.Create( particle, pos );
			Sound.FromWorld( sound, pos );
		}
	}

}
