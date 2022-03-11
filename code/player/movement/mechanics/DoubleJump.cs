using Sandbox;

namespace Facepunch.Parkour
{
	partial class DoubleJump : BaseMoveMechanic
	{

		public float DoubleJumpStrength => 320f;
		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;

		[Net, Predicted]
		public int DoubleJumpsRemaining { get; set; }

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

			if( ctrl.GroundEntity != null )
			{
				DoubleJumpsRemaining = 1;

				if ( Input.Pressed( InputButton.Jump ) )
				{
					timeUntilCanDoubleJump = .25f;
					justJumped = true;
				}
			}

			if ( justJumped ) return;
			if ( ctrl.GroundEntity != null ) return;
			if ( !Input.Released( InputButton.Jump ) ) return;
			if ( timeUntilCanDoubleJump > 0 ) return;
			if ( ctrl.GetMechanic<Glide>()?.Gliding ?? false ) return;
			if ( DoubleJumpsRemaining <= 0 ) return;

			ctrl.Velocity = ctrl.Velocity.WithZ( DoubleJumpStrength );
			DoubleJumpsRemaining--;

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
