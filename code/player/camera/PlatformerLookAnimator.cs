
using Sandbox;
using System;

namespace Platformer;

public class PlatformerLookAnimator : BaseAnimator
{

	public PlatformerLookAnimator( AnimatedEntity ent ) : base( ent ) { }

	TimeSince TimeSinceFootShuffle = 60;

	public bool LookAtMe;

	float duck;

	public override void Simulate()
	{
		var idealRotation = Pawn.Rotation;
		if ( Pawn is not PlatformerPawn p ) return;

		DoRotation( idealRotation );
		DoWalk();

		//
		// Let the animation graph know some shit
		//
		bool sitting = p.Tags.Has( "sitting" );
		bool noclip = p.Tags.Has( "noclip" ) && !sitting;
		bool skidding = p.Tags.Has( "skidding" );

		p.SetAnimParameter( "b_grounded", p.GroundEntity != null || noclip || sitting );
		p.SetAnimParameter( "b_noclip", noclip );
		p.SetAnimParameter( "b_sit", sitting );
		p.SetAnimParameter( "skid", skidding ? 1.0f : 0f );
		p.SetAnimParameter( "b_swim", p.GetWaterLevel() > 0.5f && !sitting );

		if ( Game.IsClient && p.Client.IsValid() )
		{
			p.SetAnimParameter( "voice", p.Client.Voice.LastHeard < 0.5f ? p.Client.Voice.CurrentLevel : 0.0f );
		}

		if ( LookAtMe )
		{
			Vector3 aimPos = p.EyePosition + p.Rotation.Forward * 200;
			Vector3 lookPos = aimPos;

			//
			// Look in the direction what the player's input is facing
			//
			p.SetAnimLookAt( "aim_eyes", p.EyePosition, lookPos );
			p.SetAnimLookAt( "aim_head", p.EyePosition, lookPos );
			p.SetAnimLookAt( "aim_body", p.EyePosition, aimPos );
		}

		if ( p.Tags.Has( "ducked" ) ) duck = duck.LerpTo( 1.0f, Time.Delta * 10.0f );
		else duck = duck.LerpTo( 0.0f, Time.Delta * 5.0f );

		p.SetAnimParameter( "duck", duck );

		var holdtype = p.HeldBody.IsValid() ? 4 : 0;

		p.SetAnimParameter( "holdtype", holdtype );
	}

	public virtual void DoRotation( Rotation idealRotation )
	{
		if ( Pawn is not PlatformerPawn p ) return;
		//
		// Our ideal player model rotation is the way we're facing
		//
		var allowYawDiff = p?.ActiveChild == null ? 90 : 50;

		float turnSpeed = 0.01f;
		if ( Pawn.Tags.Has( "ducked" ) ) turnSpeed = 0.1f;

		//
		// If we're moving, rotate to our ideal rotation
		//
		Pawn.Rotation = Rotation.Slerp( Pawn.Rotation, idealRotation, Pawn.Velocity.Length * Time.Delta * turnSpeed );

		//
		// Clamp the foot rotation to within 120 degrees of the ideal rotation
		//
		Pawn.Rotation = Pawn.Rotation.Clamp( idealRotation, allowYawDiff, out var change );

		//
		// If we did restrict, and are standing still, add a foot shuffle
		//
		if ( change > 1 && Pawn.Velocity.Length <= 1 ) TimeSinceFootShuffle = 0;

		Pawn.SetAnimParameter( "b_shuffle", TimeSinceFootShuffle < 0.1 );
	}

	void DoWalk()
	{
		// Move Speed
		{
			var dir = Pawn.Velocity;
			var forward = Pawn.Rotation.Forward.Dot( dir );
			var sideward = Pawn.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			Pawn.SetAnimParameter( "move_direction", angle );
			Pawn.SetAnimParameter( "move_speed", Pawn.Velocity.Length );
			Pawn.SetAnimParameter( "move_groundspeed", Pawn.Velocity.WithZ( 0 ).Length );
			Pawn.SetAnimParameter( "move_y", sideward );
			Pawn.SetAnimParameter( "move_x", forward );
			Pawn.SetAnimParameter( "move_z", Pawn.Velocity.z );
		}

		// Wish Speed
		{
			var dir = Pawn.Velocity;
			var forward = Pawn.Rotation.Forward.Dot( dir );
			var sideward = Pawn.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			Pawn.SetAnimParameter( "wish_direction", angle );
			Pawn.SetAnimParameter( "wish_speed", Pawn.Velocity.Length );
			Pawn.SetAnimParameter( "wish_groundspeed", Pawn.Velocity.WithZ( 0 ).Length );
			Pawn.SetAnimParameter( "wish_y", sideward );
			Pawn.SetAnimParameter( "wish_x", forward );
			Pawn.SetAnimParameter( "wish_z", Pawn.Velocity.z );
		}
	}

	public override void OnEvent( string name )
	{
		// DebugOverlay.Text( Pos + Vector3.Up * 100, name, 5.0f );

		if ( name == "jump" )
		{
			Pawn.SetAnimParameter( "b_jump", true );
		}

		base.OnEvent( name );
	}
}
