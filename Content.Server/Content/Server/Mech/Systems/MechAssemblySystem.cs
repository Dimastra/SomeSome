using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mech.Components;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Content.Shared.Tools.Components;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Mech.Systems
{
	// Token: 0x020003C4 RID: 964
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechAssemblySystem : EntitySystem
	{
		// Token: 0x060013CF RID: 5071 RVA: 0x00066B34 File Offset: 0x00064D34
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MechAssemblyComponent, ComponentInit>(new ComponentEventHandler<MechAssemblyComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<MechAssemblyComponent, InteractUsingEvent>(new ComponentEventHandler<MechAssemblyComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x00066B5E File Offset: 0x00064D5E
		private void OnInit(EntityUid uid, MechAssemblyComponent component, ComponentInit args)
		{
			component.PartsContainer = this._container.EnsureContainer<Container>(uid, "mech-assembly-container", null);
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x00066B78 File Offset: 0x00064D78
		private void OnInteractUsing(EntityUid uid, MechAssemblyComponent component, InteractUsingEvent args)
		{
			ToolComponent toolComp;
			if (base.TryComp<ToolComponent>(args.Used, ref toolComp) && toolComp.Qualities.Contains(component.QualityNeeded))
			{
				foreach (string tag in component.RequiredParts.Keys)
				{
					component.RequiredParts[tag] = false;
				}
				this._container.EmptyContainer(component.PartsContainer, false, null, false, null);
				return;
			}
			TagComponent tagComp;
			if (!base.TryComp<TagComponent>(args.Used, ref tagComp))
			{
				return;
			}
			foreach (KeyValuePair<string, bool> keyValuePair in component.RequiredParts)
			{
				string text;
				bool flag;
				keyValuePair.Deconstruct(out text, out flag);
				string tag2 = text;
				if (!flag && tagComp.Tags.Contains(tag2))
				{
					component.RequiredParts[tag2] = true;
					component.PartsContainer.Insert(args.Used, null, null, null, null, null);
					break;
				}
			}
			using (Dictionary<string, bool>.ValueCollection.Enumerator enumerator3 = component.RequiredParts.Values.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (!enumerator3.Current)
					{
						return;
					}
				}
			}
			base.Spawn(component.FinishedPrototype, base.Transform(uid).Coordinates);
			this.EntityManager.DeleteEntity(uid);
		}

		// Token: 0x04000C42 RID: 3138
		[Dependency]
		private readonly ContainerSystem _container;
	}
}
