﻿using Sandbox;
using Hammer;
using Sandbox.UI;


/// <summary>
/// When the player is inside the trigger it will display the location on the hud. It will fall back to the map name.
/// </summary>
[Library( "plat_areatrigger", Description = "When the player is inside the trigger it will display the location on the hud." )]
[Hammer.AutoApplyMaterial( "materials/editor/areatrigger/areatrigger.vmat" )]
[EntityTool( "Area Trigger", "Platformer", "Area Trigger." )]
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

		//pl.CurrentArea = LandMarkName;

		if ( other != null )
		{
			
			//NewArea.EnteredNewArea();

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