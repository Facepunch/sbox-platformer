using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace Sandbox.UI
{
	public class NewMajorArea : Panel
	{
		public static NewMajorArea Instance;
		public float timesince = 0;
		public Label newlandmark;
		public NewMajorArea()
		{
			StyleSheet.Load( "/ui/NewMajorArea.scss" );
			//usetext.SetText( "Interact [E]" )

			newlandmark = AddChild<Label>( "newlandmark" );

			Instance = this;
		}
		public override void Tick()
		{
			base.Tick();

			if ( Time.Now - timesince < 5 )
			{
				AddClass( "visible" );
			}
			else
			{
				RemoveClass( "visible" );
			}
		}

		public static void ShowLandmark( string title )
		{
			Instance.newlandmark.SetText( title );
			Instance.timesince = Time.Now;
		}
	}
}
