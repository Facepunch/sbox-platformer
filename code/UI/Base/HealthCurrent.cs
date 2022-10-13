
using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Platformer.UI
{
	public class HealthCurrent : Panel
	{
	//	static Color HighColor = new Color( 0.45f, 0.72f, 1f );
		
	//	static Color LowColor = new Color( 0.83f, 0.18f, 0.19f );
			
		private const int MaxHealth = 4;

		public Image HealthImage;
		//public CitizenPanel Avatar { get; set; }
			
		public HealthCurrent()
		{
			
		//	Avatar = new CitizenPanel( Local.Client );
		//	AddChild( Avatar );
			
			for ( int i = 0; i < MaxHealth; i++ )
			{
				var block = new Panel( this, $"health-block health-block-{i}" );
			}

		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not Player pl ) return;

			for( int i = 0; i < MaxHealth; i++ )
			{
				var block = Children.ElementAtOrDefault( i );
				if ( block == null ) continue;
				block.SetClass( "visible", pl.Health >= i + 1 );
							
			}
		//	Avatar.Style.BackgroundColor = pl.Health >= 1 ? HighColor : LowColor;
		}

	}
}
