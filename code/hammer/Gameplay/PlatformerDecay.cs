using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Sandbox;
using SandboxEditor;

namespace Platformer;
/// <summary>
/// A simple platform that will destroy if player stands on for too long.
/// </summary>
[Library( "plat_platform" )]
[Display( Name = "Platform Decay", GroupName = "Platformer", Description = "Platform dies after time." ), Category( "Gameplay" ), Icon( "blur_on" )]
[SupportsSolid]
[Model]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[HammerEntity]
public partial class PlatformerDecay : ModelEntity
{
	public int TimeToHold { get; set; }

	[Net, Property]
	public float DecayTime { get; set; } = 5f;
	[Net, Property]
	public float RespawnTime { get; set; } = 5f;
	[Net]
	public bool Broken { get; set; }

	[Net]
	[Property( "effect_name" ), EntityReportSource, FGDType( "particlesystem" )]
	public string EffectParticle { get; set; } = "particles/break/break.cardboard.vpcf";

	private Color DefaultColor;
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
		EnableAllCollisions = true;
		EnableTouch = true;
		DefaultColor = RenderColor;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		var bounds = PhysicsBody.GetBounds();
		var extents = (bounds.Maxs - bounds.Mins) * 0.5f;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromAABB( PhysicsMotionType.Static, -extents.WithZ( 0 ), extents.WithZ( 32 ) );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;
	}

	public override void Touch( Entity other )
	{
		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		TimeToHold++;

		ColourChange( TimeToHold );

		if ( TimeToHold / 10 >= DecayTime && !Broken )
		{
			Break();
		}

		base.Touch( other );
	}

	[Event.Tick.Server]
	public void ColourChange( float colorblend )
	{
		RenderColor = Color.Lerp( DefaultColor, Color.Red, colorblend / 10 / Math.Max( DecayTime, .1f ) );
	}

	private void Break()
	{
		Broken = true;
		EnableDrawing = false;
		EnableAllCollisions = false;

		Sound.FromEntity( "physics.glass.shard.impact", this ).SetVolume( 2f );
		Particles.Create( EffectParticle, Position );

		UnbreakAfter( RespawnTime );
	}

	private async void UnbreakAfter( float delay )
	{
		if ( !Broken )
		{
			Log.Warning( "Trying to respawn a platform that isn't decayed" );
			return;
		}

		await GameTask.DelaySeconds( delay );

		EnableDrawing = true;
		EnableAllCollisions = true;
		RenderColor = DefaultColor;
		TimeToHold = 0;
		Broken = false;
	}


}
