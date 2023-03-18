using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Cooldown
{
	// Token: 0x0200037C RID: 892
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CooldownGraphic : Control
	{
		// Token: 0x060015E7 RID: 5607 RVA: 0x0008171C File Offset: 0x0007F91C
		public CooldownGraphic()
		{
			IoCManager.InjectDependencies<CooldownGraphic>(this);
			this._shader = this._protoMan.Index<ShaderPrototype>("CooldownAnimation").InstanceUnique();
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x060015E8 RID: 5608 RVA: 0x00081746 File Offset: 0x0007F946
		// (set) Token: 0x060015E9 RID: 5609 RVA: 0x0008174E File Offset: 0x0007F94E
		public float Progress { get; set; }

		// Token: 0x060015EA RID: 5610 RVA: 0x00081758 File Offset: 0x0007F958
		protected override void Draw(DrawingHandleScreen handle)
		{
			new float[10];
			float num = 1f - MathF.Abs(this.Progress);
			Color color;
			if (this.Progress >= 0f)
			{
				color = Color.FromHsv(new ValueTuple<float, float, float, float>(0.2777778f * num, 0.75f, 0.75f, 0.5f));
			}
			else
			{
				float num2 = MathHelper.Clamp(0.5f * num, 0f, 0.5f);
				color..ctor(1f, 1f, 1f, num2);
			}
			this._shader.SetParameter("progress", this.Progress);
			handle.UseShader(this._shader);
			handle.DrawRect(base.PixelSizeBox, color, true);
			handle.UseShader(null);
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00081824 File Offset: 0x0007FA24
		public void FromTime(TimeSpan start, TimeSpan end)
		{
			TimeSpan timeSpan = end - start;
			TimeSpan curTime = this._gameTiming.CurTime;
			double totalSeconds = timeSpan.TotalSeconds;
			double num = (curTime - start).TotalSeconds / totalSeconds;
			double num2 = (num <= 1.0) ? (1.0 - num) : ((curTime - end).TotalSeconds * -5.0);
			this.Progress = MathHelper.Clamp((float)num2, -1f, 1f);
			base.Visible = (num2 > -1.0);
		}

		// Token: 0x04000B71 RID: 2929
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000B72 RID: 2930
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000B73 RID: 2931
		private readonly ShaderInstance _shader;
	}
}
