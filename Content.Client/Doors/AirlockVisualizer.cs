using System;
using System.Runtime.CompilerServices;
using Content.Client.Wires.Visualizers;
using Content.Shared.Doors.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Client.Doors
{
	// Token: 0x02000344 RID: 836
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirlockVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x060014C5 RID: 5317 RVA: 0x00079958 File Offset: 0x00077B58
		void ISerializationHooks.AfterDeserialization()
		{
			IoCManager.InjectDependencies<AirlockVisualizer>(this);
			this.CloseAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)this._delay)
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick = new AnimationTrackSpriteFlick();
			this.CloseAnimation.AnimationTracks.Add(animationTrackSpriteFlick);
			animationTrackSpriteFlick.LayerKey = DoorVisualLayers.Base;
			animationTrackSpriteFlick.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("closing", 0f));
			if (!this._simpleVisuals)
			{
				AnimationTrackSpriteFlick animationTrackSpriteFlick2 = new AnimationTrackSpriteFlick();
				this.CloseAnimation.AnimationTracks.Add(animationTrackSpriteFlick2);
				animationTrackSpriteFlick2.LayerKey = DoorVisualLayers.BaseUnlit;
				animationTrackSpriteFlick2.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("closing_unlit", 0f));
				if (this._animatedPanel)
				{
					AnimationTrackSpriteFlick animationTrackSpriteFlick3 = new AnimationTrackSpriteFlick();
					this.CloseAnimation.AnimationTracks.Add(animationTrackSpriteFlick3);
					animationTrackSpriteFlick3.LayerKey = WiresVisualLayers.MaintenancePanel;
					animationTrackSpriteFlick3.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("panel_closing", 0f));
				}
			}
			this.OpenAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)this._delay)
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick4 = new AnimationTrackSpriteFlick();
			this.OpenAnimation.AnimationTracks.Add(animationTrackSpriteFlick4);
			animationTrackSpriteFlick4.LayerKey = DoorVisualLayers.Base;
			animationTrackSpriteFlick4.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("opening", 0f));
			if (!this._simpleVisuals)
			{
				AnimationTrackSpriteFlick animationTrackSpriteFlick5 = new AnimationTrackSpriteFlick();
				this.OpenAnimation.AnimationTracks.Add(animationTrackSpriteFlick5);
				animationTrackSpriteFlick5.LayerKey = DoorVisualLayers.BaseUnlit;
				animationTrackSpriteFlick5.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("opening_unlit", 0f));
				if (this._animatedPanel)
				{
					AnimationTrackSpriteFlick animationTrackSpriteFlick6 = new AnimationTrackSpriteFlick();
					this.OpenAnimation.AnimationTracks.Add(animationTrackSpriteFlick6);
					animationTrackSpriteFlick6.LayerKey = WiresVisualLayers.MaintenancePanel;
					animationTrackSpriteFlick6.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("panel_opening", 0f));
				}
			}
			this.EmaggingAnimation = new Animation
			{
				Length = TimeSpan.FromSeconds((double)this._delay)
			};
			AnimationTrackSpriteFlick animationTrackSpriteFlick7 = new AnimationTrackSpriteFlick();
			this.EmaggingAnimation.AnimationTracks.Add(animationTrackSpriteFlick7);
			animationTrackSpriteFlick7.LayerKey = DoorVisualLayers.BaseUnlit;
			animationTrackSpriteFlick7.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("sparks", 0f));
			if (!this._simpleVisuals)
			{
				this.DenyAnimation = new Animation
				{
					Length = TimeSpan.FromSeconds((double)this._denyDelay)
				};
				AnimationTrackSpriteFlick animationTrackSpriteFlick8 = new AnimationTrackSpriteFlick();
				this.DenyAnimation.AnimationTracks.Add(animationTrackSpriteFlick8);
				animationTrackSpriteFlick8.LayerKey = DoorVisualLayers.BaseUnlit;
				animationTrackSpriteFlick8.KeyFrames.Add(new AnimationTrackSpriteFlick.KeyFrame("deny_unlit", 0f));
			}
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x00079C2E File Offset: 0x00077E2E
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			if (!this._entMan.HasComponent<AnimationPlayerComponent>(entity))
			{
				this._entMan.AddComponent<AnimationPlayerComponent>(entity);
			}
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00079C4C File Offset: 0x00077E4C
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			base.OnChangeData(component);
			SpriteComponent component2 = this._entMan.GetComponent<SpriteComponent>(component.Owner);
			AnimationPlayerComponent component3 = this._entMan.GetComponent<AnimationPlayerComponent>(component.Owner);
			DoorState doorState;
			if (!component.TryGetData<DoorState>(DoorVisuals.State, ref doorState))
			{
				doorState = DoorState.Closed;
			}
			DoorComponent component4 = this._entMan.GetComponent<DoorComponent>(component.Owner);
			string text;
			if (component.TryGetData<string>(DoorVisuals.BaseRSI, ref text))
			{
				RSIResource rsiresource;
				if (!this._resourceCache.TryGetResource<RSIResource>(SharedSpriteComponent.TextureRoot / text, ref rsiresource))
				{
					Logger.Error("Unable to load RSI '{0}'. Trace:\n{1}", new object[]
					{
						text,
						Environment.StackTrace
					});
				}
				foreach (ISpriteLayer spriteLayer in component2.AllLayers)
				{
					spriteLayer.Rsi = ((rsiresource != null) ? rsiresource.RSI : null);
				}
			}
			if (component3.HasRunningAnimation("airlock_animation"))
			{
				component3.Stop("airlock_animation");
			}
			switch (doorState)
			{
			case DoorState.Closed:
				component2.LayerSetState(DoorVisualLayers.Base, "closed");
				if (!this._simpleVisuals)
				{
					component2.LayerSetState(DoorVisualLayers.BaseUnlit, "closed_unlit");
					component2.LayerSetState(DoorVisualLayers.BaseBolted, "bolted_unlit");
					component2.LayerSetState(DoorVisualLayers.BaseEmergencyAccess, "emergency_unlit");
				}
				break;
			case DoorState.Closing:
				if (component4.CurrentlyCrushing.Count == 0)
				{
					component3.Play(this.CloseAnimation, "airlock_animation");
				}
				else
				{
					component2.LayerSetState(DoorVisualLayers.Base, "closed");
				}
				break;
			case DoorState.Open:
				component2.LayerSetState(DoorVisualLayers.Base, "open");
				if (this._openUnlitVisible && !this._simpleVisuals)
				{
					component2.LayerSetState(DoorVisualLayers.BaseUnlit, "open_unlit");
					component2.LayerSetState(DoorVisualLayers.BaseBolted, "bolted_open_unlit");
					component2.LayerSetState(DoorVisualLayers.BaseEmergencyAccess, "emergency_open_unlit");
				}
				break;
			case DoorState.Opening:
				component3.Play(this.OpenAnimation, "airlock_animation");
				break;
			case DoorState.Welded:
				break;
			case DoorState.Denying:
				component3.Play(this.DenyAnimation, "airlock_animation");
				break;
			case DoorState.Emagging:
				component3.Play(this.EmaggingAnimation, "airlock_animation");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (this._simpleVisuals)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = true;
			bool flag5;
			if (component.TryGetData<bool>(DoorVisuals.Powered, ref flag5) && flag5)
			{
				bool flag6;
				flag = (component.TryGetData<bool>(DoorVisuals.BoltLights, ref flag6) && flag6);
				bool flag7;
				flag2 = (component.TryGetData<bool>(DoorVisuals.EmergencyLights, ref flag7) && flag7);
				bool flag8;
				flag3 = (doorState == DoorState.Closing || doorState == DoorState.Opening || doorState == DoorState.Denying || (doorState == DoorState.Open && this._openUnlitVisible) || (component.TryGetData<bool>(DoorVisuals.ClosedLights, ref flag8) && flag8));
				if ((doorState == DoorState.Open || doorState == DoorState.Closed) && (flag2 || flag))
				{
					flag4 = false;
				}
			}
			component2.LayerSetVisible(DoorVisualLayers.BaseUnlit, flag3 && flag4);
			component2.LayerSetVisible(DoorVisualLayers.BaseBolted, flag3 && flag);
			if (this._emergencyAccessLayer)
			{
				component2.LayerSetVisible(DoorVisualLayers.BaseEmergencyAccess, flag3 && flag2 && !flag && doorState != DoorState.Opening && doorState != DoorState.Closing);
			}
		}

		// Token: 0x04000AC2 RID: 2754
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000AC3 RID: 2755
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000AC4 RID: 2756
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000AC5 RID: 2757
		private const string AnimationKey = "airlock_animation";

		// Token: 0x04000AC6 RID: 2758
		[DataField("animationTime", false, 1, false, false, null)]
		private float _delay = 0.8f;

		// Token: 0x04000AC7 RID: 2759
		[DataField("denyAnimationTime", false, 1, false, false, null)]
		private float _denyDelay = 0.3f;

		// Token: 0x04000AC8 RID: 2760
		[DataField("emagAnimationTime", false, 1, false, false, null)]
		private float _delayEmag = 1.5f;

		// Token: 0x04000AC9 RID: 2761
		[DataField("animatedPanel", false, 1, false, false, null)]
		private bool _animatedPanel = true;

		// Token: 0x04000ACA RID: 2762
		[DataField("simpleVisuals", false, 1, false, false, null)]
		private bool _simpleVisuals;

		// Token: 0x04000ACB RID: 2763
		[DataField("openUnlitVisible", false, 1, false, false, null)]
		private bool _openUnlitVisible;

		// Token: 0x04000ACC RID: 2764
		[DataField("emergencyAccessLayer", false, 1, false, false, null)]
		private bool _emergencyAccessLayer = true;

		// Token: 0x04000ACD RID: 2765
		private Animation CloseAnimation;

		// Token: 0x04000ACE RID: 2766
		private Animation OpenAnimation;

		// Token: 0x04000ACF RID: 2767
		private Animation DenyAnimation;

		// Token: 0x04000AD0 RID: 2768
		private Animation EmaggingAnimation;
	}
}
