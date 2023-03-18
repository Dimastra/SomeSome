using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Overlays
{
	// Token: 0x02000442 RID: 1090
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosDebugOverlay : Overlay
	{
		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06001AFE RID: 6910 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x0009B205 File Offset: 0x00099405
		internal AtmosDebugOverlay(AtmosDebugOverlaySystem system)
		{
			IoCManager.InjectDependencies<AtmosDebugOverlay>(this);
			this._atmosDebugOverlaySystem = system;
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x0009B21C File Offset: 0x0009941C
		protected override void Draw(in OverlayDrawArgs args)
		{
			AtmosDebugOverlay.<>c__DisplayClass6_0 CS$<>8__locals1;
			CS$<>8__locals1.drawHandle = args.WorldHandle;
			MapId mapId = args.Viewport.Eye.Position.MapId;
			Box2Rotated worldBounds = args.WorldBounds;
			foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapId, worldBounds, false))
			{
				TransformComponent transformComponent;
				if (this._atmosDebugOverlaySystem.HasData(mapGridComponent.Owner) && this._entManager.TryGetComponent<TransformComponent>(mapGridComponent.Owner, ref transformComponent))
				{
					DrawingHandleBase drawHandle = CS$<>8__locals1.drawHandle;
					Matrix3 worldMatrix = transformComponent.WorldMatrix;
					drawHandle.SetTransform(ref worldMatrix);
					for (int i = 0; i < 2; i++)
					{
						foreach (TileRef tile in mapGridComponent.GetTilesIntersecting(worldBounds, true, null))
						{
							AtmosDebugOverlay.<>c__DisplayClass6_1 CS$<>8__locals2;
							CS$<>8__locals2.tile = tile;
							SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? data = this._atmosDebugOverlaySystem.GetData(mapGridComponent.Owner, CS$<>8__locals2.tile.GridIndices);
							if (data != null)
							{
								AtmosDebugOverlay.<>c__DisplayClass6_2 CS$<>8__locals3;
								CS$<>8__locals3.data = data.Value;
								if (i == 0)
								{
									float num = 0f;
									switch (this._atmosDebugOverlaySystem.CfgMode)
									{
									case AtmosDebugOverlayMode.TotalMoles:
										foreach (float num2 in CS$<>8__locals3.data.Moles)
										{
											num += num2;
										}
										break;
									case AtmosDebugOverlayMode.GasMoles:
										num = CS$<>8__locals3.data.Moles[this._atmosDebugOverlaySystem.CfgSpecificGas];
										break;
									case AtmosDebugOverlayMode.Temperature:
										num = CS$<>8__locals3.data.Temperature;
										break;
									}
									float num3 = (num - this._atmosDebugOverlaySystem.CfgBase) / this._atmosDebugOverlaySystem.CfgScale;
									Color color;
									if (this._atmosDebugOverlaySystem.CfgCBM)
									{
										color = Color.InterpolateBetween(Color.Black, Color.White, num3);
									}
									else if (num3 < 0.5f)
									{
										color = Color.InterpolateBetween(Color.Red, Color.LimeGreen, num3 * 2f);
									}
									else
									{
										color = Color.InterpolateBetween(Color.LimeGreen, Color.Blue, (num3 - 0.5f) * 2f);
									}
									color = color.WithAlpha(0.75f);
									CS$<>8__locals1.drawHandle.DrawRect(Box2.FromDimensions(new Vector2((float)CS$<>8__locals2.tile.X, (float)CS$<>8__locals2.tile.Y), new Vector2(1f, 1f)), color, true);
								}
								else if (i == 1)
								{
									AtmosDebugOverlay.<Draw>g__CheckAndShowBlockDir|6_0(AtmosDirection.North, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
									AtmosDebugOverlay.<Draw>g__CheckAndShowBlockDir|6_0(AtmosDirection.South, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
									AtmosDebugOverlay.<Draw>g__CheckAndShowBlockDir|6_0(AtmosDirection.East, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
									AtmosDebugOverlay.<Draw>g__CheckAndShowBlockDir|6_0(AtmosDirection.West, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
									if (CS$<>8__locals3.data.PressureDirection != AtmosDirection.Invalid)
									{
										AtmosDebugOverlay.<Draw>g__DrawPressureDirection|6_1(CS$<>8__locals1.drawHandle, CS$<>8__locals3.data.PressureDirection, CS$<>8__locals2.tile, Color.Blue);
									}
									else if (CS$<>8__locals3.data.LastPressureDirection != AtmosDirection.Invalid)
									{
										AtmosDebugOverlay.<Draw>g__DrawPressureDirection|6_1(CS$<>8__locals1.drawHandle, CS$<>8__locals3.data.LastPressureDirection, CS$<>8__locals2.tile, Color.LightGray);
									}
									Vector2 vector;
									vector..ctor((float)CS$<>8__locals2.tile.X, (float)CS$<>8__locals2.tile.Y);
									if (CS$<>8__locals3.data.InExcitedGroup != 0)
									{
										Vector2 vector2 = vector;
										Vector2 vector3 = vector + new Vector2(1f, 1f);
										Vector2 vector4 = vector + new Vector2(0f, 1f);
										Vector2 vector5 = vector + new Vector2(1f, 0f);
										Color color2 = Color.White.WithRed((float)(CS$<>8__locals3.data.InExcitedGroup & 15)).WithGreen((float)((CS$<>8__locals3.data.InExcitedGroup & 240) >> 4)).WithBlue((float)((CS$<>8__locals3.data.InExcitedGroup & 3840) >> 8));
										CS$<>8__locals1.drawHandle.DrawLine(vector2, vector3, color2);
										CS$<>8__locals1.drawHandle.DrawLine(vector4, vector5, color2);
									}
									if (CS$<>8__locals3.data.IsSpace)
									{
										CS$<>8__locals1.drawHandle.DrawCircle(vector + Vector2.One / 2f, 0.125f, Color.Orange, true);
									}
								}
							}
						}
					}
				}
			}
			CS$<>8__locals1.drawHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x0009B6D8 File Offset: 0x000998D8
		[CompilerGenerated]
		internal static void <Draw>g__CheckAndShowBlockDir|6_0(AtmosDirection dir, ref AtmosDebugOverlay.<>c__DisplayClass6_0 A_1, ref AtmosDebugOverlay.<>c__DisplayClass6_1 A_2, ref AtmosDebugOverlay.<>c__DisplayClass6_2 A_3)
		{
			if (A_3.data.BlockDirection.HasFlag(dir))
			{
				Vector2 vector = (dir.ToAngle() - Angle.FromDegrees(90.0)).ToVec() * 0.45f;
				Vector2 vector2;
				vector2..ctor(vector.Y, -vector.X);
				Vector2 vector3 = new Vector2((float)A_2.tile.X + 0.5f, (float)A_2.tile.Y + 0.5f);
				Vector2 vector4 = vector3 + vector - vector2;
				Vector2 vector5 = vector3 + vector + vector2;
				A_1.drawHandle.DrawLine(vector4, vector5, Color.Azure);
			}
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x0009B7A0 File Offset: 0x000999A0
		[CompilerGenerated]
		internal static void <Draw>g__DrawPressureDirection|6_1(DrawingHandleWorld handle, AtmosDirection d, TileRef t, Color color)
		{
			Vector2 vector = (d.ToAngle() - Angle.FromDegrees(90.0)).ToVec() * 0.4f;
			Vector2 vector3;
			Vector2 vector2 = (vector3 = new Vector2((float)t.X + 0.5f, (float)t.Y + 0.5f)) + vector;
			handle.DrawLine(vector3, vector2, color);
		}

		// Token: 0x04000D84 RID: 3460
		private readonly AtmosDebugOverlaySystem _atmosDebugOverlaySystem;

		// Token: 0x04000D85 RID: 3461
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000D86 RID: 3462
		[Dependency]
		private readonly IMapManager _mapManager;
	}
}
