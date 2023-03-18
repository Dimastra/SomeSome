using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000611 RID: 1553
	[DataDefinition]
	public sealed class BuildMachine : IGraphAction
	{
		// Token: 0x0600213C RID: 8508 RVA: 0x000ADE6C File Offset: 0x000AC06C
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ContainerManagerComponent containerManager;
			if (!entityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(86, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have a container manager! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			MachineFrameComponent machineFrame;
			if (!entityManager.TryGetComponent<MachineFrameComponent>(uid, ref machineFrame))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(92, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have a machine frame component! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!entityManager.EntitySysManager.GetEntitySystem<MachineFrameSystem>().IsComplete(machineFrame))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have all required parts to be built! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			IContainer entBoardContainer;
			if (!containerManager.TryGetContainer("machine_board", ref entBoardContainer))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(83, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have the '");
				defaultInterpolatedStringHandler.AppendFormatted("machine_board");
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			IContainer entPartContainer;
			if (!containerManager.TryGetContainer("machine_parts", ref entPartContainer))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(83, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have the '");
				defaultInterpolatedStringHandler.AppendFormatted("machine_parts");
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (entBoardContainer.ContainedEntities.Count != 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(103, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have exactly one item in the '");
				defaultInterpolatedStringHandler.AppendFormatted("machine_board");
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			EntityUid board = entBoardContainer.ContainedEntities[0];
			MachineBoardComponent boardComponent;
			if (!entityManager.TryGetComponent<MachineBoardComponent>(board, ref boardComponent))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(91, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Machine frame entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" had an invalid entity in container \"");
				defaultInterpolatedStringHandler.AppendFormatted("machine_board");
				defaultInterpolatedStringHandler.AppendLiteral("\"! Aborting build machine action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			entBoardContainer.Remove(board, null, null, null, true, false, null, null);
			ContainerSystem containerSys = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			EntityUid machine = entityManager.SpawnEntity(boardComponent.Prototype, transform.Coordinates);
			entityManager.GetComponent<TransformComponent>(machine).LocalRotation = transform.LocalRotation;
			bool existed;
			Container boardContainer = containerSys.EnsureContainer<Container>(machine, "machine_board", ref existed, null);
			if (existed)
			{
				containerSys.CleanContainer(boardContainer);
			}
			Container partContainer = containerSys.EnsureContainer<Container>(machine, "machine_parts", ref existed, null);
			if (existed)
			{
				containerSys.CleanContainer(partContainer);
			}
			boardContainer.Insert(board, null, null, null, null, null);
			foreach (EntityUid part in entPartContainer.ContainedEntities.ToArray<EntityUid>())
			{
				entPartContainer.ForceRemove(part, null, null);
				partContainer.Insert(part, null, null, null, null, null);
			}
			ConstructionSystem constructionSystem = entityManager.EntitySysManager.GetEntitySystem<ConstructionSystem>();
			ConstructionComponent construction;
			if (entityManager.TryGetComponent<ConstructionComponent>(machine, ref construction))
			{
				constructionSystem.AddContainer(machine, "machine_board", construction);
				constructionSystem.AddContainer(machine, "machine_parts", construction);
			}
			MachineComponent machineComp;
			if (entityManager.TryGetComponent<MachineComponent>(machine, ref machineComp))
			{
				constructionSystem.RefreshParts(machineComp);
			}
			entityManager.DeleteEntity(uid);
		}
	}
}
