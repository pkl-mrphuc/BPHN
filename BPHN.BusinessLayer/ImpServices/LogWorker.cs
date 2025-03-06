using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class LogWorker : ILogWorker
    {
        public LogWorker() { }

        public Task Handle(string dataJson)
        {
            if (string.IsNullOrWhiteSpace(dataJson))
            {
                throw new Exception("Input Empty");
            }

            var history = JsonConvert.DeserializeObject<HistoryLog>(dataJson);

            if (history is null)
            {
                throw new Exception("Input Empty");
            }

            return Task.CompletedTask;
        }
    }
}
