
using Platformer.Movement;
using Sandbox;
using Sandbox.UI;

namespace Platformer;

[UseTemplate]
internal class ControlPanel : Panel
{

	private bool built;

	public override void Tick()
	{
		base.Tick();

		if ( built ) return;

		Rebuild();
	}

	private void Rebuild()
	{
		DeleteChildren();

		if ( Local.Pawn is not PlatformerPawn p ) return;
		if ( p.Controller is not PlatformerController ctrl ) return;

		built = true;

		foreach ( var mech in ctrl.Mechanics )
		{
			AddChild( new ControlEntry( mech ) );
		}
	}

	public override void OnHotloaded() => Rebuild();
	protected override void PostTemplateApplied() => Rebuild();

}
