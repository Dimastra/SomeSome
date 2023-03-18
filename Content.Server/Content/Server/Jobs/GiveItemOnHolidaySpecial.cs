using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Jobs
{
	// Token: 0x0200043C RID: 1084
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class GiveItemOnHolidaySpecial : JobSpecial
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060015F5 RID: 5621 RVA: 0x000742CF File Offset: 0x000724CF
		[DataField("holiday", false, 1, false, false, typeof(PrototypeIdSerializer<HolidayPrototype>))]
		public string Holiday { get; } = string.Empty;

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x000742D7 File Offset: 0x000724D7
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; } = string.Empty;

		// Token: 0x060015F7 RID: 5623 RVA: 0x000742E0 File Offset: 0x000724E0
		public override void AfterEquip(EntityUid mob)
		{
			if (string.IsNullOrEmpty(this.Holiday) || string.IsNullOrEmpty(this.Prototype))
			{
				return;
			}
			IEntitySystemManager sysMan = IoCManager.Resolve<IEntitySystemManager>();
			if (!sysMan.GetEntitySystem<HolidaySystem>().IsCurrentlyHoliday(this.Holiday))
			{
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid entity = entMan.SpawnEntity(this.Prototype, entMan.GetComponent<TransformComponent>(mob).Coordinates);
			sysMan.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(new EntityUid?(mob), entity, true, false, null, null);
		}
	}
}
