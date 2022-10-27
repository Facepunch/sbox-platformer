using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Platformer.Gamemodes;

namespace Platformer;

[Library( "plat_checkpoint", Description = "Defines a checkpoint where the player will respawn after falling" )]
[Model( Model = "models/circuit_board_flag/circuit_board_flag.vmdl" )]
[Display( Name = "Player Checkpoint", GroupName = "Platformer", Description = "Defines a checkpoint where the player will respawn after falling" ), Category( "Player" ), Icon( "flag_circle" )]
[BoundsHelper( "mins", "maxs", false, true )]
[HammerEntity]
public partial class Checkpoint : ModelEntity
{


	[Property( "mins", Title = "Checkpoint mins" )]
	[Net]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Checkpoint maxs" )]
	[Net]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 64 );

	[Net, Property]
	public bool IsStart { get; set; }
	[Net, Property]
	public bool IsEnd { get; set; }
	[Net, Property]
	public int Number { get; set; }
	[Net, Property]
	public bool RespawnPoint { get; set; } = true;

	private ModelEntity flag;

	private Particles lighteffect;

	public override void Spawn()
	{
		base.Spawn();
		
		Transmit = TransmitType.Always;
		EnableAllCollisions = true;
		EnableTouch = true;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		var bounds = new BBox( Position + Mins, Position + Maxs );
		var extents = (bounds.Maxs - bounds.Mins) * 0.5f;

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

		flag = new ModelEntity( "models/circuit_board_flag/circuit_board_flag_top.vmdl" );
		flag.Position = flagAttachment.Value.Position;
		flag.Rotation = flagAttachment.Value.Rotation;

		if ( this.IsStart )
		{
			flag.SetModel( "models/circuit_board_flag/circuit_board_flag_lights_start.vmdl" );
			//flag.SetMaterialGroup( "Green" );
		}

		if ( this.IsEnd )
		{
			flag.SetModel( "models/circuit_board_flag/circuit_board_flag_lights.vmdl" ); 
			//flag.SetMaterialGroup( "Checker" );
		}
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not CompetitivePlayer pl ) return;
		if ( !CanPlayerCheckpoint( pl ) ) return;
		if( Platformer.GameState != GameStates.Live ) return;
		pl.TrySetCheckpoint( this, true );

		if ( IsEnd && pl.NumberOfKeys == Platformer.Current.NumberOfCollectables ) _ = pl.CompleteCourseAsync();

		if( Competitive.Current != null && IsStart )
		{				
			if ( pl.NumberOfKeys == 0 )
					pl.ResetTimer();
		}

		if( Coop.Current != null && Coop.Current.CoopTimerState != TimerState.Finished )
		{
			if ( IsEnd && Coop.Current.NumberOfKeys == Platformer.Current.NumberOfCollectables )
			{
				_ = pl.CompleteCourseAsync();
			//	Platformer.GameLoopCoopEndAsync();
			}
			if ( IsStart ) return;

			Coop.Current.RespawnAsAlive( pl );
			EnableTouch = false;
		}
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is not CompetitivePlayer pl ) return;
		if ( !IsStart || pl.NumberOfKeys != 0 ) return;

		pl.StartCourse();
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
		if ( Local.Pawn is not CompetitivePlayer pl ) return;
		if ( IsEnd || IsStart ) return;

		var isLatestCheckpoint = pl.Checkpoints.LastOrDefault() == this;

		if ( !active && isLatestCheckpoint )
		{
			active = true;
			Platformer.Alerts( "CHECKPOINT" );
			flag.SetModel( "models/circuit_board_flag/circuit_board_flag_top.vmdl" );
			lighteffect = Particles.Create( "particles/gameplay/checkpoint_light/checkpoint_light.vpcf", this );
			flag.SetMaterialGroup( "On" );
			Particles.Create( "particles/gameplay/checkpoint_light/checkpoint_light_activated.vpcf", this );
			Sound.FromEntity( "checkpoint_light", this );
		}
		else if ( active && !isLatestCheckpoint )
		{
			active = false;

			flag.SetModel( "models/circuit_board_flag/circuit_board_flag_top.vmdl" );
			flag.SetMaterialGroup( "Off" );
			if ( lighteffect != null )
				lighteffect.Destroy( true );
		}
	}

	public void GetSpawnPoint( out Vector3 position, out Rotation rotation )
	{
		position = Position;
		rotation = Rotation;
	}

}
