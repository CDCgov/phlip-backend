using Microsoft.AspNetCore.Mvc;
using Esquire.Data;
using Microsoft.AspNetCore.Hosting;

namespace esquire_backend.Controllers
{
    [Route("api/appInfo")]
    public class AppInfoController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public AppInfoController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        /// <summary>
        /// Get assembly version / database name
        /// </summary>
        [HttpGet]
        public string GetAssemblyVersion()
        {
            var infoObject = _unitOfWork.DbInfo();
            return infoObject;
        }
    
    }
}
