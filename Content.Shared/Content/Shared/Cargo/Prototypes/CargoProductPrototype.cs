using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo.Prototypes
{
	// Token: 0x0200062C RID: 1580
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Prototype("cargoProduct", 1)]
	[Serializable]
	public sealed class CargoProductPrototype : IPrototype
	{
		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x0003FB0D File Offset: 0x0003DD0D
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06001315 RID: 4885 RVA: 0x0003FB18 File Offset: 0x0003DD18
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
				if (IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(this.Product, ref prototype))
				{
					this._name = prototype.Name;
				}
				return this._name;
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0003FB64 File Offset: 0x0003DD64
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
				if (IoCManager.Resolve<IPrototypeManager>().TryIndex<EntityPrototype>(this.Product, ref prototype))
				{
					this._description = prototype.Description;
				}
				return this._description;
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001317 RID: 4887 RVA: 0x0003FBB0 File Offset: 0x0003DDB0
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { get; } = SpriteSpecifier.Invalid;

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001318 RID: 4888 RVA: 0x0003FBB8 File Offset: 0x0003DDB8
		[DataField("product", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Product { get; } = string.Empty;

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x0003FBC0 File Offset: 0x0003DDC0
		[DataField("cost", false, 1, false, false, null)]
		public int PointCost { get; }

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x0600131A RID: 4890 RVA: 0x0003FBC8 File Offset: 0x0003DDC8
		[DataField("category", false, 1, false, false, null)]
		public string Category { get; } = string.Empty;

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x0600131B RID: 4891 RVA: 0x0003FBD0 File Offset: 0x0003DDD0
		[DataField("group", false, 1, false, false, null)]
		public string Group { get; } = string.Empty;

		// Token: 0x04001303 RID: 4867
		[DataField("name", false, 1, false, false, null)]
		private string _name = string.Empty;

		// Token: 0x04001304 RID: 4868
		[DataField("description", false, 1, false, false, null)]
		private string _description = string.Empty;
	}
}
