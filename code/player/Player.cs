
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Movement;
using Platformer.Utility;
using Platformer.Gamemodes;

namespace Platformer;

public partial class PlatformerPawn : Sandbox.Player
{
	[Net]
	public AnimatedEntity Citizen { get; set; }

	[Net] public Entity LookTarget { get; set; }

	[Net]
	public Color PlayerColor { get; set; }
	[Net]
	public bool PlayerHasGlider { get; set; }
	[Net]
	public float GliderEnergy { get; set; }
	[Net]
	public TimeUntil TimeUntilVulnerable { get; set; }
	[Net]
	public int NumberLife { get; set; } = 3;
	[Net]
	public PropCarriable HeldBody { get; set; }
	[Net]
	public string CurrentArea { get; set; }
	[Net]
	public int Coin { get; set; }
	[Net]
	public IList<Checkpoint> Checkpoints { get; set; }

	public int AreaPriority = 0;
	public bool IgnoreFallDamage = false;

	private ClothingContainer Clothing;
	private DamageInfo lastDamage;
	private TimeSince ts;
	private Particles WalkCloud;
	private Particles FakeShadowParticle;

	[Net] public string ClothingAsString { get; set; }

	public PlatformerPawn( Client cl ) : this()
	{
		Clothing = new ClothingContainer();
		Clothing.LoadFromClient( cl );
		ClothingAsString = cl.GetClientData( "avatar", "" );
	}

