using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_coincheck", Description = "A Volume that triggers if player has x amount of coins." )]
[Hammer.EditorSprite( "materials/editor/cointrigger/cointrigger.vmat" )]
[Display( Name = "Player Checkpoint", GroupName = "Platformer", Description = "A Volume that triggers if player has x amount of coins." )]
[BoundsHelper( "mins", "maxs", true, false )]
internal partial class CoinTrigger : ModelEntity
{
	private bool BeenTrigger;

	[Property( "mins", Title = "Checkpoint mins" )]
	[Net]
	[DefaultValue( "-32 -32 0" )]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Checkpoint maxs" )]
	[Net]
	[DefaultValue( "32 32 64" )]
	public Vector3 Maxs { get; set; } = new Vector3(32, 32, 64);

	[Net, Property]
	public int AmountOfCoins { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
		EnableAllCollisions = true;
		EnableTouch = true;

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromOBB( PhysicsMotionType.Static, Mins, Maxs );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;

	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

	}

	public override void StartTouch( Entity other )
	{
		base.Touch( other );

		if ( other is not PlatformerPawn pl ) return;

		if ( BeenTrigger ) return;

		if (pl.Coin >= AmountOfCoins)
		{
			_ = HasEnoughCoins.Fire( this );

			BeenTrigger = true;

		}

	}
	/// <summary>
	/// Triggers on set amount of coins.
	/// </summary>
	protected Output HasEnoughCoins { get; set; }

	/// <summary>
	/// Reset Trigger.
	/// </summary>
	[Input]
	public void ResetTrigger()
	{
		BeenTrigger = false;
	}
}

