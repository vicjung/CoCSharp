﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CoCSharp.Server
{
    public class SocketAsyncEventArgsPool : IDisposable
    {
        public SocketAsyncEventArgsPool(int capacity, EventHandler<SocketAsyncEventArgs> handler)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("capacity cannot be less that 1.");

            Capacity = capacity;
            _objLock = new object();
            _pool = new Stack<SocketAsyncEventArgs>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                var args = new SocketAsyncEventArgs();
                args.Completed += handler;

                Push(args);
            }
        }

        private bool _disposed;
        private object _objLock;
        private Stack<SocketAsyncEventArgs> _pool;

        public int Capacity { get; private set; }
        public int Count { get { return _pool.Count; } }

        public SocketAsyncEventArgs Pop()
        {
            lock(_objLock)
            {
                if (_pool.Count == 0)
                    return null;

                return _pool.Pop();
            }
        }

        public void Push(SocketAsyncEventArgs args)
        {
            lock(_objLock)
            {
                _pool.Push(args);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    for (int i = 0; i < _pool.Count; i++)
                    {
                        var args = _pool.Pop();
                        args.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}
