
using Hammer;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Platformer;

[Library( "plat_key", Description = "Key Pickup" )]
[Model( Model = "models/citizen_props/coin01.vmdl" )]
[EntityTool( "Key Pickup", "Platformer", "Key Pickup." )]
internal partial class KeyPickup : ModelEntity
{
	[Property]
	public int KeyNumber { get; set; } = 1;

	private List<Entity> PlayerCollectedKey { get; set; } = new List<Entity>();

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
		if ( pl.KeysPlayerHas.Contains( KeyNumber ) ) return;

		pl.PickedUpItem( Color.Yellow );

		pl.KeysPlayerHas.Add( KeyNumber );


		CollectedHealthPickup(To.Single (other.Client) );
		PlayerCollectedKey.Add( pl );

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
		PlayerCollectedKey.Remove( ent );
		ResetDrawing(To.Single(ent.Client));
	}

	[ClientRpc]
	public void ResetDrawing()
	{
		RenderColor = RenderColor.WithAlpha( 1 );
		//EnableDrawing = true;
	}

}
