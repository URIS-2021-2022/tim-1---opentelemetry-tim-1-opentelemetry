// <copyright file="MetricTypeExtensions.cs" company="OpenTelemetry Authors">
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

using System.Runtime.CompilerServices;

namespace OpenTelemetry.Metrics
{
    public static class MetricTypeExtensions
    {
#pragma warning disable SA1310 // field should not contain an underscore

        internal const MetricTypes METRIC_TYPE_MASK = (MetricTypes)0xf0;

        internal const MetricTypes METRIC_TYPE_SUM = (MetricTypes)0x10;
        internal const MetricTypes METRIC_TYPE_GAUGE = (MetricTypes)0x20;
        internal const MetricTypes METRIC_TYPE_HISTOGRAM = (MetricTypes)0x30;
        /* internal const byte METRIC_TYPE_SUMMARY = 0x40; // not used */

        internal const MetricTypes POINT_KIND_MASK = (MetricTypes)0x0f;

        internal const MetricTypes POINT_KIND_I1 = (MetricTypes)0x04; // signed 1-byte integer
        internal const MetricTypes POINT_KIND_U1 = (MetricTypes)0x05; // unsigned 1-byte integer
        internal const MetricTypes POINT_KIND_I2 = (MetricTypes)0x06; // signed 2-byte integer
        internal const MetricTypes POINT_KIND_U2 = (MetricTypes)0x07; // unsigned 2-byte integer
        internal const MetricTypes POINT_KIND_I4 = (MetricTypes)0x08; // signed 4-byte integer
        internal const MetricTypes POINT_KIND_U4 = (MetricTypes)0x09; // unsigned 4-byte integer
        internal const MetricTypes POINT_KIND_I8 = (MetricTypes)0x0a; // signed 8-byte integer
        internal const MetricTypes POINT_KIND_U8 = (MetricTypes)0x0b; // unsigned 8-byte integer
        internal const MetricTypes POINT_KIND_R4 = (MetricTypes)0x0c; // 4-byte floating point
        internal const MetricTypes POINT_KIND_R8 = (MetricTypes)0x0d; // 8-byte floating point

#pragma warning restore SA1310 // field should not contain an underscore

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSum(this MetricTypes self)
        {
            return (self & METRIC_TYPE_MASK) == METRIC_TYPE_SUM;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGauge(this MetricTypes self)
        {
            return (self & METRIC_TYPE_MASK) == METRIC_TYPE_GAUGE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHistogram(this MetricTypes self)
        {
            return (self & METRIC_TYPE_MASK) == METRIC_TYPE_HISTOGRAM;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDouble(this MetricTypes self)
        {
            return (self & POINT_KIND_MASK) == POINT_KIND_R8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLong(this MetricTypes self)
        {
            return (self & POINT_KIND_MASK) == POINT_KIND_I8;
        }
    }
}
