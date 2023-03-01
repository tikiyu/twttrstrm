using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;
using Twitter.Stats.API.Services;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Infrastructure.Services.Grpc
{
    public class HashTagService : IHashTagService
    {

        private readonly Hashtag.Service.HashTagGrpc.HashTagGrpcClient _client;
        private readonly ILogger<HashTagService> _logger;
        public HashTagService(GrpcClientFactory grpcClientFactory, ILogger<HashTagService> logger)
        {
            _logger = logger;
            _client = grpcClientFactory.CreateClient<Hashtag.Service.HashTagGrpc.HashTagGrpcClient>("HashTagGrpcClient");
        }

        public async Task InsertHashTagAsync(IEnumerable<HashTag> hashTags)
        {
            try
            {
                var request = new Hashtag.Service.HashtagListRequest();

                foreach (var hashTag in hashTags)
                {
                    request.Hashtags.Add(new Hashtag.Service.Hashtag
                    {
                        CreatedAt = hashTag.CreatedAt.ToTimestamp(),
                        Tag = hashTag.Tag
                    });
                }

                _ = await _client.InsertHashTagAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Error:{ex}", ex.ToString());

#if DEBUG
                using (FileStream fs = new($"{nameof(HashTagService)}.error.log", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StreamWriter write = new(fs);
                    write.BaseStream.Seek(0, SeekOrigin.End);
                    write.WriteLine($"{DateTime.Now}:{ex}");
                    write.WriteLine(Environment.NewLine);

                    write.Flush();
                    write.Close();
                    fs.Close();
                }
#endif
            }

        }

    }
}
