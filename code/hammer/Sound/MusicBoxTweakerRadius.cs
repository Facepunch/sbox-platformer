using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_musicboxtweakerradius", Description = "Music Box Tweaker Radius" )]
[EditorSprite( "editor/ent_logic.vmat" )]
[Display( Name = "Music Box Tweaker", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
//[HammerEntity]
[Sphere( "radius" )]
partial class MusicBoxTweakerRadius : ModelEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetMusicBox { get; set; } = "";
	[Net] public float Volume { get; set; } = 1;

	[Net, Property]
	public float Radius { get; set; } = 128.0f;

	public Sound PlayingSound { get; protected set; }


	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "trigger" );
		EnableAllCollisions = true;
		EnableTouch = true;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromSphere( PhysicsMotionType.Static, Vector3.Zero, Radius );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

		Transmit = TransmitType.Always;
		
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not PlatformerPawn pl ) return;

		UpdateSound( To.Single( pl.Client ), Volume );
	}

	[ClientRpc]
	public void UpdateSound( float sound)
	{
		var target = FindByName( TargetMusicBox );
		if ( target is not MusicBox ent ) return;

		var overlaps = FindInSphere( Position, Radius );

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not PlatformerPawn entity || !entity.IsValid() )
				continue;



			var targetPos = entity.PhysicsBody.MassCenter;

			var dist = Vector3.DistanceBetween( Position, targetPos );
			//Log.Info( dist );

			ent.UpdateVolume( dist / Radius );
			//PlayingSound = Sound.FromScreen( ent.SoundName ).SetVolume( Volume );
			//ent.PlayingSound.SetVolume(dist / Radius);
			//Log.Info( dist / Radius );
		}
	}

	[Event.Tick.Client]
	public void Tick()
	{

	}
}
