namespace Esquire.Services
{
    public interface ILockingService
    {
        // protocol locks
        long CreateProtocolLock(long projectId, long requestingUserId);
        bool TryGetProtocolLock(long projectId, out long userIdWithLock);
        bool TryDeleteProtocolLock(long projectId, long userId);

        // scheme locks
        long CreateSchemeLock(long projectId, long requestingUserId);
        bool TryGetSchemeLock(long projectId, out long userIdWithLock);
        bool TryDeleteSchemeLock(long projectId, long userId);

    }
    
}