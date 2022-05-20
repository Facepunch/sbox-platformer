
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer;

[Library( "plat_trigger_kill", Description = "Kills the player." )]
[AutoApplyMaterial( "materials/editor/killtrigger/killtrigger.vmat" )]
[Display( Name = "Trigger Kill", GroupName = "Platformer", Description = "Kills the player." ), Category( "Triggers" ), Icon( "church" )]
[HammerEntity]
internal partial class KillTrigger : BaseTrigger
{

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		Game.Current.DoPlayerSuicide( pl.Client );
	}

}
