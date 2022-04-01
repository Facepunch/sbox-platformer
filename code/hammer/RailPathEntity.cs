
using Sandbox;
using System.Collections.Generic;
using System.Text.Json;

namespace Platformer
{
	[Library( "platformer_path_rail" )]
	[Hammer.Path( "path_generic_node" )]
	internal partial class RailPathEntity : GenericPathEntity
	{

		[Net, Hammer.Skip, Change(nameof(OnclPathJsonChanged))]
		public string clPathJson { set; get; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Always;
			clPathJson = pathNodesJSON;
		}

		public void OnclPathJsonChanged()
		{
			PathNodes = JsonSerializer.Deserialize<List<BasePathNode>>( clPathJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true } );
		}

	}
}
