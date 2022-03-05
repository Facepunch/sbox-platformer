using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	partial class PlatformerPawn : Sandbox.Player
	{

		public const float MaxRenderDistance = 128f;

		public Clothing.Container Clothing = new();

		private Particles FakeShadow;

		private DamageInfo lastDamage;

		[Net]
		public int NumberLife { get; set; } = 3;


		[Net]
		public List<Checkpoint> Checkpoints { get; set; } = new();

		public PlatformerPawn()
		{

		}

		public PlatformerPawn( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new PlatformerWalkController();
			Animator = new StandardPlayerAnimator();
			CameraMode = new PlatformerCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			//FakeShadow = Particles.Create( "particles/gameplay/fake_shadow/fake_shadow.vpcf", this );

			base.Respawn();

			RemoveCollisionLayer( CollisionLayer.Solid );

			Health = 4;

			if ( NumberLife == 0 )
			{
				ClearCheckpoints();
				NumberLife = 3;
				ResetPickUps();
			}

			GotoBestCheckpoint();


			Tags.Add( "Platplayer" );
		}

		public void ResetPickUps()
		{
			foreach ( var item in All.OfType<LifePickup>())
			{
				item.Reset( this );
			}
		}

		public override void OnKilled()
		{
			base.OnKilled();

			NumberLife--;

			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );

			Controller = null;	

			EnableAllCollisions = false;
			EnableDrawing = false;

			CameraMode = new PlatformerRagdollCamera();

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

		[Event.Frame]
		private void UpdateRenderAlpha()
		{
			if ( Local.Pawn == this ) return;
			if ( Local.Pawn == null ) return;
			if ( !Local.Pawn.IsValid() ) return;

			var dist = Local.Pawn.Position.Distance( Position );
			var a = 1f - dist.LerpInverse( MaxRenderDistance, MaxRenderDistance * .1f );
			a = Math.Max( a, .15f );
			a = Easing.EaseOut( a );

			this.RenderColor = this.RenderColor.WithAlpha( a );

			foreach ( var child in this.Children )
			{
				if ( child is not ModelEntity m || !child.IsValid() ) continue;
				m.RenderColor = m.RenderColor.WithAlpha( a );
			}
		}
	}
}
