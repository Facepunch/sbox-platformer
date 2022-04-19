
using Sandbox;

namespace Platformer.Movement
{
	partial class DoubleJump : BaseMoveMechanic
	{

		public float DoubleJumpStrength => 320f;
		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		public int DoubleJumpsRemaining { get; set; }

		private TimeUntil timeUntilCanDoubleJump;
		private bool justJumped;

		public DoubleJump( PlatformerController controller ) : base( controller )
		{
		}

		public override void PostSimulate()
		{
			base.PostSimulate();

			if ( justJumped && !InputActions.Jump.Down() )
			{
				justJumped = false;
			}
		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			if( ctrl.GroundEntity != null )
			{
				timeUntilCanDoubleJump = .25f;
				DoubleJumpsRemaining = 1;

				if ( InputActions.Jump.Pressed() )
				{
					justJumped = true;
				}
			}

			if ( justJumped ) return;
			if ( ctrl.GroundEntity != null ) return;
			if ( !InputActions.Jump.Pressed() ) return;
			if ( timeUntilCanDoubleJump > 0 ) return;
			if ( ctrl.GetMechanic<Glide>()?.Gliding ?? false ) return;
			if ( ctrl.GetMechanic<DuckJump>().IsDuckjumping == true ) return;
			if ( DoubleJumpsRemaining <= 0 ) return;

			ctrl.Velocity = ctrl.Velocity.WithZ( DoubleJumpStrength );
			DoubleJumpsRemaining--;


			var groundslam = ctrl.GetMechanic<GroundSlam>();
			if ( groundslam != null && groundslam.IsActive )
			{
				groundslam.Cancel();
				ctrl.Velocity = ctrl.Velocity.WithZ( 220 );
			}

			DoubleJumpEffect();
		}

		private void DoubleJumpEffect()
		{
			if ( !ctrl.Pawn.IsServer ) return;

			using var _ = Prediction.Off();

			ctrl.AddEvent( "jump" );

			Particles.Create( "particles/gameplay/player/doublejump/doublejump.vpcf", ctrl.Pawn );
			Sound.FromWorld( "player.djump", ctrl.Pawn.Position );
		}

	}
}
