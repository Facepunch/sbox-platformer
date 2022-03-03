using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	partial class ParachutePawn : Sandbox.Player
	{

		public Clothing.Container Clothing = new();

		private Particles FakeShadow;

		private DamageInfo lastDamage;

		[Net]
		public List<Checkpoint> Checkpoints { get; set; } = new();

		public ParachutePawn()
		{

		}

		public ParachutePawn( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new ParachuteWalkController();
			Animator = new StandardPlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			CameraMode = new ParachuteCamera();

			FakeShadow = Particles.Create( "particles/gameplay/fake_shadow/fake_shadow.vpcf", this );

			base.Respawn();

			GotoBestCheckpoint();
		}

		public override void OnKilled()
		{
			base.OnKilled();

			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );

			Controller = null;	

			EnableAllCollisions = false;
			EnableDrawing = false;

			CameraMode = new ParachuteRagdollCamera();

			foreach ( var child in Children )
			{
				child.EnableDrawing = false;
			}

		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( Input.Pressed( InputButton.Drop ) || Input.Pressed( InputButton.Reload ) )
			{
				Game.Current.DoPlayerSuicide(cl);
			}
		}

		/// <summary>
		/// Called every frame on the client
		/// </summary>
		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );
		}
	}
}
