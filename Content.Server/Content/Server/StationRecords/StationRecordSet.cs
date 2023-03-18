using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.StationRecords
{
	// Token: 0x02000179 RID: 377
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationRecordSet
	{
		// Token: 0x06000765 RID: 1893 RVA: 0x00024A34 File Offset: 0x00022C34
		[NullableContext(2)]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<StationRecordKey, T>> GetRecordsOfType<T>()
		{
			if (!this._tables.ContainsKey(typeof(T)))
			{
				yield break;
			}
			foreach (KeyValuePair<StationRecordKey, object> keyValuePair in this._tables[typeof(T)])
			{
				StationRecordKey stationRecordKey;
				object obj;
				keyValuePair.Deconstruct(out stationRecordKey, out obj);
				StationRecordKey key = stationRecordKey;
				object entry = obj;
				if (entry is T)
				{
					T cast = (T)((object)entry);
					this._recentlyAccessed.Add(key);
					yield return new ValueTuple<StationRecordKey, T>(key, cast);
				}
			}
			Dictionary<StationRecordKey, object>.Enumerator enumerator = default(Dictionary<StationRecordKey, object>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00024A44 File Offset: 0x00022C44
		public StationRecordKey AddRecord(EntityUid station)
		{
			uint currentRecordId = this._currentRecordId;
			this._currentRecordId = currentRecordId + 1U;
			StationRecordKey key = new StationRecordKey(currentRecordId, station);
			this._keys.Add(key);
			return key;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00024A78 File Offset: 0x00022C78
		public void AddRecordEntry<[Nullable(2)] T>(StationRecordKey key, T entry)
		{
			if (!this._keys.Contains(key) || entry == null)
			{
				return;
			}
			Dictionary<StationRecordKey, object> table;
			if (!this._tables.TryGetValue(typeof(T), out table))
			{
				table = new Dictionary<StationRecordKey, object>();
				this._tables.Add(typeof(T), table);
			}
			table.Add(key, entry);
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00024AE0 File Offset: 0x00022CE0
		[NullableContext(2)]
		public bool TryGetRecordEntry<T>(StationRecordKey key, [NotNullWhen(true)] out T entry)
		{
			entry = default(T);
			Dictionary<StationRecordKey, object> table;
			object entryObject;
			if (!this._keys.Contains(key) || !this._tables.TryGetValue(typeof(T), out table) || !table.TryGetValue(key, out entryObject))
			{
				return false;
			}
			entry = (T)((object)entryObject);
			this._recentlyAccessed.Add(key);
			return true;
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00024B44 File Offset: 0x00022D44
		[NullableContext(2)]
		public bool HasRecordEntry<T>(StationRecordKey key)
		{
			Dictionary<StationRecordKey, object> table;
			return this._keys.Contains(key) && this._tables.TryGetValue(typeof(T), out table) && table.ContainsKey(key);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00024B81 File Offset: 0x00022D81
		public IEnumerable<StationRecordKey> GetRecentlyAccessed()
		{
			return this._recentlyAccessed.ToArray<StationRecordKey>();
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00024B8E File Offset: 0x00022D8E
		public void ClearRecentlyAccessed()
		{
			this._recentlyAccessed.Clear();
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00024B9C File Offset: 0x00022D9C
		public bool RemoveAllRecords(StationRecordKey key)
		{
			if (!this._keys.Remove(key))
			{
				return false;
			}
			foreach (Dictionary<StationRecordKey, object> dictionary in this._tables.Values)
			{
				dictionary.Remove(key);
			}
			return true;
		}

		// Token: 0x04000477 RID: 1143
		private uint _currentRecordId;

		// Token: 0x04000478 RID: 1144
		private HashSet<StationRecordKey> _keys = new HashSet<StationRecordKey>();

		// Token: 0x04000479 RID: 1145
		private HashSet<StationRecordKey> _recentlyAccessed = new HashSet<StationRecordKey>();

		// Token: 0x0400047A RID: 1146
		[ViewVariables]
		private Dictionary<Type, Dictionary<StationRecordKey, object>> _tables = new Dictionary<Type, Dictionary<StationRecordKey, object>>();
	}
}
