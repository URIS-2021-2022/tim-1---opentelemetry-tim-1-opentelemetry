// <auto-generated/> (Turns off StyleCop analysis in this file.)

// Licensed to the Apache Software Foundation(ASF) under one
// or more contributor license agreements.See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied. See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Buffers;
using System.Text;
using Thrift.Protocol.Entities;
using Thrift.Transport;

namespace Thrift.Protocol
{
    // ReSharper disable once InconsistentNaming
    internal abstract class TProtocol : IDisposable
    {
        public const int DefaultRecursionDepth = 64;
        private bool _isDisposed;
        protected int RecursionDepth;

        protected TTransport Trans;

        protected TProtocol(TTransport trans)
        {
            Trans = trans;
            RecursionLimit = DefaultRecursionDepth;
            RecursionDepth = 0;
        }

        public TTransport Transport => Trans;

        protected int RecursionLimit { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void IncrementRecursionDepth()
        {
            if (RecursionDepth >= RecursionLimit)
            {
                throw new TProtocolException(TProtocolException.DEPTH_LIMIT, $"Depth of recursion exceeded the limit: {RecursionLimit}");
            }

            ++RecursionDepth;
        }

        public void DecrementRecursionDepth()
        {
            --RecursionDepth;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    (Trans as IDisposable)?.Dispose();
                }
            }
            _isDisposed = true;
        }

        public abstract void WriteMessageBegin(TMessage message);

        public abstract void WriteMessageEnd();

        public abstract void WriteStructBegin(TStruct @struct);

        public abstract void WriteStructEnd();

        public abstract void WriteFieldBegin(TField field);

        public abstract void WriteFieldEnd();

        public abstract void WriteFieldStop();

        public abstract void WriteMapBegin(TMap map);

        public abstract void WriteMapEnd();

        public abstract void WriteListBegin(TList list);

        public abstract void WriteListEnd();

        public abstract void WriteSetBegin(TSet set);

        public abstract void WriteSetEnd();

        public abstract void WriteBool(bool b);

        public abstract void WriteByte(sbyte b);

        public abstract void WriteI16(short i16);

        public abstract void WriteI32(int i32);

        public abstract void WriteI64(long i64);

        public abstract void WriteDouble(double d);

        public virtual void WriteString(string s)
        {
            var buf = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(s));
            try
            {
                var numberOfBytes = Encoding.UTF8.GetBytes(s, 0, s.Length, buf, 0);

                WriteBinary(buf, 0, numberOfBytes);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buf);
            }
        }

        public abstract void WriteBinary(byte[] bytes, int offset, int count);
    }
}
