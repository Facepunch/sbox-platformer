
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Movement;

namespace Platformer
{
	partial class PlatformerPawn : Sandbox.Player
	{
		public const float MaxRenderDistance = 128f;
		public const float InvulnerableTimeAfterDamaged = 2f;

		public Clothing.Container Clothing = new();
		private Particles FakeShadow;
		private DamageInfo lastDamage;
		private float LastHealth;

		private TimeSince ts;
		private TimeSince tshealthpick;

		public string MapName => Global.MapName;
		public bool JustPickedupHealth;
		public int AmountOfFlash = 0;

		public Particles FakeShadowParticle;

		[Net]
		public Color PlayerColor { get; set; }

		public bool IgnoreFallDamage = false;
		public Color Color { get; private set; }

		[Net]
		public IList<int> KeysPlayerHas { get; set; } = new List<int>();

		[Net]
		public string CurrentArea { get; set; }
		public int AreaPriority = 0;

		[Net] public float GliderEnergy { get; set; }

		[Net]
		public TimeSince TimeSinceDamaged { get; set; }
		[Net]
		public int NumberLife { get; set; } = 3;
		[Net]
		public int Coin { get; set; }
		[Net]
		public List<Checkpoint> Checkpoints { get; set; } = new();

		public PlatformerPawn() { }

		public PlatformerPawn( Client cl )
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );

			PlayerColor = Color.Random;
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new PlatformerController();
			Animator = new PlatformerOrbitAnimator();

			if ( Input.UsingController )
			{
				CameraMode = new PlatformerShiftCamera();
			}
			else
			{
				CameraMode = new PlatformerOrbitCamera();
			}

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
				ResetHealthPickUps();
				ResetLifePickUps();
				Coin = 0;
			}

			if(CurrentArea == null)
			{
				CurrentArea = $"{MapName}"; 
			}

			GotoBestCheckpoint();


			Tags.Add( "Platplayer" );
		}

		public void ResetLifePickUps()
		{
			foreach ( var lifeitem in All.OfType<LifePickup>() )
			{
				lifeitem.Reset( this );
			}
		}

		public void ResetHealthPickUps()
		{
			foreach ( var healthitem in All.OfType<HealthPickup>() )
			{
				healthitem.Reset( this );
			}
		}

		public void PlayerBeenDamaged()
		{
			LastHealth = Health;

			if ( IsServer )
			{
				Juice.Scale( 1, 1.15f, 1f )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );

				Juice.Color( Color.White, Color.Red, Color.White )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );
			}
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( TimeSinceDamaged < InvulnerableTimeAfterDamaged ) return;


			PlayerBeenDamaged();
			
			base.TakeDamage( info );

			TimeSinceDamaged = 0;
		}

		public override void OnKilled()
		{
			base.OnKilled();

			NumberLife--;

			Coin /= 2;

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

			if ( Controller is PlatformerController controller )
			{
				GliderEnergy = (float)Math.Round(controller.Energy);
			}

			if ( InputActions.Kill.Down() )
			{
				Game.Current.DoPlayerSuicide( cl );
			}

			if (Health == 1)
			{
				if (ts > 2)
				{
					LowHealth();
					ts = 0;
				}
					
			}

		}

		[Event.Frame]
		public void PlayerShadow()
		{
			if ( FakeShadowParticle == null )
			{
				FakeShadowParticle = Particles.Create( "particles/gameplay/fake_shadow/fake_shadow.vpcf" );
			}

			var Downtrace = Trace.Ray( Position, Position + Vector3.Down * 2000 );
			Downtrace = Downtrace.WorldOnly();
			var result = Downtrace.Run();

			FakeShadowParticle.SetPosition( 0, result.EndPosition );

			//DebugOverlay.TraceResult( result );

		}


		public void PickedUpItem(Color itempickedup)
		{
			if ( IsServer )
			{
				Juice.Scale( 1, 1.1f, 1f )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );

				Juice.Color( Color.White, itempickedup, Color.White )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );
			}
		}
		public void LowHealth()
		{
			if ( IsServer )
			{
				Juice.Scale( 1, 1.05f, 1f )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );

				Juice.Color( Color.White, Color.Red, Color.White )
					.WithTarget( this )
					.WithDuration( .45f )
					.WithEasing( EasingType.EaseOut );

				Sound.FromWorld( "player.lowhealth", Position );
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

		public void ApplyForce( Vector3 force )
		{
			if ( Controller is PlatformerController controller )
			{
				controller.Impulse += force;
				
			}
		}
	}
}
