using Hammer;
using Sandbox;
using System.Linq;

[Library("para_checkpoint", Description = "Defines a checkpoint where the player will respawn after falling")]
[Model( Model = "models/gameplay/checkpoint/editor_checkpoint/editor_checkpoint.vmdl" )]
[EntityTool("Player Checkpoint", "Parachute", "Defines a checkpoint where the player will respawn after falling.")]
[BoundsHelper( "mins", "maxs", false, true )]
internal partial class Checkpoint : ModelEntity
{


	[Property("mins", Title = "Checkpoint mins")]
	[Net]
	public Vector3 Mins { get; set; } = new Vector3(-75, -75, 0);

	[Property("maxs", Title = "Checkpoint maxs")]
	[Net]
	public Vector3 Maxs { get; set; } = new Vector3(75, 75, 100);

	[Net, Property]
	public bool IsStart { get; set; }
	[Net, Property]
	public bool IsEnd { get; set; }
	[Net, Property]
	public int Number { get; set; }

	private ModelEntity flag;

	public override void Spawn()
	{
		base.Spawn();

		//SetModel( "models/gameplay/checkpoint/editor_checkpoint/editor_checkpoint.vmdl" );

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

		if ( other is not ParachutePawn pl ) return;
		if ( !CanPlayerCheckpoint( pl ) ) return;

		pl.TrySetCheckpoint( this );

		if ( IsEnd ) pl.CompleteCourse();
		else if ( IsStart ) pl.EnterStartZone();
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is not ParachutePawn pl ) return;
		if ( !IsStart ) return;

		pl.StartCourse();
	}

	private bool CanPlayerCheckpoint( ParachutePawn pl )
	{
		//if ( pl.GroundEntity == null ) return false;
		if ( pl.TimerState != TimerState.Live ) return false;

		return true;
	}

	private bool active;
	[Event.Frame]
	private void OnFrame()
	{
		if ( Local.Pawn is not ParachutePawn pl ) return;
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

}
