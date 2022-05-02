
using Sandbox;
using Hammer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Platformer
{

    [Library( "plat_gamemodeselect" )]
	[Hammer.VisGroup( Hammer.VisGroup.Logic )]
	[Hammer.EditorSprite( "materials/editor/gamemode/gamemode.vmat" )]
	[Display( Name = "Game Mode Select", GroupName = "Platformer", Description = "Game Mode Select." ), Category( "Gamemode" ), Icon( "videogame_asset" )]

	public partial class GameModeSelect : Entity
	{
		public enum ModeType
		{
			Competitive,
			Coop,
			Tag
		}

		[Property( "model_type", Title = "Model Type" ), Net]
		public ModeType ModeTypeList { get; set; } = ModeType.Competitive;

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Always;

			//var game = Game.Current as Platformer;
			//if ( !game.IsValid() ) return;

			//if ( ModeTypeList == ModeType.Competitive )
			//{
			//	game.SetGameMode(Platformer.GameModes.Competitive);
			//}
			//if ( ModeTypeList == ModeType.Coop )
			//{
			//	game.SetGameMode( Platformer.GameModes.Coop );
			//}
			//if ( ModeTypeList == ModeType.Tag )
			//{
			//	game.SetGameMode( Platformer.GameModes.Tag );
			//}

		}
	}
}
