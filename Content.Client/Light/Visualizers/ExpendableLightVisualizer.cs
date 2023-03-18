using System;
using System.Runtime.CompilerServices;
using Content.Client.Light.Components;
using Content.Shared.Light.Component;
using Robust.Client.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Light.Visualizers
{
	// Token: 0x02000266 RID: 614
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ExpendableLightVisualizer : AppearanceVisualizer
	{
		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0005EAEA File Offset: 0x0005CCEA
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0005EAF2 File Offset: 0x0005CCF2
		[DataField("iconStateSpent", false, 1, false, false, null)]
		public string IconStateSpent { get; set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0005EAFB File Offset: 0x0005CCFB
		// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x0005EB03 File Offset: 0x0005CD03
		[DataField("iconStateOn", false, 1, false, false, null)]
		public string IconStateLit { get; set; }

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0005EB0C File Offset: 0x0005CD0C
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ExpendableLightComponent expendableLightComponent;
			if (!entityManager.TryGetComponent<ExpendableLightComponent>(component.Owner, ref expendableLightComponent))
			{
				return;
			}
			SpriteComponent spriteComponent;
			if (!entityManager.TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			string text;
			LightBehaviourComponent lightBehaviourComponent;
			if (component.TryGetData<string>(ExpendableLightVisuals.Behavior, ref text) && entityManager.TryGetComponent<LightBehaviourComponent>(component.Owner, ref lightBehaviourComponent))
			{
				lightBehaviourComponent.StopLightBehaviour("", false, false);
				PointLightComponent pointLightComponent;
				if (text != string.Empty)
				{
					lightBehaviourComponent.StartLightBehaviour(text);
				}
				else if (entityManager.TryGetComponent<PointLightComponent>(component.Owner, ref pointLightComponent))
				{
					pointLightComponent.Enabled = false;
				}
			}
			ExpendableLightState expendableLightState;
			if (!component.TryGetData<ExpendableLightState>(ExpendableLightVisuals.State, ref expendableLightState))
			{
				return;
			}
			if (expendableLightState == ExpendableLightState.Lit)
			{
				IPlayingAudioStream playingStream = expendableLightComponent.PlayingStream;
				if (playingStream != null)
				{
					playingStream.Stop();
				}
				expendableLightComponent.PlayingStream = entityManager.EntitySysManager.GetEntitySystem<SharedAudioSystem>().PlayPvs(expendableLightComponent.LoopedSound, expendableLightComponent.Owner, new AudioParams?(SharedExpendableLightComponent.LoopedSoundParams));
				if (!string.IsNullOrWhiteSpace(this.IconStateLit))
				{
					spriteComponent.LayerSetState(2, this.IconStateLit);
					spriteComponent.LayerSetShader(2, "shaded");
				}
				spriteComponent.LayerSetVisible(1, true);
				return;
			}
			if (expendableLightState != ExpendableLightState.Dead)
			{
				return;
			}
			IPlayingAudioStream playingStream2 = expendableLightComponent.PlayingStream;
			if (playingStream2 != null)
			{
				playingStream2.Stop();
			}
			if (!string.IsNullOrWhiteSpace(this.IconStateSpent))
			{
				spriteComponent.LayerSetState(0, this.IconStateSpent);
				spriteComponent.LayerSetShader(0, "shaded");
			}
			spriteComponent.LayerSetVisible(1, false);
		}
	}
}
