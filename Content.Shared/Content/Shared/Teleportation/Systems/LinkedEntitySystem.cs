using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Teleportation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Teleportation.Systems
{
	// Token: 0x020000DF RID: 223
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LinkedEntitySystem : EntitySystem
	{
		// Token: 0x06000271 RID: 625 RVA: 0x0000BB50 File Offset: 0x00009D50
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LinkedEntityComponent, ComponentShutdown>(new ComponentEventHandler<LinkedEntityComponent, ComponentShutdown>(this.OnLinkShutdown), null, null);
			base.SubscribeLocalEvent<LinkedEntityComponent, ComponentGetState>(new ComponentEventRefHandler<LinkedEntityComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<LinkedEntityComponent, ComponentHandleState>(new ComponentEventRefHandler<LinkedEntityComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000BB9F File Offset: 0x00009D9F
		private void OnGetState(EntityUid uid, LinkedEntityComponent component, ref ComponentGetState args)
		{
			args.State = new LinkedEntityComponentState(component.LinkedEntities);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000BBB4 File Offset: 0x00009DB4
		private void OnHandleState(EntityUid uid, LinkedEntityComponent component, ref ComponentHandleState args)
		{
			LinkedEntityComponentState state = args.Current as LinkedEntityComponentState;
			if (state != null)
			{
				component.LinkedEntities = state.LinkedEntities;
			}
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000BBDC File Offset: 0x00009DDC
		private void OnLinkShutdown(EntityUid uid, LinkedEntityComponent component, ComponentShutdown args)
		{
			foreach (EntityUid ent in component.LinkedEntities.ToArray<EntityUid>())
			{
				LinkedEntityComponent link;
				if (!base.Deleted(ent, null) && base.LifeStage(ent, null) < 4 && base.TryComp<LinkedEntityComponent>(ent, ref link))
				{
					this.TryUnlink(uid, ent, component, link);
				}
			}
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000BC38 File Offset: 0x00009E38
		public bool TryLink(EntityUid first, EntityUid second, bool deleteOnEmptyLinks = false)
		{
			LinkedEntityComponent firstLink = base.EnsureComp<LinkedEntityComponent>(first);
			LinkedEntityComponent secondLink = base.EnsureComp<LinkedEntityComponent>(second);
			firstLink.DeleteOnEmptyLinks = deleteOnEmptyLinks;
			secondLink.DeleteOnEmptyLinks = deleteOnEmptyLinks;
			this._appearance.SetData(first, LinkedEntityVisuals.HasAnyLinks, true, null);
			this._appearance.SetData(second, LinkedEntityVisuals.HasAnyLinks, true, null);
			base.Dirty(firstLink, null);
			base.Dirty(secondLink, null);
			return firstLink.LinkedEntities.Add(second) && secondLink.LinkedEntities.Add(first);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		[NullableContext(2)]
		public bool TryUnlink(EntityUid first, EntityUid second, LinkedEntityComponent firstLink = null, LinkedEntityComponent secondLink = null)
		{
			if (!base.Resolve<LinkedEntityComponent>(first, ref firstLink, true))
			{
				return false;
			}
			if (!base.Resolve<LinkedEntityComponent>(second, ref secondLink, true))
			{
				return false;
			}
			bool result = firstLink.LinkedEntities.Remove(second) && secondLink.LinkedEntities.Remove(first);
			this._appearance.SetData(first, LinkedEntityVisuals.HasAnyLinks, firstLink.LinkedEntities.Any<EntityUid>(), null);
			this._appearance.SetData(second, LinkedEntityVisuals.HasAnyLinks, secondLink.LinkedEntities.Any<EntityUid>(), null);
			base.Dirty(firstLink, null);
			base.Dirty(secondLink, null);
			if (firstLink.LinkedEntities.Count == 0 && firstLink.DeleteOnEmptyLinks)
			{
				base.QueueDel(first);
			}
			if (secondLink.LinkedEntities.Count == 0 && secondLink.DeleteOnEmptyLinks)
			{
				base.QueueDel(second);
			}
			return result;
		}

		// Token: 0x040002D6 RID: 726
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
