using System;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Components;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003F0 RID: 1008
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignallerSystem : EntitySystem
	{
		// Token: 0x06001493 RID: 5267 RVA: 0x0006AE72 File Offset: 0x00069072
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SignallerComponent, ComponentInit>(new ComponentEventHandler<SignallerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SignallerComponent, UseInHandEvent>(new ComponentEventHandler<SignallerComponent, UseInHandEvent>(this.OnUseInHand), null, null);
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x0006AEA2 File Offset: 0x000690A2
		private void OnInit(EntityUid uid, SignallerComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureTransmitterPorts(uid, new string[]
			{
				component.Port
			});
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x0006AEBF File Offset: 0x000690BF
		private void OnUseInHand(EntityUid uid, SignallerComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this._signalSystem.InvokePort(uid, component.Port, null);
			args.Handled = true;
		}

		// Token: 0x04000CC3 RID: 3267
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;
	}
}
