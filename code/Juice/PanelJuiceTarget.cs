
using Sandbox.UI;

public class PanelJuiceTarget : IJuiceTarget
{

	private Panel panel;

	public PanelJuiceTarget( Panel panel )
	{
		this.panel = panel;
	}

	public bool IsValid()
	{
		return panel != null;
	}

	public void SetColor( Color c )
	{
		// ?
	}

	public void SetScale( float scale )
	{
		panel.Style.Set( "transform", $"scale({scale})" );
	}

	public void SetAlpha( float a )
	{
		panel.Style.Opacity = a;
	}

}

