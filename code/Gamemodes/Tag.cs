
using Platformer.UI;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer.Gamemodes;

internal partial class Tag : BaseGamemode
{

	public static Tag Current => Instance as Tag;

	[Net]
	public IList<PlatformerPawn> Tagged { get; set; }
	[Net]
	public int RoundNumber { get; set; }

	public override Platformer.GameModes Mode => Platformer.GameModes.Tag;

	protected override bool CanStart()
	{
		return PlayerCount() >= 2;
	}

	public override void ClientSpawn()
	{
		Local.Hud.AddChild<TagHud>();
	}

	protected override bool CanBreakState()
	{
		var alltagged = All.OfType<PlatformerPawn>().All( x => x is PlatformerPawn p && Tagged.Contains( p ) );
		if ( GameState == GameStates.Live && alltagged )
			return true;
		return false;
	}

	protected override async Task GamemodeLoopAsync()
	{
		RoundNumber = 1;

		while ( RoundNumber < Platformer.NumTagRounds )
		{
			GameState = GameStates.Runaway;
			StateTimer = DoSkipToLive ? 0 : (1 * 30f);

			Platformer.BeenTagged( To.Everyone, "Get Ready!" );
			FreshStart();
			StartTag();
			await WaitStateTimer();

			GameState = GameStates.Live;
			StateTimer = 3 * 60f;

			Platformer.BeenTagged( To.Everyone, "Don't get tagged!" );
			MoveTaggers();
			await WaitStateTimer();

			if ( RoundNumber < Platformer.NumTagRounds - 1 )
			{
				GameState = GameStates.GameEnd;
				StateTimer = DoSkipToLive ? 0 : (1 * 10f);
				await WaitStateTimer();
			}

			RoundNumber++;
		}
	}

	private void StartTag()
	{
		var tagger = Rand.FromList( All.OfType<PlatformerPawn>().ToList() );

		if ( !tagger.IsValid() ) return;

		Tagged.Add( tagger );
		MoveTaggers( true );
	}

	protected override void FreshStart()
	{
		base.FreshStart();

		Tagged.Clear();
	}

	public void MoveTaggers( bool toTaggerSpawn = false )
	{
		IEnumerable<Entity> spawnpoints = toTaggerSpawn
			? All.OfType<TaggerSpawn>()
			: All.OfType<SpawnPoint>();

		foreach ( var player in Tagged )
		{
			player.Respawn();

			var randomSpawn = Rand.FromList( spawnpoints.ToList() );
			if ( randomSpawn != null )
			{
				var tx = randomSpawn.Transform;
				tx.Position += Vector3.Up * 50.0f;
				player.Transform = tx;
			}
		}
	}

	public override PlatformerPawn CreatePlayerInstance( Client cl ) => new TagPlayer( cl );

	public void TagPlayer( PlatformerPawn player )
	{
		if ( !IsServer ) return;

		if ( Tagged.Contains( player ) ) 
			return;

		Tagged.Add( player );

		Platformer.BeenTagged( To.Everyone, $"{player.Client.Name} Has been Tagged." );

		using var _ = Prediction.Off();
		Sound.FromEntity( "life.pickup", player );
	}

	private static bool DoSkipToLive;
	[ConCmd.Admin]
	public static void SkipToLive()
	{
		DoSkipToLive = !DoSkipToLive;
	}

}
