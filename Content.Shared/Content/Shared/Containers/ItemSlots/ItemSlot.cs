using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Containers.ItemSlots
{
	// Token: 0x02000564 RID: 1380
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	[Access(new Type[]
	{
		typeof(ItemSlotsSystem)
	})]
	[NetSerializable]
	[Serializable]
	public sealed class ItemSlot
	{
		// Token: 0x060010B0 RID: 4272 RVA: 0x0003648C File Offset: 0x0003468C
		public ItemSlot()
		{
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00036510 File Offset: 0x00034710
		[NullableContext(1)]
		public ItemSlot(ItemSlot other)
		{
			this.CopyFrom(other);
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060010B2 RID: 4274 RVA: 0x00036599 File Offset: 0x00034799
		public string ID
		{
			get
			{
				ContainerSlot containerSlot = this.ContainerSlot;
				if (containerSlot == null)
				{
					return null;
				}
				return containerSlot.ID;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060010B3 RID: 4275 RVA: 0x000365AC File Offset: 0x000347AC
		public bool HasItem
		{
			get
			{
				ContainerSlot containerSlot = this.ContainerSlot;
				return containerSlot != null && containerSlot.ContainedEntity != null;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060010B4 RID: 4276 RVA: 0x000365D4 File Offset: 0x000347D4
		public EntityUid? Item
		{
			get
			{
				ContainerSlot containerSlot = this.ContainerSlot;
				if (containerSlot == null)
				{
					return null;
				}
				return containerSlot.ContainedEntity;
			}
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x000365FC File Offset: 0x000347FC
		[NullableContext(1)]
		public void CopyFrom(ItemSlot other)
		{
			this.Whitelist = other.Whitelist;
			this.InsertSound = other.InsertSound;
			this.EjectSound = other.EjectSound;
			this.SoundOptions = other.SoundOptions;
			this.Name = other.Name;
			this.Locked = other.Locked;
			this.InsertOnInteract = other.InsertOnInteract;
			this.EjectOnInteract = other.EjectOnInteract;
			this.EjectOnUse = other.EjectOnUse;
			this.InsertVerbText = other.InsertVerbText;
			this.EjectVerbText = other.EjectVerbText;
			this.WhitelistFailPopup = other.WhitelistFailPopup;
			this.Swap = other.Swap;
			this.Priority = other.Priority;
		}

		// Token: 0x04000FAF RID: 4015
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000FB0 RID: 4016
		[Nullable(1)]
		[DataField("insertSound", false, 1, false, false, null)]
		public SoundSpecifier InsertSound = new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg", null);

		// Token: 0x04000FB1 RID: 4017
		[Nullable(1)]
		[DataField("ejectSound", false, 1, false, false, null)]
		public SoundSpecifier EjectSound = new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg", null);

		// Token: 0x04000FB2 RID: 4018
		[DataField("soundOptions", false, 1, false, false, null)]
		[Obsolete("Use the sound specifer parameters instead")]
		public AudioParams SoundOptions = AudioParams.Default;

		// Token: 0x04000FB3 RID: 4019
		[Nullable(1)]
		[DataField("name", true, 1, false, false, null)]
		[Access]
		public string Name = string.Empty;

		// Token: 0x04000FB4 RID: 4020
		[DataField("startingItem", true, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		[Access]
		[NonSerialized]
		public string StartingItem;

		// Token: 0x04000FB5 RID: 4021
		[DataField("locked", true, 1, false, false, null)]
		[ViewVariables]
		public bool Locked;

		// Token: 0x04000FB6 RID: 4022
		[DataField("insertOnInteract", false, 1, false, false, null)]
		public bool InsertOnInteract = true;

		// Token: 0x04000FB7 RID: 4023
		[DataField("ejectOnInteract", false, 1, false, false, null)]
		public bool EjectOnInteract;

		// Token: 0x04000FB8 RID: 4024
		[DataField("ejectOnUse", false, 1, false, false, null)]
		public bool EjectOnUse;

		// Token: 0x04000FB9 RID: 4025
		[DataField("insertVerbText", false, 1, false, false, null)]
		public string InsertVerbText;

		// Token: 0x04000FBA RID: 4026
		[DataField("ejectVerbText", false, 1, false, false, null)]
		public string EjectVerbText;

		// Token: 0x04000FBB RID: 4027
		[ViewVariables]
		[NonSerialized]
		public ContainerSlot ContainerSlot;

		// Token: 0x04000FBC RID: 4028
		[DataField("ejectOnDeconstruct", false, 1, false, false, null)]
		[NonSerialized]
		public bool EjectOnDeconstruct = true;

		// Token: 0x04000FBD RID: 4029
		[DataField("ejectOnBreak", false, 1, false, false, null)]
		[NonSerialized]
		public bool EjectOnBreak;

		// Token: 0x04000FBE RID: 4030
		[Nullable(1)]
		[DataField("whitelistFailPopup", false, 1, false, false, null)]
		public string WhitelistFailPopup = string.Empty;

		// Token: 0x04000FBF RID: 4031
		[DataField("swap", false, 1, false, false, null)]
		public bool Swap = true;

		// Token: 0x04000FC0 RID: 4032
		[DataField("priority", false, 1, false, false, null)]
		public int Priority;

		// Token: 0x04000FC1 RID: 4033
		[NonSerialized]
		public bool Local = true;
	}
}
