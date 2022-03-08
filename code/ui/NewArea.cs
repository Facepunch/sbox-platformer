using Sandbox.UI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.UI.Construct;

namespace Sandbox.UI
{
	public class NewArea : Panel
	{
		public static NewArea Instance;
		public float timesince = 0;
		public Label usetext;
		public Label Destination;

		public string CArea;

		public NewArea()
		{
			Destination = Add.Label( "", "destext" );

		}
		public override void Tick()
		{

			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is not PlatformerPawn pl ) return;
			CArea = pl.CurrentArea;

			Destination.Text = $"{CArea}";

			base.Tick();

			//if ( Time.Now - timesince < 5000 )
			//{
			//	AddClass( "visible" );
			//}
			//else
			//{
			//	RemoveClass( "visible" );
			//}

		}

		//public void EnteredNewArea()
		//{
		//	Destination.Text = $"{CArea}";
		//}
	}
}
