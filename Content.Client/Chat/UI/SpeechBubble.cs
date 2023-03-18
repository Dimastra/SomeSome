using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Client.Chat.Managers;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Chat.UI
{
	// Token: 0x020003E4 RID: 996
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SpeechBubble : Control
	{
		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06001882 RID: 6274 RVA: 0x0008D72B File Offset: 0x0008B92B
		// (set) Token: 0x06001883 RID: 6275 RVA: 0x0008D733 File Offset: 0x0008B933
		public float VerticalOffset { get; set; }

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06001884 RID: 6276 RVA: 0x0008D73C File Offset: 0x0008B93C
		// (set) Token: 0x06001885 RID: 6277 RVA: 0x0008D744 File Offset: 0x0008B944
		public Vector2 ContentSize { get; private set; }

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06001886 RID: 6278 RVA: 0x0008D750 File Offset: 0x0008B950
		// (remove) Token: 0x06001887 RID: 6279 RVA: 0x0008D788 File Offset: 0x0008B988
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<EntityUid, SpeechBubble> OnDied;

		// Token: 0x06001888 RID: 6280 RVA: 0x0008D7C0 File Offset: 0x0008B9C0
		public static SpeechBubble CreateSpeechBubble(SpeechBubble.SpeechType type, string text, EntityUid senderEntity, IEyeManager eyeManager, IChatManager chatManager, IEntityManager entityManager)
		{
			switch (type)
			{
			case SpeechBubble.SpeechType.Emote:
				return new TextSpeechBubble(text, senderEntity, eyeManager, chatManager, entityManager, "emoteBox");
			case SpeechBubble.SpeechType.Say:
				return new TextSpeechBubble(text, senderEntity, eyeManager, chatManager, entityManager, "sayBox");
			case SpeechBubble.SpeechType.Whisper:
				return new TextSpeechBubble(text, senderEntity, eyeManager, chatManager, entityManager, "whisperBox");
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x0008D81C File Offset: 0x0008BA1C
		public SpeechBubble(string text, EntityUid senderEntity, IEyeManager eyeManager, IChatManager chatManager, IEntityManager entityManager, string speechStyleClass)
		{
			this._chatManager = chatManager;
			this._senderEntity = senderEntity;
			this._eyeManager = eyeManager;
			this._entityManager = entityManager;
			base.RectClipContent = true;
			Control control = this.BuildBubble(text, speechStyleClass);
			base.AddChild(control);
			base.ForceRunStyleUpdate();
			control.Measure(Vector2.Infinity);
			this.ContentSize = control.DesiredSize;
			this._verticalOffsetAchieved = -this.ContentSize.Y;
		}

		// Token: 0x0600188A RID: 6282
		protected abstract Control BuildBubble(string text, string speechStyleClass);

		// Token: 0x0600188B RID: 6283 RVA: 0x0008D8A0 File Offset: 0x0008BAA0
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this._timeLeft -= args.DeltaSeconds;
			if (this._entityManager.Deleted(this._senderEntity) || this._timeLeft <= 0f)
			{
				Timer.Spawn(0, new Action(this.Die), default(CancellationToken));
				return;
			}
			if (MathHelper.CloseToPercent(this._verticalOffsetAchieved - this.VerticalOffset, 0f, 0.1))
			{
				this._verticalOffsetAchieved = this.VerticalOffset;
			}
			else
			{
				this._verticalOffsetAchieved = MathHelper.Lerp(this._verticalOffsetAchieved, this.VerticalOffset, 10f * args.DeltaSeconds);
			}
			TransformComponent transformComponent;
			if (!this._entityManager.TryGetComponent<TransformComponent>(this._senderEntity, ref transformComponent) || transformComponent.MapID != this._eyeManager.CurrentMap)
			{
				base.Modulate = Color.White.WithAlpha(0);
				return;
			}
			if (this._timeLeft <= 0.25f)
			{
				base.Modulate = Color.White.WithAlpha(this._timeLeft / 0.25f);
			}
			else
			{
				base.Modulate = Color.White;
			}
			Vector2 vector = (-this._eyeManager.CurrentEye.Rotation).ToWorldVec() * -0.5f;
			Vector2 vector2 = transformComponent.WorldPosition + vector;
			Vector2 vector3 = this._eyeManager.WorldToScreen(vector2) / this.UIScale;
			Vector2 vector4 = vector3 - new ValueTuple<float, float>(this.ContentSize.X / 2f, this.ContentSize.Y + this._verticalOffsetAchieved);
			vector4 = (vector4 * 2f).Rounded() / 2f;
			LayoutContainer.SetPosition(this, vector4);
			float setHeight = MathF.Ceiling(MathHelper.Clamp(vector3.Y - vector4.Y, 0f, this.ContentSize.Y));
			base.SetHeight = setHeight;
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x0008DAAD File Offset: 0x0008BCAD
		private void Die()
		{
			if (base.Disposed)
			{
				return;
			}
			Action<EntityUid, SpeechBubble> onDied = this.OnDied;
			if (onDied == null)
			{
				return;
			}
			onDied(this._senderEntity, this);
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0008DACF File Offset: 0x0008BCCF
		public void FadeNow()
		{
			if (this._timeLeft > 0.25f)
			{
				this._timeLeft = 0.25f;
			}
		}

		// Token: 0x04000C78 RID: 3192
		private const float TotalTime = 4f;

		// Token: 0x04000C79 RID: 3193
		private const float FadeTime = 0.25f;

		// Token: 0x04000C7A RID: 3194
		private const float EntityVerticalOffset = 0.5f;

		// Token: 0x04000C7B RID: 3195
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000C7C RID: 3196
		private readonly EntityUid _senderEntity;

		// Token: 0x04000C7D RID: 3197
		private readonly IChatManager _chatManager;

		// Token: 0x04000C7E RID: 3198
		private readonly IEntityManager _entityManager;

		// Token: 0x04000C7F RID: 3199
		private float _timeLeft = 4f;

		// Token: 0x04000C81 RID: 3201
		private float _verticalOffsetAchieved;

		// Token: 0x020003E5 RID: 997
		[NullableContext(0)]
		public enum SpeechType : byte
		{
			// Token: 0x04000C85 RID: 3205
			Emote,
			// Token: 0x04000C86 RID: 3206
			Say,
			// Token: 0x04000C87 RID: 3207
			Whisper
		}
	}
}
