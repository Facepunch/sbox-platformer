using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using System.Collections.Generic;

[Library( "plat_healthpickup", Description = "Addition Health" )]
[Model( Model = "models/gameplay/temp/temp_health_01.vmdl" )]
[EntityTool( "Health Pickup", "Platformer", "Addition Health." )]
internal partial class HealthPickup : ModelEntity
{

	public int NumberOfHealth { get; set; } = 1;

	private List<Entity> PlayerCollectedHealth { get; set; } = new List<Entity>();

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableAllCollisions = true;

	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not PlatformerPawn pl ) return;
		if ( PlayerCollectedHealth.Contains( pl ) ) return;
		if ( pl.Health == 4 ) return;

		pl.Health ++;

		CollectedHealthPickup(To.Single (other.Client) );
		PlayerCollectedHealth.Add( pl );

	}

	[ClientRpc]
	private void CollectedHealthPickup()
	{
		Sound.FromEntity( "life.pickup", this );

		EnableDrawing = false;
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
	}

	public void Reset(Entity ent)
	{
		PlayerCollectedHealth.Remove( ent );
		ResetDrawing(To.Single(ent.Client));
	}
	[ClientRpc]
	public void ResetDrawing()
	{
		EnableDrawing = true;
	}

}
