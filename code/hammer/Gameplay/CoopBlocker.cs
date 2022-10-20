
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer
{
	/// <summary>
	/// A mesh ent that will disable and enable if coop mode.
	/// </summary>
    [Library( "plat_coop" )]
	[Solid]
	[RenderFields]
	[VisGroup( VisGroup.Dynamic )]
	[Display( Name = "Coop Blocker", GroupName = "Platformer", Description = "If its coop mode open routes." ), Category( "Gamemode" ), Icon( "piano_off" )]
	[HammerEntity]
	public partial class CoopBlocker : BrushEntity
	{

		[Net, Property( "Coop Blocker", "If set to true, it will show in coop mode, False means it will show not in coop mode." )]
		public bool ForCoop { get; set; }

		public override void Spawn()
		{
			base.Spawn();

		}

		[Event.Tick.Server]
		public void Tick()
		{

			Enabled = ForCoop ? !Platformer.CoopMode : Platformer.CoopMode;
			Collisions = ForCoop ? !Platformer.CoopMode : Platformer.CoopMode;

		}
	}
}
