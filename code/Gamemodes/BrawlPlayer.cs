
using Platformer.UI;
using Sandbox;

namespace Platformer.Gamemodes;

internal partial class BrawlPlayer : PlatformerPawn
{
	public BrawlPlayer( Client cl ) : base( cl ) { } 
	public BrawlPlayer() : base() { }

	[Net, Predicted]
	public TimeSince TimeSincePunch { get; set; }

	public override void Respawn()
	{
		base.Respawn();

		Health = 4;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Animator.SetAnimParameter( "holdtype", 5 );
		Animator.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Health > 0 && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			TryPunch();
		}
	}

	private void TryPunch()
	{
		if ( TimeSincePunch < .5f ) return;

		TimeSincePunch = 0f;
		Animator.SetAnimParameter( "b_attack", true );

		if ( !IsServer ) return;

		var hitPos = Position + Vector3.Up * 50 + Rotation.Forward * 20f;
		var hitRadius = 20f;
		var hits = Entity.FindInSphere( hitPos, hitRadius );

		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Sphere( hitPos, hitRadius, Color.Red, 5f );
		}

		foreach ( var hit in hits )
		{
			if ( hit is not BrawlPlayer pl ) continue;
			if ( hit == this ) continue;

			pl.TakeDamage( new DamageInfo()
			{
				Damage = 1,
				Force = Vector3.Up * 250 + Rotation.Forward * 1000
			} );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if ( IsServer )
			HurtOverlay.FlashTo( To.Single( Client ) );

		using var _ = Prediction.Off();
		Sound.FromWorld( "sounds/impacts/impact-bullet-flesh.sound", EyePosition )
			.SetVolume( 10 );
	}

}
