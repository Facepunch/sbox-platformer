using Sandbox;

namespace Platformer.Movement
{
	class GroundSlam : BaseMoveMechanic
	{

		public float SlamGravity => 750f;
		public bool Slamming { get; set; }

		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		private TimeSince tsGroundSlam;

		public GroundSlam( PlatformerController controller ) : base( controller )
		{
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			Slamming = false;

			if ( ctrl.Pawn is not PlatformerPawn pl ) return;

			if ( ctrl.GroundEntity != null ) return;
			if ( ctrl.Energy == 0 ) return;
			if ( !InputActions.Duck.Down() )
			{
				tsGroundSlam = 0;
				pl.IgnoreFallDamage = false;
				return;
			}
			if ( ctrl.Velocity.z > 0 ) return;
			if ( tsGroundSlam < .15f ) return;


			if ( InputActions.Duck.Down() )
			{
				Slamming = true;
				ctrl.Energy = (ctrl.Energy - ctrl.EnergyDrain * Time.Delta).Clamp( 0f, ctrl.MaxEnergy );
				ctrl.Velocity = ctrl.Velocity.WithZ( -SlamGravity );
				pl.IgnoreFallDamage = true;
			}


		}
	}
}
