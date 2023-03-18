using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.Store.Components
{
	// Token: 0x0200015A RID: 346
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class StoreComponent : Component
	{
		// Token: 0x040003D1 RID: 977
		[Nullable(2)]
		[DataField("preset", false, 1, false, false, typeof(PrototypeIdSerializer<StorePresetPrototype>))]
		public string Preset;

		// Token: 0x040003D2 RID: 978
		[DataField("categories", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<StoreCategoryPrototype>))]
		public HashSet<string> Categories = new HashSet<string>();

		// Token: 0x040003D3 RID: 979
		[ViewVariables]
		[DataField("balance", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
		public Dictionary<string, FixedPoint2> Balance = new Dictionary<string, FixedPoint2>();

		// Token: 0x040003D4 RID: 980
		[ViewVariables]
		[DataField("currencyWhitelist", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<CurrencyPrototype>))]
		public HashSet<string> CurrencyWhitelist = new HashSet<string>();

		// Token: 0x040003D5 RID: 981
		[ViewVariables]
		public EntityUid? AccountOwner;

		// Token: 0x040003D6 RID: 982
		public HashSet<ListingData> Listings = new HashSet<ListingData>();

		// Token: 0x040003D7 RID: 983
		[ViewVariables]
		public HashSet<ListingData> LastAvailableListings = new HashSet<ListingData>();

		// Token: 0x040003D8 RID: 984
		[DataField("buySuccessSound", false, 1, false, false, null)]
		public SoundSpecifier BuySuccessSound = new SoundPathSpecifier("/Audio/Effects/kaching.ogg", null);
	}
}
