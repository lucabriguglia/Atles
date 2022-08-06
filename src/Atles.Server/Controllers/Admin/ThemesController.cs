using Atles.Core;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/themes")]
public class ThemesController : AdminControllerBase
{
    public ThemesController(IDispatcher dispatcher) : base(dispatcher)
    {
    }

    private const long SizeLimit = 2097152;
    private readonly string[] _permittedExtensions = { ".zip" };

    [HttpPost("upload-theme-file")]
    public IActionResult UploadThemeFile()
    {
        try
        {
            var file = Request.Form.Files[0];

            if (file.Length <= 0)
            {
                return BadRequest("The file is empty.");
            }

            if (file.Length > SizeLimit)
            {
                const long megabyteSizeLimit = SizeLimit / 1048576;
                return BadRequest($"The file exceeds {megabyteSizeLimit:N1} MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_permittedExtensions.Contains(extension))
            {
                return BadRequest("The file type isn't permitted.");
            }

            var folderName = Path.Combine("Uploads", "Themes");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(fileName);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
}
