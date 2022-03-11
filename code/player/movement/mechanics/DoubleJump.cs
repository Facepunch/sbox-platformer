using Sandbox;

namespace Facepunch.Parkour
{
	class DoubleJump : BaseMoveMechanic
	{

		public float DoubleJumpStrength => 320f;
		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		private TimeUntil timeUntilCanDoubleJump;
		private bool justJumped;

		public DoubleJump( ParkourController controller ) : base( controller )
		{
		}

		public override void PostSimulate()
		{
			base.PostSimulate();

			if ( justJumped && !Input.Down( InputButton.Jump ) )
			{
				justJumped = false;
			}
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			if ( Input.Pressed( InputButton.Jump ) && ctrl.GroundEntity != null )
			{
				timeUntilCanDoubleJump = .25f;
				justJumped = true;
				return;
			}

			if ( justJumped ) return;
			if ( ctrl.GroundEntity != null ) return;
			if ( !Input.Released( InputButton.Jump ) ) return;
			if ( timeUntilCanDoubleJump > 0 ) return;
			if ( ctrl.GetMechanic<Glide>()?.Gliding ?? false ) return;

			ctrl.Velocity = ctrl.Velocity.WithZ( DoubleJumpStrength );
		}

	}
}
