using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayeredWFC.addons.LayeredWFC.Scripts.DTO
{
	public class RequestBody
	{
		public IEnumerable<TileDescriptionDto> TileDescriptions { get; set; }
		public int TileSize { get; set; }
	}
}
