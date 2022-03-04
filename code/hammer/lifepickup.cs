using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/editor/cordon_helper.vmdl" )]
[EntityTool( "Life Pickup", "Platformer", "Addition Life." )]
internal partial class LifePickup : ModelEntity
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	public override void Spawn()
	{
		base.Spawn();

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
		pl.NumberLife ++;
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );

		CollectedPickup(To.Single (other.Client) );

		//Delete();

	}

	[ClientRpc]
	private void CollectedPickup()
	{
		EnableDrawing = false;
		CollisionGroup = CollisionGroup.Debris;
	}

}
