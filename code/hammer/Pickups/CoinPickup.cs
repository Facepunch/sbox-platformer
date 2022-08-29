
using SandboxEditor;
using Sandbox;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_coin", Description = "Coin Pickup" )]
//[Model( Model = "models/gameplay/collect/coin/coin01.vmdl" )]
[EditorModel( "models/gameplay/collect/coin/coin01.vmdl", FixedBounds = true )]
[Display( Name = "Coin Pickup", GroupName = "Platformer", Description = "Coin Pickup." ), Category( "Gameplay" ), Icon( "currency_bitcoin" )]
[HammerEntity]
internal partial class CoinPickup : BaseCollectible
{
	private Particles CoinParticle;


	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, true );

		Tags.Add( "trigger" );
		EnableAllCollisions = true;
		PhysicsEnabled = false;
		UsePhysicsCollision = false;

		SetModel( "models/gameplay/collect/coin/coin01.vmdl" );

		SetMaterialGroup( new Random().Next( 0, 3 ).ToString() );
	}

	public void SpawnWithPhys()
	{
		PhysicsEnabled = true;
		UsePhysicsCollision = false;

	}

	protected override bool OnCollected( PlatformerPawn pl )
	{
		base.OnCollected( pl );

		pl.Coin++;
		pl.PickedUpItem( Color.Yellow );

		pl.Client.AddInt( "kills" );

		CoinParticle.Destroy();

		return true;
	}

	protected override void OnCollectedEffect()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/gameplay/player/coincollected/coincollected.vpcf", this );
	}

	[Event.Tick.Server]
	public void Tick()
	{
		Rotation = Rotation.FromYaw( Rotation.Yaw() + -75 * Time.Delta );

		if(CoinParticle == null)
		{
			CoinParticle = Particles.Create( "particles/gameplay/idle_coin/idle_coin.vpcf", this );
		}
	}
}
