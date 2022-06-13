
using Sandbox;
using SandboxEditor;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_gamemodeselect" )]
[VisGroup( VisGroup.Logic )]
[EditorSprite( "materials/editor/gamemode/gamemode.vmat" )]
[Display( Name = "Game Mode Select", GroupName = "Platformer", Description = "Game Mode Select." ), Category( "Gamemode" ), Icon( "videogame_asset" )]
[HammerEntity]
public partial class GameModeSelect : Entity
{

	[Property( "model_type", Title = "Model Type" ), Net]
	public Platformer.GameModes ModeTypeList { get; set; } = Platformer.GameModes.Competitive;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

}
