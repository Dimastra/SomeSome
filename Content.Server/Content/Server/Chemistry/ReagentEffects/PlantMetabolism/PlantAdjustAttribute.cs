using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000677 RID: 1655
	[ImplicitDataDefinitionForInheritors]
	public abstract class PlantAdjustAttribute : ReagentEffect
	{
		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06002298 RID: 8856 RVA: 0x000B46D6 File Offset: 0x000B28D6
		// (set) Token: 0x06002299 RID: 8857 RVA: 0x000B46DE File Offset: 0x000B28DE
		[DataField("amount", false, 1, false, false, null)]
		public float Amount { get; protected set; } = 1f;

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x0600229A RID: 8858 RVA: 0x000B46E7 File Offset: 0x000B28E7
		// (set) Token: 0x0600229B RID: 8859 RVA: 0x000B46EF File Offset: 0x000B28EF
		[DataField("prob", false, 1, false, false, null)]
		public float Prob { get; protected set; } = 1f;

		// Token: 0x0600229C RID: 8860 RVA: 0x000B46F8 File Offset: 0x000B28F8
		[NullableContext(1)]
		public bool CanMetabolize(EntityUid plantHolder, [Nullable(2)] [NotNullWhen(true)] out PlantHolderComponent plantHolderComponent, IEntityManager entityManager, bool mustHaveAlivePlant = true)
		{
			plantHolderComponent = null;
			return entityManager.TryGetComponent<PlantHolderComponent>(plantHolder, ref plantHolderComponent) && (!mustHaveAlivePlant || (plantHolderComponent.Seed != null && !plantHolderComponent.Dead)) && (this.Prob >= 1f || (this.Prob > 0f && RandomExtensions.Prob(IoCManager.Resolve<IRobustRandom>(), this.Prob)));
		}
	}
}