	public PlatformerPawn() { } 

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );
		
		Citizen = this;
		Controller ??= new PlatformerController();
		Animator = new PlatformerLookAnimator();
		CameraMode = new PlatformerOrbitCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		CurrentArea ??= Global.MapName;

		Clothing.DressEntity( this );

		base.Respawn();

		Health = 4;

		Tags.Add( "Platplayer" );
		GotoBestCheckpoint();
		Platformer.Current.Gamemode.DoPlayerRespawn( this );
	}

	public void ResetCollectibles<T>() where T : BaseCollectible
	{
		foreach ( var item in All.OfType<T>() )
		{
			item.Reset( this );
		}
	}

	public void SetInvulnerable( float duration )
	{
		TimeUntilVulnerable = duration;
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( TimeUntilVulnerable > 0 ) return;
		if ( info.Flags == DamageFlags.Sonic && !BaseGamemode.Instance.EnablePvP ) return;

		base.TakeDamage( info );

		if ( info.Force.z > 50f )
		{
			GroundEntity = null;
		}

		Velocity += info.Force;

		lastDamage = info;
	}

	public override void OnKilled()
	{
		base.OnKilled();

		NumberLife--;
		Coin = (int)(Coin * .5f);

		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		CameraMode = new PlatformerRagdollCamera();

		WalkCloud?.Destroy();
		WalkCloud = null;

		HeldBody?.Drop( 2 );
		HeldBody = null;

		foreach ( var child in Children )
		{
			child.EnableDrawing = false;
		}

		BaseGamemode.Instance.DoPlayerKilled( this );

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, lastDamage.BoneIndex );
	}

	private TimeUntil TimeUntilCanUse;
	protected override void TickPlayerUse()
	{
		if ( HeldBody.IsValid() )
			return;

		if ( TimeUntilCanUse > 0 )
			return;

		base.TickPlayerUse();
	}
	[Net]
	public bool TalkingToNPC { get; set; }

	[Net]
	public Entity NPCCameraTarget { get; set; }

	[Net]
	public Vector3 NPCCamera { get; set; }

	public float MaxHealth { get; set; } = 4;

	public override void Simulate( Client cl )
	{
		if ( Platformer.GameState == GameStates.GameEnd )
			return;
		
		if( TalkingToNPC )
			return;

		base.Simulate( cl );

		if ( !IsServer ) return;

		TickPlayerThrow();
		TickPlayerUse();

		if ( Controller is PlatformerController controller )
		{
			GliderEnergy = (float)Math.Round( controller.Energy );
		}
		
		if ( LookTarget.IsValid() )
		{
			if ( Animator is PlatformerLookAnimator animator )
			{
				animator.LookAtMe = true;

				SetAnimLookAt( "aim_eyes", LookTarget.Position + Vector3.Up * 64f );
				SetAnimLookAt( "aim_head", LookTarget.Position + Vector3.Up * 64f );
				SetAnimLookAt( "aim_body", LookTarget.Position + Vector3.Up * 64f );
			}
			//CameraMode = new LookAtCamera();
			
			//if ( CameraMode is LookAtCamera lookAtCamera )
			//{
			//	lookAtCamera.TargetEntity = NPCCameraTarget;
			//	lookAtCamera.TargetOffset = new Vector3( 0, 0, 64 );
			//	lookAtCamera.FieldOfView = 70;
			//	lookAtCamera.MaxFov = 70;
			//	lookAtCamera.MinFov = 50;
			//	lookAtCamera.Origin = NPCCamera;
			//}
			//DebugOverlay.Sphere( NPCCamera, 5f, Color.Red );
		}

		if ( Health == 1 && ts > 2 )
		{
			LowHealth();
			ts = 0;
		}
	}

	private void TickPlayerThrow()
	{
		if ( !HeldBody.IsValid() ) return;

		var drop = false;
		var vel = Vector3.Zero;

		if ( Input.UsingController )
		{
			if ( InputActions.Walk.Pressed() && InputActions.Duck.Down() )
			{
				drop = true;
				vel = Velocity + Rotation.Forward * 30 + Rotation.Up * 10;
			}

			if ( InputActions.Walk.Pressed() && !InputActions.Duck.Down() )
			{
				drop = true;
				//HeldParticle.Destroy(true);
				vel = Velocity + Rotation.Forward * 300 + Rotation.Up * 100;
			}

			if ( !drop ) return;
			HeldBody.Drop( vel );
			HeldBody = null;
			TimeUntilCanUse = 1f;
		}

		if ( !Input.UsingController )
		{
			if ( InputActions.Use.Pressed() && InputActions.Duck.Down() )
			{
				drop = true;
				vel = Velocity + Rotation.Forward * 30 + Rotation.Up * 10;

			}

			if ( InputActions.Use.Pressed() && !InputActions.Duck.Down() )
			{
				drop = true;
				//HeldParticle.Destroy(true);
				vel = Velocity + Rotation.Forward * 300 + Rotation.Up * 100;

			}

			if ( !drop ) return;
			HeldBody.Drop( vel );
			HeldBody = null;
			TimeUntilCanUse = 1f;
		}
	}

	public void PickedUpItem( Color itempickedup )
	{
		if ( IsServer )
		{
		}
	}

	public void LowHealth()
	{
		if ( IsServer )
		{
			Sound.FromWorld( "player.lowhealth", Position );
		}
	}

	protected override Entity FindUsable()
	{
		var startpos = Position + Vector3.Up * 5;
		var endpos = startpos + Rotation.Forward * 60f;
		var tr = Trace.Sphere( 5f, startpos, endpos )
			.Ignore( this )
			.EntitiesOnly()
			.Run();

		if ( tr.Entity.IsValid() && tr.Entity is IUse use && use.IsUsable( this ) )
			return tr.Entity;

		return null;
	}

	public void ApplyForce( Vector3 force )
	{
		if ( Controller is PlatformerController controller )
		{
			controller.Impulse += force;
		}
	}

	public void PlayerPickedUpGlider()
	{
		if ( PlayerHasGlider )
		{
			if ( Controller is PlatformerController controller )
			{
				controller.EnableGliderControl();
				PlayerHasGlider = true;
			}
		}
	}

	[Event.Frame]
	public void UpdateWalkCloud()
	{
		WalkCloud ??= Particles.Create( "particles/gameplay/player/walkcloud/walkcloud.vpcf", this );

		if ( LifeState == LifeState.Dead || GroundEntity == null )
		{
			WalkCloud.SetPosition( 6, new Vector3( 0, 0, 0 ) );
			return;
		}

		var speed = Velocity.Length.Remap( 0f, 280, 0f, 1f );
		WalkCloud.SetPosition( 6, new Vector3( speed, 0f, 0f ) );
	}

	[Event.Frame]
	public void UpdatePlayerShadow()
	{
		FakeShadowParticle ??= Particles.Create( "particles/gameplay/fake_shadow/fake_shadow.vpcf" );

		var tr = Trace.Ray( Position, Position + Vector3.Down * 2000 )
			.WorldOnly()
			.Run();

		FakeShadowParticle.SetPosition( 0, tr.EndPosition );
	}

	[Event.Frame]
	private void UpdateRenderAlpha()
	{
		const float MaxRenderDistance = 128f;

		if ( Local.Pawn == this ) return;
		if ( Local.Pawn == null ) return;
		if ( !Local.Pawn.IsValid() ) return;
		if ( Platformer.Mode != Platformer.GameModes.Competitive ) return;

		var dist = Local.Pawn.Position.Distance( Position );
		var a = 1f - dist.LerpInverse( MaxRenderDistance, MaxRenderDistance * .1f );
		a = Math.Max( a, .15f );
		a = Easing.EaseOut( a );

		this.SetRenderColorRecursive( RenderColor.WithAlpha( a ) );
	}

	[Event.Tick]
	public void PlayerHolding()
	{
		if ( HeldBody != null )
		{
			if ( Controller is PlatformerController controller )
			{
				controller.IsHolding = true;
			}
		}
		else
		{
			if ( Controller is PlatformerController controller )
			{
				controller.IsHolding = false;
			}
		}
	}

	[ConCmd.Admin]
	public static async void MapVote()
	{
		var vote = new MapVoteEntity();
		vote.VoteTimeLeft = 15f;
		await System.Threading.Tasks.Task.Delay( (int)vote.VoteTimeLeft * 1000 );
		Global.ChangeLevel( vote.WinningMap );
	}

	TimeSince timeSinceLastFootstep = 0;
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !IsServer )
			return;

		if ( foot == 0 )
		{
			var lfoot = Particles.Create( "particles/gameplay/player/footsteps/footstep_l.vpcf", pos );
			lfoot.SetOrientation( 0, Transform.Rotation );
		}
		else
		{
			var rfoot = Particles.Create( "particles/gameplay/player/footsteps/footstep_r.vpcf", pos );
			rfoot.SetOrientation( 0, Transform.Rotation );
		}

		if ( timeSinceLastFootstep < 0.2f )
			return;

		volume *= FootstepVolume();

		timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;

		tr.Surface.DoFootstep( this, tr, foot, volume * 10 );
	}
}
