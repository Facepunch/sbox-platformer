using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using System.Collections.Generic;

[Library( "plat_coin", Description = "Coin Pickup" )]
[Model( Model = "models/citizen_props/coin01.vmdl" )]
[EntityTool( "Coin Pickup", "Platformer", "Coin Pickup." )]
internal partial class CoinPickup : ModelEntity
{

	public int NumberOfCoin { get; set; } = 1;

	private List<Entity> PlayerCollectedCoin { get; set; } = new List<Entity>();

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
		if ( PlayerCollectedCoin.Contains( pl ) ) return;

		pl.Coin++;


		CollectedHealthPickup(To.Single (other.Client) );
		PlayerCollectedCoin.Add( pl );

	}

	[ClientRpc]
	private void CollectedHealthPickup()
	{
		Sound.FromEntity( "life.pickup", this );

		//EnableDrawing = false;
		RenderColor = RenderColor.WithAlpha( 0 );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
	}

	public void Reset(Entity ent)
	{
		PlayerCollectedCoin.Remove( ent );
		ResetDrawing(To.Single(ent.Client));
	}

	[ClientRpc]
	public void ResetDrawing()
	{
		RenderColor = RenderColor.WithAlpha( 1 );
		//EnableDrawing = true;
	}

}
