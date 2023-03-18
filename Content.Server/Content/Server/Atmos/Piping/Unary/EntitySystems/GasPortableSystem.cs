using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Construction.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x0200074B RID: 1867
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasPortableSystem : EntitySystem
	{
		// Token: 0x06002736 RID: 10038 RVA: 0x000CEA0D File Offset: 0x000CCC0D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasPortableComponent, AnchorAttemptEvent>(new ComponentEventHandler<GasPortableComponent, AnchorAttemptEvent>(this.OnPortableAnchorAttempt), null, null);
			base.SubscribeLocalEvent<GasPortableComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<GasPortableComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
		}

		// Token: 0x06002737 RID: 10039 RVA: 0x000CEA40 File Offset: 0x000CCC40
		private void OnPortableAnchorAttempt(EntityUid uid, GasPortableComponent component, AnchorAttemptEvent args)
		{
			TransformComponent transform;
			if (!this.EntityManager.TryGetComponent<TransformComponent>(uid, ref transform))
			{
				return;
			}
			GasPortComponent gasPortComponent;
			if (!this.FindGasPortIn(transform.GridUid, transform.Coordinates, out gasPortComponent))
			{
				args.Cancel();
			}
		}

		// Token: 0x06002738 RID: 10040 RVA: 0x000CEA7C File Offset: 0x000CCC7C
		private void OnAnchorChanged(EntityUid uid, GasPortableComponent portable, ref AnchorStateChangedEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PipeNode portableNode;
			if (!nodeContainer.TryGetNode<PipeNode>(portable.PortName, out portableNode))
			{
				return;
			}
			portableNode.ConnectionsEnabled = args.Anchored;
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, GasPortableVisuals.ConnectedState, args.Anchored, appearance);
			}
		}

		// Token: 0x06002739 RID: 10041 RVA: 0x000CEAE8 File Offset: 0x000CCCE8
		[NullableContext(2)]
		public bool FindGasPortIn(EntityUid? gridId, EntityCoordinates coordinates, [NotNullWhen(true)] out GasPortComponent port)
		{
			port = null;
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(gridId, ref grid))
			{
				return false;
			}
			foreach (EntityUid entityUid in grid.GetLocal(coordinates))
			{
				if (this.EntityManager.TryGetComponent<GasPortComponent>(entityUid, ref port))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400186A RID: 6250
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400186B RID: 6251
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
