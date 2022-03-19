
using Sandbox;

namespace Platformer.Movement
{
    class Slide : BaseMoveMechanic
	{

		public float StopSpeed => 50f;
		public float Friction => 2f;
		public float EndSlideSpeed => 120f;
		public float StartSlideSpeed => 240f;
		public float SlideBoost => 275f;
		public TimeSince TimeSinceSlide { get; set; }
		public bool Sliding { get; private set; }

		public override float EyePosMultiplier => .35f;
		public override bool TakesOverControl => true;

		private Vector3 originalMins;
		private Vector3 originalMaxs;

		public Slide(PlatformerController ctrl)
			: base(ctrl)
		{

		}

		protected override bool TryActivate()
		{
			if ( !InputActions.Duck.Down() ) return false;
			if ( ctrl.GroundEntity == null ) return false;
			if ( ctrl.Velocity.WithZ( 0 ).Length < StartSlideSpeed ) return false;
			if ( ctrl.GetMechanic<LongJump>().IsLongjumping ) return false;

			TimeSinceSlide = 0;

			var len = ctrl.Velocity.WithZ( 0 ).Length;
			var newLen = len + SlideBoost;
			ctrl.Velocity *= newLen / len;
			ctrl.SetTag( "sitting" );

			new FallCameraModifier( -300 );

			return true;
		}

		public override void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale = 1 )
		{
			base.UpdateBBox( ref mins, ref maxs, scale );

			originalMins = mins;
			originalMaxs = maxs;

			maxs = maxs.WithZ( 20 * scale );
		}

		public override float GetWishSpeed()
		{
			return 100;
		}

		public override void Simulate()
		{
			if ( !StillSliding() )
			{
				IsActive = false;

				return;
			}
			ctrl.SetTag( "sitting" );

			if ( ctrl.GetMechanic<LongJump>().IsLongjumping ) return;

			if ( ctrl.GroundNormal.z < 1 )
			{
				//Sliding = true;

				var slopeDir = Vector3.Cross( Vector3.Up, Vector3.Cross( Vector3.Up, ctrl.GroundNormal ) );
				var dot = Vector3.Dot( ctrl.Velocity.Normal, slopeDir );
				var slopeForward = Vector3.Cross( ctrl.GroundNormal, ctrl.Pawn.Rotation.Right );
				var spdGain = 50f.LerpTo( 350f, 1f - ctrl.GroundNormal.z );

				if ( dot > 0 )
					spdGain *= -1;

				ctrl.Velocity += spdGain * slopeForward * Time.Delta;
			}
			else
			{
				ctrl.Velocity = ctrl.Velocity.WithZ( 0 );
				ctrl.ApplyFriction( StopSpeed, Friction );
				//Sliding = false;
			}

			ctrl.Move();
		}

		private bool StillSliding()
		{
			if ( !InputActions.Duck.Down() ) return false;
			if ( ctrl.GroundEntity == null ) return false;
			if ( ctrl.Velocity.WithZ( 0 ).Length < EndSlideSpeed ) return false;
			return true;
		}

	}
}
