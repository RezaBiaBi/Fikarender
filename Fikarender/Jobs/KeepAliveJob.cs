using Quartz;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jobs
{
    [DisallowConcurrentExecution]
    public class KeepAliveJob : IJob
    {
        private readonly IHttpClientFactory _clientFactory;

        public KeepAliveJob(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetAsync("https://parsmvc.ir/job");
            }
        }
    }
}
