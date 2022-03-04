using Sandbox;
using Hammer;

[Library( "plat_trigger_fall", Description = "Makes the player fall" )]
[Hammer.AutoApplyMaterial("materials/editor/uf_trigger_fall.vmat")]
[EntityTool( "Trigger Fall", "Platformer", "Makes the player fall." )]
internal partial class FallTrigger : BaseTrigger
{

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		Game.Current.DoPlayerSuicide( pl.Client );
	}

}
