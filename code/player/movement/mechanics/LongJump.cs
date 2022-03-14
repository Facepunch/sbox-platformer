
using Sandbox;

namespace Platformer.Movement
{
	class LongJump : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;
		public override bool TakesOverControl => false;

		public bool IsLongjumping;

		public LongJump( PlatformerController controller )
			: base( controller )
		{

		}

		public override void PreSimulate()
		{
			base.PostSimulate();

			if ( ctrl.GroundEntity == null )
			{
				IsLongjumping = false;
				return;
			}

			if ( ctrl.GetMechanic<Slide>().TimeSinceSlide >= 0.15 ) return;
			//This controls the time we can LJ during slide. ^^^^ TimeSince start of slide.
			//This also allows for combo jumps in the player can time correctly.

			//if ( !Input.Pressed( InputButton.Jump ) ) return;
			//if ( !Input.Down( InputButton.Duck ) ) return;
			//if ( ctrl.Velocity.WithZ( 0 ).Length >= 130 ) return;
			//Some Reason this made the longjump feel bad.

			if ( Input.Pressed( InputButton.Jump ) && Input.Down( InputButton.Duck ) && Input.Down( InputButton.Forward ) && ctrl.Velocity.WithZ( 0 ).Length >= 120 )
			{
				IsLongjumping = true;

				float flGroundFactor = 1.0f;
				float flMul = 300f * 1.2f;
				float forMul = 485f * 1.2f;

				ctrl.Velocity = ctrl.Rotation.Forward * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;

				LongJumpEffect();
			}
			if ( Input.Pressed( InputButton.Jump ) && Input.Down( InputButton.Duck ) && Input.Down( InputButton.Left ) && ctrl.Velocity.WithZ( 0 ).Length >= 120 )
			{
				IsLongjumping = true;

				float flGroundFactor = 1.0f;
				float flMul = 300f * 1.2f;
				float forMul = 485f * 1.2f;

				ctrl.Velocity = ctrl.Rotation.Left * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;

				LongJumpEffect();
			}
			if ( Input.Pressed( InputButton.Jump ) && Input.Down( InputButton.Duck ) && Input.Down( InputButton.Right ) && ctrl.Velocity.WithZ( 0 ).Length >= 120 )
			{
				IsLongjumping = true;

				float flGroundFactor = 1.0f;
				float flMul = 300f * 1.2f;
				float forMul = 485f * 1.2f;

				ctrl.Velocity = ctrl.Rotation.Right * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;

				LongJumpEffect();
			}
			if ( Input.Pressed( InputButton.Jump ) && Input.Down( InputButton.Duck ) && Input.Down( InputButton.Back ) && ctrl.Velocity.WithZ( 0 ).Length >= 120 )
			{
				IsLongjumping = true;

				float flGroundFactor = 1.0f;
				float flMul = 300f * 1.2f;
				float forMul = 485f * 1.2f;

				ctrl.Velocity = ctrl.Rotation.Backward * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;

				LongJumpEffect();
			}

		}

		private void LongJumpEffect()
		{
			ctrl.AddEvent( "jump" );

			if ( !ctrl.Pawn.IsServer ) return;
			using var _ = Prediction.Off();

			var color = ctrl.Pawn is PlatformerPawn p ? p.PlayerColor : Color.Green;
			var particle = Particles.Create( "particles/gameplay/player/longjumptrail/longjumptrail.vpcf", ctrl.Pawn );
			particle.SetPosition( 6, color * 255f );
		}

	}
}
