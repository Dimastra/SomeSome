using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x0200068C RID: 1676
	[DataDefinition]
	public sealed class CreateEntityReactionEffect : ReagentEffect
	{
		// Token: 0x060022CA RID: 8906 RVA: 0x000B5164 File Offset: 0x000B3364
		public override void Effect(ReagentEffectArgs args)
		{
			TransformComponent transform = args.EntityManager.GetComponent<TransformComponent>(args.SolutionEntity);
			long quantity = (long)((ulong)this.Number * (ulong)((long)args.Quantity.Int()));
			int i = 0;
			while ((long)i < quantity)
			{
				args.EntityManager.SpawnEntity(this.Entity, transform.MapPosition);
				i++;
			}
		}

		// Token: 0x0400158C RID: 5516
		[Nullable(1)]
		[DataField("entity", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Entity;

		// Token: 0x0400158D RID: 5517
		[DataField("number", false, 1, false, false, null)]
		public uint Number = 1U;
	}
}
