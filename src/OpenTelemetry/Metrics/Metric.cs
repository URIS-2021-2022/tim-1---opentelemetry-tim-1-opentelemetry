// <copyright file="Metric.cs" company="OpenTelemetry Authors">
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
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace OpenTelemetry.Metrics
{
    public sealed class Metric
    {
        internal static readonly double[] DefaultHistogramBounds = new double[] { 0, 5, 10, 25, 50, 75, 100, 250, 500, 1000 };
        private readonly AggregatorStore aggStore;

        internal Metric(
            Instrument instrument,
            AggregationTemporalities temporality,
            string metricName,
            string metricDescription,
            double[] histogramBounds = null,
            string[] tagKeysInteresting = null)
        {
            this.Name = metricName;
            this.Description = metricDescription;
            this.Unit = instrument.Unit;
            this.Meter = instrument.Meter;
            AggregationType aggType = default;
            if (instrument is ObservableCounter<long>
                || instrument.GetType() == typeof(ObservableCounter<int>)
                || instrument is ObservableCounter<short>
                || instrument is ObservableCounter<byte>)
            {
                aggType = AggregationType.LongSumIncomingCumulative;
                this.MetricType = MetricTypes.LongSum;
            }
            else if (instrument.GetType() == typeof(Counter<long>)
                || instrument.GetType() == typeof(Counter<int>)
                || instrument.GetType() == typeof(Counter<short>)
                || instrument.GetType() == typeof(Counter<byte>))
            {
                aggType = AggregationType.LongSumIncomingDelta;
                this.MetricType = MetricTypes.LongSum;
            }
            else if ((instrument is Counter<double>)
                || instrument is Counter<float>)
            {
                aggType = AggregationType.DoubleSumIncomingDelta;
                this.MetricType = MetricTypes.DoubleSum;
            }
            else if (instrument is ObservableCounter<double> || instrument is ObservableCounter<float>)
            {
                aggType = AggregationType.DoubleSumIncomingCumulative;
                this.MetricType = MetricTypes.DoubleSum;
            }
            else if (instrument is ObservableGauge<double> || instrument is ObservableGauge<float>)
            {
                aggType = AggregationType.DoubleGauge;
                this.MetricType = MetricTypes.DoubleGauge;
            }
            else if (instrument is ObservableGauge<long>
                || instrument is ObservableGauge<int>
                || instrument is ObservableGauge<short>
                || instrument is ObservableGauge<byte>)
            {
                aggType = AggregationType.LongGauge;
                this.MetricType = MetricTypes.LongGauge;
            }
            else if (instrument is Histogram<long>
                || instrument is Histogram<int>
                || instrument is Histogram<short>
                || instrument is Histogram<byte>
                || instrument is Histogram<float>
                || instrument is Histogram<double>)
            {
                this.MetricType = MetricTypes.Histogram;

                if (histogramBounds != null
                    && histogramBounds.Length == 0)
                {
                    aggType = AggregationType.HistogramSumCount;
                }
                else
                {
                    aggType = AggregationType.Histogram;
                }
            }
            else
            {
                // TODO: Log and assign some invalid Enum.
            }

            this.aggStore = new AggregatorStore(aggType, temporality, histogramBounds ?? DefaultHistogramBounds, tagKeysInteresting);
            this.Temporality = temporality;
        }

        public MetricTypes MetricType { get; private set; }

        public AggregationTemporalities Temporality { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Unit { get; private set; }

        public Meter Meter { get; private set; }

        public BatchMetricPoint GetMetricPoints()
        {
            return this.aggStore.GetMetricPoints();
        }

        internal void UpdateLong(long value, ReadOnlySpan<KeyValuePair<string, object>> tags)
        {
            this.aggStore.Update(value, tags);
        }

        internal void UpdateDouble(double value, ReadOnlySpan<KeyValuePair<string, object>> tags)
        {
            this.aggStore.Update(value, tags);
        }

        internal void SnapShot()
        {
            this.aggStore.SnapShot();
        }
    }
}
