using Hammer;
using Sandbox;
using Sandbox.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Platformer;

[Library( "plat_checkpoint", Description = "Defines a checkpoint where the player will respawn after falling" )]
[Model( Model = "models/gameplay/checkpoint/editor_checkpoint/editor_checkpoint.vmdl" )]
[Display( Name = "Player Checkpoint", GroupName = "Platformer", Description = "Defines a checkpoint where the player will respawn after falling" )]
[BoundsHelper( "mins", "maxs", false, true )]
internal partial class Checkpoint : ModelEntity
{


	[Property( "mins", Title = "Checkpoint mins" )]
	[Net]
	[DefaultValue( "-32 -32 0" )]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Checkpoint maxs" )]
	[Net]
	[DefaultValue( "32 32 64" )]
	public Vector3 Maxs { get; set; } = new Vector3(32, 32, 64);

	[Net, Property]
	public bool IsStart { get; set; }
	[Net, Property]
	public bool IsEnd { get; set; }
	[Net, Property]
	public int Number { get; set; }
	[Net, Property]
	public bool RespawnPoint { get; set; } = true;

	private ModelEntity flag;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
		EnableAllCollisions = true;
		EnableTouch = true;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		var bounds = new BBox( Position + Mins, Position + Maxs );
		var extents = ( bounds.Maxs - bounds.Mins ) * 0.5f;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromAABB( PhysicsMotionType.Static, -extents.WithZ( 0 ), extents.WithZ( 128 ) );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		var flagAttachment = GetAttachment( "Flag" );

		flag = new ModelEntity( "models/flag/flag_pole.vmdl" );
		flag.Position = flagAttachment.Value.Position;
		flag.Rotation = flagAttachment.Value.Rotation;

		if ( this.IsStart )
		{
			flag.SetModel( "models/flag/flag.vmdl" );
			flag.SetMaterialGroup( "Green" );
		}

		if ( this.IsEnd )
		{
			flag.SetModel( "models/flag/flag.vmdl" );
			flag.SetMaterialGroup( "Checker" );
		}
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not PlatformerPawn pl ) return;
		if ( !CanPlayerCheckpoint( pl ) ) return;

		pl.TrySetCheckpoint( this );

		if ( IsEnd && pl.NumberOfKeys == Platformer.NumberOfCollectables ) _ = pl.CompleteCourseAsync();
		else if ( IsStart )
		{
			if ( pl.NumberOfKeys == 0 )
			pl.ResetTimer();
		}


		if ( Platformer.CurrentGameMode == Platformer.GameModes.Coop )
		{
			CoopRespawn(pl);
		}

		if ( !IsStart ) return;
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is not PlatformerPawn pl ) return;

		if ( !IsStart ) return;

		if ( pl.NumberOfKeys == 0 )
		{
			pl.StartCourse();
		}
	}

	private bool CanPlayerCheckpoint( PlatformerPawn pl )
	{
		if ( pl.TimerState != TimerState.Live ) return false;

		return true;
	}

	private bool active;
	[Event.Frame]
	private void OnFrame()
	{
		if ( Local.Pawn is not PlatformerPawn pl ) return;
		if ( this.IsEnd || this.IsStart ) return;

		var isLatestCheckpoint = pl.Checkpoints.LastOrDefault() == this;

		if ( !active && isLatestCheckpoint )
		{
			active = true;

			flag.SetModel( "models/flag/flag.vmdl" );
		}
		else if( active && !isLatestCheckpoint )
		{
			active = false;

			flag.SetModel( "models/flag/flag_pole.vmdl" );
		}
	}

	public void GetSpawnPoint( out Vector3 position, out Rotation rotation )
	{
		position = Position;
		rotation = Rotation;
	}

	public void CoopRespawn(PlatformerPawn toucher)
	{
		Platformer.RespawnAsAlive(toucher);
		EnableTouch = false;


	}
}
