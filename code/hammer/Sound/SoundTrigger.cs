using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Platformer;

[Library( "plat_soundscape", Description = "Platformer Soundscape" )]
[Hammer.EditorSprite( "materials/editor/soundscape/soundscape.vmat" )]
[Display( Name = "Platformer Soundscape", GroupName = "Platformer", Description = "Platformer Soundscape" )]
[BoundsHelper( "mins", "maxs", true, false )]
internal partial class SoundTrigger : Entity
{
	[Property( Title = "SoundScapeName" )]
	public string SoundScapeName { get; set; }

	[Property( "mins", Title = "Checkpoint mins" )]
	[DefaultValue( "-32 -32 0" )]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Checkpoint maxs" )]
	[DefaultValue( "32 32 64" )]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 64 );

	[Property(Title = "Sound" ), FGDType("sound")]
	public string sound { get; set; }

	public static Sound SoundInstance;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromOBB( PhysicsMotionType.Static, Mins, Maxs );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

	}
	

	public override void StartTouch( Entity other )
	{
		base.Touch( other );

		if ( other is not PlatformerPawn pl ) return;

		Play( To.Single( pl.Client ), sound );
	}

	[ClientRpc]
	public void Play( string sound )
	{
		if ( SoundInstance.ToString() == sound ) return;
		SoundInstance.Stop();
		SoundInstance = Sound.FromScreen( sound);
	}
}
