
using Hammer;
using Sandbox;

namespace Platformer;

[Library( "plat_glider", Description = "Glider Pickup" )]
[Model( Model = "models/citizen_props/coin01.vmdl" )]
[EntityTool( "Glider Pickup", "Platformer", "Glider Pickup." )]
internal partial class GliderPickup : BaseCollectible
{

	protected override bool OnCollected( PlatformerPawn pl )
	{
		base.OnCollected( pl );
		pl.PlayerHasGlider = true;

		pl.PlayerPickedUpGlider();

		return true;
	}

	protected override void OnCollectedEffect()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );
	}

}
