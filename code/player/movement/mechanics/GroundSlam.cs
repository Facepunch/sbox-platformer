
using Sandbox;

namespace Platformer.Movement
{
	class GroundSlam : BaseMoveMechanic
	{

		public float SlamGravity => 2250f;

		public override bool TakesOverControl => true;
		public override bool AlwaysSimulate => false;

		public GroundSlam( PlatformerController controller ) : base( controller )
		{
		}

		protected override bool TryActivate()
		{
			if ( ctrl.GroundEntity.IsValid() ) return false;
			if ( !InputActions.Duck.Pressed() ) return false;

			ctrl.Velocity *= .35f;

			return true;
		}

		public override void Simulate()
		{
			base.Simulate();

			if ( ctrl.Pawn is not PlatformerPawn pl )
			{
				IsActive = false;
				return;
			}

			if ( ctrl.GroundEntity != null )
			{
				pl.IgnoreFallDamage = false;
				IsActive = false;
				return;
			}

			pl.IgnoreFallDamage = true;
			
			var tr = Trace.Ray( ctrl.Position, ctrl.Position + Vector3.Down * 12 )
				.Ignore( ctrl.Pawn )
				.Radius( 4 )
				.Run();

			if ( tr.Hit )
			{
				var damageInfo = DamageInfo.Generic( 80 );
				var box = tr.Entity;
				box.TakeDamage( damageInfo );
				GroundEffect();
			}

			ctrl.Velocity += ctrl.Velocity.WithZ( -SlamGravity ) * Time.Delta;
			ctrl.Move();
		}

		private void GroundEffect()
		{
			if ( !ctrl.Pawn.IsServer ) return;
			using var _ = Prediction.Off();

			ctrl.AddEvent( "sitting" );

			Particles.Create( "particles/gameplay/player/slamland/slamland.vpcf", ctrl.Pawn );
			Sound.FromWorld( "player.slam.land", ctrl.Pawn.Position );


		}

	}
}
