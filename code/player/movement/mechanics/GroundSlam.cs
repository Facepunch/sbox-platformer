﻿
using Sandbox;
using System;

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

			Sound.FromWorld( "player.slam.land", ctrl.Position );
			Particles.Create( "particles/gameplay/player/slamland/slamland.vpcf", ctrl.Position );

			var effectRadius = 100f;
			var overlaps = Entity.FindInSphere( ctrl.Position, effectRadius );

			foreach( var overlap in overlaps )
			{
				if ( overlap is not ModelEntity ent || !ent.IsValid() )
					continue;

				if ( ent.LifeState != LifeState.Alive )
					continue;

				if ( !ent.PhysicsBody.IsValid() )
					continue;

				if ( ent.IsWorld )
					continue;

				if ( ent is PlatformerPawn )
					continue;

				var targetPos = ent.PhysicsBody.MassCenter;

				var dist = Vector3.DistanceBetween( ctrl.Position, targetPos );
				if ( dist > effectRadius )
					continue;

				var forceMult = ent is PropGib ? 60f : 6f;
				var distanceMul = 1.0f - Math.Clamp( dist / effectRadius, 0.0f, 1.0f );
				var force = (forceMult * distanceMul) * ent.PhysicsBody.Mass;
				var forceDir = (targetPos - ctrl.Position).Normal;

				ent.ApplyAbsoluteImpulse( forceDir * force );
			}
		}

	}
}
