// <copyright file="JaegerSpan.cs" company="OpenTelemetry Authors">
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
    internal readonly struct JaegerSpan : TUnionBase
    {
        public JaegerSpan(
            string peerServiceName,
            long traceIdLow,
            long traceIdHigh,
            long spanId,
            long parentSpanId,
            string operationName,
            int flags,
            long startTime,
            long duration,
            in PooledList<JaegerSpanRef> references,
            in PooledList<JaegerTag> tags,
            in PooledList<JaegerLog> logs)
        {
            this.PeerServiceName = peerServiceName;
            this.TraceIdLow = traceIdLow;
            this.TraceIdHigh = traceIdHigh;
            this.SpanId = spanId;
            this.ParentSpanId = parentSpanId;
            this.OperationName = operationName;
            this.Flags = flags;
            this.StartTime = startTime;
            this.Duration = duration;
            this.References = references;
            this.Tags = tags;
            this.Logs = logs;
        }

        public string PeerServiceName { get; }

        public long TraceIdLow { get; }

        public long TraceIdHigh { get; }

        public long SpanId { get; }

        public long ParentSpanId { get; }

        public string OperationName { get; }

        public PooledList<JaegerSpanRef> References { get; }

        public int Flags { get; }

        public long StartTime { get; }

        public long Duration { get; }

        public PooledList<JaegerTag> Tags { get; }

        public PooledList<JaegerLog> Logs { get; }

        public void Write(TProtocol tProtocol)
        {
            tProtocol.IncrementRecursionDepth();
            try
            {
                var struc = new TStruct("Span");
                tProtocol.WriteStructBegin(struc);

                var field = new TField
                {
                    Name = "traceIdLow",
                    Type = TType.I64,
                    ID = 1,
                };

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.TraceIdLow);
                tProtocol.WriteFieldEnd();

                field.Name = "traceIdHigh";
                field.Type = TType.I64;
                field.ID = 2;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.TraceIdHigh);
                tProtocol.WriteFieldEnd();

                field.Name = "spanId";
                field.Type = TType.I64;
                field.ID = 3;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.SpanId);
                tProtocol.WriteFieldEnd();

                field.Name = "parentSpanId";
                field.Type = TType.I64;
                field.ID = 4;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.ParentSpanId);
                tProtocol.WriteFieldEnd();

                field.Name = "operationName";
                field.Type = TType.String;
                field.ID = 5;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteString(this.OperationName);
                tProtocol.WriteFieldEnd();

                if (!this.References.IsEmpty)
                {
                    field.Name = "references";
                    field.Type = TType.List;
                    field.ID = 6;
                    tProtocol.WriteFieldBegin(field);
                    this.WriteList(tProtocol);

                    tProtocol.WriteFieldEnd();
                }

                field.Name = "flags";
                field.Type = TType.I32;
                field.ID = 7;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI32(this.Flags);
                tProtocol.WriteFieldEnd();

                field.Name = "startTime";
                field.Type = TType.I64;
                field.ID = 8;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.StartTime);
                tProtocol.WriteFieldEnd();

                field.Name = "duration";
                field.Type = TType.I64;
                field.ID = 9;

                tProtocol.WriteFieldBegin(field);
                tProtocol.WriteI64(this.Duration);
                tProtocol.WriteFieldEnd();

                if (!this.Tags.IsEmpty)
                {
                    field.Name = "JaegerTags";
                    field.Type = TType.List;
                    field.ID = 10;

                    tProtocol.WriteFieldBegin(field);
                    this.WriteInList(tProtocol);

                    tProtocol.WriteFieldEnd();
                }

                if (!this.Logs.IsEmpty)
                {
                    field.Name = "logs";
                    field.Type = TType.List;
                    field.ID = 11;
                    tProtocol.WriteFieldBegin(field);

                    this.SepMet(tProtocol);

                    tProtocol.WriteFieldEnd();
                }

                tProtocol.WriteFieldStop();
                tProtocol.WriteStructEnd();
            }
            finally
            {
                tProtocol.DecrementRecursionDepth();
            }
        }

        public void Return()
        {
            this.References.Return();
            this.Tags.Return();
            if (!this.Logs.IsEmpty)
            {
                for (int i = 0; i < this.Logs.Count; i++)
                {
                    this.Logs[i].Fields.Return();
                }

                this.Logs.Return();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Span(");
            sb.Append(", TraceIdLow: ");
            sb.Append(this.TraceIdLow);
            sb.Append(", TraceIdHigh: ");
            sb.Append(this.TraceIdHigh);
            sb.Append(", SpanId: ");
            sb.Append(this.SpanId);
            sb.Append(", ParentSpanId: ");
            sb.Append(this.ParentSpanId);
            sb.Append(", OperationName: ");
            sb.Append(this.OperationName);
            if (!this.References.IsEmpty)
            {
                sb.Append(", References: ");
                sb.Append(this.References);
            }

            sb.Append(", Flags: ");
            sb.Append(this.Flags);
            sb.Append(", StartTime: ");
            sb.Append(this.StartTime);
            sb.Append(", Duration: ");
            sb.Append(this.Duration);
            if (!this.Tags.IsEmpty)
            {
                sb.Append(", JaegerTags: ");
                sb.Append(this.Tags);
            }

            if (!this.Logs.IsEmpty)
            {
                sb.Append(", Logs: ");
                sb.Append(this.Logs);
            }

            sb.Append(')');
            return sb.ToString();
        }

        private void SepMet(TProtocol tProtocol)
        {
            tProtocol.WriteListBegin(new TList(TType.Struct, this.Logs.Count));

            for (int i = 0; i < this.Logs.Count; i++)
            {
                this.Logs[i].Write(tProtocol);
            }

            tProtocol.WriteListEnd();
        }

        private void WriteInList(TProtocol oprot)
        {
            oprot.WriteListBegin(new TList(TType.Struct, this.Tags.Count));

            for (int i = 0; i < this.Tags.Count; i++)
            {
                this.Tags[i].Write(oprot);
            }

            oprot.WriteListEnd();
        }

        private void WriteList(TProtocol oprot)
        {
            oprot.WriteListBegin(new TList(TType.Struct, this.References.Count));

            for (int i = 0; i < this.References.Count; i++)
            {
                this.References[i].Write(oprot);
            }

            oprot.WriteListEnd();
        }
    }
}
