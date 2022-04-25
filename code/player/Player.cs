
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Movement;
using Platformer.Utility;

namespace Platformer
{
	partial class PlatformerPawn : Sandbox.Player
	{
		[Net]
		public Color PlayerColor { get; set; }

		[Net]
		public bool PlayerHasGlider { get; set; } = false;

		[Net] 
		public float GliderEnergy { get; set; }

		[Net]
		public int Coin { get; set; }

		[Net]
		public IList<int> KeysPlayerHas { get; set; } = new List<int>();

		[Net]
		public float NumberOfKeys { get; set; }

		[Net]
		public string CurrentArea { get; set; }
		public int AreaPriority = 0;

		[Net]
		public TimeUntil TimeUntilVulnerable { get; set; }

		[Net]
		public int NumberLife { get; set; } = 3;

		[Net]
		public List<Checkpoint> Checkpoints { get; set; } = new();

		[Net]
		public PropCarriable HeldBody { get; set; }


		public const float MaxRenderDistance = 128f;
		public Clothing.Container Clothing = new();
		private DamageInfo lastDamage;
		private float LastHealth;
		private TimeSince ts;
		public string MapName => Global.MapName;
		public bool JustPickedupHealth;
		public int AmountOfFlash = 0;

		public bool IgnoreFallDamage = false;
		public Color Color { get; private set; }

		public Particles HeldParticle { get; set; }
		private Particles WalkCloud;
		public Particles FakeShadowParticle;

		[Net]
		public bool Tagged { get; set; }

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

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			TagArrowParticle?.Destroy();
			TagArrowParticle = null;

			Clothing.DressEntity( this );

			base.Respawn();

			if ( Platformer.CurrentGameMode == Platformer.GameModes.Competitive )
			{
				RemoveCollisionLayer( CollisionLayer.Solid );
			}

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

			if ( CurrentArea == null )
			{
				CurrentArea = $"{MapName}";
			}

			GotoBestCheckpoint();


			Tags.Add( "Platplayer" );

			this.SetRenderColorRecursive( Tagged ? Color.Red : Color.White );
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
				//Juice.Scale( 1, 1.15f, 1f )
				//	.WithTarget( this )
				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );

