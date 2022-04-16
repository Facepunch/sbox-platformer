using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.UI;
using Sandbox.UI.Construct;

class WorldSignPanel : WorldPanel
{
	public WorldSignPanel()
	{
		var w = 2040;
		var h = 1560;
		PanelBounds = new Rect( -(w / 2), -(h / 2), w, h );

		StyleSheet.Load( "/ui/World/WorldSignPanel.scss" );
	}

	public override void Tick()
	{
		base.Tick();

		var w = 2080;
		var h = 1760;
		PanelBounds = new Rect( -(w / 2), -(h / 2), w, h );
	}
}
