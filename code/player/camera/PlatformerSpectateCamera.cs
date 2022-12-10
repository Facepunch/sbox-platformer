using Sandbox;
using System;
using System.Linq;

namespace Platformer
{
	public partial class PlatformerSpectateCamera : BaseCamera
	{
		Vector3 lastPos;

		public PlatformerSpectateCamera()
		{
			var pawn = Game.LocalPawn as PlatformerDeadPawn;
			if ( pawn == null ) return;

			Camera.Position = pawn.EyePosition;
			Camera.Rotation = pawn.EyeRotation;

			lastPos = Camera.Position;
		}

		public override void Update()
		{
			var pawn = Game.LocalPawn as PlatformerDeadPawn;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePosition;
			if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
			{
				Camera.Position = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 20.0f * Time.Delta );
			}
			else
			{
				Camera.Position = eyePos;
			}

			Camera.Rotation = pawn.EyeRotation;

			lastPos = Camera.Position;
		}
	}
}
