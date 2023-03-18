using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Disposal.UI
{
	// Token: 0x02000352 RID: 850
	public sealed class PressureBar : ProgressBar
	{
		// Token: 0x0600151A RID: 5402 RVA: 0x0007C0A0 File Offset: 0x0007A2A0
		public bool UpdatePressure(TimeSpan fullTime)
		{
			TimeSpan curTime = IoCManager.Resolve<IGameTiming>().CurTime;
			float num = (float)Math.Min(1.0, 1.0 - (fullTime.TotalSeconds - curTime.TotalSeconds) * 0.05000000074505806);
			this.UpdatePressureBar(num);
			return num >= 1f;
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x0007C100 File Offset: 0x0007A300
		private void UpdatePressureBar(float pressure)
		{
			this.Value = pressure;
			float num = pressure / base.MaxValue;
			float num2;
			if (num <= 0.5f)
			{
				num /= 0.5f;
				num2 = MathHelper.Lerp(0f, 0.066f, num);
			}
			else
			{
				num = (num - 0.5f) / 0.5f;
				num2 = MathHelper.Lerp(0.066f, 0.33f, num);
			}
			if (base.ForegroundStyleBoxOverride == null)
			{
				base.ForegroundStyleBoxOverride = new StyleBoxFlat();
			}
			((StyleBoxFlat)base.ForegroundStyleBoxOverride).BackgroundColor = Color.FromHsv(new Vector4(num2, 1f, 0.8f, 1f));
		}
	}
}
