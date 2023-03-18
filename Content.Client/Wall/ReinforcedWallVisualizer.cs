using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wall;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Wall
{
	// Token: 0x02000041 RID: 65
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReinforcedWallVisualizer : AppearanceVisualizer
	{
		// Token: 0x0600011C RID: 284 RVA: 0x0000A2E8 File Offset: 0x000084E8
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			int stage;
			if (component.TryGetData<int>(ReinforcedWallVisuals.DeconstructionStage, ref stage))
			{
				this.SetDeconstructionStage(component, stage);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000A314 File Offset: 0x00008514
		public void SetDeconstructionStage(AppearanceComponent component, int stage)
		{
			EntityUid owner = component.Owner;
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(owner, ref spriteComponent))
			{
				return;
			}
			int num = spriteComponent.LayerMapReserveBlank(ReinforcedWallVisualLayers.Deconstruction);
			if (stage < 0)
			{
				spriteComponent.LayerSetVisible(num, false);
				return;
			}
			spriteComponent.LayerSetVisible(num, true);
			SpriteComponent spriteComponent2 = spriteComponent;
			int num2 = num;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
			defaultInterpolatedStringHandler.AppendLiteral("reinf_construct-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(stage);
			spriteComponent2.LayerSetState(num2, defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
