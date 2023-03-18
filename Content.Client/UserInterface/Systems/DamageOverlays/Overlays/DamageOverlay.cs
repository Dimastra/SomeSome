using System;
using System.Runtime.CompilerServices;
using Content.Shared.Mobs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.DamageOverlays.Overlays
{
	// Token: 0x0200009E RID: 158
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageOverlay : Overlay
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00016244 File Offset: 0x00014444
		public DamageOverlay()
		{
			IoCManager.InjectDependencies<DamageOverlay>(this);
			this._oxygenShader = this._prototypeManager.Index<ShaderPrototype>("GradientCircleMask").InstanceUnique();
			this._critShader = this._prototypeManager.Index<ShaderPrototype>("GradientCircleMask").InstanceUnique();
			this._bruteShader = this._prototypeManager.Index<ShaderPrototype>("GradientCircleMask").InstanceUnique();
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x000162C4 File Offset: 0x000144C4
		protected override void Draw(in OverlayDrawArgs args)
		{
			IEntityManager entityManager = this._entityManager;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EyeComponent eyeComponent;
			if (!entityManager.TryGetComponent<EyeComponent>((localPlayer != null) ? localPlayer.ControlledEntity : null, ref eyeComponent))
			{
				return;
			}
			if (args.Viewport.Eye != eyeComponent.Eye)
			{
				return;
			}
			Box2 worldAABB = args.WorldAABB;
			DrawingHandleWorld worldHandle = args.WorldHandle;
			int width = args.ViewportBounds.Width;
			float num = (float)this._timing.RealTime.TotalSeconds;
			float lastFrameTime = (float)this._timing.FrameTime.TotalSeconds;
			if (this.State != MobState.Dead)
			{
				this.DeadLevel = 1f;
			}
			else if (!MathHelper.CloseTo(0f, this.DeadLevel, 0.001f))
			{
				float value = -this.DeadLevel;
				this.DeadLevel += this.GetDiff(value, lastFrameTime);
			}
			else
			{
				this.DeadLevel = 0f;
			}
			if (!MathHelper.CloseTo(this._oldBruteLevel, this.BruteLevel, 0.001f))
			{
				float value2 = this.BruteLevel - this._oldBruteLevel;
				this._oldBruteLevel += this.GetDiff(value2, lastFrameTime);
			}
			else
			{
				this._oldBruteLevel = this.BruteLevel;
			}
			if (!MathHelper.CloseTo(this._oldOxygenLevel, this.OxygenLevel, 0.001f))
			{
				float value3 = this.OxygenLevel - this._oldOxygenLevel;
				this._oldOxygenLevel += this.GetDiff(value3, lastFrameTime);
			}
			else
			{
				this._oldOxygenLevel = this.OxygenLevel;
			}
			if (!MathHelper.CloseTo(this._oldCritLevel, this.CritLevel, 0.001f))
			{
				float value4 = this.CritLevel - this._oldCritLevel;
				this._oldCritLevel += this.GetDiff(value4, lastFrameTime);
			}
			else
			{
				this._oldCritLevel = this.CritLevel;
			}
			float num2 = this._oldBruteLevel;
			if (num2 > 0f && this._oldCritLevel <= 0f)
			{
				float num3 = 3f;
				float x = num * num3;
				float num4 = 2f * (float)width;
				float num5 = 0.8f * (float)width;
				float num6 = 0.6f * (float)width;
				float num7 = 0.2f * (float)width;
				float num8 = num4 - num2 * (num4 - num5);
				float num9 = num6 - num2 * (num6 - num7);
				float num10 = MathF.Max(0f, MathF.Sin(x));
				this._bruteShader.SetParameter("time", num10);
				this._bruteShader.SetParameter("color", new Vector3(1f, 0f, 0f));
				this._bruteShader.SetParameter("darknessAlphaOuter", 0.8f);
				this._bruteShader.SetParameter("outerCircleRadius", num8);
				this._bruteShader.SetParameter("outerCircleMaxRadius", num8 + 0.2f * (float)width);
				this._bruteShader.SetParameter("innerCircleRadius", num9);
				this._bruteShader.SetParameter("innerCircleMaxRadius", num9 + 0.02f * (float)width);
				worldHandle.UseShader(this._bruteShader);
				worldHandle.DrawRect(worldAABB, Color.White, true);
			}
			else
			{
				this._oldBruteLevel = this.BruteLevel;
			}
			num2 = ((this.State != MobState.Critical) ? this._oldOxygenLevel : 1f);
			if (num2 > 0f)
			{
				float num11 = 0.6f * (float)width;
				float num12 = 0.06f * (float)width;
				float num13 = 0.02f * (float)width;
				float num14 = 0.02f * (float)width;
				float num15 = num11 - num2 * (num11 - num12);
				float num16 = num13 - num2 * (num13 - num14);
				float num19;
				if (this._oldCritLevel > 0f)
				{
					float num17 = num * 2f;
					float num18 = MathF.Max(0f, MathF.Sin(num17) + 2f * MathF.Sin(2f * num17 / 4f) + MathF.Sin(num17 / 4f) - 3f);
					if (num18 > 0f)
					{
						num19 = 1f - num18 / 1.5f;
					}
					else
					{
						num19 = 1f;
					}
				}
				else
				{
					num19 = MathF.Min(0.98f, 0.3f * MathF.Log(num2) + 1f);
				}
				this._oxygenShader.SetParameter("time", 0f);
				this._oxygenShader.SetParameter("color", new Vector3(0f, 0f, 0f));
				this._oxygenShader.SetParameter("darknessAlphaOuter", num19);
				this._oxygenShader.SetParameter("innerCircleRadius", num16);
				this._oxygenShader.SetParameter("innerCircleMaxRadius", num16);
				this._oxygenShader.SetParameter("outerCircleRadius", num15);
				this._oxygenShader.SetParameter("outerCircleMaxRadius", num15 + 0.2f * (float)width);
				worldHandle.UseShader(this._oxygenShader);
				worldHandle.DrawRect(worldAABB, Color.White, true);
			}
			num2 = ((this.State != MobState.Dead) ? this._oldCritLevel : this.DeadLevel);
			if (num2 > 0f)
			{
				float num20 = 2f * (float)width;
				float num21 = 1f * (float)width;
				float num22 = 0.6f * (float)width;
				float num23 = 0.02f * (float)width;
				float num24 = num20 - num2 * (num20 - num21);
				float num25 = num22 - num2 * (num22 - num23);
				float num26 = MathF.Max(0f, MathF.Sin(num));
				this._critShader.SetParameter("time", num26);
				this._critShader.SetParameter("color", new Vector3(1f, 1f, 1f));
				this._critShader.SetParameter("darknessAlphaOuter", 1f);
				this._critShader.SetParameter("innerCircleRadius", num25);
				this._critShader.SetParameter("innerCircleMaxRadius", num25 + 0.005f * (float)width);
				this._critShader.SetParameter("outerCircleRadius", num24);
				this._critShader.SetParameter("outerCircleMaxRadius", num24 + 0.2f * (float)width);
				worldHandle.UseShader(this._critShader);
				worldHandle.DrawRect(worldAABB, Color.White, true);
			}
			worldHandle.UseShader(null);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000168EC File Offset: 0x00014AEC
		private float GetDiff(float value, float lastFrameTime)
		{
			float num = value * 5f * lastFrameTime;
			if (value < 0f)
			{
				num = Math.Clamp(num, value, -value);
			}
			else
			{
				num = Math.Clamp(num, -value, value);
			}
			return num;
		}

		// Token: 0x040001C7 RID: 455
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040001C8 RID: 456
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001C9 RID: 457
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040001CA RID: 458
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001CB RID: 459
		private readonly ShaderInstance _critShader;

		// Token: 0x040001CC RID: 460
		private readonly ShaderInstance _oxygenShader;

		// Token: 0x040001CD RID: 461
		private readonly ShaderInstance _bruteShader;

		// Token: 0x040001CE RID: 462
		public MobState State = MobState.Alive;

		// Token: 0x040001CF RID: 463
		public float BruteLevel;

		// Token: 0x040001D0 RID: 464
		private float _oldBruteLevel;

		// Token: 0x040001D1 RID: 465
		public float OxygenLevel;

		// Token: 0x040001D2 RID: 466
		private float _oldOxygenLevel;

		// Token: 0x040001D3 RID: 467
		public float CritLevel;

		// Token: 0x040001D4 RID: 468
		private float _oldCritLevel;

		// Token: 0x040001D5 RID: 469
		public float DeadLevel = 1f;
	}
}
