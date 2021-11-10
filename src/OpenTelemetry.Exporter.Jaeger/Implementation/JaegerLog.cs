// <copyright file="JaegerLog.cs" company="OpenTelemetry Authors">
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

using System.Text;
using OpenTelemetry.Internal;
using Thrift.Protocol;
using Thrift.Protocol.Entities;

namespace OpenTelemetry.Exporter.Jaeger.Implementation
{
    internal readonly struct JaegerLog : TUnionBase
    {
        public JaegerLog(long timestamp, in PooledList<JaegerTag> fields)
        {
            this.Timestamp = timestamp;
            this.Fields = fields;
        }

        public long Timestamp { get; }

        public PooledList<JaegerTag> Fields { get; }

        public void Write(TProtocol tProtocol)
        {
            tProtocol.IncrementRecursionDepth();
            try
            {
                var struc = new TStruct("Log");
                tProtocol.WriteStructBegin(struc);

                var field = new TField
                {
                    Name = "timestamp",
                    Type = TType.I64,
                    ID = 1,
                };

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.Timestamp);
                tProtocol.WriteFieldEnd();

                field.Name = "fields";
                field.Type = TType.List;
                field.ID = 2;

                tProtocol.WriteFieldBegin(field);

                this.WriteList(tProtocol);

                tProtocol.WriteFieldEnd();
                tProtocol.WriteFieldStop();
                tProtocol.WriteStructEnd();
            }
            finally
            {
                tProtocol.DecrementRecursionDepth();
            }
        }


       // private void WriteList(TProtocol tProtocol)

        public override string ToString()
        {
            var sb = new StringBuilder("Log(");
            sb.Append(", Timestamp: ");
            sb.Append(this.Timestamp);
            sb.Append(", Fields: ");
            sb.Append(this.Fields);
            sb.Append(')');
            return sb.ToString();
        }

        private void WriteList(TProtocol tProtocol)
        {
            tProtocol.WriteListBegin(new TList(TType.Struct, this.Fields.Count));

            for (int i = 0; i < this.Fields.Count; i++)
            {
                this.Fields[i].Write(tProtocol);
            }

            tProtocol.WriteListEnd();
        }
    }
}
