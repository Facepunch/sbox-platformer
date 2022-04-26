
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Movement;
using Platformer.Utility;

namespace Platformer
{
	partial class PlatformerDeadPawn : Sandbox.Player
	{

		[Net]
		public string CurrentArea { get; set; } = "Dead";

		[Net]
		public Color PlayerColor { get; set; }

		public PlatformerDeadPawn() { }

		public PlatformerDeadPawn( Client cl )
		{

		}

		public override void Spawn()
		{

			SetModel( "models/gameplay/spectator_head/spectator_head.vmdl" );

			Controller = new FlyingController();
			Animator = new StandardPlayerAnimator();
			CameraMode = new PlatformerSpectateCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			PlayerColor = Color.Random;

			RenderColor = PlayerColor;
			RenderColor = RenderColor.WithAlpha( .5f );
			

			base.Spawn();

		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			SimulateActiveChild( cl, ActiveChild );

		}
	}
}
