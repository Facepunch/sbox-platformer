
using Sandbox;

namespace Platformer.Movement
{
	class LedgeGrab : BaseMoveMechanic
	{
		public override bool TakesOverControl => true;

        public Vector3 LedgeDestination;
        public Vector3 LedgeGrabLocation;
        public Vector3 GrabNormal;

        public Vector3 TargetLocation;

        public float TimeUntilNextGrab;
        public float TimeToDisengage = -1;

		public LedgeGrab( PlatformerController controller ) : base( controller )
		{
		}

        protected override bool TryActivate()
        {
            if( TimeUntilNextGrab > Time.Now ) 
                return false;

            if( TryGrabUpperLedge() )
                return true;
            
            return false;
        }

        public override void Simulate()
        {
            base.Simulate();

            ctrl.Velocity = 0;

            ctrl.Position = Vector3.Lerp( ctrl.Position, TargetLocation, Time.Delta * 10.0f );

            if( TimeToDisengage > 0 && Time.Now > TimeToDisengage )
            {
                IsActive = false;
                TimeToDisengage = -1;
                return;
            }

            if( InputActions.Jump.Down() )
            {
                TimeToDisengage = Time.Now + 1.0f;
                TargetLocation = LedgeDestination;
            }

        }

        internal bool TryGrabUpperLedge()
        {
            // Need to be on air to check for upper ledge
            if ( ctrl.GroundEntity != null ) 
                return false;

            float playerRadius = 20.0f;

            var center = ctrl.Position;
            center.z += 64;
            var dest = (center + ( ctrl.Pawn.Rotation.Forward * 58.0f ) );

            var tr = Trace.Ray( center, dest )
				.Ignore( ctrl.Pawn )
				.Radius( 8 )
				.Run();

            DebugOverlay.Line( center, dest, tr.Hit ? Color.Red : Color.Green, 5.0f, false );

            if( tr.Hit )
            {
                var normal = tr.Normal;
                var destinationTestPos = tr.EndPosition - ( normal * playerRadius ) + ( Vector3.Up * 64.0f);
                var originTestPos = tr.EndPosition + ( normal * playerRadius );
                
                tr = Trace.Ray( destinationTestPos, destinationTestPos - ( Vector3.Up * 64.0f) )
                    .Ignore( ctrl.Pawn )
                    .Run();

                if( tr.Hit )
                {
                    // That's a valid position, set our destination pos
                    destinationTestPos = tr.EndPosition;
                     // Adjust grab position
                    originTestPos = originTestPos.WithZ( destinationTestPos.z - 64.0f );

                    // Then check if we have enough room to climb
                    
                    tr = Trace.Ray( destinationTestPos + ( Vector3.Up * playerRadius + 1.0f), destinationTestPos + ( Vector3.Up * 64.0f) )
                        .Ignore( ctrl.Pawn )
                        .Radius( playerRadius )
                        .Run();
                    
                    DebugOverlay.TraceResult( tr, 5.0f );

                    if( tr.Hit )
                    {
                        // We can't climb
                        return false;
                    }
                    else
                    {
                       

                        // Yeah, we can climb
                        LedgeDestination = destinationTestPos;
                        LedgeGrabLocation = originTestPos;
                        GrabNormal = normal;
                        TimeUntilNextGrab = Time.Now + 1.5f;

                        //Default bottom ledge to grab
                        TargetLocation = LedgeGrabLocation;

                        return true;
                    }
                }
                
            }

            return false;
        }

        internal bool TryGrabBottomLedge()
        {
            // Need to be on ground to check for bottom ledge
            if ( ctrl.GroundEntity == null ) 
                return false;
            return false;
        }


	}
}
