
using Sandbox;
using Hammer;
using System.ComponentModel.DataAnnotations;

namespace Platformer
{
	/// <summary>
	/// A generic brush/mesh that can toggle its visibilty and collisions, and can be broken.
	/// </summary>
    [Library( "plat_coop" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	[Display( Name = "Coop Blocker", GroupName = "Platformer", Description = "If its coop mode open routes." )]

	public partial class CoopBlocker : BrushEntity
	{

		[Net, Property( "Coop Blocker", "If set to true, it will show in coop mode, False means it will show not in coop mode." )]
		public bool ForCoop { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			//SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			//EnableAllCollisions = true;



		}

		[Event.Tick.Server]
		public void Tick()
		{

			Enabled = ForCoop ? !Platformer.CoopMode : Platformer.CoopMode;
			Solid = ForCoop ? !Platformer.CoopMode : Platformer.CoopMode;


		}
	}
}
