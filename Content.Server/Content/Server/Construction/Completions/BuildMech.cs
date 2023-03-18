using System;
using System.Runtime.CompilerServices;
using Content.Server.Mech.Components;
using Content.Server.Mech.Systems;
using Content.Server.Power.Components;
using Content.Shared.Construction;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000612 RID: 1554
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class BuildMech : IGraphAction
	{
		// Token: 0x0600213E RID: 8510 RVA: 0x000AE21C File Offset: 0x000AC41C
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ContainerManagerComponent containerManager;
			if (!entityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(84, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Mech construct entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have a container manager! Aborting build mech action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			SharedContainerSystem entitySystem = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			MechSystem mechSys = entityManager.System<MechSystem>();
			IContainer container;
			if (!entitySystem.TryGetContainer(uid, this.Container, ref container, containerManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(91, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Mech construct entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have the specified '");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build mech action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (container.ContainedEntities.Count != 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(111, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Mech construct entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have exactly one item in the specified '");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build mech action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			EntityUid cell = container.ContainedEntities[0];
			BatteryComponent batteryComponent;
			if (!entityManager.TryGetComponent<BatteryComponent>(cell, ref batteryComponent))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(89, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Mech construct entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" had an invalid entity in container \"");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("\"! Aborting build mech action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			container.Remove(cell, null, null, null, true, false, null, null);
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			EntityUid mech = entityManager.SpawnEntity(this.MechPrototype, transform.Coordinates);
			MechComponent mechComp;
			if (entityManager.TryGetComponent<MechComponent>(mech, ref mechComp) && mechComp.BatterySlot.ContainedEntity == null)
			{
				mechSys.InsertBattery(mech, cell, mechComp, batteryComponent);
				mechComp.BatterySlot.Insert(cell, null, null, null, null, null);
			}
			entityManager.DeleteEntity(uid);
		}

		// Token: 0x04001476 RID: 5238
		[DataField("mechPrototype", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string MechPrototype = string.Empty;

		// Token: 0x04001477 RID: 5239
		[DataField("container", false, 1, false, false, null)]
		public string Container = "battery-container";
	}
}
