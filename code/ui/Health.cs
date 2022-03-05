
using System.Linq;

namespace Sandbox.UI
{
	public class Health : Panel
	{

		private const int MaxHealth = 4;

		public Health()
		{
			for ( int i = 0; i < MaxHealth; i++ )
			{
				var block = new Panel( this, "health-block" );
				var fill = new Panel( block, "fill" );
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

				block.SetClass( "lowhealth", pl.Health <= 2 );
				block.SetClass( "visible", pl.Health >= i + 1 );
			}
		}

	}
}
