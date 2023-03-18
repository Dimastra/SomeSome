using System;
using System.Runtime.CompilerServices;
using Content.Shared.Solar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Power
{
	// Token: 0x0200019D RID: 413
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolarControlNotARadar : Control
	{
		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x0003FDC8 File Offset: 0x0003DFC8
		public int SizeFull
		{
			get
			{
				return (int)(290f * this.UIScale);
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0003FDD7 File Offset: 0x0003DFD7
		public int RadiusCircle
		{
			get
			{
				return (int)(140f * this.UIScale);
			}
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x0003FDE8 File Offset: 0x0003DFE8
		public SolarControlNotARadar()
		{
			base.MinSize = new ValueTuple<float, float>((float)this.SizeFull, (float)this.SizeFull);
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x0003FE5D File Offset: 0x0003E05D
		public void UpdateState(SolarControlConsoleBoundInterfaceState ls)
		{
			this._lastState = ls;
			this._lastStateTime = this._gameTiming.CurTime;
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000AFA RID: 2810 RVA: 0x0003FE78 File Offset: 0x0003E078
		public Angle PredictedPanelRotation
		{
			get
			{
				return this._lastState.Rotation + this._lastState.AngularVelocity * (this._gameTiming.CurTime - this._lastStateTime).TotalSeconds;
			}
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0003FECC File Offset: 0x0003E0CC
		protected override void Draw(DrawingHandleScreen handle)
		{
			int num = this.SizeFull / 2;
			Color color;
			color..ctor(0.08f, 0.08f, 0.08f, 1f);
			Color color2;
			color2..ctor(0.08f, 0.08f, 0.08f, 1f);
			int num2 = 4;
			int num3 = 8;
			int num4 = 8;
			handle.DrawCircle(new ValueTuple<float, float>((float)num, (float)num), (float)(this.RadiusCircle + 1), color, true);
			handle.DrawCircle(new ValueTuple<float, float>((float)num, (float)num), (float)this.RadiusCircle, Color.Black, true);
			for (int i = 0; i < num4; i++)
			{
				handle.DrawCircle(new ValueTuple<float, float>((float)num, (float)num), (float)(this.RadiusCircle / num4 * i), color2, false);
			}
			for (int j = 0; j < num3; j++)
			{
				Vector2 vector = (3.141592653589793 / (double)num3 * (double)j).ToVec() * (float)this.RadiusCircle;
				handle.DrawLine(new ValueTuple<float, float>((float)num, (float)num) - vector, new ValueTuple<float, float>((float)num, (float)num) + vector, color2);
			}
			Vector2 vector2 = new ValueTuple<float, float>(1f, -1f);
			Angle angle;
			angle..ctor(-1.5707963267948966);
			Vector2 vector3 = (this.PredictedPanelRotation + angle).ToVec() * vector2 * (float)this.RadiusCircle;
			Vector2 vector4 = new ValueTuple<float, float>(vector3.Y, -vector3.X);
			handle.DrawLine(new ValueTuple<float, float>((float)num, (float)num) - vector4, new ValueTuple<float, float>((float)num, (float)num) + vector4, Color.White);
			handle.DrawLine(new ValueTuple<float, float>((float)num, (float)num) + vector3 / (float)num2, new ValueTuple<float, float>((float)num, (float)num) + vector3 - vector3 / (float)num2, Color.DarkGray);
			Vector2 vector5 = (this._lastState.TowardsSun + angle).ToVec() * vector2 * (float)this.RadiusCircle;
			handle.DrawLine(new ValueTuple<float, float>((float)num, (float)num) + vector5, new ValueTuple<float, float>((float)num, (float)num), Color.Yellow);
		}

		// Token: 0x0400054C RID: 1356
		private IGameTiming _gameTiming = IoCManager.Resolve<IGameTiming>();

		// Token: 0x0400054D RID: 1357
		private SolarControlConsoleBoundInterfaceState _lastState = new SolarControlConsoleBoundInterfaceState(0f, 0f, 0f, 0f);

		// Token: 0x0400054E RID: 1358
		private TimeSpan _lastStateTime = TimeSpan.Zero;

		// Token: 0x0400054F RID: 1359
		public const int StandardSizeFull = 290;

		// Token: 0x04000550 RID: 1360
		public const int StandardRadiusCircle = 140;
	}
}
