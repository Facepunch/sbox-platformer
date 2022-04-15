
using Sandbox;

namespace Platformer.Movement
{
	class GroundSlam : BaseMoveMechanic
	{

		public float SlamGravity => 2250f;

		public override bool TakesOverControl => true;
		public override bool AlwaysSimulate => false;

		private TimeUntil FreezeTimer;

		public GroundSlam( PlatformerController controller ) : base( controller )
		{
		}

		protected override bool TryActivate()
		{
			if ( ctrl.GroundEntity.IsValid() ) return false;
			if ( !InputActions.Duck.Pressed() ) return false;

			ctrl.Velocity = 0f;
			FreezeTimer = .25f;

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
				GroundEffect();
				IsActive = false;
				pl.IgnoreFallDamage = false;
				return;
			}

			if ( FreezeTimer > 0 )
			{
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

		public void Cancel()
		{
			IsActive = false;
		}

		private void GroundEffect()
		{
			ctrl.AddEvent( "sitting" );

			if ( !ctrl.Pawn.IsServer ) return;

			using var _ = Prediction.Off();

			new ExplosionEntity()
			{
				Position = ctrl.Position,
				Radius = 6f,//Low so doesn't effect others players as much for now.
				Damage = 0,
				ForceScale = 25f,
				ParticleOverride = "particles/gameplay/player/slamland/slamland.vpcf",
				SoundOverride = "player.slam.land"
			}.Explode( ctrl.Pawn );
		}

	}
}
