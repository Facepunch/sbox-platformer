
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer;

/// <summary>
/// When the player is inside the trigger it will display the location on the hud. It will fall back to the map name.
/// </summary>
[Library( "plat_areatrigger")]
[AutoApplyMaterial( "materials/editor/areatrigger/areatrigger.vmat" )]
[Display( Name = "Area Trigger",GroupName = "Platformer", Description = "When the player is inside the trigger it will display the location on the hud."), Category( "Triggers" ), Icon( "follow_the_signs" )]
[HammerEntity]
internal partial class AreaTrigger : BaseTrigger
{
	/// <summary>
	/// Name of the location.
	/// </summary>
	[Property( "landmarkname", Title = "Area Name" )]
	public string LandMarkName { get; set; } = "";


	/// <summary>
	/// Priority of the volume.
	/// </summary>
	[Property( "priority", Title = "Priority" )]
	public int Priority { get; set; } = 0;

	/// <summary>
	/// Fall back if there is no other volume.
	/// </summary>
	[Property( "mainarea", Title = "Main Area" )]
	public bool MainArea { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();


		EnableTouchPersists = true;

	}
	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		if (MainArea == true && pl.AreaPriority == 0)
		{
			pl.CurrentArea = LandMarkName;
		}
		if ( MainArea == false && pl.AreaPriority <= 1 )
		{
			pl.CurrentArea = LandMarkName;
		}
	}
	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		pl.AreaPriority = Priority;

	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;

		pl.AreaPriority = 0;

	}

}
