using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Light.Component;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Components
{
	// Token: 0x02000274 RID: 628
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class LightBehaviourComponent : SharedLightBehaviourComponent, ISerializationHooks
	{
		// Token: 0x06000FFB RID: 4091 RVA: 0x0005FBA4 File Offset: 0x0005DDA4
		void ISerializationHooks.AfterDeserialization()
		{
			int num = 0;
			foreach (LightBehaviourAnimationTrack lightBehaviourAnimationTrack in this.Behaviours)
			{
				Animation animation = new Animation
				{
					AnimationTracks = 
					{
						lightBehaviourAnimationTrack
					}
				};
				this._animations.Add(new LightBehaviourComponent.AnimationContainer(num, animation, lightBehaviourAnimationTrack));
				num++;
			}
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0005FC1C File Offset: 0x0005DE1C
		protected override void Startup()
		{
			base.Startup();
			ComponentExt.EnsureComponent<AnimationPlayerComponent>(base.Owner);
			AnimationPlayerComponent animationPlayerComponent;
			if (this._entMan.TryGetComponent<AnimationPlayerComponent>(base.Owner, ref animationPlayerComponent))
			{
				animationPlayerComponent.AnimationCompleted += this.OnAnimationCompleted;
			}
			foreach (LightBehaviourComponent.AnimationContainer animationContainer in this._animations)
			{
				animationContainer.LightBehaviour.Initialize(base.Owner, this._random, this._entMan);
			}
			foreach (LightBehaviourComponent.AnimationContainer animationContainer2 in this._animations)
			{
				if (animationContainer2.LightBehaviour.Enabled)
				{
					this.StartLightBehaviour(animationContainer2.LightBehaviour.ID);
				}
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x0005FD18 File Offset: 0x0005DF18
		private void OnAnimationCompleted(string key)
		{
			LightBehaviourComponent.AnimationContainer animationContainer = this._animations.FirstOrDefault((LightBehaviourComponent.AnimationContainer x) => x.FullKey == key);
			if (animationContainer == null)
			{
				return;
			}
			if (animationContainer.LightBehaviour.IsLooped)
			{
				animationContainer.LightBehaviour.UpdatePlaybackValues(animationContainer.Animation);
				AnimationPlayerComponent animationPlayerComponent;
				if (this._entMan.TryGetComponent<AnimationPlayerComponent>(base.Owner, ref animationPlayerComponent))
				{
					animationPlayerComponent.Play(animationContainer.Animation, animationContainer.FullKey);
				}
			}
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0005FD94 File Offset: 0x0005DF94
		private void CopyLightSettings(string property)
		{
			PointLightComponent pointLightComponent;
			if (this._entMan.TryGetComponent<PointLightComponent>(base.Owner, ref pointLightComponent))
			{
				object animatableProperty = AnimationHelper.GetAnimatableProperty(pointLightComponent, property);
				if (animatableProperty != null)
				{
					this._originalPropertyValues.Add(property, animatableProperty);
					return;
				}
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
				defaultInterpolatedStringHandler.AppendFormatted(this._entMan.GetComponent<MetaDataComponent>(base.Owner).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" has a ");
				defaultInterpolatedStringHandler.AppendFormatted("LightBehaviourComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" but it has no ");
				defaultInterpolatedStringHandler.AppendFormatted("PointLightComponent");
				defaultInterpolatedStringHandler.AppendLiteral("! Check the prototype!");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x0005FE40 File Offset: 0x0005E040
		public void StartLightBehaviour(string id = "")
		{
			AnimationPlayerComponent animationPlayerComponent;
			if (!this._entMan.TryGetComponent<AnimationPlayerComponent>(base.Owner, ref animationPlayerComponent))
			{
				return;
			}
			foreach (LightBehaviourComponent.AnimationContainer animationContainer in this._animations)
			{
				if ((animationContainer.LightBehaviour.ID == id || id == string.Empty) && !animationPlayerComponent.HasRunningAnimation("LightBehaviourComponent" + animationContainer.Key.ToString()))
				{
					this.CopyLightSettings(animationContainer.LightBehaviour.Property);
					animationContainer.LightBehaviour.UpdatePlaybackValues(animationContainer.Animation);
					animationPlayerComponent.Play(animationContainer.Animation, "LightBehaviourComponent" + animationContainer.Key.ToString());
				}
			}
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x0005FF30 File Offset: 0x0005E130
		public void StopLightBehaviour(string id = "", bool removeBehaviour = false, bool resetToOriginalSettings = false)
		{
			AnimationPlayerComponent animationPlayerComponent;
			if (!this._entMan.TryGetComponent<AnimationPlayerComponent>(base.Owner, ref animationPlayerComponent))
			{
				return;
			}
			List<LightBehaviourComponent.AnimationContainer> list = new List<LightBehaviourComponent.AnimationContainer>();
			foreach (LightBehaviourComponent.AnimationContainer animationContainer in this._animations)
			{
				if (animationContainer.LightBehaviour.ID == id || id == string.Empty)
				{
					if (animationPlayerComponent.HasRunningAnimation("LightBehaviourComponent" + animationContainer.Key.ToString()))
					{
						animationPlayerComponent.Stop("LightBehaviourComponent" + animationContainer.Key.ToString());
					}
					if (removeBehaviour)
					{
						list.Add(animationContainer);
					}
				}
			}
			foreach (LightBehaviourComponent.AnimationContainer item in list)
			{
				this._animations.Remove(item);
			}
			PointLightComponent pointLightComponent;
			if (resetToOriginalSettings && this._entMan.TryGetComponent<PointLightComponent>(base.Owner, ref pointLightComponent))
			{
				foreach (KeyValuePair<string, object> keyValuePair in this._originalPropertyValues)
				{
					string text;
					object obj;
					keyValuePair.Deconstruct(out text, out obj);
					string text2 = text;
					object obj2 = obj;
					AnimationHelper.SetAnimatableProperty(pointLightComponent, text2, obj2);
				}
			}
			this._originalPropertyValues.Clear();
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x000600CC File Offset: 0x0005E2CC
		public bool HasRunningBehaviours()
		{
			AnimationPlayerComponent animation;
			return this._entMan.TryGetComponent<AnimationPlayerComponent>(base.Owner, ref animation) && this._animations.Any((LightBehaviourComponent.AnimationContainer container) => animation.HasRunningAnimation("LightBehaviourComponent" + container.Key.ToString()));
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x00060114 File Offset: 0x0005E314
		public void AddNewLightBehaviour(LightBehaviourAnimationTrack behaviour, bool playImmediately = true)
		{
			int key = 0;
			while (this._animations.Any((LightBehaviourComponent.AnimationContainer x) => x.Key == key))
			{
				int key2 = key;
				key = key2 + 1;
			}
			Animation animation = new Animation
			{
				AnimationTracks = 
				{
					behaviour
				}
			};
			behaviour.Initialize(base.Owner, this._random, this._entMan);
			LightBehaviourComponent.AnimationContainer item = new LightBehaviourComponent.AnimationContainer(key, animation, behaviour);
			this._animations.Add(item);
			if (playImmediately)
			{
				this.StartLightBehaviour(behaviour.ID);
			}
		}

		// Token: 0x040007E6 RID: 2022
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040007E7 RID: 2023
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040007E8 RID: 2024
		private const string KeyPrefix = "LightBehaviourComponent";

		// Token: 0x040007E9 RID: 2025
		[ViewVariables]
		[DataField("behaviours", false, 1, false, false, null)]
		public readonly List<LightBehaviourAnimationTrack> Behaviours = new List<LightBehaviourAnimationTrack>();

		// Token: 0x040007EA RID: 2026
		[ViewVariables]
		private readonly List<LightBehaviourComponent.AnimationContainer> _animations = new List<LightBehaviourComponent.AnimationContainer>();

		// Token: 0x040007EB RID: 2027
		[ViewVariables]
		private Dictionary<string, object> _originalPropertyValues = new Dictionary<string, object>();

		// Token: 0x02000275 RID: 629
		[Nullable(0)]
		public sealed class AnimationContainer
		{
			// Token: 0x06001004 RID: 4100 RVA: 0x000601D5 File Offset: 0x0005E3D5
			public AnimationContainer(int key, Animation animation, LightBehaviourAnimationTrack track)
			{
				this.Key = key;
				this.Animation = animation;
				this.LightBehaviour = track;
			}

			// Token: 0x1700036C RID: 876
			// (get) Token: 0x06001005 RID: 4101 RVA: 0x000601F4 File Offset: 0x0005E3F4
			public string FullKey
			{
				get
				{
					return "LightBehaviourComponent" + this.Key.ToString();
				}
			}

			// Token: 0x1700036D RID: 877
			// (get) Token: 0x06001006 RID: 4102 RVA: 0x00060219 File Offset: 0x0005E419
			// (set) Token: 0x06001007 RID: 4103 RVA: 0x00060221 File Offset: 0x0005E421
			public int Key { get; set; }

			// Token: 0x1700036E RID: 878
			// (get) Token: 0x06001008 RID: 4104 RVA: 0x0006022A File Offset: 0x0005E42A
			// (set) Token: 0x06001009 RID: 4105 RVA: 0x00060232 File Offset: 0x0005E432
			public Animation Animation { get; set; }

			// Token: 0x1700036F RID: 879
			// (get) Token: 0x0600100A RID: 4106 RVA: 0x0006023B File Offset: 0x0005E43B
			// (set) Token: 0x0600100B RID: 4107 RVA: 0x00060243 File Offset: 0x0005E443
			public LightBehaviourAnimationTrack LightBehaviour { get; set; }
		}
	}
}
