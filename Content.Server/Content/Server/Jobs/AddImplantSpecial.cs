using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Jobs
{
	// Token: 0x0200043B RID: 1083
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AddImplantSpecial : JobSpecial
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060015F2 RID: 5618 RVA: 0x0007421B File Offset: 0x0007241B
		[DataField("implants", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<EntityPrototype>))]
		public HashSet<string> Implants { get; } = new HashSet<string>();

		// Token: 0x060015F3 RID: 5619 RVA: 0x00074224 File Offset: 0x00072424
		public override void AfterEquip(EntityUid mob)
		{
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			SharedSubdermalImplantSystem implantSystem = entMan.System<SharedSubdermalImplantSystem>();
			TransformComponent xform;
			if (!entMan.GetEntityQuery<TransformComponent>().TryGetComponent(mob, ref xform))
			{
				return;
			}
			foreach (string implantId in this.Implants)
			{
				EntityUid implant = entMan.SpawnEntity(implantId, xform.Coordinates);
				SubdermalImplantComponent implantComp;
				if (!entMan.TryGetComponent<SubdermalImplantComponent>(implant, ref implantComp))
				{
					break;
				}
				implantSystem.ForceImplant(mob, implant, implantComp);
			}
		}
	}
}
