
using Sandbox;

namespace Platformer.Movement
{
	partial class WallJump : BaseMoveMechanic
	{
        private float WallJumpConnectangle => 0.75f;
		public float WallJumpStrength => 240f;
        public float WallJumpKickStrength => 500f;
        public float WallJumpFriction => 7.0f;
		public override bool TakesOverControl => true;
		public override bool AlwaysSimulate => false;
        public float TimeUntilNextWallJump = Time.Now;

        private Vector3 HitNormal;
		public WallJump( PlatformerController controller ) : base( controller )
		{
		}

        protected override bool TryActivate()
		{
            if( ctrl.Pawn == null )
                return false;

            // Don't bother if we're on the ground
            if( ctrl.GroundEntity != null )
                return false;

            // We have to be falling
            if( ctrl.Velocity.z >= 0 )
                return false;

            // Wait until we can grab again
            if( TimeUntilNextWallJump > Time.Now ) 
                return false;

            // We need to have some horizontal speed to grab
            if( ctrl.Velocity.WithZ(0).Length < 1.0f )
                return false;

            // first try just moving to the destination
            var center = ctrl.Position;
            center.z += 48;
            var dest = (center + ( ctrl.Pawn.Rotation.Forward * 40.0f ) );

            var tr = Trace.Ray( center, dest )
				.Ignore( ctrl.Pawn )
				.Radius( 8 )
				.Run();

            //DebugOverlay.Line( center, dest, Color.Green, 5.0f, false );

            if( tr.Hit )
            {
                //If we are on a desired angle, then we can grab the wall
                bool canGrab = tr.Normal.Dot( -ctrl.Pawn.Rotation.Forward ) > 1.0 - WallJumpConnectangle;

                if( canGrab )
                {
                    GrabWall( tr );
                    return true;
                }
            }

            return false;
		}

        public override void Simulate()
        {   
            base.Simulate();

            if ( InputActions.Jump.Pressed() )
            {
                DoWallJump();
                return;
            }

            if( InputActions.Duck.Pressed() )
            {
                Cancel();
                return;
            }
            
            ctrl.SetTag( "grabbing_wall" );
            
            // Shitty gravity
            ctrl.Velocity *= new Vector3(1).WithZ( 1.0f - ( Time.Delta * WallJumpFriction ) );
            ctrl.Rotation =  Rotation.LookAt( HitNormal * 10.0f, Vector3.Up );
            
            Particles.Create( "particles/gameplay/player/Doublejump/Doublejump.vpcf" , ctrl.Pawn );

            //If ground entity, then we hit the ground, stop simulating
            if( ctrl.GroundEntity != null )
            {
                Cancel();
                return;
            }

            //If no longer holding wall, then we have nothing to hold on, stop simulating

            ctrl.Move();
        }

        private void GrabWall( TraceResult tr )
        {
            HitNormal = tr.Normal;
            ctrl.Velocity = 0;

            // Stick to wall
            ctrl.Position = ( tr.EndPosition + ( HitNormal * 16.0f ) ).WithZ( ctrl.Position.z );

            ctrl.Move();
        }

        private void DoWallJump()
        {
            var jumpVec = HitNormal * WallJumpKickStrength;

            TimeUntilNextWallJump = Time.Now + 0.25f;

            WallJumpEffect();

            ctrl.Velocity = ctrl.Velocity.WithZ( WallJumpStrength );
            ctrl.Velocity += jumpVec;
            ctrl.Position += ctrl.Velocity * Time.Delta;
            IsActive = false;

            ctrl.Pawn.Rotation =  HitNormal.EulerAngles.ToRotation();
            ctrl.Pawn.EyeRotation = HitNormal.EulerAngles.ToRotation();

            // Consume our remaining double jumps
            ctrl.GetMechanic<DoubleJump>().TimeUntilCanDoubleJump += .25f;
            ctrl.GetMechanic<DoubleJump>().DoubleJumpsRemaining = 0;

        }

        private void Cancel()
        {
            IsActive = false;
        }

		private void WallJumpEffect()
		{
			if ( !ctrl.Pawn.IsServer ) return;

			using var _ = Prediction.Off();

			ctrl.AddEvent( "jump" );

			Particles.Create( "particles/gameplay/player/Walljump/Walljump.vpcf", ctrl.Pawn );
			Sound.FromWorld( "player.djump", ctrl.Pawn.Position );
		}

	}
}
