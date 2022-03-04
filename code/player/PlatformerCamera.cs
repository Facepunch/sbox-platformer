
namespace Sandbox
{
	public class PlatformerCamera : CameraMode
	{
		[ConVar.Replicated]
		public static bool Thirdperson_orbit { get; set; } = false;

		[ConVar.Replicated]
		public static bool Thirdperson_collision { get; set; } = true;

		private Angles orbitAngles;
		private float orbitDistance = 250;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Position = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + Vector3.Up * 76;

			if ( Thirdperson_orbit )
			{
				Position += Vector3.Up * (pawn.CollisionBounds.Center.z * pawn.Scale);
				Rotation = Rotation.From( orbitAngles );

				targetPos = Position + Rotation.Backward * orbitDistance;
			}
			else
			{
				Position = center;
				Rotation = Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

				float distance = 350.0f * pawn.Scale;
				targetPos = Position + Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 0) * pawn.Scale);
				targetPos += Input.Rotation.Forward * -distance;
			}

			if ( Thirdperson_collision )
			{
				var tr = Trace.Ray( Position, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					.Run();

				Position = tr.EndPosition;
			}
			else
			{
				Position = targetPos;
			}

			FieldOfView = 70;

			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			if ( Thirdperson_orbit && input.Down( InputButton.Walk ) )
			{
				if ( input.Down( InputButton.Attack1 ) )
				{
					orbitDistance += input.AnalogLook.pitch;
					orbitDistance = orbitDistance.Clamp( 0, 1000 );
				}
				else
				{
					orbitAngles.yaw += input.AnalogLook.yaw;
					orbitAngles.pitch += input.AnalogLook.pitch;
					orbitAngles = orbitAngles.Normal;
					orbitAngles.pitch = orbitAngles.pitch.Clamp( -89, 89 );
				}

				input.AnalogLook = Angles.Zero;

				input.Clear();
				input.StopProcessing = true;
			}

			base.BuildInput( input );
		}
	}
}