				//Juice.Color( Color.White, Color.Red, Color.White )
				//	.WithTarget( this )
				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );
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

			WalkCloud?.Destroy();
			

			if (HeldBody != null)
			{
				HeldBody.Drop( 2 );
				HeldBody = null;
			}

			foreach ( var child in Children )
			{
				child.EnableDrawing = false;
			}

			if(Platformer.CurrentGameMode == Platformer.GameModes.Tag)
			{
				if ( Platformer.CurrentState == Platformer.GameStates.Warmup || Platformer.CurrentState == Platformer.GameStates.Runaway ) return;
				Tagged = true;
			}

			WalkCloud = null;
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( !Tagged ) return;
			if ( other is not PlatformerPawn pl ) return;

			pl.Tagged = true;
		}

		public void FinishedReset()
		{
			KeysPlayerHas.Clear();
			NumberOfKeys = 0;
		}

		public void ResetTagged()
		{
			Tagged = false;

			this.SetRenderColorRecursive( Color.White );
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

		private Vector3 Mins => new( -64, -64, 0 );
		private Vector3 Maxs => new( 64, 64, 64 );

		public Particles TagArrowParticle { get; set; }

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			if ( Platformer.CurrentState == Platformer.GameStates.GameEnd )
				return;

			base.Simulate( cl );

			if ( !IsServer ) return;

			TickPlayerThrow();
			TickPlayerUse();

			if ( Controller is PlatformerController controller )
			{
				GliderEnergy = (float)Math.Round( controller.Energy );
			}

			if ( InputActions.Kill.Down() )
			{
				if ( TimerState == TimerState.Finished )
				{
					if ( Platformer.CurrentState == Platformer.GameStates.Warmup && Tagged || Platformer.CurrentState == Platformer.GameStates.Runaway && Tagged ) return;
					KeysPlayerHas.Clear();
					NumberLife = 3;
					NumberOfKeys = 0;
					Game.Current.DoPlayerSuicide( cl );
				}
				else
				{
					if ( Platformer.CurrentState == Platformer.GameStates.Warmup && Tagged || Platformer.CurrentState == Platformer.GameStates.Runaway && Tagged ) return;
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

			if ( Platformer.CurrentGameMode == Platformer.GameModes.Tag )
			{
				if ( Tagged )
				{
					//this is awful.
					var lift = 4;

					var bbox = new BBox( Mins, Maxs.WithZ( Maxs.z - lift ) );
					var start = Position + Vector3.Up * lift;
					var end = Position + Vector3.Down * (lift - 36);
					var tr = Trace.Box( bbox, start, end )
					.Ignore( this )
					.Run();

					if ( tr.Hit )
					{
						if ( tr.Entity is not PlatformerPawn pl ) return;
						if ( pl.Tagged ) return;
						pl.Tagged = true;
						pl.BeenTagged();
						Client.AddInt( "kills" );
					}
				}
			}
		}

		public void BeenTagged()
		{
			if( !Tagged ) return;

			Sound.FromEntity( "life.pickup", this );

			RenderColor = Color.Red;

			foreach ( var child in this.Children )
			{
				if ( child is not ModelEntity m || !child.IsValid() ) continue;
				m.RenderColor = Color.Red;
			}
		}

		private void TickPlayerThrow()
		{
			if ( !HeldBody.IsValid() ) return;

			var drop = false;
			var vel = Vector3.Zero;

			if(Input.UsingController)
			{
				if ( InputActions.Walk.Pressed() && InputActions.Duck.Down() )
				{
					drop = true;
					vel = Velocity + Rotation.Forward * 30 + Rotation.Up * 10;

				}

				if ( InputActions.Walk.Pressed() && !InputActions.Duck.Down() )
				{
					drop = true;
					//HeldParticle.Destroy(true);
					vel = Velocity + Rotation.Forward * 300 + Rotation.Up * 100;

				}

				if ( !drop ) return;
				HeldBody.Drop( vel );
				HeldBody = null;
				TimeUntilCanUse = 1f;
			}
			if ( !Input.UsingController )
			{
				if ( InputActions.Use.Pressed() && InputActions.Duck.Down() )
				{
					drop = true;
					vel = Velocity + Rotation.Forward * 30 + Rotation.Up * 10;

				}

				if ( InputActions.Use.Pressed() && !InputActions.Duck.Down() )
				{
					drop = true;
					//HeldParticle.Destroy(true);
					vel = Velocity + Rotation.Forward * 300 + Rotation.Up * 100;

				}

				if ( !drop ) return;
				HeldBody.Drop( vel );
				HeldBody = null;
				TimeUntilCanUse = 1f;
			}
		}

		[Event.Frame]
		public void WalkCloudParticle()
		{
			if ( WalkCloud == null )
			{
				WalkCloud = Particles.Create( "particles/gameplay/player/walkcloud/walkcloud.vpcf" );
				WalkCloud.SetEntity( 0, this );
			}

			if(LifeState == LifeState.Dead)
			{
				WalkCloud.SetPosition( 6, new Vector3( 0, 0, 0 ) );
			}

			if ( GroundEntity != null && Velocity.Length >= .2f )
			{
				var speed = Velocity.Length.Remap( 0f, 280, 0f, 1f );
				WalkCloud.SetPosition( 6, new Vector3( speed, 0f, 0f ) );
			}
			else
			{
				WalkCloud.SetPosition( 6, new Vector3( 0, 0, 0 ) );
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

		public void PickedUpItem( Color itempickedup )
		{
			if ( IsServer )
			{
				//Juice.Scale( 1, 1, 1 )

				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );

				//Juice.Color( Color.White, itempickedup, Color.White )
				//	.WithTarget( this )
				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );
			}
		}

		public void LowHealth()
		{
			if ( IsServer )
			{
				//Juice.Scale( 1, 1, 1 )
				//	.WithTarget( this )
				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );

				//Juice.Color( Color.White, Color.Red, Color.White )
				//	.WithTarget( this )
				//	.WithDuration( .45f )
				//	.WithEasing( EasingType.EaseOut );

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
			if ( Platformer.CurrentGameMode != Platformer.GameModes.Competitive ) return;

			var dist = Local.Pawn.Position.Distance( Position );
			var a = 1f - dist.LerpInverse( MaxRenderDistance, MaxRenderDistance * .1f );
			a = Math.Max( a, .15f );
			a = Easing.EaseOut( a );

			this.SetRenderColorRecursive( RenderColor.WithAlpha( a ) );
		}

		[Event.Frame]
		private void EnsureTagParticle()
		{
			var create = Tagged && TagArrowParticle == null;
			var destroy = !Tagged && TagArrowParticle != null;

			if ( create )
			{
				TagArrowParticle = Particles.Create( "particles/gameplay/player/tag_arrow/tag_arrow.vpcf", this );
				TagArrowParticle.SetPosition( 6, Color.Red * 255 );
			}

			if ( destroy )
			{
				TagArrowParticle.Destroy();
				TagArrowParticle = null;
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
			if ( PlayerHasGlider )
			{
				if ( Controller is PlatformerController controller )
				{
					controller.EnableGliderControl();
					PlayerHasGlider = true;
				}
			}
		}

		[Event.Tick]
		public void PlayerHolding()
		{
			if ( HeldBody != null )
			{
				if ( Controller is PlatformerController controller )
				{
					controller.IsHolding = true;
				}
			}
			else
			{
				if ( Controller is PlatformerController controller )
				{
					controller.IsHolding = false;
				}
			}
		}

		[AdminCmd]
		public static void MapVote()
		{
			var vote = new MapVoteEntity();
		}

		TimeSince timeSinceLastFootstep = 0;


		public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
		{
			if ( LifeState != LifeState.Alive )
				return;

			if ( !IsServer )
				return;

			if ( foot == 0 )
			{
				var lfoot = Particles.Create( "particles/gameplay/player/footsteps/footstep_l.vpcf", pos );
				lfoot.SetOrientation( 0, Transform.Rotation );
			}
			else
			{
				var rfoot = Particles.Create( "particles/gameplay/player/footsteps/footstep_r.vpcf", pos);
				rfoot.SetOrientation( 0, Transform.Rotation );
			}

			if ( timeSinceLastFootstep < 0.2f )
				return;



			volume *= FootstepVolume();

			timeSinceLastFootstep = 0;

			//DebugOverlay.Box( 1, pos, -1, 1, Color.Red );
			//DebugOverlay.Text( pos, $"{volume}", Color.White, 5 );

			var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
				.Radius( 1 )
				.Ignore( this )
				.Run();

			if ( !tr.Hit ) return;

			tr.Surface.DoFootstep( this, tr, foot, volume * 10 );
		}
	}
}
