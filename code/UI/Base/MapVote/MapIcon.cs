
using Sandbox;
using Sandbox.UI;
using System;
using System.Threading.Tasks;

namespace Platformer.UI;

partial class MapIcon : Panel
{
	public string VoteCount { get; set; } = "0";
	public string Title { get; set; } = "...";
	public string Org { get; set; } = "...";
	public string Ident { get; internal set; }
	public Panel OrgAvatar { get; set; }
	public Panel ThumbnailIcon { get; set; }

	public MapIcon( string fullIdent )
	{
		Ident = fullIdent;
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( firstTime )
		{
			_ = FetchMapInformation();
		}
	}

	async Task FetchMapInformation()
	{
		var package = await Package.Fetch( Ident, true );
		if ( package == null ) return;
		if ( package.PackageType != Package.Type.Map ) return;

		Title = package.Title;
		Org = package.Org.Title;

		await ThumbnailIcon.Style.SetBackgroundImageAsync( package.Thumb );
		await OrgAvatar.Style.SetBackgroundImageAsync( package.Org.Thumb );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( VoteCount );
	}
}

