using System;
using System.Runtime.CompilerServices;
using Content.Shared.MedicalScanner;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.MedicalScanner
{
	// Token: 0x02000232 RID: 562
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MedicalScannerVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000E6C RID: 3692 RVA: 0x000570C8 File Offset: 0x000552C8
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent component2 = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(component.Owner);
			SharedMedicalScannerComponent.MedicalScannerStatus status;
			if (!component.TryGetData<SharedMedicalScannerComponent.MedicalScannerStatus>(SharedMedicalScannerComponent.MedicalScannerVisuals.Status, ref status))
			{
				return;
			}
			component2.LayerSetState(MedicalScannerVisualizer.MedicalScannerVisualLayers.Machine, this.StatusToMachineStateId(status));
			component2.LayerSetState(MedicalScannerVisualizer.MedicalScannerVisualLayers.Terminal, this.StatusToTerminalStateId(status));
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x00057130 File Offset: 0x00055330
		private string StatusToMachineStateId(SharedMedicalScannerComponent.MedicalScannerStatus status)
		{
			switch (status)
			{
			case SharedMedicalScannerComponent.MedicalScannerStatus.Off:
				return "closed";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Open:
				return "open";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Red:
				return "occupied";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Death:
				return "occupied";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Green:
				return "occupied";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Yellow:
				return "occupied";
			default:
				throw new ArgumentOutOfRangeException("status", status, "unknown MedicalScannerStatus");
			}
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00057198 File Offset: 0x00055398
		private string StatusToTerminalStateId(SharedMedicalScannerComponent.MedicalScannerStatus status)
		{
			switch (status)
			{
			case SharedMedicalScannerComponent.MedicalScannerStatus.Off:
				return "off_unlit";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Open:
				return "idle_unlit";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Red:
				return "red_unlit";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Death:
				return "off_unlit";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Green:
				return "idle_unlit";
			case SharedMedicalScannerComponent.MedicalScannerStatus.Yellow:
				return "maint_unlit";
			default:
				throw new ArgumentOutOfRangeException("status", status, "unknown MedicalScannerStatus");
			}
		}

		// Token: 0x02000233 RID: 563
		[NullableContext(0)]
		public enum MedicalScannerVisualLayers : byte
		{
			// Token: 0x0400072D RID: 1837
			Machine,
			// Token: 0x0400072E RID: 1838
			Terminal
		}
	}
}
