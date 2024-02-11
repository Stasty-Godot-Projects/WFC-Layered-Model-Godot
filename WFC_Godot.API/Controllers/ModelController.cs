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
        private static byte[] _imageTMP = null;

        public ModelController(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        [HttpPost]
        [Route("AddImageForTheModel")]
        public async Task<IActionResult> AddImageForTheModel([FromForm(Name = "file")] IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    using(var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);
                        _imageTMP = memoryStream.ToArray();
                    }
                    return Ok("Model is ready to start with labels");
                }
                else
                    return BadRequest("No data");
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpPost]
        [Route("CreateModel")]
        public async Task<IActionResult> CreateModel([FromBody] TileDescriptionEncapsulation tileDescriptions)
        {
            try
            {
                if(_imageTMP == null)
                    return BadRequest("No image to process");
                if (CreateModelJob.IsUnderway())
                    return BadRequest("Model is under inder training");
                var job = new CreateModelJob();
                var image = _imageTMP;
                _imageTMP = null;
                job.Exec(image, tileDescriptions.TileDescriptions, tileDescriptions.TileSize);
                return Ok("Model is processes");
            }
            catch (Exception exception)
            {
                return BadRequest($"Error: {exception.Message}");
            }
        }

        [HttpGet]
        [Route("GetIsModelTrainedNow")]
        public async Task<bool> GetIsModelTrained() => CreateModelJob.IsUnderway();

        [HttpPost]
        [Route("Recognize/{tileSize}")]
        public async Task<ActionResult<IEnumerable<TileDescription>>> Recognize([FromForm(Name = "file")] IFormFile image, int tileSize)
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
