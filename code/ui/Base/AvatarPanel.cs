﻿
using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Platformer.UI
{
	public class AvatarPanel : Panel
	{
		static Color HighColor = new Color( 0.45f, 0.72f, 1f );
		
		static Color LowColor = new Color( 0.83f, 0.18f, 0.19f );

		public CitizenPanel Avatar { get; set; }
			
		public AvatarPanel()
		{			
			Avatar = new CitizenPanel( Game.LocalClient );
			AddChild( Avatar );
			
		}

	}
}
