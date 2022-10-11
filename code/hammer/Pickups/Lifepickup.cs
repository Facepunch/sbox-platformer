
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

namespace Platformer;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/gameplay/collect/life/life_pickup.vmdl" )]
[Display( Name = "Life Pickup", GroupName = "Platformer", Description = "Addition Life" ), Category( "Gameplay" ), Icon( "woman" )]
[HammerEntity]
internal partial class LifePickup : BaseCollectible
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	private Vector3 OGPos;

	public override void Spawn()
	{
		base.Spawn();

		OGPos = Position;

	}

	[Event.Tick.Server]
	public void Tick()
	{
		Position = Position.WithZ( Position.z + MathF.Sin( Time.Now ) / 6);

		Rotation = Rotation.FromYaw( Rotation.Yaw() + -50 * Time.Delta );

	}

	protected override bool OnCollected( PlatformerPawn pl )
	{
		pl.NumberLife++;
		pl.PickedUpItem( Color.Orange );

		Particles.Create( "particles/gameplay/player/lifepickup/lifepickup.vpcf", pl );

		return true;
	}

	protected override void OnCollectedEffect()
	{
		base.OnCollectedEffect();

		Sound.FromEntity( "life.pickup", this );
	}

}
