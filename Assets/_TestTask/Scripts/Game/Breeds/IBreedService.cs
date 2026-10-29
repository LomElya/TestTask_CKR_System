using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTask_CKR_System.Game.Breeds
{
    public interface IBreedService
    {
        UniTask<IReadOnlyList<BreedSummary>> GetBreedsAsync(CancellationToken cancellationToken);
        UniTask<BreedDetails> GetBreedAsync(string breedId, CancellationToken cancellationToken);
    }
}