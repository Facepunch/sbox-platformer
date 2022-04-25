
using Sandbox;

namespace Platformer.Utility;

internal static class EntityExtensions
{

	public static void SetRenderColorRecursive( this Entity e, Color color )
	{
		if ( !e.IsValid() ) return;

		if ( e is ModelEntity m )
			m.RenderColor = color;

		foreach ( var child in e.Children )
		{
			child.SetRenderColorRecursive( color );
		}
	}

}
