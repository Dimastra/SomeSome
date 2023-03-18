using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000685 RID: 1669
	public sealed class HasTag : ReagentEffectCondition
	{
		// Token: 0x060022B8 RID: 8888 RVA: 0x000B4D10 File Offset: 0x000B2F10
		public override bool Condition(ReagentEffectArgs args)
		{
			TagComponent tag;
			return args.EntityManager.TryGetComponent<TagComponent>(args.SolutionEntity, ref tag) && (EntitySystem.Get<TagSystem>().HasTag(tag, this.Tag) ^ this.Invert);
		}

		// Token: 0x04001575 RID: 5493
		[Nullable(1)]
		[DataField("tag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
		public string Tag;

		// Token: 0x04001576 RID: 5494
		[DataField("invert", false, 1, false, false, null)]
		public bool Invert;
	}
}
