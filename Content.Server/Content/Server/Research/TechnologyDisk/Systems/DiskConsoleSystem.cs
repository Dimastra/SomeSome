using System;
using System.Runtime.CompilerServices;
using Content.Server.Research.Systems;
using Content.Server.Research.TechnologyDisk.Components;
using Content.Server.UserInterface;
using Content.Shared.Research;
using Content.Shared.Research.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Research.TechnologyDisk.Systems
{
	// Token: 0x0200023A RID: 570
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiskConsoleSystem : EntitySystem
	{
		// Token: 0x06000B5E RID: 2910 RVA: 0x0003C008 File Offset: 0x0003A208
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DiskConsoleComponent, DiskConsolePrintDiskMessage>(new ComponentEventHandler<DiskConsoleComponent, DiskConsolePrintDiskMessage>(this.OnPrintDisk), null, null);
			base.SubscribeLocalEvent<DiskConsoleComponent, ResearchServerPointsChangedEvent>(new ComponentEventRefHandler<DiskConsoleComponent, ResearchServerPointsChangedEvent>(this.OnPointsChanged), null, null);
			base.SubscribeLocalEvent<DiskConsoleComponent, ResearchRegistrationChangedEvent>(new ComponentEventRefHandler<DiskConsoleComponent, ResearchRegistrationChangedEvent>(this.OnRegistrationChanged), null, null);
			base.SubscribeLocalEvent<DiskConsoleComponent, BeforeActivatableUIOpenEvent>(new ComponentEventHandler<DiskConsoleComponent, BeforeActivatableUIOpenEvent>(this.OnBeforeUiOpen), null, null);
			base.SubscribeLocalEvent<DiskConsolePrintingComponent, ComponentShutdown>(new ComponentEventHandler<DiskConsolePrintingComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0003C07C File Offset: 0x0003A27C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<DiskConsolePrintingComponent, DiskConsoleComponent, TransformComponent> valueTuple in base.EntityQuery<DiskConsolePrintingComponent, DiskConsoleComponent, TransformComponent>(false))
			{
				DiskConsolePrintingComponent printing = valueTuple.Item1;
				DiskConsoleComponent console = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				if (!(printing.FinishTime > this._timing.CurTime))
				{
					base.RemComp(printing.Owner, printing);
					this.EntityManager.SpawnEntity(console.DiskPrototype, xform.Coordinates);
				}
			}
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0003C11C File Offset: 0x0003A31C
		private void OnPrintDisk(EntityUid uid, DiskConsoleComponent component, DiskConsolePrintDiskMessage args)
		{
			if (base.HasComp<DiskConsolePrintingComponent>(uid))
			{
				return;
			}
			EntityUid? server;
			ResearchServerComponent serverComp;
			if (!this._research.TryGetClientServer(uid, out server, out serverComp, null))
			{
				return;
			}
			if (serverComp.Points < component.PricePerDisk)
			{
				return;
			}
			this._research.AddPointsToServer(server.Value, -component.PricePerDisk, serverComp);
			this._audio.PlayPvs(component.PrintSound, uid, null);
			base.EnsureComp<DiskConsolePrintingComponent>(uid).FinishTime = this._timing.CurTime + component.PrintDuration;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x0003C1B6 File Offset: 0x0003A3B6
		private void OnPointsChanged(EntityUid uid, DiskConsoleComponent component, ref ResearchServerPointsChangedEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x0003C1C0 File Offset: 0x0003A3C0
		private void OnRegistrationChanged(EntityUid uid, DiskConsoleComponent component, ref ResearchRegistrationChangedEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x0003C1CA File Offset: 0x0003A3CA
		private void OnBeforeUiOpen(EntityUid uid, DiskConsoleComponent component, BeforeActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x0003C1D4 File Offset: 0x0003A3D4
		[NullableContext(2)]
		public void UpdateUserInterface(EntityUid uid, DiskConsoleComponent component = null)
		{
			if (!base.Resolve<DiskConsoleComponent>(uid, ref component, false))
			{
				return;
			}
			int totalPoints = 0;
			EntityUid? entityUid;
			ResearchServerComponent server;
			if (this._research.TryGetClientServer(uid, out entityUid, out server, null))
			{
				totalPoints = server.Points;
			}
			bool canPrint = !base.HasComp<DiskConsolePrintingComponent>(uid) && totalPoints >= component.PricePerDisk;
			DiskConsoleBoundUserInterfaceState state = new DiskConsoleBoundUserInterfaceState(totalPoints, component.PricePerDisk, canPrint);
			this._ui.TrySetUiState(uid, DiskConsoleUiKey.Key, state, null, null, true);
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x0003C248 File Offset: 0x0003A448
		private void OnShutdown(EntityUid uid, DiskConsolePrintingComponent component, ComponentShutdown args)
		{
			this.UpdateUserInterface(uid, null);
		}

		// Token: 0x04000705 RID: 1797
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000706 RID: 1798
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04000707 RID: 1799
		[Dependency]
		private readonly ResearchSystem _research;

		// Token: 0x04000708 RID: 1800
		[Dependency]
		private readonly UserInterfaceSystem _ui;
	}
}
