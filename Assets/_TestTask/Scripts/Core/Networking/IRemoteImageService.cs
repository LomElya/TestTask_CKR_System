using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTask_CKR_System.Core.Networking
{
    public interface IRemoteImageService
    {
        UniTask<byte[]> GetAsync(string absoluteUrl, CancellationToken cancellationToken);
    }
}