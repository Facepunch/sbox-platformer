using Sandbox;
using System.Threading;

namespace Platformer.Movement
{
	class GroundSlam : BaseMoveMechanic
	{

		[Net]
		public float SlamGravity => 750f;
		public bool Slamming { get; set; }

		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		private TimeSince tsGroundSlam;
		private bool HasStartedSlam;

		public GroundSlam( PlatformerController controller ) : base( controller )
		{
		}

		public override async void PreSimulate()
		{
			base.PreSimulate();

			if ( ctrl.Pawn is not PlatformerPawn pl ) return;

			if ( ctrl.GroundEntity != null )
			{
				pl.IgnoreFallDamage = false;
				Slamming = false;
				HasStartedSlam = false;
				tsGroundSlam = 0;
				return;
			}

			if ( InputActions.Duck.Pressed() && Slamming == false )
			{
				ctrl.Velocity = 0;
				ctrl.Velocity = ctrl.Velocity.WithZ( 150 );
				
				if ( HasStartedSlam ) return;
				HasStartedSlam = true;
				await GameTask.Delay( 250 );
				Slamming = true;
				pl.IgnoreFallDamage = true;
			}
			SlameTime();
		}

		[Event.Tick]
		public void SlameTime()
		{
			if ( Slamming )
			{

				ctrl.Velocity = ctrl.Velocity.WithZ( -SlamGravity );
			}
		}
	}
}
