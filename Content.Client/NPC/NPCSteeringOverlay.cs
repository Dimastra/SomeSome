using System;
using System.Runtime.CompilerServices;
using Content.Client.Physics.Controllers;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.NPC
{
	// Token: 0x0200020D RID: 525
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NPCSteeringOverlay : Overlay
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00052AC8 File Offset: 0x00050CC8
		public NPCSteeringOverlay(IEntityManager entManager)
		{
			this._entManager = entManager;
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00052AD8 File Offset: 0x00050CD8
		protected override void Draw(in OverlayDrawArgs args)
		{
			foreach (ValueTuple<NPCSteeringComponent, InputMoverComponent, TransformComponent> valueTuple in this._entManager.EntityQuery<NPCSteeringComponent, InputMoverComponent, TransformComponent>(true))
			{
				NPCSteeringComponent item = valueTuple.Item1;
				InputMoverComponent item2 = valueTuple.Item2;
				TransformComponent item3 = valueTuple.Item3;
				if (!(item3.MapID != args.MapId))
				{
					Vector2 item4 = item3.GetWorldPositionRotation().Item1;
					if (args.WorldAABB.Contains(item4, true))
					{
						args.WorldHandle.DrawCircle(item4, 1f, Color.Green, false);
						Angle parentGridAngle = this._entManager.System<MoverController>().GetParentGridAngle(item2);
						foreach (Vector2 vector in item.DangerPoints)
						{
							args.WorldHandle.DrawCircle(vector, 0.1f, Color.Red.WithAlpha(0.6f), true);
						}
						for (int i = 0; i < 12; i++)
						{
							float num = item.DangerMap[i];
							float num2 = item.InterestMap[i];
							Angle angle = Angle.FromDegrees((double)(i * 30));
							DrawingHandleBase worldHandle = args.WorldHandle;
							Vector2 vector2 = item4;
							Vector2 vector3 = item4;
							Angle angle2 = parentGridAngle + angle;
							Vector2 vector4 = new Vector2(num2, 0f);
							worldHandle.DrawLine(vector2, vector3 + angle2.RotateVec(ref vector4), Color.LimeGreen);
							DrawingHandleBase worldHandle2 = args.WorldHandle;
							Vector2 vector5 = item4;
							Vector2 vector6 = item4;
							angle2 = parentGridAngle + angle;
							vector4 = new Vector2(num, 0f);
							worldHandle2.DrawLine(vector5, vector6 + angle2.RotateVec(ref vector4), Color.Red);
						}
						args.WorldHandle.DrawLine(item4, item4 + parentGridAngle.RotateVec(ref item.Direction), Color.Cyan);
					}
				}
			}
		}

		// Token: 0x040006C6 RID: 1734
		private readonly IEntityManager _entManager;
	}
}
