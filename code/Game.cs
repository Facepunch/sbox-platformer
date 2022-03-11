
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

		[ConVar.Replicated( "plat_coop" )]
		public static bool CoopMode { get; set; } = false;

		[ConVar.Replicated( "plat_debug" )]
		public static bool PlatDebug { get; set; } = true;

		private List<string> killMessages = new()
		{
			"{0} Died",
			"{0} Couldn't stand"
		};

		public Platformer()
		{
			if ( IsServer )
			{
				var hud = new PlatformerHud();
				hud.Parent = this;
			}
		}



		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

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
		}

		public override void OnKilled( Client client, Entity pawn )
		{
			base.OnKilled( client, pawn );

			PlatformerKillfeed.AddEntryOnClient( To.Everyone, GetRandomFallMessage( client.Name ), client.NetworkIdent );
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
	}

}
