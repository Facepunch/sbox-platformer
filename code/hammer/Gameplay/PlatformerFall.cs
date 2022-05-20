using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Sandbox;
using SandboxEditor;

namespace Platformer;
/// <summary>
/// A simple platform that will fall if the player touches.
/// </summary>
[Library( "plat_platform_fall" )]
[Display( Name = "Platform Fall", GroupName = "Platformer", Description = "Platform starts falling on touched." ), Category( "Gameplay" ), Icon( "water_drop" )]
[SupportsSolid]
[Model]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[HammerEntity]
public partial class PlatformerFall : ModelEntity
{
	public int TimeToHold { get; set; }

	[Net, Property] public float GiveTime { get; set; } = 2f;

	[Net, Property] public float RespawnTime { get; set; } = 10f;

	[Net, Property] public float FallSpeed { get; set; } = 50f;

	[Net,Property] public bool DontReturn { get; set; } = false;

	[Net, Property] 
	public bool MoveOnEndTouch { get; private set; } = false;

	[Net]
	private bool StartFalling { get; set; }

	[Net]
	public Vector3 StartPos { get; private set; }

	[Net]
	public bool StartMoving { get; private set; }


	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
		EnableAllCollisions = true;
		EnableTouch = true;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		var bounds = PhysicsBody.GetBounds();
		var extents = (bounds.Maxs - bounds.Mins) * 0.5f;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromAABB( PhysicsMotionType.Static, -extents.WithZ( 0 ), extents.WithZ( 32 ) );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

		StartPos = Position;
	}

	public override void Touch( Entity other )
	{

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		base.Touch( other );

		TimeToHold++;

		if ( !MoveOnEndTouch )
		{
			if ( TimeToHold / 10 >= GiveTime )
			{
				StartFalling = true;
				if ( !DontReturn )
				{
					RespawnPlat();
				}
			}
		}
	}

	public override void EndTouch( Entity other )
	{

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		base.EndTouch( other );

		if ( MoveOnEndTouch )
		{

			StartFalling = true;
			if ( !DontReturn )
			{
				RespawnPlat();
			}
		}
	}

	[Event.Tick.Server]
	public void FallPlatformer()
	{
		if ( StartFalling )
		{
			Position = Position.WithZ( Position.z + Time.Delta * -FallSpeed );
		}
	}

	[Event.Tick.Server]
	public void ColourChange(float colorblend)
	{
		RenderColor = Color.Lerp( Color.White, Color.Red, (colorblend/ 10) / GiveTime );
	}

	public async void RespawnPlat()
	{
		await GameTask.DelaySeconds( RespawnTime * 10 );

		Position = StartPos;
		StartFalling = false;
	}


}
