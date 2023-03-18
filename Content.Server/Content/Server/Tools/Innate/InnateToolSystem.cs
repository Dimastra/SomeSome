using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Hands.Components;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Tools.Innate
{
	// Token: 0x02000117 RID: 279
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InnateToolSystem : EntitySystem
	{
		// Token: 0x0600050E RID: 1294 RVA: 0x00018954 File Offset: 0x00016B54
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InnateToolComponent, ComponentStartup>(new ComponentEventHandler<InnateToolComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<InnateToolComponent, ComponentShutdown>(new ComponentEventHandler<InnateToolComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<InnateToolComponent, DestructionEventArgs>(new ComponentEventHandler<InnateToolComponent, DestructionEventArgs>(this.OnDestroyed), null, null);
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x000189A4 File Offset: 0x00016BA4
		private void OnStartup(EntityUid uid, InnateToolComponent component, ComponentStartup args)
		{
			if (component.Tools.Count == 0)
			{
				return;
			}
			EntityCoordinates spawnCoord = base.Transform(uid).Coordinates;
			HandsComponent hands;
			if (base.TryComp<HandsComponent>(uid, ref hands) && hands.Count >= component.Tools.Count)
			{
				foreach (string entry in EntitySpawnCollection.GetSpawns(component.Tools, this._robustRandom))
				{
					EntityUid item = base.Spawn(entry, spawnCoord);
					base.AddComp<UnremoveableComponent>(item);
					if (!this._sharedHandsSystem.TryPickupAnyHand(uid, item, false, false, null, null))
					{
						base.QueueDel(item);
					}
					else
					{
						component.ToolUids.Add(item);
					}
				}
			}
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x00018A78 File Offset: 0x00016C78
		private void OnShutdown(EntityUid uid, InnateToolComponent component, ComponentShutdown args)
		{
			foreach (EntityUid tool in component.ToolUids)
			{
				base.RemComp<UnremoveableComponent>(tool);
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x00018ACC File Offset: 0x00016CCC
		private void OnDestroyed(EntityUid uid, InnateToolComponent component, DestructionEventArgs args)
		{
			this.Cleanup(uid, component);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00018AD8 File Offset: 0x00016CD8
		public void Cleanup(EntityUid uid, InnateToolComponent component)
		{
			foreach (EntityUid tool in component.ToolUids)
			{
				if (this._tagSystem.HasTag(tool, "InnateDontDelete"))
				{
					base.RemComp<UnremoveableComponent>(tool);
				}
				else
				{
					base.Del(tool);
				}
				HandsComponent hands;
				if (base.TryComp<HandsComponent>(uid, ref hands))
				{
					foreach (KeyValuePair<string, Hand> hand in hands.Hands)
					{
						SharedHandsSystem sharedHandsSystem = this._sharedHandsSystem;
						Hand value = hand.Value;
						SharedHandsComponent handsComp = hands;
						sharedHandsSystem.TryDrop(uid, value, null, false, true, handsComp);
					}
				}
			}
			component.ToolUids.Clear();
		}

		// Token: 0x040002F1 RID: 753
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040002F2 RID: 754
		[Dependency]
		private readonly SharedHandsSystem _sharedHandsSystem;

		// Token: 0x040002F3 RID: 755
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
