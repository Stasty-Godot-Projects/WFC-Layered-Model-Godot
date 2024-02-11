using System.Collections.Generic;

namespace LayeredWFC.addons.LayeredWFC.Scripts.DTO
{
	public class TileDescriptionDto
	{
		public IEnumerable<int> SidesKind { get; set; }
		public IEnumerable<int> CornersKind { get; set; }
	}
}
