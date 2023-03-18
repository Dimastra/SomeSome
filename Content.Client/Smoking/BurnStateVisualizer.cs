using System;
using System.Runtime.CompilerServices;
using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Smoking
{
	// Token: 0x0200013E RID: 318
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BurnStateVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x06000862 RID: 2146 RVA: 0x00030AFA File Offset: 0x0002ECFA
		void ISerializationHooks.AfterDeserialization()
		{
			IoCManager.InjectDependencies<BurnStateVisualizer>(this);
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x00030B04 File Offset: 0x0002ED04
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!this._entMan.TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			SmokableState smokableState;
			if (!component.TryGetData<SmokableState>(SmokingVisuals.Smoking, ref smokableState))
			{
				return;
			}
			string text;
			if (smokableState != SmokableState.Lit)
			{
				if (smokableState != SmokableState.Burnt)
				{
					text = this._unlitIcon;
				}
				else
				{
					text = this._burntIcon;
				}
			}
			else
			{
				text = this._litIcon;
			}
			string text2 = text;
			spriteComponent.LayerSetState(0, text2);
		}

		// Token: 0x0400043B RID: 1083
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400043C RID: 1084
		[DataField("burntIcon", false, 1, false, false, null)]
		private string _burntIcon = "burnt-icon";

		// Token: 0x0400043D RID: 1085
		[DataField("litIcon", false, 1, false, false, null)]
		private string _litIcon = "lit-icon";

		// Token: 0x0400043E RID: 1086
		[DataField("unlitIcon", false, 1, false, false, null)]
		private string _unlitIcon = "icon";
	}
}
