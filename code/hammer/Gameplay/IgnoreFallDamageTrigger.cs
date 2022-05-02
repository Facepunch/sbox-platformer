
using Sandbox;
using Hammer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer;

[Library( "plat_ignorefalldamage", Description = "Ignore Fall Damage." )]
[Hammer.AutoApplyMaterial( "materials/editor/ignorefalldamage/ignorefalldamage.vmat" )]
[Display( Name = "Ignore Fall Damage", GroupName = "Platformer", Description = "Ignore Fall Damage" ), Category( "Triggers" ), Icon( "do_not_step" )]
internal partial class IgnoreFallDamageTrigger : BaseTrigger
{

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		pl.IgnoreFallDamage = true;
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		pl.IgnoreFallDamage = false;
	}

}
