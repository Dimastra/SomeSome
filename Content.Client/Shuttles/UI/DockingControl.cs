using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;

namespace Content.Client.Shuttles.UI
{
	// Token: 0x02000149 RID: 329
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class DockingControl : Control
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x0003140D File Offset: 0x0002F60D
		private int MinimapRadius
		{
			get
			{
				return (int)Math.Min(base.Size.X, base.Size.Y) / 2;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600087F RID: 2175 RVA: 0x0003142D File Offset: 0x0002F62D
		private Vector2 MidPoint
		{
			get
			{
				return base.Size / 2f * this.UIScale;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x0003144A File Offset: 0x0002F64A
		private int SizeFull
		{
			get
			{
				return (int)((float)(this.MinimapRadius * 2) * this.UIScale);
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x0003145D File Offset: 0x0002F65D
		private int ScaledMinimapRadius
		{
			get
			{
				return (int)((float)this.MinimapRadius * this.UIScale);
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000882 RID: 2178 RVA: 0x0003146E File Offset: 0x0002F66E
		private float MinimapScale
		{
			get
			{
				if (this._range == 0f)
				{
					return 0f;
				}
				return (float)this.ScaledMinimapRadius / this._range;
			}
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00031494 File Offset: 0x0002F694
		public DockingControl()
		{
			this._entManager = IoCManager.Resolve<IEntityManager>();
			this._mapManager = IoCManager.Resolve<IMapManager>();
			this._rangeSquared = this._range * this._range;
			base.MinSize = new ValueTuple<float, float>((float)this.SizeFull, (float)this.SizeFull);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00031504 File Offset: 0x0002F704
		protected override void Draw(DrawingHandleScreen handle)
		{
			base.Draw(handle);
			Color color;
			color..ctor(0.08f, 0.08f, 0.08f, 1f);
			handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), (float)(this.ScaledMinimapRadius + 1), color, true);
			handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), (float)this.ScaledMinimapRadius, Color.Black, true);
			Color color2;
			color2..ctor(0.08f, 0.08f, 0.08f, 1f);
			int num = 8;
			int num2 = (int)Math.Floor((double)(this._range / 32f));
			for (int i = 1; i < num2 + 1; i++)
			{
				handle.DrawCircle(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y), 32f * this.MinimapScale * (float)i, color2, false);
			}
			for (int j = 0; j < num; j++)
			{
				Vector2 vector = (3.141592653589793 / (double)num * (double)j).ToVec() * (float)this.ScaledMinimapRadius;
				handle.DrawLine(new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y) - vector, new ValueTuple<float, float>(this.MidPoint.X, this.MidPoint.Y) + vector, color2);
			}
			TransformComponent transformComponent;
			if (this.Coordinates == null || this.Angle == null || !this._entManager.TryGetComponent<TransformComponent>(this.GridEntity, ref transformComponent))
			{
				return;
			}
			Matrix3 matrix = Matrix3.CreateRotation(-this.Angle.Value + 3.141592653589793);
			Matrix3 matrix2 = Matrix3.CreateTranslation(-this.Coordinates.Value.Position);
			FixturesComponent fixturesComponent;
			if (this._entManager.TryGetComponent<FixturesComponent>(this.GridEntity, ref fixturesComponent))
			{
				foreach (KeyValuePair<string, Fixture> keyValuePair in fixturesComponent.Fixtures)
				{
					string text;
					Fixture fixture;
					keyValuePair.Deconstruct(out text, out fixture);
					PolygonShape polygonShape = (PolygonShape)fixture.Shape;
					for (int k = 0; k < polygonShape.VertexCount; k++)
					{
						Vector2 vector2 = matrix2.Transform(polygonShape.Vertices[k]);
						Vector2 vector3 = matrix2.Transform(polygonShape.Vertices[(k + 1) % polygonShape.VertexCount]);
						bool flag = vector2.LengthSquared > this._rangeSquared;
						bool flag2 = vector3.LengthSquared > this._rangeSquared;
						if (!flag || !flag2)
						{
							vector2.Y = -vector2.Y;
							vector3.Y = -vector3.Y;
							if (flag)
							{
								Vector2? vector4;
								if (!MathHelper.TryGetIntersecting(vector2, vector3, this._range, ref vector4))
								{
									goto IL_33F;
								}
								vector2 = vector4.Value;
							}
							else if (flag2)
							{
								Vector2? vector5;
								if (!MathHelper.TryGetIntersecting(vector3, vector2, this._range, ref vector5))
								{
									goto IL_33F;
								}
								vector3 = vector5.Value;
							}
							handle.DrawLine(this.ScalePosition(vector2), this.ScalePosition(vector3), Color.Goldenrod);
						}
						IL_33F:;
					}
				}
			}
			handle.DrawRect(new UIBox2(this.ScalePosition(matrix.Transform(new Vector2(-0.2f, -0.7f))), this.ScalePosition(matrix.Transform(new Vector2(0.2f, -0.5f)))), Color.Aquamarine, true);
			handle.DrawRect(new UIBox2(this.ScalePosition(matrix.Transform(new Vector2(-0.5f, 0.5f))), this.ScalePosition(matrix.Transform(new Vector2(0.5f, -0.5f)))), Color.Green, true);
			Vector2 vector6 = transformComponent.WorldMatrix.Transform(this.Coordinates.Value.Position);
			Matrix3 invWorldMatrix = transformComponent.InvWorldMatrix;
			Matrix3 matrix3;
			Matrix3.Multiply(ref invWorldMatrix, ref matrix2, ref matrix3);
			EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
			foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(transformComponent.MapID, new Box2(vector6 - this._range, vector6 + this._range), false))
			{
				FixturesComponent fixturesComponent2;
				if (!(mapGridComponent.Owner == this.GridEntity) && this._entManager.TryGetComponent<FixturesComponent>(mapGridComponent.Owner, ref fixturesComponent2))
				{
					Matrix3 worldMatrix = entityQuery.GetComponent(mapGridComponent.Owner).WorldMatrix;
					Matrix3 matrix4;
					Matrix3.Multiply(ref worldMatrix, ref matrix3, ref matrix4);
					foreach (KeyValuePair<string, Fixture> keyValuePair in fixturesComponent2.Fixtures)
					{
						string text;
						Fixture fixture;
						keyValuePair.Deconstruct(out text, out fixture);
						PolygonShape polygonShape2 = (PolygonShape)fixture.Shape;
						for (int l = 0; l < polygonShape2.VertexCount; l++)
						{
							Vector2 vector7 = polygonShape2.Vertices[l];
							Vector2 vector8 = polygonShape2.Vertices[(l + 1) % polygonShape2.VertexCount];
							Vector2 vector9 = matrix4.Transform(vector7);
							Vector2 vector10 = matrix4.Transform(vector8);
							bool flag3 = vector9.LengthSquared > this._rangeSquared;
							bool flag4 = vector10.LengthSquared > this._rangeSquared;
							if (!flag3 || !flag4)
							{
								vector9.Y = -vector9.Y;
								vector10.Y = -vector10.Y;
								if (flag3)
								{
									Vector2? vector11;
									if (!MathHelper.TryGetIntersecting(vector9, vector10, this._range, ref vector11))
									{
										goto IL_615;
									}
									vector9 = vector11.Value;
								}
								else if (flag4)
								{
									Vector2? vector12;
									if (!MathHelper.TryGetIntersecting(vector10, vector9, this._range, ref vector12))
									{
										goto IL_615;
									}
									vector10 = vector12.Value;
								}
								handle.DrawLine(this.ScalePosition(vector9), this.ScalePosition(vector10), Color.Aquamarine);
							}
							IL_615:;
						}
					}
					List<DockingInterfaceState> list;
					if (this.Docks.TryGetValue(mapGridComponent.Owner, out list))
					{
						foreach (DockingInterfaceState dockingInterfaceState in list)
						{
							if (matrix4.Transform(dockingInterfaceState.Coordinates.Position).Length <= this._range - 0.8f)
							{
								Matrix3 matrix5 = Matrix3.CreateRotation(dockingInterfaceState.Angle);
								Vector2[] array = new Vector2[]
								{
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + matrix5.Transform(new Vector2(-0.2f, -0.7f))),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + matrix5.Transform(new Vector2(0.2f, -0.7f))),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + matrix5.Transform(new Vector2(0.2f, -0.5f))),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + matrix5.Transform(new Vector2(-0.2f, -0.5f)))
								};
								for (int m = 0; m < array.Length; m++)
								{
									Vector2 vector13 = array[m];
									vector13.Y = -vector13.Y;
									array[m] = this.ScalePosition(vector13);
								}
								handle.DrawPrimitives(2, array, Color.Turquoise);
								array = new Vector2[]
								{
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + new Vector2(-0.5f, -0.5f)),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + new Vector2(0.5f, -0.5f)),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + new Vector2(0.5f, 0.5f)),
									matrix4.Transform(dockingInterfaceState.Coordinates.Position + new Vector2(-0.5f, 0.5f))
								};
								for (int n = 0; n < array.Length; n++)
								{
									Vector2 vector14 = array[n];
									vector14.Y = -vector14.Y;
									array[n] = this.ScalePosition(vector14);
								}
								handle.DrawPrimitives(2, array, Color.Green);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00031E9C File Offset: 0x0003009C
		private Vector2 ScalePosition(Vector2 value)
		{
			return value * this.MinimapScale + this.MidPoint;
		}

		// Token: 0x04000455 RID: 1109
		private readonly IEntityManager _entManager;

		// Token: 0x04000456 RID: 1110
		private readonly IMapManager _mapManager;

		// Token: 0x04000457 RID: 1111
		private float _range = 8f;

		// Token: 0x04000458 RID: 1112
		private float _rangeSquared;

		// Token: 0x04000459 RID: 1113
		private const float GridLinesDistance = 32f;

		// Token: 0x0400045A RID: 1114
		public EntityUid? ViewedDock;

		// Token: 0x0400045B RID: 1115
		public EntityUid? GridEntity;

		// Token: 0x0400045C RID: 1116
		public EntityCoordinates? Coordinates;

		// Token: 0x0400045D RID: 1117
		public Angle? Angle;

		// Token: 0x0400045E RID: 1118
		public Dictionary<EntityUid, List<DockingInterfaceState>> Docks = new Dictionary<EntityUid, List<DockingInterfaceState>>();
	}
}
