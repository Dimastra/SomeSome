using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000290 RID: 656
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExtensionCableSystem : EntitySystem
	{
		// Token: 0x06000D2F RID: 3375 RVA: 0x00045258 File Offset: 0x00043458
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExtensionCableProviderComponent, ComponentStartup>(new ComponentEventHandler<ExtensionCableProviderComponent, ComponentStartup>(this.OnProviderStarted), null, null);
			base.SubscribeLocalEvent<ExtensionCableProviderComponent, ComponentShutdown>(new ComponentEventHandler<ExtensionCableProviderComponent, ComponentShutdown>(this.OnProviderShutdown), null, null);
			base.SubscribeLocalEvent<ExtensionCableReceiverComponent, ComponentStartup>(new ComponentEventHandler<ExtensionCableReceiverComponent, ComponentStartup>(this.OnReceiverStarted), null, null);
			base.SubscribeLocalEvent<ExtensionCableReceiverComponent, ComponentShutdown>(new ComponentEventHandler<ExtensionCableReceiverComponent, ComponentShutdown>(this.OnReceiverShutdown), null, null);
			base.SubscribeLocalEvent<ExtensionCableReceiverComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ExtensionCableReceiverComponent, AnchorStateChangedEvent>(this.OnReceiverAnchorStateChanged), null, null);
			base.SubscribeLocalEvent<ExtensionCableReceiverComponent, ReAnchorEvent>(new ComponentEventRefHandler<ExtensionCableReceiverComponent, ReAnchorEvent>(this.OnReceiverReAnchor), null, null);
			base.SubscribeLocalEvent<ExtensionCableProviderComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ExtensionCableProviderComponent, AnchorStateChangedEvent>(this.OnProviderAnchorStateChanged), null, null);
			base.SubscribeLocalEvent<ExtensionCableProviderComponent, ReAnchorEvent>(new ComponentEventRefHandler<ExtensionCableProviderComponent, ReAnchorEvent>(this.OnProviderReAnchor), null, null);
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x0004530B File Offset: 0x0004350B
		[NullableContext(2)]
		public void SetProviderTransferRange(EntityUid uid, int range, ExtensionCableProviderComponent provider = null)
		{
			if (!base.Resolve<ExtensionCableProviderComponent>(uid, ref provider, true))
			{
				return;
			}
			provider.TransferRange = range;
			this.ResetReceivers(provider);
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00045328 File Offset: 0x00043528
		private void OnProviderStarted(EntityUid uid, ExtensionCableProviderComponent provider, ComponentStartup args)
		{
			this.Connect(uid, provider);
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00045334 File Offset: 0x00043534
		private void OnProviderShutdown(EntityUid uid, ExtensionCableProviderComponent provider, ComponentShutdown args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (this._mapManager.TryGetGrid(xform.GridUid, ref grid) && base.MetaData(grid.Owner).EntityLifeStage > 3)
			{
				return;
			}
			this.Disconnect(uid, provider);
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x0004537B File Offset: 0x0004357B
		private void OnProviderAnchorStateChanged(EntityUid uid, ExtensionCableProviderComponent provider, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				this.Connect(uid, provider);
				return;
			}
			this.Disconnect(uid, provider);
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00045398 File Offset: 0x00043598
		private void Connect(EntityUid uid, ExtensionCableProviderComponent provider)
		{
			provider.Connectable = true;
			foreach (ExtensionCableReceiverComponent receiver in this.FindAvailableReceivers(uid, (float)provider.TransferRange))
			{
				ExtensionCableProviderComponent provider2 = receiver.Provider;
				if (provider2 != null)
				{
					provider2.LinkedReceivers.Remove(receiver);
				}
				receiver.Provider = provider;
				provider.LinkedReceivers.Add(receiver);
				base.RaiseLocalEvent<ExtensionCableSystem.ProviderConnectedEvent>(receiver.Owner, new ExtensionCableSystem.ProviderConnectedEvent(provider), false);
				base.RaiseLocalEvent<ExtensionCableSystem.ReceiverConnectedEvent>(uid, new ExtensionCableSystem.ReceiverConnectedEvent(receiver), false);
			}
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x0004543C File Offset: 0x0004363C
		private void Disconnect(EntityUid uid, ExtensionCableProviderComponent provider)
		{
			provider.Connectable = false;
			this.ResetReceivers(provider);
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0004544C File Offset: 0x0004364C
		private void OnProviderReAnchor(EntityUid uid, ExtensionCableProviderComponent component, ref ReAnchorEvent args)
		{
			this.Disconnect(uid, component);
			this.Connect(uid, component);
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x00045460 File Offset: 0x00043660
		private void ResetReceivers(ExtensionCableProviderComponent provider)
		{
			ExtensionCableReceiverComponent[] receivers = provider.LinkedReceivers.ToArray();
			provider.LinkedReceivers.Clear();
			foreach (ExtensionCableReceiverComponent receiver in receivers)
			{
				receiver.Provider = null;
				base.RaiseLocalEvent<ExtensionCableSystem.ProviderDisconnectedEvent>(receiver.Owner, new ExtensionCableSystem.ProviderDisconnectedEvent(provider), false);
				base.RaiseLocalEvent<ExtensionCableSystem.ReceiverDisconnectedEvent>(provider.Owner, new ExtensionCableSystem.ReceiverDisconnectedEvent(receiver), false);
			}
			foreach (ExtensionCableReceiverComponent receiver2 in receivers)
			{
				if (!this.EntityManager.IsQueuedForDeletion(receiver2.Owner) && base.MetaData(receiver2.Owner).EntityLifeStage <= 3)
				{
					this.TryFindAndSetProvider(receiver2, null);
				}
			}
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x0004550C File Offset: 0x0004370C
		private IEnumerable<ExtensionCableReceiverComponent> FindAvailableReceivers(EntityUid owner, float range)
		{
			TransformComponent xform = base.Transform(owner);
			EntityCoordinates coordinates = xform.Coordinates;
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				yield break;
			}
			IEnumerable<EntityUid> nearbyEntities = grid.GetCellsInSquareArea(coordinates, (int)Math.Ceiling((double)(range / (float)grid.TileSize)));
			foreach (EntityUid entity in nearbyEntities)
			{
				ExtensionCableReceiverComponent receiver;
				if (!(entity == owner) && !this.EntityManager.IsQueuedForDeletion(entity) && base.MetaData(entity).EntityLifeStage <= 3 && base.TryComp<ExtensionCableReceiverComponent>(entity, ref receiver) && receiver.Connectable && receiver.Provider == null && (base.Transform(entity).LocalPosition - xform.LocalPosition).Length < Math.Min(range, (float)receiver.ReceptionRange))
				{
					yield return receiver;
				}
			}
			IEnumerator<EntityUid> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0004552C File Offset: 0x0004372C
		[NullableContext(2)]
		public void SetReceiverReceptionRange(EntityUid uid, int range, ExtensionCableReceiverComponent receiver = null)
		{
			if (!base.Resolve<ExtensionCableReceiverComponent>(uid, ref receiver, true))
			{
				return;
			}
			ExtensionCableProviderComponent provider = receiver.Provider;
			receiver.Provider = null;
			base.RaiseLocalEvent<ExtensionCableSystem.ProviderDisconnectedEvent>(uid, new ExtensionCableSystem.ProviderDisconnectedEvent(provider), false);
			if (provider != null)
			{
				base.RaiseLocalEvent<ExtensionCableSystem.ReceiverDisconnectedEvent>(provider.Owner, new ExtensionCableSystem.ReceiverDisconnectedEvent(receiver), false);
				provider.LinkedReceivers.Remove(receiver);
			}
			receiver.ReceptionRange = range;
			this.TryFindAndSetProvider(receiver, null);
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x00045594 File Offset: 0x00043794
		private void OnReceiverStarted(EntityUid uid, ExtensionCableReceiverComponent receiver, ComponentStartup args)
		{
			PhysicsComponent physicsComponent;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(receiver.Owner, ref physicsComponent))
			{
				receiver.Connectable = (physicsComponent.BodyType == 4);
			}
			if (receiver.Provider == null)
			{
				this.TryFindAndSetProvider(receiver, null);
			}
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x000455D5 File Offset: 0x000437D5
		private void OnReceiverShutdown(EntityUid uid, ExtensionCableReceiverComponent receiver, ComponentShutdown args)
		{
			this.Disconnect(uid, receiver);
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x000455DF File Offset: 0x000437DF
		private void OnReceiverAnchorStateChanged(EntityUid uid, ExtensionCableReceiverComponent receiver, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				this.Connect(uid, receiver);
				return;
			}
			this.Disconnect(uid, receiver);
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x000455FA File Offset: 0x000437FA
		private void OnReceiverReAnchor(EntityUid uid, ExtensionCableReceiverComponent receiver, ref ReAnchorEvent args)
		{
			this.Disconnect(uid, receiver);
			this.Connect(uid, receiver);
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x0004560C File Offset: 0x0004380C
		private void Connect(EntityUid uid, ExtensionCableReceiverComponent receiver)
		{
			receiver.Connectable = true;
			if (receiver.Provider == null)
			{
				this.TryFindAndSetProvider(receiver, null);
			}
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x00045628 File Offset: 0x00043828
		private void Disconnect(EntityUid uid, ExtensionCableReceiverComponent receiver)
		{
			receiver.Connectable = false;
			base.RaiseLocalEvent<ExtensionCableSystem.ProviderDisconnectedEvent>(uid, new ExtensionCableSystem.ProviderDisconnectedEvent(receiver.Provider), false);
			if (receiver.Provider != null)
			{
				base.RaiseLocalEvent<ExtensionCableSystem.ReceiverDisconnectedEvent>(receiver.Provider.Owner, new ExtensionCableSystem.ReceiverDisconnectedEvent(receiver), false);
				receiver.Provider.LinkedReceivers.Remove(receiver);
			}
			receiver.Provider = null;
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x00045688 File Offset: 0x00043888
		private void TryFindAndSetProvider(ExtensionCableReceiverComponent receiver, [Nullable(2)] TransformComponent xform = null)
		{
			if (!receiver.Connectable)
			{
				return;
			}
			ExtensionCableProviderComponent provider;
			if (!this.TryFindAvailableProvider(receiver.Owner, (float)receiver.ReceptionRange, out provider, xform))
			{
				return;
			}
			receiver.Provider = provider;
			provider.LinkedReceivers.Add(receiver);
			base.RaiseLocalEvent<ExtensionCableSystem.ProviderConnectedEvent>(receiver.Owner, new ExtensionCableSystem.ProviderConnectedEvent(provider), false);
			base.RaiseLocalEvent<ExtensionCableSystem.ReceiverConnectedEvent>(provider.Owner, new ExtensionCableSystem.ReceiverConnectedEvent(receiver), false);
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x000456F0 File Offset: 0x000438F0
		[NullableContext(2)]
		private bool TryFindAvailableProvider(EntityUid owner, float range, [NotNullWhen(true)] out ExtensionCableProviderComponent foundProvider, TransformComponent xform = null)
		{
			MapGridComponent grid;
			if (!base.Resolve<TransformComponent>(owner, ref xform, true) || !this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				foundProvider = null;
				return false;
			}
			EntityCoordinates coordinates = xform.Coordinates;
			foreach (EntityUid entity in grid.GetCellsInSquareArea(coordinates, (int)Math.Ceiling((double)(range / (float)grid.TileSize))))
			{
				ExtensionCableProviderComponent provider;
				if (!(entity == owner) && this.EntityManager.TryGetComponent<ExtensionCableProviderComponent>(entity, ref provider) && !this.EntityManager.IsQueuedForDeletion(entity) && base.MetaData(entity).EntityLifeStage <= 3 && provider.Connectable && (base.Transform(entity).LocalPosition - xform.LocalPosition).Length <= Math.Min(range, (float)provider.TransferRange))
				{
					foundProvider = provider;
					return true;
				}
			}
			foundProvider = null;
			return false;
		}

		// Token: 0x040007F4 RID: 2036
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x02000940 RID: 2368
		[Nullable(0)]
		public sealed class ProviderConnectedEvent : EntityEventArgs
		{
			// Token: 0x060031C2 RID: 12738 RVA: 0x000FFCE5 File Offset: 0x000FDEE5
			public ProviderConnectedEvent(ExtensionCableProviderComponent provider)
			{
				this.Provider = provider;
			}

			// Token: 0x04001F8C RID: 8076
			public ExtensionCableProviderComponent Provider;
		}

		// Token: 0x02000941 RID: 2369
		[NullableContext(2)]
		[Nullable(0)]
		public sealed class ProviderDisconnectedEvent : EntityEventArgs
		{
			// Token: 0x060031C3 RID: 12739 RVA: 0x000FFCF4 File Offset: 0x000FDEF4
			public ProviderDisconnectedEvent(ExtensionCableProviderComponent provider)
			{
				this.Provider = provider;
			}

			// Token: 0x04001F8D RID: 8077
			public ExtensionCableProviderComponent Provider;
		}

		// Token: 0x02000942 RID: 2370
		[Nullable(0)]
		public sealed class ReceiverConnectedEvent : EntityEventArgs
		{
			// Token: 0x060031C4 RID: 12740 RVA: 0x000FFD03 File Offset: 0x000FDF03
			public ReceiverConnectedEvent(ExtensionCableReceiverComponent receiver)
			{
				this.Receiver = receiver;
			}

			// Token: 0x04001F8E RID: 8078
			public ExtensionCableReceiverComponent Receiver;
		}

		// Token: 0x02000943 RID: 2371
		[Nullable(0)]
		public sealed class ReceiverDisconnectedEvent : EntityEventArgs
		{
			// Token: 0x060031C5 RID: 12741 RVA: 0x000FFD12 File Offset: 0x000FDF12
			public ReceiverDisconnectedEvent(ExtensionCableReceiverComponent receiver)
			{
				this.Receiver = receiver;
			}

			// Token: 0x04001F8F RID: 8079
			public ExtensionCableReceiverComponent Receiver;
		}
	}
}
