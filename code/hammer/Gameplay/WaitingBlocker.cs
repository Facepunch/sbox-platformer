
using Platformer.Gamemodes;
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

/// <summary>
/// A generic brush/mesh that will disable after the waiting period.
/// </summary>
[Library( "plat_wait" )]
[Solid]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[Display( Name = "Waiting Blocker", GroupName = "Platformer", Description = "Waiting Blocker." ), Category( "Gameplay" ), Icon( "av_timer" )]
[HammerEntity]
public partial class WaitingBlocker : BrushEntity
{

	public override void Spawn()
	{
		base.Spawn();
	}

	[Event.Tick.Server]
	public void Tick()
	{
		if ( Platformer.GameState == GameStates.Live )
		{
			Enabled = false;
			Collisions = false;
		}
	}
}
