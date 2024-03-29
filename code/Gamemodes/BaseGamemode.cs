﻿
using Platformer.UI;
using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer.Gamemodes;

public partial class BaseGamemode : Entity
{

	public static BaseGamemode Instance;

	[Net]
	public GameStates GameState { get; set; } = GameStates.Warmup;
	[Net]
	public RealTimeUntil StateTimer { get; set; } = 0f;
	[Net]
	public bool EnablePvP { get; set; }
	public virtual Platformer.GameModes Mode => Platformer.GameModes.Coop;

	public BaseGamemode()
	{
		Instance = this;
	}

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	private bool GameLoopExecuting;
	public async Task GameLoopAsync()
	{
		Assert.False( GameLoopExecuting );

		GameLoopExecuting = true;

		while ( !CanStart() )
		{
			Platformer.Waiting( To.Everyone, "Waiting For Players" );
			await Task.Delay( 1000 );
		}

		GameState = GameStates.Warmup;
		StateTimer = 30;
		await WaitStateTimer();

		GameState = GameStates.Live;
		StateTimer = Platformer.GameTime * 60;
		FreshStart();

		OnGameLive();

		await GamemodeLoopAsync();

		GameState = GameStates.GameEnd;
		StateTimer = 10;
		await WaitStateTimer();

		GameState = GameStates.MapVote;
		var mapVote = new MapVoteEntity();
		mapVote.VoteTimeLeft = 15f;
		StateTimer = mapVote.VoteTimeLeft;
		await WaitStateTimer();

		Game.ChangeLevel( mapVote.WinningMap );
	}

	protected async Task WaitStateTimer()
	{
		while ( StateTimer > 0 )
		{
			if ( CanBreakState() )
			{
				break;
			}
			await Task.DelayRealtimeSeconds( Math.Min( StateTimer, 1f ) );
		}
	}

	protected virtual async Task GamemodeLoopAsync()
	{
		await WaitStateTimer();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Game.RootPanel.AddChild<DefaultHud>();
	}

	public virtual void DoClientJoined( IClient cl )
	{
		cl.Pawn = CreatePlayerInstance( cl );
		(cl.Pawn as PlatformerPawn).Respawn();

		var spawnpoints = All.OfType<SpawnPoint>();
		var randomSpawnPoint = spawnpoints.OrderBy( x => Game.Random.Int( 9999 ) ).FirstOrDefault();

		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			cl.Pawn.Position = tx.Position;
			cl.Pawn.Rotation = tx.Rotation;
		}

		PlatformerChatBox.AddChatEntry( To.Everyone, cl.Name, "has joined the game", cl.SteamId, null, false );
	}

	public virtual PlatformerPawn CreatePlayerInstance( IClient cl ) => new PlatformerPawn( cl );
	public virtual void DoPlayerRespawn( PlatformerPawn player ) { }
	public virtual void DoPlayerKilled( PlatformerPawn player ) { }
	protected virtual bool CanStart() => false;
	protected virtual bool CanBreakState() => false;
	protected virtual void OnGameLive() { }

	protected virtual void FreshStart()
	{
		foreach ( var cl in Game.Clients )
		{
			cl.SetInt( "kills", 0 );
			cl.SetInt( "deaths", 0 );
			cl.SetInt( "tagged", 0 );

			if ( cl.Pawn is not PlatformerPawn pl ) continue;
			pl.ResetTimer();
			pl.ResetBestTime();
			pl.ResetCollectibles<BaseCollectible>();
			pl.NumberLife = 3;
			pl.Coin = 0;
			pl.GotoBestCheckpoint();
		}

		All.OfType<Player>().ToList().ForEach( x =>
		{
			x.Respawn();
		} );
	}

	protected int PlayerCount() => All.OfType<PlatformerPawn>().Count();

	[ConCmd.Admin]
	public static void SkipStage()
	{
		Platformer.Current.Gamemode.StateTimer = 1;
	}

}

public enum GameStates
{
	Warmup,
	Runaway,
	Live,
	GameEnd,
	MapVote
}
