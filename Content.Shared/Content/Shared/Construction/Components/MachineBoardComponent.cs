using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058D RID: 1421
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MachineBoardComponent : Component
	{
		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600116C RID: 4460 RVA: 0x00039167 File Offset: 0x00037367
		// (set) Token: 0x0600116D RID: 4461 RVA: 0x0003916F File Offset: 0x0003736F
		[Nullable(2)]
		[ViewVariables]
		[DataField("prototype", false, 1, false, false, null)]
		public string Prototype { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x00039178 File Offset: 0x00037378
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<KeyValuePair<StackPrototype, int>> MaterialRequirements
		{
			[return: Nullable(new byte[]
			{
				1,
				0,
				1
			})]
			get
			{
				foreach (KeyValuePair<string, int> keyValuePair in this.MaterialIdRequirements)
				{
					string text;
					int num;
					keyValuePair.Deconstruct(out text, out num);
					string materialId = text;
					int amount = num;
					StackPrototype material = this._prototypeManager.Index<StackPrototype>(materialId);
					yield return new KeyValuePair<StackPrototype, int>(material, amount);
				}
				Dictionary<string, int>.Enumerator enumerator = default(Dictionary<string, int>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0400100F RID: 4111
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001010 RID: 4112
		[DataField("requirements", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MachinePartPrototype>))]
		public readonly Dictionary<string, int> Requirements = new Dictionary<string, int>();

		// Token: 0x04001011 RID: 4113
		[DataField("materialRequirements", false, 1, false, false, null)]
		public readonly Dictionary<string, int> MaterialIdRequirements = new Dictionary<string, int>();

		// Token: 0x04001012 RID: 4114
		[DataField("tagRequirements", false, 1, false, false, null)]
		public readonly Dictionary<string, GenericPartInfo> TagRequirements = new Dictionary<string, GenericPartInfo>();

		// Token: 0x04001013 RID: 4115
		[DataField("componentRequirements", false, 1, false, false, null)]
		public readonly Dictionary<string, GenericPartInfo> ComponentRequirements = new Dictionary<string, GenericPartInfo>();
	}
}
