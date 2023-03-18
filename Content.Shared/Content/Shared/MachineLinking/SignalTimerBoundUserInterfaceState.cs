using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x0200034B RID: 843
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SignalTimerBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060009E3 RID: 2531 RVA: 0x0002068C File Offset: 0x0001E88C
		public string CurrentText { get; }

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060009E4 RID: 2532 RVA: 0x00020694 File Offset: 0x0001E894
		public string CurrentDelayMinutes { get; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060009E5 RID: 2533 RVA: 0x0002069C File Offset: 0x0001E89C
		public string CurrentDelaySeconds { get; }

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060009E6 RID: 2534 RVA: 0x000206A4 File Offset: 0x0001E8A4
		public bool ShowText { get; }

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060009E7 RID: 2535 RVA: 0x000206AC File Offset: 0x0001E8AC
		public TimeSpan TriggerTime { get; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x000206B4 File Offset: 0x0001E8B4
		public bool TimerStarted { get; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060009E9 RID: 2537 RVA: 0x000206BC File Offset: 0x0001E8BC
		public bool? HasAccess { get; }

		// Token: 0x060009EA RID: 2538 RVA: 0x000206C4 File Offset: 0x0001E8C4
		public SignalTimerBoundUserInterfaceState(string currentText, string currentDelayMinutes, string currentDelaySeconds, bool showText, TimeSpan triggerTime, bool timerStarted, bool? hasAccess)
		{
			this.CurrentText = currentText;
			this.CurrentDelayMinutes = currentDelayMinutes;
			this.CurrentDelaySeconds = currentDelaySeconds;
			this.ShowText = showText;
			this.TriggerTime = triggerTime;
			this.TimerStarted = timerStarted;
			this.HasAccess = hasAccess;
		}
	}
}
