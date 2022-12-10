
using Sandbox;
using Editor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer;

/// <summary>
/// Triggers when Prop Carriable is inside volume.
/// </summary>
[Library( "plat_buttontrigger", Description = "Triggers when Prop Carriable is inside volume" )]
[AutoApplyMaterial( "materials/editor/areatrigger/areatrigger.vmat" )]
[Display( Name = "Button Trigger", GroupName = "Platformer", Description = "Triggers when Prop Carriable is inside volume" ), Category( "Triggers" ), Icon( "radio_button_checked" )]
[HammerEntity]
internal partial class ButtonTrigger : BaseTrigger
{

	public override void Spawn()
	{
		base.Spawn();


		EnableTouchPersists = true;

	}
	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( !Game.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;



	}
	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !Game.IsServer ) return;
		if(other is PropCarriable prop)
		{
			_ = OnPressed.Fire( this );
		}

	}


	protected Output OnPressed { get; set; }

	protected Output UnPressed { get; set; }

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( !Game.IsServer ) return;

		if ( other is PropCarriable prop )
		{
			_ = UnPressed.Fire( this );
		}

	}

}
