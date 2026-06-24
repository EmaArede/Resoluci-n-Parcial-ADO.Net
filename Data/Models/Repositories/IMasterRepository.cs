using DronSimulator.Data.Models;
using System.Collections.Generic;

namespace DronSimulator.Data.Repositories
{
    public interface IMasterRepository
    {
        int InsertMaster(MasterControl master);
        void InsertDetLogs(int masterId, List<DetLog> logs);
        List<DetLog> GetLastFiveLogs(int masterId);
    }
}