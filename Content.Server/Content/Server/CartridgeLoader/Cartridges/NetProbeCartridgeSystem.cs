using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.CartridgeLoader.Cartridges
{
	// Token: 0x020006DD RID: 1757
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetProbeCartridgeSystem : EntitySystem
	{
		// Token: 0x060024B1 RID: 9393 RVA: 0x000BF0AF File Offset: 0x000BD2AF
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NetProbeCartridgeComponent, CartridgeUiReadyEvent>(new ComponentEventHandler<NetProbeCartridgeComponent, CartridgeUiReadyEvent>(this.OnUiReady), null, null);
			base.SubscribeLocalEvent<NetProbeCartridgeComponent, CartridgeAfterInteractEvent>(new ComponentEventHandler<NetProbeCartridgeComponent, CartridgeAfterInteractEvent>(this.AfterInteract), null, null);
		}

		// Token: 0x060024B2 RID: 9394 RVA: 0x000BF0E0 File Offset: 0x000BD2E0
		private void AfterInteract(EntityUid uid, NetProbeCartridgeComponent component, CartridgeAfterInteractEvent args)
		{
			if (args.InteractEvent.Handled || !args.InteractEvent.CanReach || args.InteractEvent.Target == null)
			{
				return;
			}
			EntityUid target = args.InteractEvent.Target.Value;
			DeviceNetworkComponent networkComponent = null;
			if (!base.Resolve<DeviceNetworkComponent>(target, ref networkComponent, false))
			{
				return;
			}
			using (List<ProbedNetworkDevice>.Enumerator enumerator = component.ProbedDevices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Address == networkComponent.Address)
					{
						return;
					}
				}
			}
			AudioParams audioParams = AudioParams.Default.WithVolume(-2f).WithPitchScale((float)this._random.Next(12, 21) / 10f);
			this._audioSystem.PlayEntity(component.SoundScan, args.InteractEvent.User, target, new AudioParams?(audioParams));
			this._popupSystem.PopupCursor(Loc.GetString("net-probe-scan", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("device", target)
			}), args.InteractEvent.User, PopupType.Small);
			if (component.ProbedDevices.Count >= component.MaxSavedDevices)
			{
				component.ProbedDevices.RemoveAt(0);
			}
			string name = base.Name(target, null);
			string address = networkComponent.Address;
			DeviceNetworkComponent deviceNetworkComponent = networkComponent;
			ProbedNetworkDevice device = new ProbedNetworkDevice(name, address, ((deviceNetworkComponent.ReceiveFrequency != null) ? deviceNetworkComponent.ReceiveFrequency.GetValueOrDefault().FrequencyToString() : null) ?? string.Empty, networkComponent.DeviceNetId.DeviceNetIdToLocalizedName());
			component.ProbedDevices.Add(device);
			this.UpdateUiState(uid, args.Loader, component);
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x000BF2A8 File Offset: 0x000BD4A8
		private void OnUiReady(EntityUid uid, NetProbeCartridgeComponent component, CartridgeUiReadyEvent args)
		{
			this.UpdateUiState(uid, args.Loader, component);
		}

		// Token: 0x060024B4 RID: 9396 RVA: 0x000BF2B8 File Offset: 0x000BD4B8
		[NullableContext(2)]
		private void UpdateUiState(EntityUid uid, EntityUid loaderUid, NetProbeCartridgeComponent component)
		{
			if (!base.Resolve<NetProbeCartridgeComponent>(uid, ref component, true))
			{
				return;
			}
			NetProbeUiState state = new NetProbeUiState(component.ProbedDevices);
			CartridgeLoaderSystem cartridgeLoaderSystem = this._cartridgeLoaderSystem;
			if (cartridgeLoaderSystem == null)
			{
				return;
			}
			cartridgeLoaderSystem.UpdateCartridgeUiState(loaderUid, state, null, null);
		}

		// Token: 0x0400168B RID: 5771
		[Nullable(2)]
		[Dependency]
		private readonly CartridgeLoaderSystem _cartridgeLoaderSystem;

		// Token: 0x0400168C RID: 5772
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400168D RID: 5773
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x0400168E RID: 5774
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;
	}
}
