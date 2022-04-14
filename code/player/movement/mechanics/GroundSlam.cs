
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
				GroundEffect();
				return;
			}

			pl.IgnoreFallDamage = true;

			var ents = Entity.FindInSphere( ctrl.Position, 30f );
			foreach( var ent in ents )
			{
				if ( ent is PlatformerPawn ) continue;
				ent.TakeDamage( DamageInfo.Generic( 80 ) );
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
