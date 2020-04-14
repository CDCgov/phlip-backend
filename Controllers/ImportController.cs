using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Consumes("multipart/form-data")]
    [Route("api/imports")]
    [Authorize]
    public class ImportController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private bool _headerParsed;
        private int _jurisdictionsModified = 0;

        public ImportController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        /// <summary>
        /// Upload Help file. Must be named PHLIP-Help-Guide.pdf
        /// </summary>
        /// <remarks>Upload Help file.</remarks>
        /// <param name="helpFile">The Help pdf file.</param>
        [HttpPost("helpfile")]
        public async Task<IActionResult> UploadHelpfile(IFormFile helpFile)
        {
            if (helpFile == null || helpFile.Length == 0)
            {
                return BadRequest("HelpFile is missing or empty");
            }

            if (helpFile.FileName != "PHLIP-Help-Guide.pdf")
            {
                return BadRequest("HelpFile must be named PHLIP-Help-Guide.pdf");
            }

            var helpFilePath = Path.Combine(_environment.WebRootPath, "PHLIP-Help-Guide.pdf");
            
            using (var stream = new FileStream(helpFilePath, FileMode.Create))
            {
                await helpFile.CopyToAsync(stream);
            }

            return Ok(new {helpFilePath});
            
        }
        
        //public class HelpFileUploadOperation : IOperationFilter
        //{
        //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //    {
        //        if (operation.OperationId.ToLower() == "apiimportshelpfilepost")
        //        {
        //            operation.Parameters.Clear();
        //            operation.Parameters.Add(new NonBodyParameter
        //            {
        //                Name = "helpFile",
        //                In = "formData",
        //                Description = "Help PDF File",
        //                Required = true,
        //                Type = "file"
        //            });
        //            operation.Consumes.Add("multipart/form-data");
        //        }
        //    }
        //}

        
        /// <summary>
        /// Upload jurisdictions from a CSV file.
        /// </summary>
        /// <remarks>Upload jurisdictions from a CSV file.</remarks>
        /// <param name="jurisdictionCsvFile">The CSV file containing jurisdictions.</param>
        [HttpPost("jurisdictions")]
        public async Task<IActionResult> UploadJurisdictions(IFormFile jurisdictionCsvFile)
        {
            
            if (jurisdictionCsvFile == null || jurisdictionCsvFile.Length == 0)
                return Content("Jurisdiction list is missing or empty");

            var path = await SaveJursidictionImportFile(jurisdictionCsvFile);

            await ParseJursidictionImportFile(path);
            
            if (!_headerParsed)
            {
                return BadRequest(
                    "The file was not properly formatted. Column names and order should be: ANSI/GNIS Code, FIPS Code/GENC, Jurisdiction Name. Tag");
            }

            Request.HttpContext.Response.Headers.Add("Modified-Count", _jurisdictionsModified.ToString()); 
            return Ok(new { path});
            
        }
        
        //public class JuridictionFileUploadOperation : IOperationFilter
        //{
        //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //    {
        //        if (operation.OperationId.ToLower() == "apiimportsjurisdictionspost")
        //        {
        //            operation.Parameters.Clear();
        //            operation.Parameters.Add(new NonBodyParameter
        //            {
        //                Name = "jurisdictionCsvFile",
        //                In = "formData",
        //                Description = "Jursidictions CSV File",
        //                Required = true,
        //                Type = "file"
        //            });
        //            operation.Consumes.Add("multipart/form-data");
        //        }
        //    }
        //}


        private DirectoryInfo GetJurisdicitionImportDirectoryInfo()
        {

            var jurisdictionDir = Path.Combine(_environment.WebRootPath, "imports", "jursidictions");
            var jursidictionDirectoryInfo = new DirectoryInfo(jurisdictionDir);

            if (!jursidictionDirectoryInfo.Exists)
            {
                // try to create the directory for the jurisdiction imports
                jursidictionDirectoryInfo.Create();
            }
 
            return jursidictionDirectoryInfo;

        }

        
        private async Task<String> SaveJursidictionImportFile(IFormFile jurisdictionCsvFile)
        {

            var jursidictionDirectoryInfo = GetJurisdicitionImportDirectoryInfo();
            
            var jurisdictionFilePath = Path.Combine(jursidictionDirectoryInfo.FullName, jurisdictionCsvFile.FileName);

            using (var stream = new FileStream(jurisdictionFilePath, FileMode.Create))
            {
                await jurisdictionCsvFile.CopyToAsync(stream);
            }

            return jurisdictionFilePath;

        }

        private async Task<bool> ParseJursidictionImportFile(String jurisdictionFilePath)
        {
            Regex csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            
            using (var streamReader = System.IO.File.OpenText(jurisdictionFilePath))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = await streamReader.ReadLineAsync();
                    
                    
                    // extract the fields
                    String[] fields = csvParser.Split(line);


                    // clean up the fields (remove " and leading spaces)
                    for (int i = 0; i < fields.Length; i++)
                    {
                        fields[i] = fields[i].TrimStart(' ', '"');
                        fields[i] = fields[i].TrimEnd(' ', '"');
                    }

                    // Require all fields - skip if there's a blank
                    var nameFromCsv = fields[2];
                    var gnisCodeFromCsv = fields[0];
                    var fipsCodeFromCsv = fields[1];
                    var tag = fields[3];

                    if (nameFromCsv.Equals("") || gnisCodeFromCsv.Equals("") || fipsCodeFromCsv.Equals(""))
                    {
                        continue;
                    }
                    
                    if (nameFromCsv.Equals("Jurisdiction Name") && gnisCodeFromCsv.Equals("ANSI/GNIS Code") && fipsCodeFromCsv.Equals("FIPS Code/GENC") && tag.Equals("Tag"))
                    {
                        _headerParsed = true;
                        continue;
                    }

                    if (!_headerParsed)
                    {
                        return false;
                    }

                    var gnisFipsConcatenation = gnisCodeFromCsv + fipsCodeFromCsv;
                    
                    // Check to see if we have a jurisdiction matching this hashcode
                    var jurisdictionMatch =
                        await _unitOfWork.Jurisdictions.SingleOrDefaultAsync(j => j.GnisFipsConcatenation == gnisFipsConcatenation);

                    if (jurisdictionMatch != null)
                    {
                        if(jurisdictionMatch.Name != nameFromCsv) jurisdictionMatch.Name = nameFromCsv;

                        if (jurisdictionMatch.Tag != tag) jurisdictionMatch.Tag = tag;
                        
                        continue;
                    }

                    //Console.WriteLine("Adding new jurisdiction - gnisFipsHashCode: " +gnisFipsHashCode +" Name: " +nameFromCsv +" GnisCode: " +gnisCodeFromCsv +" FipsCode: " +fipsCodeFromCsv +" Tag: " +tag);
                    
                    var jursidiction = new Jurisdiction { GnisFipsConcatenation = gnisFipsConcatenation, Name = nameFromCsv, GnisCode = gnisCodeFromCsv, FipsCode = fipsCodeFromCsv, Tag = tag};
                    
//                    Console.WriteLine("Parsed jurisdiction -> Name = {0}, GNIS Code = {1}, FIPS Code = {2}", 
//                        jursidiction.Name, jursidiction.GnisCode, jursidiction.FipsCode);
                    _unitOfWork.Jurisdictions.Add(jursidiction);
                }
                
            }

            _jurisdictionsModified = await _unitOfWork.Complete();
            
            return true;
        }
        

    }
}