using System;
using System.Runtime.CompilerServices;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.CartridgeLoader.Cartridges
{
	// Token: 0x020006DF RID: 1759
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NotekeeperCartridgeSystem : EntitySystem
	{
		// Token: 0x060024B7 RID: 9399 RVA: 0x000BF30D File Offset: 0x000BD50D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NotekeeperCartridgeComponent, CartridgeMessageEvent>(new ComponentEventHandler<NotekeeperCartridgeComponent, CartridgeMessageEvent>(this.OnUiMessage), null, null);
			base.SubscribeLocalEvent<NotekeeperCartridgeComponent, CartridgeUiReadyEvent>(new ComponentEventHandler<NotekeeperCartridgeComponent, CartridgeUiReadyEvent>(this.OnUiReady), null, null);
		}

		// Token: 0x060024B8 RID: 9400 RVA: 0x000BF33D File Offset: 0x000BD53D
		private void OnUiReady(EntityUid uid, NotekeeperCartridgeComponent component, CartridgeUiReadyEvent args)
		{
			this.UpdateUiState(uid, args.Loader, component);
		}

		// Token: 0x060024B9 RID: 9401 RVA: 0x000BF350 File Offset: 0x000BD550
		private void OnUiMessage(EntityUid uid, NotekeeperCartridgeComponent component, CartridgeMessageEvent args)
		{
			NotekeeperUiMessageEvent message = args as NotekeeperUiMessageEvent;
			if (message == null)
			{
				return;
			}
			if (message.Action == NotekeeperUiAction.Add)
			{
				component.Notes.Add(message.Note);
			}
			else
			{
				component.Notes.Remove(message.Note);
			}
			this.UpdateUiState(uid, args.LoaderUid, component);
		}

		// Token: 0x060024BA RID: 9402 RVA: 0x000BF3A4 File Offset: 0x000BD5A4
		[NullableContext(2)]
		private void UpdateUiState(EntityUid uid, EntityUid loaderUid, NotekeeperCartridgeComponent component)
		{
			if (!base.Resolve<NotekeeperCartridgeComponent>(uid, ref component, true))
			{
				return;
			}
			NotekeeperUiState state = new NotekeeperUiState(component.Notes);
			CartridgeLoaderSystem cartridgeLoaderSystem = this._cartridgeLoaderSystem;
			if (cartridgeLoaderSystem == null)
			{
				return;
			}
			cartridgeLoaderSystem.UpdateCartridgeUiState(loaderUid, state, null, null);
		}

		// Token: 0x04001690 RID: 5776
		[Nullable(2)]
		[Dependency]
		private readonly CartridgeLoaderSystem _cartridgeLoaderSystem;
	}
}
