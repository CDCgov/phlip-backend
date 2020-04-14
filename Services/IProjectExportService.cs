using System.Threading.Tasks;

namespace Esquire.Services
{
    public interface IProjectExportService
    {
        Task<ExportResult> ExportProjectData(long projectId, string type, long userId);
        
    }
}