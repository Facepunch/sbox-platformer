using Sandbox;

namespace Facepunch.Parkour
{
	class Glide : BaseMoveMechanic
	{

		public float GlideGravity => 20f;

		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		public Glide( ParkourController controller ) : base( controller )
		{
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			if ( ctrl.GroundEntity != null ) return;
			if ( !Input.Down( InputButton.Jump ) ) return;
			if ( ctrl.Velocity.z > 0 ) return;

			ctrl.Velocity = ctrl.Velocity.WithZ( -GlideGravity );
		}

	}
}
