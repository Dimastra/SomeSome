using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Storage.Components
{
	// Token: 0x0200016E RID: 366
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedStorageComponent))]
	public sealed class ServerStorageComponent : SharedStorageComponent
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x00024315 File Offset: 0x00022515
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0002431D File Offset: 0x0002251D
		[DataField("storageOpenSound", false, 1, false, false, null)]
		public SoundSpecifier StorageOpenSound { get; set; } = new SoundCollectionSpecifier("storageRustle", null);

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x00024326 File Offset: 0x00022526
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0002432E File Offset: 0x0002252E
		[DataField("storageInsertSound", false, 1, false, false, null)]
		public SoundSpecifier StorageInsertSound { get; set; } = new SoundCollectionSpecifier("storageRustle", null);

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x00024337 File Offset: 0x00022537
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x0002433F File Offset: 0x0002253F
		[DataField("storageRemoveSound", false, 1, false, false, null)]
		public SoundSpecifier StorageRemoveSound { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00024348 File Offset: 0x00022548
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x00024350 File Offset: 0x00022550
		[DataField("storageCloseSound", false, 1, false, false, null)]
		public SoundSpecifier StorageCloseSound { get; set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600074C RID: 1868 RVA: 0x00024359 File Offset: 0x00022559
		[ViewVariables]
		public override IReadOnlyList<EntityUid> StoredEntities
		{
			get
			{
				Container storage = this.Storage;
				if (storage == null)
				{
					return null;
				}
				return storage.ContainedEntities;
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x0002436C File Offset: 0x0002256C
		// (set) Token: 0x0600074E RID: 1870 RVA: 0x00024374 File Offset: 0x00022574
		[ViewVariables]
		[DataField("occludesLight", false, 1, false, false, null)]
		public bool OccludesLight
		{
			get
			{
				return this._occludesLight;
			}
			set
			{
				this._occludesLight = value;
				if (this.Storage != null)
				{
					this.Storage.OccludesLight = value;
				}
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00024391 File Offset: 0x00022591
		public override bool Remove(EntityUid entity)
		{
			return true;
		}

		// Token: 0x04000448 RID: 1096
		[Nullable(1)]
		public string LoggerName = "Storage";

		// Token: 0x04000449 RID: 1097
		public Container Storage;

		// Token: 0x0400044A RID: 1098
		[Nullable(1)]
		public readonly Dictionary<EntityUid, int> SizeCache = new Dictionary<EntityUid, int>();

		// Token: 0x0400044B RID: 1099
		private bool _occludesLight = true;

		// Token: 0x0400044C RID: 1100
		[DataField("quickInsert", false, 1, false, false, null)]
		public bool QuickInsert;

		// Token: 0x0400044D RID: 1101
		[DataField("clickInsert", false, 1, false, false, null)]
		public bool ClickInsert = true;

		// Token: 0x0400044E RID: 1102
		[DataField("areaInsert", false, 1, false, false, null)]
		public bool AreaInsert;

		// Token: 0x0400044F RID: 1103
		[DataField("areaInsertRadius", false, 1, false, false, null)]
		public int AreaInsertRadius = 1;

		// Token: 0x04000450 RID: 1104
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000451 RID: 1105
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;

		// Token: 0x04000452 RID: 1106
		[DataField("popup", false, 1, false, false, null)]
		public bool ShowPopup = true;

		// Token: 0x04000453 RID: 1107
		public bool IsOpen;

		// Token: 0x04000454 RID: 1108
		public int StorageUsed;

		// Token: 0x04000455 RID: 1109
		[DataField("capacity", false, 1, false, false, null)]
		public int StorageCapacityMax = 10000;
	}
}
