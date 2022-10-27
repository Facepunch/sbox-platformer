
using Sandbox;
using System.Linq;

namespace Platformer.Gamemodes
{
	internal class Competitive : BaseGamemode
	{

		public static Competitive Current => Instance as Competitive;

		public override Platformer.GameModes Mode => Platformer.GameModes.Competitive;

		protected override bool CanStart()
		{
			return PlayerCount() > 0;
		}

		public override PlatformerPawn CreatePlayerInstance( Client cl ) => new CompetitivePlayer( cl );

		public override void DoPlayerRespawn( PlatformerPawn player )
		{
			base.DoPlayerRespawn( player );

			if ( player is not CompetitivePlayer pl ) return;

			if ( pl.NumberLife == 0 )
			{
				pl.ClearCheckpoints();
				pl.GotoBestCheckpoint();
				pl.ResetCollectibles<LifePickup>();
				pl.ResetCollectibles<HealthPickup>();
				pl.NumberLife = 3;
				pl.Coin = 0;
				pl.KeysPlayerHas.Clear();
				pl.NumberOfKeys = 0;
			}
		}

		protected override void FreshStart()
		{
			base.FreshStart();

			foreach( var pl in All.OfType<CompetitivePlayer>() )
			{
				pl.KeysPlayerHas.Clear();
				pl.NumberOfKeys = 0;
			}
		}

	}
}
