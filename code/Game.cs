
using Sandbox;
using System;
using System.Linq;
using Platformer.UI;
using System.Collections.Generic;
using Platformer.Gamemodes;

namespace Platformer;

public partial class Platformer : Sandbox.Game
{

	public new static Platformer Current;
	public static GameStates GameState => Current.Gamemode?.GameState ?? GameStates.Warmup;
	public static GameModes Mode => Current.Gamemode?.Mode ?? GameModes.Coop;

	[Net]
	public BaseGamemode Gamemode { get; set; } = new();
	[Net]
	public float NumberOfCollectables { get; set; }

	[ConVar.Replicated( "plat_coop" )]
	public static bool CoopMode { get; set; } = false;

	public Platformer()
	{
		Current = this;

		if ( IsClient )
		{
			_ = new Hud();
		}
	}

	[Event.Entity.PostSpawn]
	public void PostEntitySpawn()
	{
		NumberOfCollectables = All.OfType<KeyPickup>().Count();

		All.OfType<GenericPathEntity>()
			.ToList()
			.ForEach( x => x.Transmit = TransmitType.Always );

		var mapgm = All.FirstOrDefault( x => x is GameModeSelect ) as GameModeSelect;

		if ( mapgm.IsValid() )
		{
			if ( mapgm.ModeTypeList == GameModes.Coop )
				Gamemode = new Coop();
			else if ( mapgm.ModeTypeList == GameModes.Tag )
				Gamemode = new Tag();
			else if ( mapgm.ModeTypeList == GameModes.Competitive )
				Gamemode = new Competitive();
			else if ( mapgm.ModeTypeList == GameModes.Brawl )
				Gamemode = new Brawl();
		}

		_ = Gamemode.GameLoopAsync();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		Gamemode.DoClientJoined( client );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );

		PlatformerChatBox.AddInformation( To.Everyone, $"{client.Name} has left the game", $"avatar:{client.PlayerId}" );
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		var msg = string.Format( Rand.FromList( killMessages ), client.Name );
		PlatformerKillfeed.AddEntryOnClient( To.Everyone, msg, client.NetworkIdent );
	}

	private List<string> killMessages = new()
	{
		"{0} Died",
		"{0} Couldn't stand"
	};

	[ClientRpc]
	public static void PropCarryBreak(Vector3 pos, string particle, string sound )
	{
		Particles.Create( particle, pos );
		Sound.FromWorld( sound, pos );
	}

	[ClientRpc]
	public static void Alerts( string Title )
	{
		NewMajorArea.ShowLandmark( Title );
	}

	public enum GameModes
	{
		Competitive = 0,
		Coop = 1,
		Tag = 2,
		Brawl = 3
	}

}
