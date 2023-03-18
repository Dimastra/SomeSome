using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dice;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Dice
{
	// Token: 0x02000358 RID: 856
	public sealed class DiceSystem : SharedDiceSystem
	{
		// Token: 0x0600152A RID: 5418 RVA: 0x0007C614 File Offset: 0x0007A814
		[NullableContext(2)]
		protected override void UpdateVisuals(EntityUid uid, DiceComponent die = null)
		{
			SpriteComponent spriteComponent;
			if (!base.Resolve<DiceComponent>(uid, ref die, true) || !base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			string name = spriteComponent.LayerGetState(0).Name;
			if (name == null)
			{
				return;
			}
			string value = name.Substring(0, name.IndexOf('_'));
			SpriteComponent spriteComponent2 = spriteComponent;
			int num = 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted<int>(die.CurrentValue);
			spriteComponent2.LayerSetState(num, defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
