using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Sandbox.UI
{
	public class Health : Panel
	{
		public Label Label;
		private float _health;

		private RealTimeUntil HealthGrowTime { get; set; }
		private float LastHealth { get; set; }

		public Health()
		{
			Label = Add.Label( "100", "value" );
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;
			var health = player.Health;
			_health = _health.LerpTo( health, Time.Delta * 10 );

			Label.SetClass( "healthlow", health < 100 * .25f );

			Label.Text = $"{(int)(_health + 0.5f)}";

			//Label.SetClass( "low", _health < 25 );


			if ( health != LastHealth && HealthGrowTime )
			{
				HealthGrowTime = 0.02f;
			}

			LastHealth = health;
		}

	}
}
