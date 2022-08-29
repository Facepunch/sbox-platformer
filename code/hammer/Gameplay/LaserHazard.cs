
using SandboxEditor;
using Sandbox;
using Sandbox.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_LaserHazard", Description = "Laser Beam Hazard" )]
[Model( Model = "models/gameplay/temp/temp_heart_01.vmdl" )]
[Display( Name = "Laser Beam Hazard", GroupName = "Platformer", Description = "Laser Beam Hazard" ), Category( "Gameplay" ), Icon( "flash_on" )]
[DrawAngles]
[HammerEntity]
public partial class LaserHazard : ModelEntity
{

	[Net]
	[Property( "MaxDistance", Title = "Max Distance" )]
	public float maxDist { get; set; } = 500f;

	[Net]
	[Property( "effect_name" ), EntityReportSource, FGDType( "particlesystem" )]
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
		var trace = Trace.Ray( Position + Rotation.Forward * 5, Position + dir * maxDist )
			.UseHitboxes()
			.Radius( 2.0f )
			.WithAnyTags( new[] { "player", "world", "solid" } )
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
