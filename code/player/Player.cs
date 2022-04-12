
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

		public Clothing.Container Clothing = new();
		private DamageInfo lastDamage;
		private float LastHealth;

		private TimeSince ts;

		public string MapName => Global.MapName;
		public bool JustPickedupHealth;
		public int AmountOfFlash = 0;

		public Particles FakeShadowParticle;

		[Net]
		public Color PlayerColor { get; set; }

		public bool IgnoreFallDamage = false;
		public Color Color { get; private set; }

		public bool PlayerHasGlider { get; set; } = false;

		[Net]
		public IList<int> KeysPlayerHas { get; set; } = new List<int>();

		[Net]
		public float NumberOfKeys { get; set; }

		[Net]
		public string CurrentArea { get; set; }
		public int AreaPriority = 0;

		[Net] public float GliderEnergy { get; set; }

		[Net]
		public TimeUntil TimeUntilVulnerable { get; set; }
		[Net]
		public int NumberLife { get; set; } = 3;
		[Net]
		public int Coin { get; set; }
		[Net]
		public List<Checkpoint> Checkpoints { get; set; } = new();

		[Net]
		public PropCarriable HeldBody { get; set; }

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
			CameraMode = new PlatformerOrbitCamera();

			//if ( Input.UsingController )
			//{
			//	CameraMode = new PlatformerShiftCamera();
			//}
			//else
			//{
			//	CameraMode = new PlatformerOrbitCamera();
			//}

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

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
				KeysPlayerHas.Clear();
				NumberOfKeys = 0;
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

		public void SetInvulnerable( float duration )
		{
			TimeUntilVulnerable = duration;
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( TimeUntilVulnerable > 0 ) return;

			base.TakeDamage( info );

			PlayerBeenDamaged();
			Velocity += info.Force;
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

		public void FinishedReset()
		{
			KeysPlayerHas.Clear();
			NumberOfKeys = 0;
		}

		protected override void TickPlayerUse()
		{
			if ( HeldBody.IsValid() ) 
				return;

			if ( TimeUntilCanUse > 0 )
				return;

			base.TickPlayerUse();
		}

		private TimeUntil TimeUntilCanUse;

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			if ( Platformer.CurrentState == Platformer.GameStates.GameEnd )
				return;

			base.Simulate( cl );

			if ( !IsServer ) return;

			if ( HeldBody.IsValid() && InputActions.Use.Pressed() )
			{
				HeldBody.Throw();
				HeldBody = null;
				TimeUntilCanUse = 1f;
			}

			TickPlayerUse();

			if ( Controller is PlatformerController controller )
			{
				GliderEnergy = (float)Math.Round( controller.Energy );
			}

			if ( InputActions.Kill.Down() )
			{
				if ( TimerState == TimerState.Finished )
				{
					KeysPlayerHas.Clear();
					NumberLife = 3;
					NumberOfKeys = 0;
					Game.Current.DoPlayerSuicide( cl );
				}
				else
				{
					Game.Current.DoPlayerSuicide( cl );
				}
			}

			if ( Health == 1 && ts > 2 )
			{
				LowHealth();
				ts = 0;
			}

			if ( LifeState == LifeState.Alive )
			{
				if ( GetActiveController() == DevController )
				{
					ResetTimer();
					KeysPlayerHas.Clear();
					NumberOfKeys = 0;
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
				Juice.Scale( 1, 1, 1 )
					
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
				Juice.Scale( 1,1,1 )
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

		protected override Entity FindUsable()
		{
			var startpos = Position + Vector3.Up * 5;
			var endpos = startpos + Rotation.Forward * 60f;
			var tr = Trace.Sphere( 5f, startpos, endpos )
				.Ignore( this )
				.EntitiesOnly()
				.Run();

			if ( tr.Entity.IsValid() && tr.Entity is IUse use && use.IsUsable( this ) )
				return tr.Entity;

			return null;
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

		public void PlayerPickedUpGlider()
		{
			if(PlayerHasGlider)
			{
				if ( Controller is PlatformerController controller )
				{
					controller.EnableGliderControl();
				}
			}
		}

		[AdminCmd]
		public static void MapVote()
		{
			var vote = new MapVoteEntity();
		}

	}
}
