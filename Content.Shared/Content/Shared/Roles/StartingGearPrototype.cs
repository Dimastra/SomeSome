using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles
{
	// Token: 0x020001EB RID: 491
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("startingGear", 1)]
	public sealed class StartingGearPrototype : IPrototype
	{
		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x000141F4 File Offset: 0x000123F4
		public IReadOnlyDictionary<string, string> Inhand
		{
			get
			{
				return this._inHand;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x000141FC File Offset: 0x000123FC
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; } = string.Empty;

		// Token: 0x06000587 RID: 1415 RVA: 0x00014204 File Offset: 0x00012404
		public string GetGear(string slot, [Nullable(2)] HumanoidCharacterProfile profile)
		{
			if (profile != null)
			{
				if (slot == "jumpsuit" && profile.Clothing == ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(this._innerClothingSkirt))
				{
					return this._innerClothingSkirt;
				}
				if (slot == "back" && profile.Backpack == BackpackPreference.Satchel && !string.IsNullOrEmpty(this._satchel))
				{
					return this._satchel;
				}
				if (slot == "back" && profile.Backpack == BackpackPreference.Duffelbag && !string.IsNullOrEmpty(this._duffelbag))
				{
					return this._duffelbag;
				}
			}
			string equipment;
			if (!this._equipment.TryGetValue(slot, out equipment))
			{
				return string.Empty;
			}
			return equipment;
		}

		// Token: 0x04000591 RID: 1425
		[DataField("equipment", false, 1, false, false, null)]
		private Dictionary<string, string> _equipment = new Dictionary<string, string>();

		// Token: 0x04000592 RID: 1426
		[DataField("innerclothingskirt", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		private string _innerClothingSkirt = string.Empty;

		// Token: 0x04000593 RID: 1427
		[DataField("satchel", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		private string _satchel = string.Empty;

		// Token: 0x04000594 RID: 1428
		[DataField("duffelbag", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		private string _duffelbag = string.Empty;

		// Token: 0x04000595 RID: 1429
		[DataField("inhand", false, 1, false, false, null)]
		private Dictionary<string, string> _inHand = new Dictionary<string, string>(0);
	}
}
