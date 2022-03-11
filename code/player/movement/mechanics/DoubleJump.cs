using Sandbox;

namespace Facepunch.Parkour
{
	class DoubleJump : BaseMoveMechanic
	{

		public float DoubleJumpStrength => 320f;
		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		private bool canDoubleJump;

		public DoubleJump( ParkourController controller ) : base( controller )
		{
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			if ( Input.Pressed( InputButton.Jump ) && ctrl.GroundEntity != null )
			{
				canDoubleJump = true;
				return;
			}

			if ( !canDoubleJump ) return;
			if ( !Input.Pressed( InputButton.Jump ) ) return;

			ctrl.Velocity = ctrl.Velocity.WithZ( DoubleJumpStrength );
		}

	}
}
