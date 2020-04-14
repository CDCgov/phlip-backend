using System.Collections.Generic;

namespace Esquire.Services
{
    public class LockingService : ILockingService
    {

        private readonly Dictionary<long, long> _protocolLocks; 
        private readonly Dictionary<long, long> _schemeLocks; 

        public LockingService()
        {
            
            _protocolLocks = new Dictionary<long, long>();
            _schemeLocks = new Dictionary<long, long>();

        }
        
        // protocol locks
        public long CreateProtocolLock(long projectId, long requestingUserId)
        {
            // if already locked return user who locked it
            if (_protocolLocks.TryAdd(projectId, requestingUserId) == false)
                return  _protocolLocks.GetValueOrDefault(projectId);

            // success, requesting user has lock
            return requestingUserId;
                
        }

        public bool TryGetProtocolLock(long projectId, out long userIdWithLock)
        {

            // try to get a lock with project id
            var success = _protocolLocks.TryGetValue(projectId, out var userId);
            userIdWithLock = userId;
            
            return success;
          
        }

        public bool TryDeleteProtocolLock(long projectId, long requestingUserId)
        {

            // try to get a lock with project id
            if (_protocolLocks.TryGetValue(projectId, out var userIdWithLock))
                if (requestingUserId == userIdWithLock)
                {
                    _protocolLocks.Remove(projectId);
                    return true;
                }

            return false;

        }

        // scheme locks
        public long CreateSchemeLock(long projectId, long requestingUserId)
        {
            // if already locked return user who locked it
            if (_schemeLocks.TryAdd(projectId, requestingUserId) == false)
                return  _schemeLocks.GetValueOrDefault(projectId);

            // success, requesting user has lock
            return requestingUserId;
                
        }

        public bool TryGetSchemeLock(long projectId, out long userIdWithLock)
        {

            // try to get a lock with project id
            var success = _schemeLocks.TryGetValue(projectId, out var userId);
            userIdWithLock = userId;
            
            return success;
          
        }

        public bool TryDeleteSchemeLock(long projectId, long requestingUserId)
        {

            // try to get a lock with project id
            if (_schemeLocks.TryGetValue(projectId, out var userIdWithLock))
                if (requestingUserId == userIdWithLock)
                {
                    _schemeLocks.Remove(projectId);
                    return true;
                }

            return false;

        }

    }
}