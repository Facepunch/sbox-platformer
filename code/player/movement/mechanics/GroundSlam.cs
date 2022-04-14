using Sandbox;
using System.Threading;

namespace Platformer.Movement
{
	class GroundSlam : BaseMoveMechanic
	{

		[Net]
		public float SlamGravity => 750f;
		public bool Slamming { get; set; }

		public override bool TakesOverControl => false;
		public override bool AlwaysSimulate => true;



		private bool HasStartedSlam;

		private bool JustLanded;


		public GroundSlam( PlatformerController controller ) : base( controller )
		{
		}

		public override async void PreSimulate()
		{
			base.PreSimulate();

			if ( ctrl.Pawn is not PlatformerPawn pl ) return;

			if ( ctrl.GroundEntity != null )
			{
				pl.IgnoreFallDamage = false;
				Slamming = false;
				HasStartedSlam = false;
				JustLanded = false;
				return;
			}

			if ( InputActions.Duck.Pressed() && Slamming == false )
			{
				ctrl.Velocity = 0;
				ctrl.Velocity = ctrl.Velocity.WithZ( 150 );

				if ( HasStartedSlam ) return;
				HasStartedSlam = true;
				await GameTask.Delay( 250 );
				Slamming = true;
				pl.IgnoreFallDamage = true;
			}
			SlameTime();


		}

		[Event.Tick]
		public void SlameTime()
		{
			if ( Slamming )
			{
				ctrl.Velocity = ctrl.Velocity.WithZ( -SlamGravity );

				var tr = Trace.Ray( ctrl.Position, ctrl.Position + Vector3.Down * 12 )
					.Ignore( ctrl.Pawn )
					.Radius( 4 )
					.Run();
				if(tr.Hit )
				{
					var damageInfo = DamageInfo.Generic( 80 );

					var box = tr.Entity;
					Log.Info( box );
					box.TakeDamage( damageInfo );
					GroundEffect();

				}
			}
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
