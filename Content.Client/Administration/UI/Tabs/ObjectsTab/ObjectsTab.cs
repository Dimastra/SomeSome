﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Station;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;

namespace Content.Client.Administration.UI.Tabs.ObjectsTab
{
	// Token: 0x0200049A RID: 1178
	[GenerateTypedNameReferences]
	public sealed class ObjectsTab : Control
	{
		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06001D04 RID: 7428 RVA: 0x000A8C94 File Offset: 0x000A6E94
		// (remove) Token: 0x06001D05 RID: 7429 RVA: 0x000A8CCC File Offset: 0x000A6ECC
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<BaseButton.ButtonEventArgs> OnEntryPressed;

		// Token: 0x06001D06 RID: 7430 RVA: 0x000A8D04 File Offset: 0x000A6F04
		public ObjectsTab()
		{
			ObjectsTab.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<ObjectsTab>(this);
			this.ObjectTypeOptions.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs ev)
			{
				this.ObjectTypeOptions.SelectId(ev.Id);
				this.RefreshObjectList(this._selections[ev.Id]);
			};
			foreach (object obj in Enum.GetValues(typeof(ObjectsTab.ObjectsTabSelection)))
			{
				this._selections.Add((ObjectsTab.ObjectsTabSelection)obj);
				this.ObjectTypeOptions.AddItem(Enum.GetName<ObjectsTab.ObjectsTabSelection>((ObjectsTab.ObjectsTabSelection)obj), null);
			}
			this.RefreshObjectList(this._selections[this.ObjectTypeOptions.SelectedId]);
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x000A8DE8 File Offset: 0x000A6FE8
		private void RefreshObjectList(ObjectsTab.ObjectsTabSelection selection)
		{
			List<EntityUid> list;
			switch (selection)
			{
			case ObjectsTab.ObjectsTabSelection.Grids:
				list = (from x in this._entityManager.EntityQuery<MapGridComponent>(true)
				select x.Owner).ToList<EntityUid>();
				break;
			case ObjectsTab.ObjectsTabSelection.Maps:
				list = (from x in this._entityManager.EntityQuery<MapComponent>(true)
				select x.Owner).ToList<EntityUid>();
				break;
			case ObjectsTab.ObjectsTabSelection.Stations:
				list = this._entityManager.EntitySysManager.GetEntitySystem<StationSystem>().Stations.ToList<EntityUid>();
				break;
			default:
				throw new ArgumentOutOfRangeException("selection", selection, null);
			}
			List<EntityUid> list2 = list;
			foreach (ObjectsTabEntry objectsTabEntry in this._objects)
			{
				this.ObjectList.RemoveChild(objectsTabEntry);
			}
			this._objects.Clear();
			foreach (EntityUid entityUid in list2)
			{
				MetaDataComponent componentOrNull = EntityManagerExt.GetComponentOrNull<MetaDataComponent>(this._entityManager, entityUid);
				ObjectsTabEntry objectsTabEntry2 = new ObjectsTabEntry(((componentOrNull != null) ? componentOrNull.EntityName : null) ?? "Unknown Entity", entityUid);
				this._objects.Add(objectsTabEntry2);
				this.ObjectList.AddChild(objectsTabEntry2);
				objectsTabEntry2.OnPressed += delegate(BaseButton.ButtonEventArgs args)
				{
					Action<BaseButton.ButtonEventArgs> onEntryPressed = this.OnEntryPressed;
					if (onEntryPressed == null)
					{
						return;
					}
					onEntryPressed(args);
				};
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001D08 RID: 7432 RVA: 0x000A8F98 File Offset: 0x000A7198
		private OptionButton ObjectTypeOptions
		{
			get
			{
				return base.FindControl<OptionButton>("ObjectTypeOptions");
			}
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x06001D09 RID: 7433 RVA: 0x000A8FA5 File Offset: 0x000A71A5
		private BoxContainer ObjectList
		{
			get
			{
				return base.FindControl<BoxContainer>("ObjectList");
			}
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x000A8FF0 File Offset: 0x000A71F0
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.ObjectsTab.ObjectsTab.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Control control = new Label
			{
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 0.5f,
				Text = (string)new LocExtension("Object type:").ProvideValue()
			};
			boxContainer2.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "ObjectTypeOptions";
			control = optionButton;
			context.RobustNameScope.Register("ObjectTypeOptions", control);
			optionButton.HorizontalExpand = true;
			optionButton.SizeFlagsStretchRatio = 0.25f;
			control = optionButton;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			control = new HSeparator();
			boxContainer.XamlChildren.Add(control);
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.HorizontalExpand = true;
			scrollContainer.VerticalExpand = true;
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.Name = "ObjectList";
			control = boxContainer3;
			context.RobustNameScope.Register("ObjectList", control);
			control = boxContainer3;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x000A91DF File Offset: 0x000A73DF
		private static void !XamlIlPopulateTrampoline(ObjectsTab A_0)
		{
			ObjectsTab.Populate:Content.Client.Administration.UI.Tabs.ObjectsTab.ObjectsTab.xaml(null, A_0);
		}

		// Token: 0x04000E82 RID: 3714
		[Nullable(1)]
		[Dependency]
		private readonly EntityManager _entityManager;

		// Token: 0x04000E83 RID: 3715
		[Nullable(1)]
		private readonly List<ObjectsTabEntry> _objects = new List<ObjectsTabEntry>();

		// Token: 0x04000E84 RID: 3716
		[Nullable(1)]
		private List<ObjectsTab.ObjectsTabSelection> _selections = new List<ObjectsTab.ObjectsTabSelection>();

		// Token: 0x0200049B RID: 1179
		private enum ObjectsTabSelection
		{
			// Token: 0x04000E87 RID: 3719
			Grids,
			// Token: 0x04000E88 RID: 3720
			Maps,
			// Token: 0x04000E89 RID: 3721
			Stations
		}
	}
}