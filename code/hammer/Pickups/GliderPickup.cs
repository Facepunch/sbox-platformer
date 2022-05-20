
using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_glider", Description = "Glider Pickup" )]
[EditorModel( "models/gameplay/glider/handglider.vmdl",FixedBounds = true )]
[Display( Name = "Glider Pickup", GroupName = "Platformer", Description = "Glider Pickup." ), Category( "Gameplay" ), Icon( "paragliding" )]
[HammerEntity]
internal partial class GliderPickup : BaseCollectible
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/gameplay/glider/handglider.vmdl");
	}
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

	[Event.Tick.Server]
	public void Tick()
	{
		Rotation = Rotation.FromYaw( Rotation.Yaw() + 100 * Time.Delta );
	}
}
