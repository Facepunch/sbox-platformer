
using Sandbox;
using System.Collections.Generic;

namespace Platformer.Gamemodes;

internal partial class CompetitivePlayer : PlatformerPawn
{

	[Net]
	public IList<int> KeysPlayerHas { get; set; } = new List<int>();
	[Net]
	public float NumberOfKeys { get; set; }

	public CompetitivePlayer( IClient cl ) : base( cl ) { }
	public CompetitivePlayer() : base() { }

	public override void Respawn()
	{
		base.Respawn();

		Tags.Add( "Platplayer" );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( !Game.IsServer ) return;
		if ( LifeState != LifeState.Alive ) return;

		if ( GetActiveController() == DevController )
		{
			ResetTimer();
			KeysPlayerHas.Clear();
			NumberOfKeys = 0;
		}

		if ( InputActions.Kill.Down() )
		{
			if ( TimerState == TimerState.Finished )
			{
				KeysPlayerHas.Clear();
				NumberLife = 3;
				NumberOfKeys = 0;
				TakeDamage( new() { Damage = 9999 } );
			}
			else
			{
				TakeDamage( new() { Damage = 9999 } );
			}
		}
	}

}
