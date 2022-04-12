
using Sandbox;
using Hammer;
using System.ComponentModel.DataAnnotations;

namespace Platformer
{
	/// <summary>
	/// A generic brush/mesh that can toggle its visibilty and collisions, and can be broken.
	/// </summary>
    [Library( "plat_wait" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	[Display( Name = "Waiting Blocker", GroupName = "Platformer", Description = "Waiting Blocker." )]

	public partial class WaitingBlocker : BrushEntity
	{

		public override void Spawn()
		{
			base.Spawn();
		}

		[Event.Tick.Server]
		public void Tick()
		{
			if(Platformer.CurrentState == Platformer.GameStates.Live)
			{
				Enabled = false;
				Solid = false;
			}	
		}
	}
}
