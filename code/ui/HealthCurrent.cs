
using Sandbox.UI.Construct;
using System.Linq;

namespace Sandbox.UI
{
	public class HealthCurrent : Panel
	{

		private const int MaxHealth = 4;

		public Image HealthImage;

		public HealthCurrent()
		{
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
		}

	}
}
