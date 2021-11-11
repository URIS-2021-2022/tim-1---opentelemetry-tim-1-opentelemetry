// <copyright file="PooledList.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace OpenTelemetry.Internal
{
    internal readonly struct PooledList<T> : IEnumerable<T>, ICollection
    {
        private static int lastAllocatedSize = 64;

        private readonly T[] buffer;

        private PooledList(T[] buffer, int count)
        {
            this.buffer = buffer;
            this.Count = count;
        }

        public int Count { get; }

        public bool IsEmpty => this.Count == 0;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public ref T this[int index]
        {
            get => ref this.buffer[index];
        }

        public static PooledList<T> Create()
        {
            return new PooledList<T>(ArrayPool<T>.Shared.Rent(lastAllocatedSize), 0);
        }

        public static void Add(ref PooledList<T> list, T item)
        {
            Guard.Null(list.buffer, $"{nameof(list)}.{nameof(list.buffer)}");

            var b = list.buffer;

            if (list.Count >= b.Length)
            {
                lastAllocatedSize = b.Length * 2;
                var previousBuffer = b;

                b = ArrayPool<T>.Shared.Rent(lastAllocatedSize);

                var span = previousBuffer.AsSpan();
                span.CopyTo(b);
                ArrayPool<T>.Shared.Return(previousBuffer);
            }

            b[list.Count] = item;
            list = new PooledList<T>(b, list.Count + 1);
        }

        public static void Clear(ref PooledList<T> list)
        {
            list = new PooledList<T>(list.buffer, 0);
        }

        public void Return()
        {
            var buffer2 = this.buffer;
            if (buffer2 != null)
            {
                ArrayPool<T>.Shared.Return(buffer2);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(this.buffer, 0, array, index, this.Count);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(in this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(in this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(in this);
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private readonly T[] buffer;
            private readonly int count;
            private int index;
            private T current;

            public Enumerator(in PooledList<T> list)
            {
                this.buffer = list.buffer;
                this.count = list.Count;
                this.index = 0;
                this.current = default;
            }

            public T Current { get => this.current; }

            object IEnumerator.Current { get => this.Current; }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.index < this.count)
                {
                    this.current = this.buffer[this.index++];
                    return true;
                }

                this.index = this.count + 1;
                this.current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                this.index = 0;
                this.current = default;
            }
        }
    }
}
