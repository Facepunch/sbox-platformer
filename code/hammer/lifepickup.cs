using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using System.Collections.Generic;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/editor/cordon_helper.vmdl" )]
[EntityTool( "Life Pickup", "Platformer", "Addition Life." )]
internal partial class LifePickup : ModelEntity
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	private List<Entity> PlayerCollected { get; set; } = new List<Entity>();

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
		if ( PlayerCollected.Contains( pl ) ) return;
		
		pl.NumberLife ++;

		CollectedPickup(To.Single (other.Client) );
		PlayerCollected.Add( pl );

	}

	[ClientRpc]
	private void CollectedPickup()
	{
		Sound.FromEntity( "life.pickup", this );

		EnableDrawing = false;
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
	}

	public void Reset(Entity ent)
	{
		PlayerCollected.Remove( ent );
		ResetDrawing(To.Single(ent.Client));
	}
	[ClientRpc]
	public void ResetDrawing()
	{
		EnableDrawing = true;
	}

}
