using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x0200068F RID: 1679
	[DataDefinition]
	public sealed class SmokeAreaReactionEffect : AreaReactionEffect
	{
		// Token: 0x060022D3 RID: 8915 RVA: 0x000B5322 File Offset: 0x000B3522
		[NullableContext(2)]
		protected override SolutionAreaEffectComponent GetAreaEffectComponent(EntityUid entity)
		{
			return EntityManagerExt.GetComponentOrNull<SmokeSolutionAreaEffectComponent>(IoCManager.Resolve<IEntityManager>(), entity);
		}
	}
}
