using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Shuttles.UI
{
	// Token: 0x0200014E RID: 334
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadarControl : Control
	{
		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x00032FB7 File Offset: 0x000311B7
		// (set) Token: 0x060008B9 RID: 2233 RVA: 0x00032FBF File Offset: 0x000311BF
		public float RadarRange { get; private set; } = 256f;

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x00032FC8 File Offset: 0x000311C8
		// (set) Token: 0x060008BB RID: 2235 RVA: 0x00032FD0 File Offset: 0x000311D0
		public float MaxRadarRange { get; private set; } = 2560f;

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x0003140D File Offset: 0x0002F60D
		private int MinimapRadius
		{
			get
			{
				return (int)Math.Min(base.Size.X, base.Size.Y) / 2;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x0003142D File Offset: 0x0002F62D
		private Vector2 MidPoint
		{
			get
			{
				return base.Size / 2f * this.UIScale;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x00032FD9 File Offset: 0x000311D9
		private int SizeFull
		{
			get
			{
				return (int)((float)(this.MinimapRadius * 2) * this.UIScale);
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x00032FEC File Offset: 0x000311EC
		private int ScaledMinimapRadius
		{
			get
			{
				return (int)((float)this.MinimapRadius * this.UIScale);
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060008C0 RID: 2240 RVA: 0x00032FFD File Offset: 0x000311FD
		private float MinimapScale
		{
			get
			{
				if (this.RadarRange == 0f)
				{
					return 0f;
				}
				return (float)this.ScaledMinimapRadius / this.RadarRange;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060008C1 RID: 2241 RVA: 0x00033020 File Offset: 0x00031220
		// (set) Token: 0x060008C2 RID: 2242 RVA: 0x00033028 File Offset: 0x00031228
		public bool ShowIFF { get; set; } = true;

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x00033031 File Offset: 0x00031231
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x00033039 File Offset: 0x00031239
		public bool ShowDocks { get; set; } = true;

		// Token: 0x060008C5 RID: 2245 RVA: 0x00033044 File Offset: 0x00031244
		public RadarControl()
		{
			IoCManager.InjectDependencies<RadarControl>(this);
			base.MinSize = new ValueTuple<float, float>((float)this.SizeFull, (float)this.SizeFull);
			base.RectClipContent = true;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000330DE File Offset: 0x000312DE
		public void SetMatrix(EntityCoordinates? coordinates, Angle? angle)
		{
			this._coordinates = coordinates;
			this._rotation = angle;
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x000330F0 File Offset: 0x000312F0
		public void UpdateState(RadarConsoleBoundInterfaceState ls)
		{
			this._radarMaxRange = ls.MaxRange;
			if (this._radarMaxRange < this.RadarRange)
			{
				this._actualRadarRange = this._radarMaxRange;
			}
			if (this._radarMaxRange < this._radarMinRange)
			{
				this._radarMinRange = this._radarMaxRange;
			}
			this._docks.Clear();
			foreach (DockingInterfaceState dockingInterfaceState in ls.Docks)
			{
				EntityCoordinates coordinates = dockingInterfaceState.Coordinates;
				Extensions.GetOrNew<EntityUid, List<DockingInterfaceState>>(this._docks, coordinates.EntityId).Add(dockingInterfaceState);
			}
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x000331A8 File Offset: 0x000313A8
		protected override void MouseWheel(GUIMouseWheelEventArgs args)
		{
			base.MouseWheel(args);
			this.AddRadarRange(-args.Delta.Y * 1f / 8f * this.RadarRange);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x000331D6 File Offset: 0x000313D6
		public void AddRadarRange(float value)
		{
			this._actualRadarRange = Math.Clamp(this._actualRadarRange + value, this._radarMinRange, this._radarMaxRange);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000331F8 File Offset: 0x000313F8
		protected override void Draw(DrawingHandleScreen handle)
		{
			if (!this._actualRadarRange.Equals(this.RadarRange))
			{
				float num = this._actualRadarRange - this.RadarRange;
				float num2 = 10f;
				this.RadarRange += (float)Math.Clamp((double)num, (double)(-(double)num2 * MathF.Abs(num)) * this._timing.FrameTime.TotalSeconds, (double)(num2 * MathF.Abs(num)) * this._timing.FrameTime.TotalSeconds);
				Action<float> onRadarRangeChanged = this.OnRadarRangeChanged;
				if (onRadarRangeChanged != null)
				{
					onRadarRangeChanged(this.RadarRange);
				}
			}
			Color color;
			color..ctor(0.08f, 0.08f, 0.08f, 1f);
			handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), (float)(this.ScaledMinimapRadius + 1), color, true);
			handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), (float)this.ScaledMinimapRadius, Color.Black, true);
			if (this._coordinates == null || this._rotation == null)
			{
				this.Clear();
				return;
			}
			Color color2;
			color2..ctor(0.08f, 0.08f, 0.08f, 1f);
			int num3 = 8;
			int num4 = (int)Math.Floor((double)(this.RadarRange / 32f));
			for (int i = 1; i < num4 + 1; i++)
			{
				handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), 32f * this.MinimapScale * (float)i, color2, false);
			}
			for (int j = 0; j < num3; j++)
			{
				Vector2 vector = (3.141592653589793 / (double)num3 * (double)j).ToVec() * (float)this.ScaledMinimapRadius;
				handle.DrawLine(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y) - vector, new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y) + vector, color2);
			}
			EntityQuery<MetaDataComponent> entityQuery = this._entManager.GetEntityQuery<MetaDataComponent>();
			EntityQuery<TransformComponent> entityQuery2 = this._entManager.GetEntityQuery<TransformComponent>();
			EntityQuery<FixturesComponent> entityQuery3 = this._entManager.GetEntityQuery<FixturesComponent>();
			EntityQuery<PhysicsComponent> entityQuery4 = this._entManager.GetEntityQuery<PhysicsComponent>();
			MapCoordinates mapCoordinates = this._coordinates.Value.ToMap(this._entManager);
			TransformComponent transformComponent;
			if (mapCoordinates.MapId == MapId.Nullspace || !entityQuery2.TryGetComponent(this._coordinates.Value.EntityId, ref transformComponent))
			{
				this.Clear();
				return;
			}
			Vector2 position = this._coordinates.Value.Position;
			Angle angle = transformComponent.WorldRotation - this._rotation.Value;
			Matrix3 matrix = Matrix3.CreateInverseTransform(ref mapCoordinates.Position, ref angle);
			EntityUid? gridUid = this._coordinates.Value.GetGridUid(this._entManager);
			if (gridUid != null)
			{
				Matrix3 worldMatrix = entityQuery2.GetComponent(gridUid.Value).WorldMatrix;
				FixturesComponent component = entityQuery3.GetComponent(gridUid.Value);
				Matrix3 matrix2;
				Matrix3.Multiply(ref worldMatrix, ref matrix, ref matrix2);
				this.DrawGrid(handle, matrix2, component, Color.Yellow);
				this.DrawDocks(handle, gridUid.Value, matrix2);
			}
			Vector2 vector2 = this._coordinates.Value.Position - position;
			vector2.Y = -vector2.Y;
			handle.DrawCircle(this.ScalePosition(vector2), 5f, Color.Lime, true);
			HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
			foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapCoordinates.MapId, new Box2(mapCoordinates.Position - this.MaxRadarRange, mapCoordinates.Position + this.MaxRadarRange), false))
			{
				EntityUid owner = mapGridComponent.Owner;
				EntityUid? entityUid = gridUid;
				FixturesComponent component2;
				if (!(owner == entityUid) && entityQuery3.TryGetComponent(mapGridComponent.Owner, ref component2))
				{
					PhysicsComponent component3 = entityQuery4.GetComponent(mapGridComponent.Owner);
					if (component3.Mass < 10f)
					{
						this.ClearLabel(mapGridComponent.Owner);
					}
					else
					{
						IFFComponent iffcomponent;
						this._entManager.TryGetComponent<IFFComponent>(mapGridComponent.Owner, ref iffcomponent);
						if (iffcomponent == null || (iffcomponent.Flags & IFFFlags.Hide) == IFFFlags.None)
						{
							hashSet.Add(mapGridComponent.Owner);
							string text = entityQuery.GetComponent(mapGridComponent.Owner).EntityName;
							if (text == string.Empty)
							{
								text = Loc.GetString("shuttle-console-unknown");
							}
							Matrix3 worldMatrix2 = entityQuery2.GetComponent(mapGridComponent.Owner).WorldMatrix;
							Matrix3 matrix3;
							Matrix3.Multiply(ref worldMatrix2, ref matrix, ref matrix3);
							Color color3 = (iffcomponent != null) ? iffcomponent.Color : IFFComponent.IFFColor;
							if (this.ShowIFF && (iffcomponent == null || (iffcomponent.Flags & IFFFlags.HideLabel) == IFFFlags.None))
							{
								Box2 localAABB = mapGridComponent.LocalAABB;
								Control control;
								Label label;
								if (!this._iffControls.TryGetValue(mapGridComponent.Owner, out control))
								{
									label = new Label
									{
										HorizontalAlignment = 1
									};
									this._iffControls[mapGridComponent.Owner] = label;
									base.AddChild(label);
								}
								else
								{
									label = (Label)control;
								}
								label.FontColorOverride = new Color?(color3);
								Vector2 vector3 = matrix3.Transform(component3.LocalCenter);
								vector3.Y = -vector3.Y;
								float length = vector3.Length;
								float num5 = Math.Max(localAABB.Height, localAABB.Width) * this.MinimapScale / 1.8f / this.UIScale;
								Vector2 vector4 = this.ScalePosition(vector3) / this.UIScale - new Vector2(label.Width / 2f, -num5);
								vector4..ctor(Math.Clamp(vector4.X, 0f, base.Width - label.Width), Math.Clamp(vector4.Y, 10f, base.Height - label.Height));
								label.Visible = true;
								Label label2 = label;
								string text2 = "shuttle-console-iff-label";
								ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
								array[0] = new ValueTuple<string, object>("name", text);
								int num6 = 1;
								string item = "distance";
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
								defaultInterpolatedStringHandler.AppendFormatted<float>(length, "0.0");
								array[num6] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
								label2.Text = Loc.GetString(text2, array);
								LayoutContainer.SetPosition(label, vector4);
							}
							else
							{
								this.ClearLabel(mapGridComponent.Owner);
							}
							this.DrawGrid(handle, matrix3, component2, color3);
							this.DrawDocks(handle, mapGridComponent.Owner, matrix3);
						}
					}
				}
			}
			foreach (KeyValuePair<EntityUid, Control> keyValuePair in this._iffControls)
			{
				EntityUid owner;
				Control control2;
				keyValuePair.Deconstruct(out owner, out control2);
				EntityUid entityUid2 = owner;
				if (!hashSet.Contains(entityUid2))
				{
					this.ClearLabel(entityUid2);
				}
			}
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x000339A4 File Offset: 0x00031BA4
		private void Clear()
		{
			foreach (KeyValuePair<EntityUid, Control> keyValuePair in this._iffControls)
			{
				EntityUid entityUid;
				Control control;
				keyValuePair.Deconstruct(out entityUid, out control);
				control.Dispose();
			}
			this._iffControls.Clear();
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00033A0C File Offset: 0x00031C0C
		private void ClearLabel(EntityUid uid)
		{
			Control control;
			if (!this._iffControls.TryGetValue(uid, out control))
			{
				return;
			}
			control.Dispose();
			this._iffControls.Remove(uid);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00033A40 File Offset: 0x00031C40
		private void DrawDocks(DrawingHandleScreen handle, EntityUid uid, Matrix3 matrix)
		{
			if (!this.ShowDocks)
			{
				return;
			}
			List<DockingInterfaceState> list;
			if (this._docks.TryGetValue(uid, out list))
			{
				foreach (DockingInterfaceState dockingInterfaceState in list)
				{
					EntityUid entity = dockingInterfaceState.Entity;
					Vector2 position = dockingInterfaceState.Coordinates.Position;
					Vector2 vector = matrix.Transform(position);
					if (vector.Length <= this.RadarRange - 1.2f)
					{
						EntityUid? highlightedDock = this.HighlightedDock;
						EntityUid entityUid = entity;
						Color color = (highlightedDock != null && (highlightedDock == null || highlightedDock.GetValueOrDefault() == entityUid)) ? dockingInterfaceState.HighlightedColor : dockingInterfaceState.Color;
						vector.Y = -vector.Y;
						Vector2[] array = new Vector2[]
						{
							matrix.Transform(position + new Vector2(-1.2f, -1.2f)),
							matrix.Transform(position + new Vector2(1.2f, -1.2f)),
							matrix.Transform(position + new Vector2(1.2f, 1.2f)),
							matrix.Transform(position + new Vector2(-1.2f, 1.2f))
						};
						for (int i = 0; i < array.Length; i++)
						{
							Vector2 vector2 = array[i];
							vector2.Y = -vector2.Y;
							array[i] = this.ScalePosition(vector2);
						}
						handle.DrawPrimitives(2, array, color);
					}
				}
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00033C24 File Offset: 0x00031E24
		private void DrawGrid(DrawingHandleScreen handle, Matrix3 matrix, FixturesComponent component, Color color)
		{
			foreach (Fixture fixture in component.Fixtures.Values)
			{
				bool flag = false;
				PolygonShape polygonShape = (PolygonShape)fixture.Shape;
				Vector2[] array = new Vector2[polygonShape.VertexCount + 1];
				for (int i = 0; i < polygonShape.VertexCount; i++)
				{
					Vector2 vector = matrix.Transform(polygonShape.Vertices[i]);
					if (vector.Length > this.RadarRange)
					{
						flag = true;
						break;
					}
					vector.Y = -vector.Y;
					array[i] = this.ScalePosition(vector);
				}
				if (!flag)
				{
					array[polygonShape.VertexCount] = array[0];
					handle.DrawPrimitives(5, array, color);
				}
			}
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00033D18 File Offset: 0x00031F18
		private Vector2 ScalePosition(Vector2 value)
		{
			return value * this.MinimapScale + this.MidPoint;
		}

		// Token: 0x04000467 RID: 1127
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000468 RID: 1128
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000469 RID: 1129
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400046A RID: 1130
		private const float ScrollSensitivity = 8f;

		// Token: 0x0400046B RID: 1131
		private const float GridLinesDistance = 32f;

		// Token: 0x0400046C RID: 1132
		private EntityCoordinates? _coordinates;

		// Token: 0x0400046D RID: 1133
		private Angle? _rotation;

		// Token: 0x0400046E RID: 1134
		private float _radarMinRange = 64f;

		// Token: 0x0400046F RID: 1135
		private float _radarMaxRange = 256f;

		// Token: 0x04000471 RID: 1137
		private float _actualRadarRange = 256f;

		// Token: 0x04000473 RID: 1139
		private Dictionary<EntityUid, Control> _iffControls = new Dictionary<EntityUid, Control>();

		// Token: 0x04000474 RID: 1140
		private Dictionary<EntityUid, List<DockingInterfaceState>> _docks = new Dictionary<EntityUid, List<DockingInterfaceState>>();

		// Token: 0x04000477 RID: 1143
		public EntityUid? HighlightedDock;

		// Token: 0x04000478 RID: 1144
		[Nullable(2)]
		public Action<float> OnRadarRangeChanged;
	}
}
