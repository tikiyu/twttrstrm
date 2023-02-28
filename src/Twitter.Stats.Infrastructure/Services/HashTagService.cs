using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Twitter.Stats.Application.Common.Interfaces;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Infrastructure.Services
{
    public class HashTagService : IHashTagService
    {

        private readonly Hashtag.Service.HashTagGrpc.HashTagGrpcClient _client;
        public HashTagService(GrpcClientFactory grpcClientFactory)
        {
            _client = grpcClientFactory.CreateClient<Hashtag.Service.HashTagGrpc.HashTagGrpcClient>("HashTagGrpcClient");
        }

        public async Task InsertHashTag(IEnumerable<HashTag> hashTags)
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
