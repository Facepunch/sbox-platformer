
using Hammer;
using Sandbox;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_key", Description = "Key Pickup" )]
[Model( Model = "models/citizen_props/coin01.vmdl" )]
[Display( Name = "Key Pickup", GroupName = "Platformer", Description = "Key Pickup" )]
internal partial class KeyPickup : AnimEntity
{
	/// <summary>
	/// 🍆💦 😝
	/// </summary>
	[Property, Net]
	public string KeyEmoji { get; set; } = "🍆";

	[Property]
	[Net]
	public int KeyNumber { get; set; } = 1;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableAllCollisions = true;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not PlatformerPawn pl ) return;
		if ( pl.KeysPlayerHas.Contains( KeyNumber ) ) return;

		pl.PickedUpItem( Color.Yellow );
		pl.KeysPlayerHas.Add( KeyNumber );
		pl.NumberOfKeys++;

		CollectedHealthPickup(To.Single (other.Client) );
	}

	[ClientRpc]
	private void CollectedHealthPickup()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );

	}

	[Event.Tick.Server]
	public void Tick()
	{
		Rotation = Rotation.FromYaw( Rotation.Yaw() + 500 * Time.Delta );
	}

	[Event.Tick.Client]
	private void ClientTick()
	{
		var a = ShouldRender() ? 1 : 0;
		RenderColor = RenderColor.WithAlpha( a );
	}

	private bool ShouldRender()
	{
		if ( !Local.Pawn.IsValid() ) return true;
		if ( Local.Pawn is not PlatformerPawn pl ) return true;

		return !pl.KeysPlayerHas.Contains( KeyNumber );
	}

}
