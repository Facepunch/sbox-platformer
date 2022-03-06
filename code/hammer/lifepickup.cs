using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using System.Collections.Generic;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/gameplay/temp/temp_heart_01.vmdl" )]
[EntityTool( "Life Pickup", "Platformer", "Addition Life." )]
internal partial class LifePickup : ModelEntity
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	private List<Entity> PlayerCollectedLife { get; set; } = new List<Entity>();

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
		if ( PlayerCollectedLife.Contains( pl ) ) return;
		
		pl.NumberLife ++;

		CollectedLifePickup(To.Single (other.Client) );
		PlayerCollectedLife.Add( pl );

	}

	[ClientRpc]
	private void CollectedLifePickup()
	{
		Sound.FromEntity( "life.pickup", this );

		//EnableDrawing = false;
		RenderColor = RenderColor.WithAlpha( 0.1f );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
	}

	public void Reset(Entity ent)
	{
		PlayerCollectedLife.Remove( ent );
		ResetDrawing(To.Single(ent.Client));
	}
	[ClientRpc]
	public void ResetDrawing()
	{
		RenderColor = RenderColor.WithAlpha( 1 );
		//EnableDrawing = true;
	}

}
