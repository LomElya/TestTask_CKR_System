using System;
using Cysharp.Threading.Tasks;

namespace TestTask_CKR_System.Core.Networking
{
    public class RequestHandle<T>
    {
        private readonly Action<Guid> _cancel;

        internal RequestHandle(Guid id, UniTask<T> completion, System.Action<Guid> cancel)
        {
            Id = id;
            Completion = completion;
            _cancel = cancel;
        }

        public Guid Id { get; }
        public UniTask<T> Completion { get; }

        public void Cancel()
        {
            _cancel(Id);
        }
    }
}