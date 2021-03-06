
using Platformer.UI;
using Sandbox;
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
			Platformer.Alerts( To.Everyone, "Waiting For Players" );
			await Task.Delay( 1000 );
		}

		GameState = GameStates.Warmup;
		StateTimer = 30;
		await WaitStateTimer();

		GameState = GameStates.Live;
		StateTimer = 15 * 60;
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

		Global.ChangeLevel( mapVote.WinningMap );
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

	public virtual void DoClientJoined( Client cl )
	{
		cl.Pawn = CreatePlayerInstance();
		(cl.Pawn as PlatformerPawn).Respawn();

		var spawnpoints = All.OfType<SpawnPoint>();
		var randomSpawnPoint = spawnpoints.OrderBy( x => Rand.Int( 9999 ) ).FirstOrDefault();

		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			cl.Pawn.Transform = tx;
		}

		PlatformerChatBox.AddInformation( To.Everyone, $"{cl.Name} has joined the game", $"avatar:{cl.PlayerId}" );
	}

	public virtual PlatformerPawn CreatePlayerInstance() => new PlatformerPawn();
	public virtual void DoPlayerRespawn( PlatformerPawn player ) { }
	public virtual void DoPlayerKilled( PlatformerPawn player ) { }
	protected virtual bool CanStart() => false;
	protected virtual bool CanBreakState() => false;
	protected virtual void OnGameLive() { }

	StandardPostProcess postProcess;
	[Event.Frame]
	protected virtual void DoPostProcess()
	{
		if ( postProcess == null )
		{
			postProcess = new();
			PostProcess.Add( postProcess );
		}

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

		if ( GameState == GameStates.Warmup )
		{
			postProcess.FilmGrain.Intensity = 0.4f;
			postProcess.FilmGrain.Response = 0.5f;

			postProcess.Saturate.Amount = 0.5f;
		}
	}

	protected virtual void FreshStart()
	{
		foreach ( var cl in Client.All )
		{
			cl.SetInt( "points", 0 );
			cl.SetInt( "deaths", 0 );

			if ( cl.Pawn is not PlatformerPawn pl ) continue;
			pl.ResetTimer();
			pl.ResetBestTime();
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
