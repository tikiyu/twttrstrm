using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Stats.Domain.Entities;

namespace Twitter.Stats.Application.Common.Interfaces
{
    public interface IHashTagService
    {
        Task InsertHashTag(IEnumerable<HashTag> hashTags);
    }
}
