﻿
using Sandbox;

namespace Platformer.Movement
{
	class AirDash : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;
		public override bool TakesOverControl => false;

		public bool IsAirDashing;

		private bool CanDash;

		public AirDash( PlatformerController controller )
			: base( controller )
		{

		}

		public override void PreSimulate()
		{
			base.PostSimulate();

			if ( ctrl.GroundEntity != null )
			{
				CanDash = true;
				IsAirDashing = false;
				return;
			}

			//if ( ctrl.GetMechanic<Slide>().TimeSinceSlide >= 0.20 ) return;
			//This controls the time we can LJ during slide. ^^^^ TimeSince start of slide.
			//This also allows for combo jumps in the player can time correctly.

			//if ( !Input.Pressed( InputButton.Jump ) ) return;
			//if ( !Input.Down( InputButton.Duck ) ) return;
			//if ( ctrl.Velocity.WithZ( 0 ).Length >= 130 ) return;
			//Some Reason this made the longjump feel bad.


			if ( ctrl.GroundEntity == null && InputActions.Walk.Pressed() && CanDash == true )
			{
				IsAirDashing = true;

				CanDash = false;

				float flGroundFactor = 1.0f;
				float flMul = 100f * 1.2f;
				float forMul = 585f * 1.2f;

				ctrl.Velocity = ctrl.Rotation.Forward * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;

				if( ctrl.GetMechanic<GroundSlam>()?.IsActive ?? false )
				{
					ctrl.Velocity = ctrl.Velocity.WithZ( 400 );
				}

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
			Sound.FromWorld( "player.ljump", ctrl.Pawn.Position );
			particle.SetPosition( 6, color * 255f );
		}

	}
}
