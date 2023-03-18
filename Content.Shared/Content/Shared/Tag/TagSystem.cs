using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tag
{
	// Token: 0x020000EC RID: 236
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TagSystem : EntitySystem
	{
		// Token: 0x0600028C RID: 652 RVA: 0x0000C2C8 File Offset: 0x0000A4C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TagComponent, ComponentInit>(new ComponentEventHandler<TagComponent, ComponentInit>(this.OnTagInit), null, null);
			ComponentEventRefHandler<TagComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = TagSystem.<>O.<0>__OnTagGetState) == null)
			{
				componentEventRefHandler = (TagSystem.<>O.<0>__OnTagGetState = new ComponentEventRefHandler<TagComponent, ComponentGetState>(TagSystem.OnTagGetState));
			}
			base.SubscribeLocalEvent<TagComponent, ComponentGetState>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<TagComponent, ComponentHandleState>(new ComponentEventRefHandler<TagComponent, ComponentHandleState>(this.OnTagHandleState), null, null);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000C328 File Offset: 0x0000A528
		private void OnTagHandleState(EntityUid uid, TagComponent component, ref ComponentHandleState args)
		{
			TagComponentState state = args.Current as TagComponentState;
			if (state == null)
			{
				return;
			}
			component.Tags.Clear();
			foreach (string tag in state.Tags)
			{
				this.GetTagOrThrow(tag);
				component.Tags.Add(tag);
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000C380 File Offset: 0x0000A580
		private static void OnTagGetState(EntityUid uid, TagComponent component, ref ComponentGetState args)
		{
			string[] tags = new string[component.Tags.Count];
			int i = 0;
			foreach (string tag in component.Tags)
			{
				tags[i] = tag;
				i++;
			}
			args.State = new TagComponentState(tags);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000C3F4 File Offset: 0x0000A5F4
		private void OnTagInit(EntityUid uid, TagComponent component, ComponentInit args)
		{
			foreach (string tag in component.Tags)
			{
				this.GetTagOrThrow(tag);
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000C448 File Offset: 0x0000A648
		private TagPrototype GetTagOrThrow(string id)
		{
			return this._proto.Index<TagPrototype>(id);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000C456 File Offset: 0x0000A656
		public bool AddTag(EntityUid entity, string id)
		{
			return this.AddTag(base.EnsureComp<TagComponent>(entity), id);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000C466 File Offset: 0x0000A666
		public bool AddTags(EntityUid entity, params string[] ids)
		{
			return this.AddTags(base.EnsureComp<TagComponent>(entity), ids);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000C476 File Offset: 0x0000A676
		public bool AddTags(EntityUid entity, IEnumerable<string> ids)
		{
			return this.AddTags(base.EnsureComp<TagComponent>(entity), ids);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000C488 File Offset: 0x0000A688
		public bool TryAddTag(EntityUid entity, string id)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.AddTag(component, id);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000C4AC File Offset: 0x0000A6AC
		public bool TryAddTags(EntityUid entity, params string[] ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.AddTags(component, ids);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000C4D0 File Offset: 0x0000A6D0
		public bool TryAddTags(EntityUid entity, IEnumerable<string> ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.AddTags(component, ids);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000C4F4 File Offset: 0x0000A6F4
		public bool HasTag(EntityUid entity, string id)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.HasTag(component, id);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000C518 File Offset: 0x0000A718
		public bool HasTag(EntityUid entity, string id, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TagComponent> tagQuery)
		{
			TagComponent component;
			return tagQuery.TryGetComponent(entity, ref component) && this.HasTag(component, id);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000C53C File Offset: 0x0000A73C
		public bool HasAllTags(EntityUid entity, params string[] ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.HasAllTags(component, ids);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000C560 File Offset: 0x0000A760
		public bool HasAllTags(EntityUid entity, IEnumerable<string> ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.HasAllTags(component, ids);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000C584 File Offset: 0x0000A784
		public bool HasAnyTag(EntityUid entity, params string[] ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.HasAnyTag(component, ids);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
		public bool HasAnyTag(EntityUid entity, IEnumerable<string> ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.HasAnyTag(component, ids);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000C5CC File Offset: 0x0000A7CC
		public bool RemoveTag(EntityUid entity, string id)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.RemoveTag(component, id);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
		public bool RemoveTags(EntityUid entity, params string[] ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.RemoveTags(component, ids);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000C614 File Offset: 0x0000A814
		public bool RemoveTags(EntityUid entity, IEnumerable<string> ids)
		{
			TagComponent component;
			return base.TryComp<TagComponent>(entity, ref component) && this.RemoveTags(component, ids);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000C636 File Offset: 0x0000A836
		public bool AddTag(TagComponent component, string id)
		{
			this.GetTagOrThrow(id);
			if (component.Tags.Add(id))
			{
				base.Dirty(component, null);
				return true;
			}
			return false;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000C659 File Offset: 0x0000A859
		public bool AddTags(TagComponent component, params string[] ids)
		{
			return this.AddTags(component, ids.AsEnumerable<string>());
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000C668 File Offset: 0x0000A868
		public bool AddTags(TagComponent component, IEnumerable<string> ids)
		{
			int count = component.Tags.Count;
			foreach (string id in ids)
			{
				this.GetTagOrThrow(id);
				component.Tags.Add(id);
			}
			if (component.Tags.Count > count)
			{
				base.Dirty(component, null);
				return true;
			}
			return false;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000C6E4 File Offset: 0x0000A8E4
		public bool HasTag(TagComponent component, string id)
		{
			this.GetTagOrThrow(id);
			return component.Tags.Contains(id);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000C6FA File Offset: 0x0000A8FA
		public bool HasAllTags(TagComponent component, params string[] ids)
		{
			return this.HasAllTags(component, ids.AsEnumerable<string>());
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000C70C File Offset: 0x0000A90C
		public bool HasAllTags(TagComponent component, IEnumerable<string> ids)
		{
			foreach (string id in ids)
			{
				this.GetTagOrThrow(id);
				if (!component.Tags.Contains(id))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000C76C File Offset: 0x0000A96C
		public bool HasAnyTag(TagComponent component, params string[] ids)
		{
			return this.HasAnyTag(component, ids.AsEnumerable<string>());
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000C77C File Offset: 0x0000A97C
		public bool HasAnyTag(TagComponent component, IEnumerable<string> ids)
		{
			foreach (string id in ids)
			{
				this.GetTagOrThrow(id);
				if (component.Tags.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000C7DC File Offset: 0x0000A9DC
		public bool RemoveTag(TagComponent component, string id)
		{
			this.GetTagOrThrow(id);
			if (component.Tags.Remove(id))
			{
				base.Dirty(component, null);
				return true;
			}
			return false;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000C7FF File Offset: 0x0000A9FF
		public bool RemoveTags(TagComponent component, params string[] ids)
		{
			return this.RemoveTags(component, ids.AsEnumerable<string>());
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000C810 File Offset: 0x0000AA10
		public bool RemoveTags(TagComponent component, IEnumerable<string> ids)
		{
			int count = component.Tags.Count;
			foreach (string id in ids)
			{
				this.GetTagOrThrow(id);
				component.Tags.Remove(id);
			}
			if (component.Tags.Count < count)
			{
				base.Dirty(component, null);
				return true;
			}
			return false;
		}

		// Token: 0x040002F5 RID: 757
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x02000795 RID: 1941
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040017A6 RID: 6054
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<TagComponent, ComponentGetState> <0>__OnTagGetState;
		}
	}
}
