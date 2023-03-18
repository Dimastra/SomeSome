using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Light.Components
{
	// Token: 0x0200026F RID: 623
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	[Serializable]
	public abstract class LightBehaviourAnimationTrack : AnimationTrackProperty
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x0005F2CC File Offset: 0x0005D4CC
		// (set) Token: 0x06000FCD RID: 4045 RVA: 0x0005F2D4 File Offset: 0x0005D4D4
		[DataField("id", false, 1, false, false, null)]
		public string ID { get; set; } = string.Empty;

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000FCE RID: 4046 RVA: 0x0005F2DD File Offset: 0x0005D4DD
		// (set) Token: 0x06000FCF RID: 4047 RVA: 0x0005F2E5 File Offset: 0x0005D4E5
		[DataField("property", false, 1, false, false, null)]
		public virtual string Property { get; protected set; } = "Radius";

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x0005F2EE File Offset: 0x0005D4EE
		// (set) Token: 0x06000FD1 RID: 4049 RVA: 0x0005F2F6 File Offset: 0x0005D4F6
		[DataField("isLooped", false, 1, false, false, null)]
		public bool IsLooped { get; set; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x0005F2FF File Offset: 0x0005D4FF
		// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x0005F307 File Offset: 0x0005D507
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0005F310 File Offset: 0x0005D510
		// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x0005F318 File Offset: 0x0005D518
		[DataField("startValue", false, 1, false, false, null)]
		public float StartValue { get; set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x0005F321 File Offset: 0x0005D521
		// (set) Token: 0x06000FD7 RID: 4055 RVA: 0x0005F329 File Offset: 0x0005D529
		[DataField("endValue", false, 1, false, false, null)]
		public float EndValue { get; set; } = 2f;

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0005F332 File Offset: 0x0005D532
		// (set) Token: 0x06000FD9 RID: 4057 RVA: 0x0005F33A File Offset: 0x0005D53A
		[DataField("minDuration", false, 1, false, false, null)]
		public float MinDuration { get; set; } = -1f;

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0005F343 File Offset: 0x0005D543
		// (set) Token: 0x06000FDB RID: 4059 RVA: 0x0005F34B File Offset: 0x0005D54B
		[DataField("maxDuration", false, 1, false, false, null)]
		public float MaxDuration { get; set; } = 2f;

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x0005F354 File Offset: 0x0005D554
		// (set) Token: 0x06000FDD RID: 4061 RVA: 0x0005F35C File Offset: 0x0005D55C
		[DataField("interpolate", false, 1, false, false, null)]
		public AnimationInterpolationMode InterpolateMode { get; set; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000FDE RID: 4062 RVA: 0x0005F365 File Offset: 0x0005D565
		// (set) Token: 0x06000FDF RID: 4063 RVA: 0x0005F36D File Offset: 0x0005D56D
		[ViewVariables]
		protected float MaxTime { get; set; }

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0005F378 File Offset: 0x0005D578
		public void Initialize(EntityUid parent, IRobustRandom random, IEntityManager entMan)
		{
			this._random = random;
			this._entMan = entMan;
			this._parent = parent;
			PointLightComponent pointLightComponent;
			if (this.Enabled && this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
			{
				pointLightComponent.Enabled = true;
			}
			this.OnInitialize();
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0005F3C4 File Offset: 0x0005D5C4
		public void UpdatePlaybackValues(Animation owner)
		{
			PointLightComponent pointLightComponent;
			if (this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
			{
				pointLightComponent.Enabled = true;
			}
			if (this.MinDuration > 0f)
			{
				this.MaxTime = (float)this._random.NextDouble() * (this.MaxDuration - this.MinDuration) + this.MinDuration;
			}
			else
			{
				this.MaxTime = this.MaxDuration;
			}
			owner.Length = TimeSpan.FromSeconds((double)this.MaxTime);
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0005F441 File Offset: 0x0005D641
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"KeyFrameIndex",
			"FramePlayingTime"
		})]
		public override ValueTuple<int, float> InitPlayback()
		{
			this.OnStart();
			return new ValueTuple<int, float>(-1, this._maxTime);
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0005F458 File Offset: 0x0005D658
		protected void ApplyProperty(object value)
		{
			if (this.Property == null)
			{
				throw new InvalidOperationException("Property parameter is null! Check the prototype!");
			}
			PointLightComponent pointLightComponent;
			if (this._entMan.TryGetComponent<PointLightComponent>(this._parent, ref pointLightComponent))
			{
				AnimationHelper.SetAnimatableProperty(pointLightComponent, this.Property, value);
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0005F49A File Offset: 0x0005D69A
		protected override void ApplyProperty(object context, object value)
		{
			this.ApplyProperty(value);
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void OnInitialize()
		{
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x0001B008 File Offset: 0x00019208
		public virtual void OnStart()
		{
		}

		// Token: 0x040007D0 RID: 2000
		protected IEntityManager _entMan;

		// Token: 0x040007D1 RID: 2001
		protected IRobustRandom _random;

		// Token: 0x040007DC RID: 2012
		private float _maxTime;

		// Token: 0x040007DD RID: 2013
		private EntityUid _parent;
	}
}
