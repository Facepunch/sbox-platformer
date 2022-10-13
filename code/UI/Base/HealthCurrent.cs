
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace Platformer.UI
{
	public class HealthCurrent : Panel
	{
		private const int MaxHealth = 4;
		public Image HealthImage;

		List<Panel> Blocks { get; set; } = new();

		public HealthCurrent()
		{
			for ( int i = 0; i < MaxHealth; i++ )
				Blocks.Add( Add.Panel( $"health-block health-block-{i}" ) );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not Player pl ) return;

			for ( int i = 0; i < Blocks.Count; i++ )
			{
				var block = Blocks[i];
				if ( block == null ) continue;

				block.SetClass( "visible", pl.Health >= i + 1 );
			}
		}

	}
}
