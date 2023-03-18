using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Construction.Components;
using Content.Shared.Construction;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000610 RID: 1552
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class BuildComputer : IGraphAction
	{
		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06002138 RID: 8504 RVA: 0x000ADBF1 File Offset: 0x000ABDF1
		// (set) Token: 0x06002139 RID: 8505 RVA: 0x000ADBF9 File Offset: 0x000ABDF9
		[DataField("container", false, 1, false, false, null)]
		public string Container { get; private set; } = string.Empty;

		// Token: 0x0600213A RID: 8506 RVA: 0x000ADC04 File Offset: 0x000ABE04
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			ContainerManagerComponent containerManager;
			if (!entityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(82, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Computer entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have a container manager! Aborting build computer action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			ContainerSystem containerSystem = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
			IContainer container;
			if (!containerSystem.TryGetContainer(uid, this.Container, ref container, containerManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(89, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Computer entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have the specified '");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build computer action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (container.ContainedEntities.Count != 1)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(109, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Computer entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" did not have exactly one item in the specified '");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("' container! Aborting build computer action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			EntityUid board = container.ContainedEntities[0];
			ComputerBoardComponent boardComponent;
			if (!entityManager.TryGetComponent<ComputerBoardComponent>(board, ref boardComponent))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(87, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Computer entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" had an invalid entity in container \"");
				defaultInterpolatedStringHandler.AppendFormatted(this.Container);
				defaultInterpolatedStringHandler.AppendLiteral("\"! Aborting build computer action.");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			container.Remove(board, null, null, null, true, false, null, null);
			TransformComponent transform = entityManager.GetComponent<TransformComponent>(uid);
			EntityUid computer = entityManager.SpawnEntity(boardComponent.Prototype, transform.Coordinates);
			entityManager.GetComponent<TransformComponent>(computer).LocalRotation = transform.LocalRotation;
			Container computerContainer = containerSystem.EnsureContainer<Container>(computer, this.Container, null);
			foreach (EntityUid ent in computerContainer.ContainedEntities.ToArray<EntityUid>())
			{
				computerContainer.ForceRemove(ent, null, null);
				entityManager.DeleteEntity(ent);
			}
			computerContainer.Insert(board, null, null, null, null, null);
			entityManager.EntitySysManager.GetEntitySystem<ConstructionSystem>().AddContainer(computer, this.Container, null);
			entityManager.DeleteEntity(uid);
		}
	}
}
