using Sandbox;

namespace Facepunch.Parkour
{
	class Glide : BaseMoveMechanic
	{

		public float GlideGravity => 20f;
		public bool Gliding { get; private set; }

		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		private TimeSince tsJumpHold;

		public Glide( ParkourController controller ) : base( controller )
		{
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			Gliding = false;

			if ( ctrl.GroundEntity != null ) return;
			if ( !Input.Down( InputButton.Jump ) )
			{
				tsJumpHold = 0;
				return;
			}
			if ( ctrl.Velocity.z > 0 ) return;
			if( tsJumpHold < .15f ) return;

			Gliding = true;

			ctrl.Velocity = ctrl.Velocity.WithZ( -GlideGravity );
		}

	}
}
