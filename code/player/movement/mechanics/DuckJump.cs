﻿
using Sandbox;

namespace Platformer.Movement
{
	class DuckJump : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;
		public override bool TakesOverControl => false;

		public bool IsDuckjumping;

		public float JumpPower => 644f;

		public DuckJump( PlatformerController controller )
			: base( controller )
		{

		}

		public override void PreSimulate()
		{
			base.PostSimulate();

			if ( ctrl.GroundEntity == null )
			{
				IsDuckjumping = false;
				return;
			}

			//if ( ctrl.GetMechanic<Slide>().TimeSinceSlide <= 0) return;
			//This controls the time we can LJ during slide. ^^^^ TimeSince start of slide.
			//This also allows for combo jumps in the player can time correctly.

			//if ( !Input.Pressed( InputButton.Jump ) ) return;
			//if ( !Input.Down( InputButton.Duck ) ) return;
			//if ( ctrl.Velocity.WithZ( 0 ).Length >= 130 ) return;
			//Some Reason this made the longjump feel bad.

			if ( Input.Pressed( InputButton.Jump ) && Input.Down( InputButton.Duck ) && ctrl.Velocity.WithZ( 0 ).Length <= 5 )
			{
				IsDuckjumping = true;

				var flGroundFactor = 1.0f;
				var flMul = JumpPower;
				var startz = ctrl.Velocity.z;
				var jumpPower = startz + flMul * flGroundFactor;

				ctrl.ClearGroundEntity();
				ctrl.Velocity = ctrl.Rotation.Backward * 64 * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( jumpPower );
				ctrl.AddEvent( "jump" );

				DuckJumpEffect();
			}

		}

		private void DuckJumpEffect()
		{
			ctrl.AddEvent( "jump" );

			if ( !ctrl.Pawn.IsServer ) return;
			using var _ = Prediction.Off();

		}

	}
}