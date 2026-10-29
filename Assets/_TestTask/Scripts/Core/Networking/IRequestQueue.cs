using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTask_CKR_System.Core.Networking
{
    public class RequestQueue : IRequestQueue, IDisposable
    {
        private readonly LinkedList<IQueueItem> _pending = new();
        private IQueueItem _active;
        private bool _isProcessing;
        private bool _isDisposed;

        public RequestHandle<T> Enqueue<T>(
            string ownerId,
            Func<CancellationToken, UniTask<T>> request)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(RequestQueue));
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("An owner id is required.", nameof(ownerId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var item = new QueueItem<T>(ownerId, request);
            _pending.AddLast(item);
            ProcessPendingAsync();
            return new RequestHandle<T>(item.Id, item.Completion, Cancel);
        }

        public void Cancel(Guid requestId)
        {
            if (_active != null && _active.Id == requestId)
            {
                _active.Cancel();
                return;
            }

            var node = _pending.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.Id == requestId)
                {
                    _pending.Remove(node);
                    node.Value.Cancel();
                    return;
                }

                node = next;
            }
        }

        public void CancelOwner(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
                return;

            if (_active != null && _active.OwnerId == ownerId)
                _active.Cancel();

            var node = _pending.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.OwnerId == ownerId)
                {
                    _pending.Remove(node);
                    node.Value.Cancel();
                }

                node = next;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _active?.Cancel();

            foreach (var item in _pending)
                item.Cancel();

            _pending.Clear();
        }

        private async void ProcessPendingAsync()
        {
            if (_isProcessing || _isDisposed)
                return;

            _isProcessing = true;
            try
            {
                while (!_isDisposed && _pending.Count > 0)
                {
                    _active = _pending.First.Value;
                    _pending.RemoveFirst();
                    await _active.ExecuteAsync();
                    _active = null;
                }
            }
            finally
            {
                _active = null;
                _isProcessing = false;

                if (!_isDisposed && _pending.Count > 0)
                    ProcessPendingAsync();
            }
        }

        private interface IQueueItem
        {
            Guid Id { get; }
            string OwnerId { get; }
            UniTask ExecuteAsync();
            void Cancel();
        }

        private class QueueItem<T> : IQueueItem
        {
            private readonly Func<CancellationToken, UniTask<T>> _request;
            private readonly CancellationTokenSource _cancellation = new();
            private readonly UniTaskCompletionSource<T> _completion = new();
            private bool _started;

            public Guid Id { get; }
            public string OwnerId { get; }
            public UniTask<T> Completion => _completion.Task;

            public QueueItem(string ownerId, Func<CancellationToken, UniTask<T>> request)
            {
                Id = Guid.NewGuid();
                OwnerId = ownerId;
                _request = request;
            }

            public async UniTask ExecuteAsync()
            {
                if (_cancellation.IsCancellationRequested)
                    return;

                _started = true;
                try
                {
                    var result = await _request(_cancellation.Token);
                    _completion.TrySetResult(result);
                }
                catch (OperationCanceledException)
                {
                    _completion.TrySetCanceled();
                }
                catch (Exception exception)
                {
                    _completion.TrySetException(exception);
                }
                finally
                {
                    _cancellation.Dispose();
                }
            }

            public void Cancel()
            {
                if (_completion.Task.Status.IsCompleted())
                    return;

                _cancellation.Cancel();
                if (!_started)
                {
                    _completion.TrySetCanceled();
                    _cancellation.Dispose();
                }
            }
        }
    }

    public interface IRequestQueue
    {
        RequestHandle<T> Enqueue<T>(
            string ownerId,
            Func<CancellationToken, UniTask<T>> request);

        void Cancel(Guid requestId);
        void CancelOwner(string ownerId);
    }
}