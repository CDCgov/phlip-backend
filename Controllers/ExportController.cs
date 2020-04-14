using System;
using System.IO;
using System.Threading.Tasks;
using Esquire.Data;
using Esquire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/exports")]
    [Authorize]
    public class ExportController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly IProjectExportService _exportService;
        private ILogger<ExportController> _logger;

        public ExportController(IUnitOfWork unitOfWork, IWebHostEnvironment environment, IProjectExportService exportService, ILogger<ExportController> logger)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _exportService = exportService;
            _logger = logger;
        }

        /// <summary>
        /// Export validated answer data for project into CSV file and download it.
        /// </summary>
        /// <remarks>Download validated answer data for project </remarks>
        /// <param name="id">The id of the project to use to create exported data.</param>
        /// <param name="type">The type of export. Options are "text", "numeric" or "codebook".</param>
        /// <param name="userId">The user id to select coded answers.  this parm is optional</param>
        [HttpGet("project/{id}/data")]
        public async Task<IActionResult> ExportProjectData(long id, string type, long userId = -1 )
        {

            FileStreamResult fsr = null;
            var filePath = "";
            
            // check for valid project
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            
            // check for valid export type parameter
            switch (type)
            {
                case "text":
                case "numeric":
                case "codebook":
                    break;
                default:
                    _logger.LogError("Invalid export type parameter = {Type}", type);
                    return BadRequest();
            }
            // check if userid passed,  if yes then export data for the requested user
            ExportResult exportResult;
            exportResult = await _exportService.ExportProjectData(id, type, userId);  
            
            if (exportResult.IsSuccessful())
                filePath = exportResult.FilePath;
            else
            {
                Console.WriteLine("Error in export");
                Response.Headers.Add("Content-Type", "application/json");
                return Json(exportResult);
            }

            // response...
            var cd = new System.Net.Mime.ContentDisposition
            {  
                FileName =  Uri.EscapeUriString(Path.GetFileName(filePath)),
                Inline = false  // false = prompt the user for downloading, true = browser to show the file inline
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            try
            {
                Stream tempFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fsr = new FileStreamResult(tempFileStream, "text/csv");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read: {0}, exception {1} ", filePath, e);
                return fsr;
            }

            return fsr;
            
        } 
        
        /// <summary>
        /// Export help file.
        /// </summary>
        /// <remarks>Export help file </remarks>
        [HttpGet("helpfile")]
        public async Task<IActionResult> HelpFile()
        {
            var dirInfo = Path.Combine(_environment.WebRootPath);
            
            var filePath = Path.Combine(dirInfo, "PHLIP-Help-Guide.pdf");
            
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(filePath));  
            
        }

        
        
    }
    
}