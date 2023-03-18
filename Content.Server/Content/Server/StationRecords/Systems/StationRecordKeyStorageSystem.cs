using System;
using System.Runtime.CompilerServices;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;

namespace Content.Server.StationRecords.Systems
{
	// Token: 0x0200017B RID: 379
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class StationRecordKeyStorageSystem : EntitySystem
	{
		// Token: 0x06000773 RID: 1907 RVA: 0x00024E24 File Offset: 0x00023024
		public void AssignKey(EntityUid uid, StationRecordKey key, StationRecordKeyStorageComponent keyStorage = null)
		{
			if (!base.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true))
			{
				return;
			}
			keyStorage.Key = new StationRecordKey?(key);
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00024E40 File Offset: 0x00023040
		public StationRecordKey? RemoveKey(EntityUid uid, StationRecordKeyStorageComponent keyStorage = null)
		{
			if (!base.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true) || keyStorage.Key == null)
			{
				return null;
			}
			StationRecordKey? key = keyStorage.Key;
			keyStorage.Key = null;
			return key;
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00024E82 File Offset: 0x00023082
		public bool CheckKey(EntityUid uid, StationRecordKeyStorageComponent keyStorage = null)
		{
			return base.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true) && keyStorage.Key != null;
		}
	}
}
