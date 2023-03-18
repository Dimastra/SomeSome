using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Pow3r
{
	// Token: 0x0200027C RID: 636
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerState
	{
		// Token: 0x040007C8 RID: 1992
		public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
		{
			IncludeFields = true,
			Converters = 
			{
				new PowerState.NodeIdJsonConverter()
			}
		};

		// Token: 0x040007C9 RID: 1993
		public PowerState.GenIdStorage<PowerState.Supply> Supplies = new PowerState.GenIdStorage<PowerState.Supply>();

		// Token: 0x040007CA RID: 1994
		public PowerState.GenIdStorage<PowerState.Network> Networks = new PowerState.GenIdStorage<PowerState.Network>();

		// Token: 0x040007CB RID: 1995
		public PowerState.GenIdStorage<PowerState.Load> Loads = new PowerState.GenIdStorage<PowerState.Load>();

		// Token: 0x040007CC RID: 1996
		public PowerState.GenIdStorage<PowerState.Battery> Batteries = new PowerState.GenIdStorage<PowerState.Battery>();

		// Token: 0x040007CD RID: 1997
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public List<List<PowerState.Network>> GroupedNets;

		// Token: 0x02000931 RID: 2353
		[NullableContext(0)]
		public readonly struct NodeId : IEquatable<PowerState.NodeId>
		{
			// Token: 0x17000801 RID: 2049
			// (get) Token: 0x06003179 RID: 12665 RVA: 0x000FEDA5 File Offset: 0x000FCFA5
			public long Combined
			{
				get
				{
					return (long)((ulong)this.Index | (ulong)((ulong)((long)this.Generation) << 32));
				}
			}

			// Token: 0x0600317A RID: 12666 RVA: 0x000FEDB9 File Offset: 0x000FCFB9
			public NodeId(int index, int generation)
			{
				this.Index = index;
				this.Generation = generation;
			}

			// Token: 0x0600317B RID: 12667 RVA: 0x000FEDC9 File Offset: 0x000FCFC9
			public NodeId(long combined)
			{
				this.Index = (int)combined;
				this.Generation = (int)(combined >> 32);
			}

			// Token: 0x0600317C RID: 12668 RVA: 0x000FEDDE File Offset: 0x000FCFDE
			public bool Equals(PowerState.NodeId other)
			{
				return this.Index == other.Index && this.Generation == other.Generation;
			}

			// Token: 0x0600317D RID: 12669 RVA: 0x000FEE00 File Offset: 0x000FD000
			[NullableContext(2)]
			public override bool Equals(object obj)
			{
				if (obj is PowerState.NodeId)
				{
					PowerState.NodeId other = (PowerState.NodeId)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x0600317E RID: 12670 RVA: 0x000FEE25 File Offset: 0x000FD025
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.Index, this.Generation);
			}

			// Token: 0x0600317F RID: 12671 RVA: 0x000FEE38 File Offset: 0x000FD038
			public static bool operator ==(PowerState.NodeId left, PowerState.NodeId right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003180 RID: 12672 RVA: 0x000FEE42 File Offset: 0x000FD042
			public static bool operator !=(PowerState.NodeId left, PowerState.NodeId right)
			{
				return !left.Equals(right);
			}

			// Token: 0x06003181 RID: 12673 RVA: 0x000FEE50 File Offset: 0x000FD050
			[NullableContext(1)]
			public override string ToString()
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Index);
				defaultInterpolatedStringHandler.AppendLiteral(" (G");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Generation);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}

			// Token: 0x04001F1D RID: 7965
			public readonly int Index;

			// Token: 0x04001F1E RID: 7966
			public readonly int Generation;
		}

		// Token: 0x02000932 RID: 2354
		[NullableContext(0)]
		public static class GenIdStorage
		{
			// Token: 0x06003182 RID: 12674 RVA: 0x000FEE9F File Offset: 0x000FD09F
			[NullableContext(1)]
			public static PowerState.GenIdStorage<T> FromEnumerable<[Nullable(2)] T>([Nullable(new byte[]
			{
				1,
				0,
				1
			})] IEnumerable<ValueTuple<PowerState.NodeId, T>> enumerable)
			{
				return PowerState.GenIdStorage<T>.FromEnumerable(enumerable);
			}
		}

		// Token: 0x02000933 RID: 2355
		[Nullable(0)]
		public sealed class GenIdStorage<[Nullable(2)] T>
		{
			// Token: 0x17000802 RID: 2050
			// (get) Token: 0x06003183 RID: 12675 RVA: 0x000FEEA7 File Offset: 0x000FD0A7
			// (set) Token: 0x06003184 RID: 12676 RVA: 0x000FEEAF File Offset: 0x000FD0AF
			public int Count { get; private set; }

			// Token: 0x17000803 RID: 2051
			public T this[PowerState.NodeId id]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					PowerState.GenIdStorage<T>.Slot[] storage = this._storage;
					int index = id.Index;
					if (storage[index].Generation != id.Generation)
					{
						PowerState.GenIdStorage<T>.ThrowKeyNotFound();
					}
					return ref storage[index].Value;
				}
			}

			// Token: 0x06003186 RID: 12678 RVA: 0x000FEEE3 File Offset: 0x000FD0E3
			public GenIdStorage()
			{
				this._storage = Array.Empty<PowerState.GenIdStorage<T>.Slot>();
			}

			// Token: 0x06003187 RID: 12679 RVA: 0x000FEF04 File Offset: 0x000FD104
			public static PowerState.GenIdStorage<T> FromEnumerable([Nullable(new byte[]
			{
				1,
				0,
				1
			})] IEnumerable<ValueTuple<PowerState.NodeId, T>> enumerable)
			{
				PowerState.GenIdStorage<T> storage = new PowerState.GenIdStorage<T>();
				ValueTuple<PowerState.NodeId, T>[] cache = enumerable.ToArray<ValueTuple<PowerState.NodeId, T>>();
				if (cache.Length == 0)
				{
					return storage;
				}
				int maxSize = cache.Max((ValueTuple<PowerState.NodeId, T> tup) => tup.Item1.Index) + 1;
				storage._storage = new PowerState.GenIdStorage<T>.Slot[maxSize];
				foreach (ValueTuple<PowerState.NodeId, T> valueTuple in cache)
				{
					PowerState.NodeId id = valueTuple.Item1;
					T value = valueTuple.Item2;
					PowerState.GenIdStorage<T>.Slot[] storage2 = storage._storage;
					int index = id.Index;
					storage2[index].Generation = id.Generation;
					storage2[index].Value = value;
				}
				int nextFree = int.MaxValue;
				for (int i = 0; i < storage._storage.Length; i++)
				{
					ref PowerState.GenIdStorage<T>.Slot slot = ref storage._storage[i];
					if (slot.Generation == 0)
					{
						slot.NextSlot = nextFree;
						nextFree = i;
					}
				}
				storage.Count = cache.Length;
				storage._nextFree = nextFree;
				return storage;
			}

			// Token: 0x06003188 RID: 12680 RVA: 0x000FF000 File Offset: 0x000FD200
			public ref T Allocate(out PowerState.NodeId id)
			{
				if (this._nextFree == 2147483647)
				{
					this.ReAllocate();
				}
				int idx = this._nextFree;
				ref PowerState.GenIdStorage<T>.Slot slot = ref this._storage[idx];
				this.Count++;
				this._nextFree = slot.NextSlot;
				slot.NextSlot = -1;
				id = new PowerState.NodeId(idx, slot.Generation);
				return ref slot.Value;
			}

			// Token: 0x06003189 RID: 12681 RVA: 0x000FF070 File Offset: 0x000FD270
			public void Free(PowerState.NodeId id)
			{
				int idx = id.Index;
				ref PowerState.GenIdStorage<T>.Slot slot = ref this._storage[idx];
				if (slot.Generation != id.Generation)
				{
					PowerState.GenIdStorage<T>.ThrowKeyNotFound();
				}
				if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
				{
					slot.Value = default(T);
				}
				this.Count--;
				slot.Generation++;
				slot.NextSlot = this._nextFree;
				this._nextFree = idx;
			}

			// Token: 0x0600318A RID: 12682 RVA: 0x000FF0E4 File Offset: 0x000FD2E4
			[MethodImpl(MethodImplOptions.NoInlining)]
			private void ReAllocate()
			{
				int newLength = Math.Max(this._storage.Length, 2) * 2;
				this.ReAllocateTo(newLength);
			}

			// Token: 0x0600318B RID: 12683 RVA: 0x000FF10C File Offset: 0x000FD30C
			private void ReAllocateTo(int newSize)
			{
				int oldLength = this._storage.Length;
				Array.Resize<PowerState.GenIdStorage<T>.Slot>(ref this._storage, newSize);
				for (int i = oldLength; i < newSize - 1; i++)
				{
					PowerState.GenIdStorage<T>.Slot[] storage = this._storage;
					int num = i;
					storage[num].NextSlot = i + 1;
					storage[num].Generation = 1;
				}
				PowerState.GenIdStorage<T>.Slot[] storage2 = this._storage;
				storage2[storage2.Length - 1].NextSlot = this._nextFree;
				this._nextFree = oldLength;
			}

			// Token: 0x17000804 RID: 2052
			// (get) Token: 0x0600318C RID: 12684 RVA: 0x000FF179 File Offset: 0x000FD379
			[Nullable(0)]
			public PowerState.GenIdStorage<T>.ValuesCollection Values
			{
				[NullableContext(0)]
				get
				{
					return new PowerState.GenIdStorage<T>.ValuesCollection(this);
				}
			}

			// Token: 0x0600318D RID: 12685 RVA: 0x000FF181 File Offset: 0x000FD381
			[MethodImpl(MethodImplOptions.NoInlining)]
			private static void ThrowKeyNotFound()
			{
				throw new KeyNotFoundException();
			}

			// Token: 0x04001F1F RID: 7967
			private int _nextFree = int.MaxValue;

			// Token: 0x04001F20 RID: 7968
			[Nullable(new byte[]
			{
				1,
				0,
				0
			})]
			private PowerState.GenIdStorage<T>.Slot[] _storage;

			// Token: 0x02000BB5 RID: 2997
			[NullableContext(0)]
			private struct Slot
			{
				// Token: 0x04002C29 RID: 11305
				public int NextSlot;

				// Token: 0x04002C2A RID: 11306
				public int Generation;

				// Token: 0x04002C2B RID: 11307
				[Nullable(1)]
				public T Value;
			}

			// Token: 0x02000BB6 RID: 2998
			[Nullable(0)]
			public readonly struct ValuesCollection : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
			{
				// Token: 0x06003A9A RID: 15002 RVA: 0x0013464A File Offset: 0x0013284A
				public ValuesCollection(PowerState.GenIdStorage<T> owner)
				{
					this._owner = owner;
				}

				// Token: 0x06003A9B RID: 15003 RVA: 0x00134653 File Offset: 0x00132853
				[NullableContext(0)]
				public PowerState.GenIdStorage<T>.ValuesCollection.Enumerator GetEnumerator()
				{
					return new PowerState.GenIdStorage<T>.ValuesCollection.Enumerator(this._owner);
				}

				// Token: 0x170008EE RID: 2286
				// (get) Token: 0x06003A9C RID: 15004 RVA: 0x00134660 File Offset: 0x00132860
				public int Count
				{
					get
					{
						return this._owner.Count;
					}
				}

				// Token: 0x06003A9D RID: 15005 RVA: 0x0013466D File Offset: 0x0013286D
				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				// Token: 0x06003A9E RID: 15006 RVA: 0x0013467A File Offset: 0x0013287A
				IEnumerator<T> IEnumerable<!0>.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				// Token: 0x04002C2C RID: 11308
				private readonly PowerState.GenIdStorage<T> _owner;

				// Token: 0x02000BCF RID: 3023
				[Nullable(0)]
				public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
				{
					// Token: 0x06003AD9 RID: 15065 RVA: 0x0013526C File Offset: 0x0013346C
					public Enumerator(PowerState.GenIdStorage<T> owner)
					{
						this._owner = owner._storage;
						this.Current = default(T);
						this._index = -1;
					}

					// Token: 0x06003ADA RID: 15066 RVA: 0x0013529C File Offset: 0x0013349C
					public bool MoveNext()
					{
						PowerState.GenIdStorage<T>.Slot slot;
						for (;;)
						{
							this._index++;
							if (this._index >= this._owner.Length)
							{
								break;
							}
							slot = ref this._owner[this._index];
							if (slot.NextSlot < 0)
							{
								goto Block_1;
							}
						}
						return false;
						Block_1:
						this.Current = slot.Value;
						return true;
					}

					// Token: 0x06003ADB RID: 15067 RVA: 0x001352F1 File Offset: 0x001334F1
					public void Reset()
					{
						this._index = -1;
					}

					// Token: 0x170008EF RID: 2287
					// (get) Token: 0x06003ADC RID: 15068 RVA: 0x001352FA File Offset: 0x001334FA
					object IEnumerator.Current
					{
						get
						{
							return this.Current;
						}
					}

					// Token: 0x170008F0 RID: 2288
					// (get) Token: 0x06003ADD RID: 15069 RVA: 0x00135307 File Offset: 0x00133507
					// (set) Token: 0x06003ADE RID: 15070 RVA: 0x0013530F File Offset: 0x0013350F
					public T Current { readonly get; private set; }

					// Token: 0x06003ADF RID: 15071 RVA: 0x00135318 File Offset: 0x00133518
					public void Dispose()
					{
					}

					// Token: 0x04002C73 RID: 11379
					[Nullable(new byte[]
					{
						1,
						0,
						0
					})]
					private readonly PowerState.GenIdStorage<T>.Slot[] _owner;

					// Token: 0x04002C74 RID: 11380
					private int _index;
				}
			}
		}

		// Token: 0x02000934 RID: 2356
		[Nullable(0)]
		public sealed class NodeIdJsonConverter : JsonConverter<PowerState.NodeId>
		{
			// Token: 0x0600318E RID: 12686 RVA: 0x000FF188 File Offset: 0x000FD388
			public override PowerState.NodeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				return new PowerState.NodeId(reader.GetInt64());
			}

			// Token: 0x0600318F RID: 12687 RVA: 0x000FF195 File Offset: 0x000FD395
			public override void Write(Utf8JsonWriter writer, PowerState.NodeId value, JsonSerializerOptions options)
			{
				writer.WriteNumberValue(value.Combined);
			}
		}

		// Token: 0x02000935 RID: 2357
		[NullableContext(0)]
		public sealed class Supply
		{
			// Token: 0x04001F22 RID: 7970
			[ViewVariables]
			public PowerState.NodeId Id;

			// Token: 0x04001F23 RID: 7971
			[ViewVariables]
			public bool Enabled = true;

			// Token: 0x04001F24 RID: 7972
			[ViewVariables]
			public bool Paused;

			// Token: 0x04001F25 RID: 7973
			[ViewVariables]
			public float MaxSupply;

			// Token: 0x04001F26 RID: 7974
			[ViewVariables]
			public float SupplyRampRate = 5000f;

			// Token: 0x04001F27 RID: 7975
			[ViewVariables]
			public float SupplyRampTolerance = 5000f;

			// Token: 0x04001F28 RID: 7976
			[ViewVariables]
			public float CurrentSupply;

			// Token: 0x04001F29 RID: 7977
			[ViewVariables]
			[JsonIgnore]
			public float SupplyRampTarget;

			// Token: 0x04001F2A RID: 7978
			[ViewVariables]
			public float SupplyRampPosition;

			// Token: 0x04001F2B RID: 7979
			[ViewVariables]
			[JsonIgnore]
			public PowerState.NodeId LinkedNetwork;

			// Token: 0x04001F2C RID: 7980
			[JsonIgnore]
			public float AvailableSupply;
		}

		// Token: 0x02000936 RID: 2358
		[NullableContext(0)]
		public sealed class Load
		{
			// Token: 0x04001F2D RID: 7981
			[ViewVariables]
			public PowerState.NodeId Id;

			// Token: 0x04001F2E RID: 7982
			[ViewVariables]
			public bool Enabled = true;

			// Token: 0x04001F2F RID: 7983
			[ViewVariables]
			public bool Paused;

			// Token: 0x04001F30 RID: 7984
			[ViewVariables]
			public float DesiredPower;

			// Token: 0x04001F31 RID: 7985
			[ViewVariables]
			public float ReceivingPower;

			// Token: 0x04001F32 RID: 7986
			[ViewVariables]
			[JsonIgnore]
			public PowerState.NodeId LinkedNetwork;
		}

		// Token: 0x02000937 RID: 2359
		[NullableContext(0)]
		public sealed class Battery
		{
			// Token: 0x04001F33 RID: 7987
			[ViewVariables]
			public PowerState.NodeId Id;

			// Token: 0x04001F34 RID: 7988
			[ViewVariables]
			public bool Enabled = true;

			// Token: 0x04001F35 RID: 7989
			[ViewVariables]
			public bool Paused;

			// Token: 0x04001F36 RID: 7990
			[ViewVariables]
			public bool CanDischarge = true;

			// Token: 0x04001F37 RID: 7991
			[ViewVariables]
			public bool CanCharge = true;

			// Token: 0x04001F38 RID: 7992
			[ViewVariables]
			public float Capacity;

			// Token: 0x04001F39 RID: 7993
			[ViewVariables]
			public float MaxChargeRate;

			// Token: 0x04001F3A RID: 7994
			[ViewVariables]
			public float MaxThroughput;

			// Token: 0x04001F3B RID: 7995
			[ViewVariables]
			public float MaxSupply;

			// Token: 0x04001F3C RID: 7996
			[ViewVariables]
			public float SupplyRampTolerance = 5000f;

			// Token: 0x04001F3D RID: 7997
			[ViewVariables]
			public float SupplyRampRate = 5000f;

			// Token: 0x04001F3E RID: 7998
			[ViewVariables]
			public float Efficiency = 1f;

			// Token: 0x04001F3F RID: 7999
			[ViewVariables]
			public float SupplyRampPosition;

			// Token: 0x04001F40 RID: 8000
			[ViewVariables]
			public float CurrentSupply;

			// Token: 0x04001F41 RID: 8001
			[ViewVariables]
			public float CurrentStorage;

			// Token: 0x04001F42 RID: 8002
			[ViewVariables]
			public float CurrentReceiving;

			// Token: 0x04001F43 RID: 8003
			[ViewVariables]
			public float LoadingNetworkDemand;

			// Token: 0x04001F44 RID: 8004
			[ViewVariables]
			[JsonIgnore]
			public bool SupplyingMarked;

			// Token: 0x04001F45 RID: 8005
			[ViewVariables]
			[JsonIgnore]
			public bool LoadingMarked;

			// Token: 0x04001F46 RID: 8006
			[ViewVariables]
			[JsonIgnore]
			public float AvailableSupply;

			// Token: 0x04001F47 RID: 8007
			[ViewVariables]
			[JsonIgnore]
			public float DesiredPower;

			// Token: 0x04001F48 RID: 8008
			[ViewVariables]
			[JsonIgnore]
			public float SupplyRampTarget;

			// Token: 0x04001F49 RID: 8009
			[ViewVariables]
			[JsonIgnore]
			public PowerState.NodeId LinkedNetworkCharging;

			// Token: 0x04001F4A RID: 8010
			[ViewVariables]
			[JsonIgnore]
			public PowerState.NodeId LinkedNetworkDischarging;

			// Token: 0x04001F4B RID: 8011
			[ViewVariables]
			public float MaxEffectiveSupply;
		}

		// Token: 0x02000938 RID: 2360
		[Nullable(0)]
		public sealed class Network
		{
			// Token: 0x04001F4C RID: 8012
			[ViewVariables]
			public PowerState.NodeId Id;

			// Token: 0x04001F4D RID: 8013
			[ViewVariables]
			public List<PowerState.NodeId> Supplies = new List<PowerState.NodeId>();

			// Token: 0x04001F4E RID: 8014
			[ViewVariables]
			public List<PowerState.NodeId> Loads = new List<PowerState.NodeId>();

			// Token: 0x04001F4F RID: 8015
			[ViewVariables]
			public List<PowerState.NodeId> BatteryLoads = new List<PowerState.NodeId>();

			// Token: 0x04001F50 RID: 8016
			[ViewVariables]
			public List<PowerState.NodeId> BatterySupplies = new List<PowerState.NodeId>();

			// Token: 0x04001F51 RID: 8017
			[ViewVariables]
			public float LastCombinedSupply;

			// Token: 0x04001F52 RID: 8018
			[ViewVariables]
			public float LastCombinedMaxSupply;

			// Token: 0x04001F53 RID: 8019
			[ViewVariables]
			[JsonIgnore]
			public int Height;
		}
	}
}
