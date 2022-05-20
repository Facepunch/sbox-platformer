
using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_healthpickup", Description = "Addition Health" )]
[Model( Model = "models/gameplay/temp/temp_health_01.vmdl" )]
[Display( Name = "Health Pickup", GroupName = "Platformer", Description = "Addition Health." ), Category( "Gameplay" ), Icon( "heart_broken" )]
[HammerEntity]
internal partial class HealthPickup : BaseCollectible
{

	public int NumberOfHealth { get; set; } = 1;

	protected override bool OnCollected( PlatformerPawn pl )
	{
		base.OnCollected( pl );

		if ( pl.Health == 4 ) return false;

		Particles.Create( "particles/gameplay/player/healthpickup/healthpickup.vpcf", pl );

		pl.Health++;
		pl.PickedUpItem( Color.Green );

		return true;
	}

	protected override void OnCollectedEffect()
	{
		base.OnCollectedEffect();

		Sound.FromEntity( "life.pickup", this );
	}

}
