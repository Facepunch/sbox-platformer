
using Sandbox;
using Hammer;

namespace Platformer;

[Library( "plat_trigger_kill", Description = "Kills the player." )]
[Hammer.AutoApplyMaterial( "materials/editor/killtrigger/killtrigger.vmat" )]
[EntityTool( "Trigger Kill", "Platformer", "Kills the player." )]
internal partial class KillTrigger : BaseTrigger
{

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		Game.Current.DoPlayerSuicide( pl.Client );
	}

}
