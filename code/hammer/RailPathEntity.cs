
using Sandbox;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Platformer
{
	[Library( "platformer_path_rail" )]
	[Display( Name = "Path Rail", GroupName = "Platformer", Description = "A rail path the player can slide along." )]
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

		public Vector3 NearestPoint( Vector3 from, out int node, out float t )
		{
			// todo: distance between nodes isn't always the same
			// so basing this on T kinda sucks

			node = 0;
			t = 0f;

			var result = Vector3.Zero;
			var bestDist = float.MaxValue;

			for ( int i = 0; i < PathNodes.Count - 1; i++ )
			{
				var nodea = PathNodes[i];
				var nodeb = PathNodes[i + 1];

				for ( float j = 0; j <= 1; j += .1f )
				{
					var point = GetPointBetweenNodes( nodea, nodeb, j );
					var dist = from.Distance( point );
					if ( dist < bestDist )
					{
						bestDist = dist;
						result = point;
						t = j;
						node = i;
					}
				}
			}

			return result;
		}

	}
}
