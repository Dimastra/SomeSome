using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005FB RID: 1531
	[NetworkedComponent]
	[ComponentProtoName("Injector")]
	public abstract class SharedInjectorComponent : Component
	{
		// Token: 0x04001157 RID: 4439
		[DataField("isInjecting", false, 1, false, false, null)]
		public bool IsInjecting;

		// Token: 0x02000855 RID: 2133
		[NetSerializable]
		[Serializable]
		public sealed class InjectorComponentState : ComponentState
		{
			// Token: 0x1700052E RID: 1326
			// (get) Token: 0x0600195D RID: 6493 RVA: 0x0004FE5A File Offset: 0x0004E05A
			public FixedPoint2 CurrentVolume { get; }

			// Token: 0x1700052F RID: 1327
			// (get) Token: 0x0600195E RID: 6494 RVA: 0x0004FE62 File Offset: 0x0004E062
			public FixedPoint2 TotalVolume { get; }

			// Token: 0x17000530 RID: 1328
			// (get) Token: 0x0600195F RID: 6495 RVA: 0x0004FE6A File Offset: 0x0004E06A
			public SharedInjectorComponent.InjectorToggleMode CurrentMode { get; }

			// Token: 0x06001960 RID: 6496 RVA: 0x0004FE72 File Offset: 0x0004E072
			public InjectorComponentState(FixedPoint2 currentVolume, FixedPoint2 totalVolume, SharedInjectorComponent.InjectorToggleMode currentMode)
			{
				this.CurrentVolume = currentVolume;
				this.TotalVolume = totalVolume;
				this.CurrentMode = currentMode;
			}
		}

		// Token: 0x02000856 RID: 2134
		public enum InjectorToggleMode : byte
		{
			// Token: 0x04001975 RID: 6517
			Inject,
			// Token: 0x04001976 RID: 6518
			Draw
		}
	}
}
