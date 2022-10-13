
using Sandbox;
using SandboxEditor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Model( Model = "models/citizen/citizen.vmdl" )]
[Library( "plat_quest_boy" )]
[Display( Name = "Quest", GroupName = "Platformer", Description = "A pad that launches players toward a target entity" ), Category( "Gameplay" ), Icon( "sports_gymnastics" )]
[BoundsHelper( "mins", "maxs", true, false )]
[HammerEntity]
public partial class QuestBoy : AnimatedEntity
{
	[Property( "mins", Title = "Checkpoint mins" )]
	[DefaultValue( "-75 -75 0" )]
	[Net]
	public Vector3 Mins { get; set; } = new Vector3( -75, -75, 0 );

	[Property( "maxs", Title = "Checkpoint maxs" )]
	[DefaultValue( "75 75 100" )]
	[Net]
	public Vector3 Maxs { get; set; } = new Vector3( 75, 75, 100);

	[Net, Property, FGDType( "target_destination" )] public string TargetEntity { get; set; } = "";
	[Net, Property, FGDType( "target_destination" )] public string PLTargetEntity { get; set; } = "";

	[Net] 
	public Entity LookTarget { get; set; }
	public PlatformerLookAnimator Animator { get; private set; }
	public Vector3 WishVelocity { get; set; }

	public override void Spawn()
	{
		Animator = new PlatformerLookAnimator();
		SetModel("models/citizen/citizen.vmdl");
		
		EnableTouch = true;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		EnableTouch = true;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromOBB( PhysicsMotionType.Static, Mins, Maxs );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

		base.Spawn();
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( !IsServer ) return;
		
		if ( other is not PlatformerPawn pl ) return;

		var target = FindByName( TargetEntity );
		//var pltarget = FindByName( PLTargetEntity );
		//pl.Position = pltarget.Position;
		LookTarget = pl;
		LookAtPlayer( LookTarget );
		pl.NPCCameraTarget = this;
		if ( target != null )
		{
			pl.NPCCamera = target.Position;
		}
		pl.LookTarget = this;
		Freeze(pl);
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is not PlatformerPawn pl ) return;
		LookTarget = null;
		pl.LookTarget = null;
		pl.CameraMode = new PlatformerOrbitCamera();
	}

	public bool OnUse( Entity user )
	{
		if ( user is not PlatformerPawn pl ) return false;

		pl.Health++;

		Log.Info( "avvoof" );

		return true;
	}
	public void LookAtPlayer( Entity pl )
	{
		if ( LookTarget.IsValid() )
		{
			if ( Animator is PlatformerLookAnimator animator )
			{
				animator.LookAtMe = true;

				WishVelocity = Velocity;

				SetAnimLookAt( "aim_eyes", pl.Position + Vector3.Up * 2f );
				SetAnimLookAt( "aim_head", pl.Position + Vector3.Up * 2f );
				SetAnimLookAt( "aim_body", pl.Position + Vector3.Up * 2f );

				var defaultPosition = Rotation.LookAt( pl.Position - Position ).Angles();
				
				Rotation = Rotation.Slerp( Rotation, Rotation.From( defaultPosition ), Time.Delta * .5f);

				SetAnimParameter( "b_shuffle", Rotation != Rotation.From( defaultPosition ) );
			}
		}
	}
	public void Freeze(PlatformerPawn pl )
	{
	}
}
