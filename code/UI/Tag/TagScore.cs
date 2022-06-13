
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace Platformer.UI;

[UseTemplate]
internal class TagScore : Panel
{

	public int TaggedCount { get; set; }
	public int NotTaggedCount { get; set; }

	public override void Tick()
	{
		base.Tick();

		var tag = Tag.Current;
		if ( !tag.IsValid() ) return;

		TaggedCount = Entity.All.OfType<PlatformerPawn>().Count( e => tag.Tagged.Contains( e ) );
		NotTaggedCount = Entity.All.OfType<PlatformerPawn>().Count( e => !tag.Tagged.Contains( e ) );
	}

}
