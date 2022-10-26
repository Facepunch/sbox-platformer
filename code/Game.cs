
using Sandbox;
using System;
using System.Linq;
using Platformer.UI;
using System.Collections.Generic;
using Platformer.Gamemodes;
using System.Threading.Tasks;

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


	[ConVar.Replicated( "pl_gametime" )]
	public static int GameTime { get; set; } = 10;

	[ConVar.Replicated( "pl_tagrounds" )]
	public static int NumTagRounds { get; set; } = 5;


	public Platformer()
	{
		Current = this;

		if ( IsClient )
		{
			_ = new Hud();
		}
	}

	/// <summary>
	/// Someone is speaking via voice chat. This might be someone in your game,
	/// or in your party, or in your lobby.
	/// </summary>
	public override void OnVoicePlayed( Client cl )
	{
		VoiceChatList.Current?.OnVoicePlayed( cl.PlayerId, cl.VoiceLevel );
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

		PlatformerChatBox.AddInformation( To.Everyone, $"{client.Name} has left the game", client.PlayerId );
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		var msg = Rand.FromList( killMessages );


		PlatformerChatBox.AddChatEntry( To.Everyone, client.Name, msg, client.PlayerId, null, false );
	}

	private List<string> killMessages = new()
	{
		"died",
		"couldn't stand"
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

	[ClientRpc]
	public static void BeenTagged( string Title )
	{
		WaitingForPlayers.ShowWaitingForPlayers( Title );
	}

	[ClientRpc]
	public static void Waiting( string Title )
	{
		WaitingForPlayers.ShowWaitingForPlayers( Title );
	}

	public enum GameModes
	{
		Competitive = 0,
		Coop = 1,
		Tag = 2,
		Brawl = 3
	}
	public static async void SubmitScore( string bucket, Client client, int score )
	{

		var leaderboard = await Leaderboard.FindOrCreate( bucket, false );

		await leaderboard.Value.Submit( client, score );

	}

	public static async Task<LeaderboardEntry?> GetScore( string bucket, Client client )
	{

		var leaderboard = await Leaderboard.FindOrCreate( bucket, false );

		return await leaderboard.Value.GetScore( client.PlayerId );

	}
}
