using System;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Components;
using Content.Shared.Audio;
using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003F2 RID: 1010
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalSwitchSystem : EntitySystem
	{
		// Token: 0x060014AF RID: 5295 RVA: 0x0006C46B File Offset: 0x0006A66B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SignalSwitchComponent, ComponentInit>(new ComponentEventHandler<SignalSwitchComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SignalSwitchComponent, ActivateInWorldEvent>(new ComponentEventHandler<SignalSwitchComponent, ActivateInWorldEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0006C49B File Offset: 0x0006A69B
		private void OnInit(EntityUid uid, SignalSwitchComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureTransmitterPorts(uid, new string[]
			{
				component.OnPort,
				component.OffPort
			});
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x0006C4C4 File Offset: 0x0006A6C4
		private void OnActivated(EntityUid uid, SignalSwitchComponent component, ActivateInWorldEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			component.State = !component.State;
			this._signalSystem.InvokePort(uid, component.State ? component.OnPort : component.OffPort, null);
			SoundSystem.Play(component.ClickSound.GetSound(null, null), Filter.Pvs(component.Owner, 2f, null, null, null), component.Owner, new AudioParams?(AudioHelpers.WithVariation(0.125f).WithVolume(8f)));
			args.Handled = true;
		}

		// Token: 0x04000CC8 RID: 3272
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;
	}
}
