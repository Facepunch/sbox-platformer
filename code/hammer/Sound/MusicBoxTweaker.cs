using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_musicboxtweaker", Description = "Music Box Tweaker" )]
[EditorSprite( "editor/ent_logic.vmat" )]
[Display( Name = "Music Box Tweaker", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
[BoundsHelper( "mins", "maxs", false, true )]
partial class MusicBoxTweaker : ModelEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetMusicBox { get; set; } = "";
	[Net] public float Volume { get; set; } = 1;

	[Property( "mins", Title = "Tweaker mins" )]
	[Net]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Tweaker maxs" )]
	[Net]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 64 );

	public Sound PlayingSound { get; protected set; }

	[Net]
	private float speed { get; set; } = 10f;

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "trigger" );
		EnableAllCollisions = true;
		EnableTouch = true;

		var bounds = new BBox( Position + Mins, Position + Maxs );
		var extents = (bounds.Maxs - bounds.Mins) * 0.5f;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromAABB( PhysicsMotionType.Static, Mins, Maxs );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

		Transmit = TransmitType.Always;

	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not PlatformerPawn pl ) return;

		UpdateSound( To.Single( pl.Client ), true );
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );


		if ( other is not PlatformerPawn pl ) return;

		UpdateSound( To.Single( pl.Client ), false );

	}

	[ClientRpc]
	public void UpdateSound( bool Enter )
	{
		var target = FindByName( TargetMusicBox );
		if ( target is not MusicBox ent ) return;

		if ( Enter )
		{
			ent.UpdateVolume( 0.01f.LerpTo( 1, Time.Delta * speed ) );
		}
		else
		{
			ent.UpdateVolume( 1f.LerpTo( 0.01f, Time.Delta * speed ) );
		}

	}

	[Event.Tick.Client]
	public void Tick()
	{

	}
}
