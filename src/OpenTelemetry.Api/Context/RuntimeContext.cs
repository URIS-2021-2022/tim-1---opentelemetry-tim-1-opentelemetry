// <copyright file="RuntimeContext.cs" company="OpenTelemetry Authors">
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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using OpenTelemetry.Internal;

namespace OpenTelemetry.Context
{
    /// <summary>
    /// Generic runtime context management API.
    /// </summary>
    public static class RuntimeContext
    {
        private static readonly ConcurrentDictionary<string, object> Slots = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Gets or sets the actual context carrier implementation.
        /// </summary>
        public static Type ContextSlotType { get; set; } = typeof(AsyncLocalRuntimeContextSlot<>);

        /// <summary>
        /// Register a named context slot.
        /// </summary>
        /// <param name="slotName">The name of the context slot.</param>
        /// <typeparam name="T">The type of the underlying value.</typeparam>
        /// <returns>The slot registered.</returns>
        public static RuntimeContextSlot<T> RegisterSlot<T>(string slotName)
        {
            Guard.NullOrEmpty(slotName, nameof(slotName));

            lock (Slots)
            {
                if (Slots.ContainsKey(slotName))
                {
                    throw new InvalidOperationException($"Context slot already registered: '{slotName}'");
                }

                var type = ContextSlotType.MakeGenericType(typeof(T));
                var ctor = type.GetConstructor(new Type[] { typeof(string) });
                var slot = (RuntimeContextSlot<T>)ctor.Invoke(new object[] { slotName });
                Slots[slotName] = slot;
                return slot;
            }
        }

        /// <summary>
        /// Get a registered slot from a given name.
        /// </summary>
        /// <summary>
        /// <param name="slotName">The name of the context slot.</param>
        /// <typeparam name="T">The type of the underlying value.</typeparam>
        /// <returns>The slot previously registered.</returns>
        /// </summary>
        /// <summary>
        /// Sets the value to a registered slot.
        /// </summary>
        /// <returns>Documentation for element.</returns>
        public static RuntimeContextSlot<T> GetSlot<T>(string slotName)
        {
            Guard.NullOrEmpty(slotName, nameof(slotName));
            var slot = GuardNotFound(slotName);
            var contextSlot = Guard.Type<RuntimeContextSlot<T>>(slot, nameof(slot));
            return contextSlot;
        }

        /// <summary>
        /// Gets the value from a registered slot.
        /// </summary>
        /// <param name="slotName">The name of the context slot.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>The value retrieved from the context slot.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValue<T>(string slotName)
        {
            return GetSlot<T>(slotName).Get();
        }

        /// <summary>
        /// Gets the value from a registered slot.
        /// </summary>
        /// <param name="slotName">The name of the context slot.</param>
        /// <returns>The value retrieved from the context slot.</returns>
        public static object GetValue(string slotName)
        {
            Guard.NullOrEmpty(slotName, nameof(slotName));
            var slot = GuardNotFound(slotName);
            var runtimeContextSlotValueAccessor = Guard.Type<IRuntimeContextSlotValueAccessor>(slot, nameof(slot));
            return runtimeContextSlotValueAccessor.Value;
        }

        /// <summary>
        /// Sets the value to a registered slot.
        /// </summary>
        /// <param name="slotName">The name of the context slot.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetValue(string slotName, object value)
        {
            Guard.NullOrEmpty(slotName, nameof(slotName));
            var slot = GuardNotFound(slotName);
            var runtimeContextSlotValueAccessor = Guard.Type<IRuntimeContextSlotValueAccessor>(slot, nameof(slot));
            runtimeContextSlotValueAccessor.Value = value;
        }

        // For testing purpose
        internal static void Clear()
        {
            Slots.Clear();
        }

        private static object GuardNotFound(string slotName)
        {
            if (!Slots.TryGetValue(slotName, out var slot))
            {
                throw new ArgumentException($"Context slot not found: '{slotName}'");
            }

            return slot;
        }
    }
}
