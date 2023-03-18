using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Materials;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Research.Prototypes
{
	// Token: 0x02000202 RID: 514
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Prototype("latheRecipe", 1)]
	[Serializable]
	public sealed class LatheRecipePrototype : IPrototype
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x0001492C File Offset: 0x00012B2C
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x00014934 File Offset: 0x00012B34
		[ViewVariables]
		public string Name
		{
			get
			{
				if (this._name.Trim().Length != 0)
				{
					return this._name;
				}
				EntityPrototype prototype;
				IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(this.Result, ref prototype);
				if (prototype != null && prototype.Name != null)
				{
					this._name = prototype.Name;
				}
				return this._name;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x0001498C File Offset: 0x00012B8C
		[ViewVariables]
		public string Description
		{
			get
			{
				if (this._description.Trim().Length != 0)
				{
					return this._description;
				}
				EntityPrototype prototype;
				IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(this.Result, ref prototype);
				if (prototype != null && prototype.Description != null)
				{
					this._description = prototype.Description;
				}
				return this._description;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x000149E2 File Offset: 0x00012BE2
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x000149EA File Offset: 0x00012BEA
		[ViewVariables]
		public Dictionary<string, int> RequiredMaterials
		{
			get
			{
				return this._requiredMaterials;
			}
			private set
			{
				this._requiredMaterials = value;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x000149F3 File Offset: 0x00012BF3
		[ViewVariables]
		public TimeSpan CompleteTime
		{
			get
			{
				return this._completeTime;
			}
		}

		// Token: 0x040005CF RID: 1487
		[DataField("name", false, 1, false, false, null)]
		private string _name = string.Empty;

		// Token: 0x040005D0 RID: 1488
		[DataField("description", false, 1, false, false, null)]
		private string _description = string.Empty;

		// Token: 0x040005D1 RID: 1489
		[DataField("result", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Result = string.Empty;

		// Token: 0x040005D2 RID: 1490
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon;

		// Token: 0x040005D3 RID: 1491
		[DataField("completetime", false, 1, false, false, null)]
		private TimeSpan _completeTime = TimeSpan.FromSeconds(5.0);

		// Token: 0x040005D4 RID: 1492
		[DataField("materials", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MaterialPrototype>))]
		private Dictionary<string, int> _requiredMaterials = new Dictionary<string, int>();

		// Token: 0x040005D5 RID: 1493
		[DataField("applyMaterialDiscount", false, 1, false, false, null)]
		public bool ApplyMaterialDiscount = true;
	}
}
