using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000322 RID: 802
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(UtensilSystem)
	})]
	public sealed class UtensilComponent : Component
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x00055453 File Offset: 0x00053653
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x0005545B File Offset: 0x0005365B
		[ViewVariables]
		public UtensilType Types
		{
			get
			{
				return this._types;
			}
			set
			{
				if (this._types.Equals(value))
				{
					return;
				}
				this._types = value;
			}
		}

		// Token: 0x040009B9 RID: 2489
		[DataField("types", false, 1, false, false, null)]
		private UtensilType _types;

		// Token: 0x040009BA RID: 2490
		[DataField("breakChance", false, 1, false, false, null)]
		public float BreakChance;

		// Token: 0x040009BB RID: 2491
		[Nullable(1)]
		[DataField("breakSound", false, 1, false, false, null)]
		public SoundSpecifier BreakSound = new SoundPathSpecifier("/Audio/Items/snap.ogg", null);
	}
}
