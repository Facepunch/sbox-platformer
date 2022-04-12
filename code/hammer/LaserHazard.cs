
using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_LaserHazard", Description = "Laser Beam Hazard" )]
[Model( Model = "models/gameplay/temp/temp_heart_01.vmdl" )]
[Display( Name = "Laser Beam Hazard", GroupName = "Platformer", Description = "Laser Beam Hazard" )]
[Hammer.DrawAngles]
public partial class LaserHazard : ModelEntity
{

	[Net]
	[Property( "MaxDistance", Title = "Max Distance" )]
	[DefaultValue( "500" )]
	public float maxDist { get; set; }

	//public string BeamParticle = "particles/gameplay/laser_beam/beam_lazer.vpcf";

	[Net]
	[Property( "effect_name" ), Hammer.EntityReportSource, FGDType( "particlesystem" )]
	public string BeamParticle { get; set; }

	private Particles Beam;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Beam = Particles.Create( BeamParticle );
	}

	[Event.Tick]
	public void UpdateBeam()
	{
		var dir = Rotation.Forward;
		var trace = Trace.Ray( Position, Position + dir * maxDist )
			.UseHitboxes()
			.Radius( 2.0f )
			.HitLayer( CollisionLayer.Player )
			.Run();

		if ( IsClient )
		{
			Beam.SetPosition( 0, this.Position );
			Beam.SetPosition( 1, trace.EndPosition );
		}

		if ( IsServer )
		{
			
			if( trace.Entity is PlatformerPawn pl )
			{
				DoDamage( pl );
			}
		}
	}

	private void DoDamage( PlatformerPawn pl )
	{
		Host.AssertServer();

		if ( pl.TimeUntilVulnerable > 0f ) return;

		var damage = 1;
		var force = Rotation.Forward * 200 + Vector3.Up * 200;

		pl.TakeDamage( new() { Damage = damage, Force = force } );
		pl.SetInvulnerable( 1f );
	}
}
