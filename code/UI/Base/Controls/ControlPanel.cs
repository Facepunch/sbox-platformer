
using Platformer.Movement;
using Sandbox;
using Sandbox.UI;

namespace Platformer;

[UseTemplate]
internal class ControlPanel : Panel
{

	private bool built;
	private bool alwaysOpen => false;

	public override void Tick()
	{
		base.Tick();

		SetClass( "open", alwaysOpen || ( Input.Down( InputButton.Score ) ) );

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
			if ( string.IsNullOrEmpty( mech.HudName ) ) 
				continue;

			AddChild( new ControlEntry()
			{
				Name = mech.HudName,
				Description = mech.HudDescription
			} );
		}
	}

	public override void OnHotloaded() => Rebuild();
	protected override void PostTemplateApplied() => Rebuild();

}
