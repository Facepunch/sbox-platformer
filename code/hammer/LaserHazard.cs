using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;


namespace Platformer;

[Library( "plat_LaserHazard", Description = "Laser Beam Hazard" )]
[Model( Model = "models/gameplay/temp/temp_heart_01.vmdl" )]
[EntityTool( "Laser Beam Hazard", "Platformer", "Laser Beam Hazard." )]
[Hammer.DrawAngles]
public partial class LaserHazard : ModelEntity
{
	private DamageInfo damage = 10;

	[Net]
	[Property( "MaxDistance", Title = "Max Distance" )]
	[DefaultValue( "500" )]
	public float maxDist { get; set; }


	//public string BeamParticle = "particles/gameplay/laser_beam/beam_lazer.vpcf";

	[Net]
	[Property( "effect_name" ), Hammer.EntityReportSource, FGDType( "particlesystem" )]
	public string BeamParticle { get; set; }

	private Particles Beam { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		var BeamPart = Particles.Create( BeamParticle );
		Beam = BeamPart;
	}

	[Event.Tick]
	public void LBeam()
	{
		var Dir = Rotation.Forward;

		var trace = Trace.Ray( Position, Position + Dir * maxDist )
			.UseHitboxes()
			.Radius( 2.0f )
			.HitLayer( CollisionLayer.Player )
			.Run();

		if ( Beam == null )
		{
			var BeamPart = Particles.Create( BeamParticle );
			Beam = BeamPart;
		}
		Beam.SetPosition( 0, this.Position );
		Beam.SetPosition( 1, trace.EndPosition );

		DebugOverlay.TraceResult( trace );

		var pl = trace.Entity as PlatformerPawn;
		if ( trace.Hit == true )
		{
			DamagePlayer( pl );
		}
	}

	[ClientRpc]
	public void DamagePlayer( Entity pl )
	{
		if ( pl is not PlatformerPawn p ) return;
		pl.TakeDamage( damage );
		Log.Info( pl );
	}
}
