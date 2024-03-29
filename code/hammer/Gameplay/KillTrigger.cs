﻿
using Sandbox;
using Editor;
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

		if ( !Game.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		pl.TakeDamage( new() { Damage = 9999 } );
	}

}
