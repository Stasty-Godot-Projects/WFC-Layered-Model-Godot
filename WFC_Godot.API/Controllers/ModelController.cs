using Microsoft.AspNetCore.Mvc;
using WFC_Godot.API.Jobs;
using WFC_Godot.API.Model;
using WFC_Godot.API.Services.Interfaces;

namespace WFC_Godot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IRecognitionService _recognitionService;

        public ModelController(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        [HttpPost]
        [Route("CreateModel")]
        public async Task<IActionResult> CreateModel([FromForm] IFormFile image, [FromBody] TileDescriptionEncapsulation tileDescriptions)
        {
            try
            {
                if (image != null)
                {
                    if (CreateModelJob.IsUnderway())
                        return BadRequest("Model is under inder training");
                    var job = new CreateModelJob();
                    job.Exec(image, tileDescriptions.TileDescriptions, tileDescriptions.TileSize);
                    return Ok("Model is processes");
                }
                else
                    return BadRequest("No data");
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpGet]
        [Route("GetIsModelTrained")]
        public async Task<bool> GetIsModelTrained() => CreateModelJob.IsUnderway();

        [HttpPost]
        [Route("Recognize")]
        public async Task<ActionResult<IEnumerable<TileDescription>>> Recognize([FromForm] IFormFile image, [FromQuery] int tileSize)
        {
            try
            {
                if (image != null)
                {
                    if (CreateModelJob.IsUnderway())
                        return BadRequest("Model is under training");
                    var results = _recognitionService.RecognizeTiles(image, tileSize);
                    return Ok(results);
                }
                else
                    return BadRequest("No data");
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }
    }
}
