using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Server.Containers
{
	// Token: 0x020005EA RID: 1514
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContainerFillSystem : EntitySystem
	{
		// Token: 0x06002048 RID: 8264 RVA: 0x000A83B8 File Offset: 0x000A65B8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ContainerFillComponent, MapInitEvent>(new ComponentEventHandler<ContainerFillComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x06002049 RID: 8265 RVA: 0x000A83D4 File Offset: 0x000A65D4
		private void OnMapInit(EntityUid uid, ContainerFillComponent component, MapInitEvent args)
		{
			ContainerManagerComponent containerComp;
			if (!base.TryComp<ContainerManagerComponent>(uid, ref containerComp))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" with a ");
				defaultInterpolatedStringHandler.AppendFormatted("ContainerFillComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" has no ");
				defaultInterpolatedStringHandler.AppendFormatted("ContainerManagerComponent");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			TransformComponent xform = base.Transform(uid);
			EntityCoordinates coords;
			coords..ctor(uid, Vector2.Zero);
			foreach (KeyValuePair<string, List<string>> keyValuePair in component.Containers)
			{
				string text;
				List<string> list;
				keyValuePair.Deconstruct(out text, out list);
				string contaienrId = text;
				List<string> prototypes = list;
				IContainer container;
				if (!this._containerSystem.TryGetContainer(uid, contaienrId, ref container, containerComp))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 3);
					defaultInterpolatedStringHandler.AppendLiteral("Entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(" with a ");
					defaultInterpolatedStringHandler.AppendFormatted("ContainerFillComponent");
					defaultInterpolatedStringHandler.AppendLiteral(" is missing a container (");
					defaultInterpolatedStringHandler.AppendFormatted(contaienrId);
					defaultInterpolatedStringHandler.AppendLiteral(").");
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					foreach (string proto in prototypes)
					{
						EntityUid ent = base.Spawn(proto, coords);
						if (!container.Insert(ent, this.EntityManager, null, xform, null, null))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 3);
							defaultInterpolatedStringHandler.AppendLiteral("Entity ");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
							defaultInterpolatedStringHandler.AppendLiteral(" with a ");
							defaultInterpolatedStringHandler.AppendFormatted("ContainerFillComponent");
							defaultInterpolatedStringHandler.AppendLiteral(" failed to insert an entity: ");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(ent));
							defaultInterpolatedStringHandler.AppendLiteral(".");
							Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
							base.Transform(ent).AttachToGridOrMap();
							break;
						}
					}
				}
			}
		}

		// Token: 0x04001401 RID: 5121
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
