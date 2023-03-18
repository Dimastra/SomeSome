using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Body.UI
{
	// Token: 0x0200041D RID: 1053
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BodyScannerDisplay : DefaultWindow
	{
		// Token: 0x060019CE RID: 6606 RVA: 0x00093FA8 File Offset: 0x000921A8
		public BodyScannerDisplay(BodyScannerBoundUserInterface owner)
		{
			IoCManager.InjectDependencies<BodyScannerDisplay>(this);
			this.Owner = owner;
			base.Title = Loc.GetString("body-scanner-display-title");
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			Control.OrderedChildCollection children = boxContainer.Children;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.HorizontalExpand = true;
			scrollContainer.Children.Add(this.BodyPartList = new ItemList());
			children.Add(scrollContainer);
			Control.OrderedChildCollection children2 = boxContainer.Children;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.HorizontalExpand = true;
			Control.OrderedChildCollection children3 = boxContainer2.Children;
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.VerticalExpand = true;
			boxContainer3.Children.Add(this.BodyPartLabel = new Label());
			Control.OrderedChildCollection children4 = boxContainer3.Children;
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			boxContainer4.Children.Add(new Label
			{
				Text = Loc.GetString("body-scanner-display-health-label") + " "
			});
			boxContainer4.Children.Add(this.BodyPartHealth = new Label());
			children4.Add(boxContainer4);
			Control.OrderedChildCollection children5 = boxContainer3.Children;
			ScrollContainer scrollContainer2 = new ScrollContainer();
			scrollContainer2.VerticalExpand = true;
			scrollContainer2.Children.Add(this.MechanismList = new ItemList());
			children5.Add(scrollContainer2);
			children3.Add(boxContainer3);
			Control.OrderedChildCollection children6 = boxContainer2.Children;
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.VerticalExpand = true;
			RichTextLabel richTextLabel2 = richTextLabel;
			this.MechanismInfoLabel = richTextLabel;
			children6.Add(richTextLabel2);
			children2.Add(boxContainer2);
			BoxContainer boxContainer5 = boxContainer;
			base.Contents.AddChild(boxContainer5);
			this.BodyPartList.OnItemSelected += this.BodyPartOnItemSelected;
			this.MechanismList.OnItemSelected += this.MechanismOnItemSelected;
			base.MinSize = (base.SetSize = new ValueTuple<float, float>(800f, 600f));
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060019CF RID: 6607 RVA: 0x0009417F File Offset: 0x0009237F
		public BodyScannerBoundUserInterface Owner { get; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x00094187 File Offset: 0x00092387
		private ItemList BodyPartList { get; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x060019D1 RID: 6609 RVA: 0x0009418F File Offset: 0x0009238F
		private Label BodyPartLabel { get; }

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x00094197 File Offset: 0x00092397
		private Label BodyPartHealth { get; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060019D3 RID: 6611 RVA: 0x0009419F File Offset: 0x0009239F
		private ItemList MechanismList { get; }

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x060019D4 RID: 6612 RVA: 0x000941A7 File Offset: 0x000923A7
		private RichTextLabel MechanismInfoLabel { get; }

		// Token: 0x060019D5 RID: 6613 RVA: 0x000941B0 File Offset: 0x000923B0
		public void UpdateDisplay(EntityUid entity)
		{
			this._currentEntity = new EntityUid?(entity);
			this.BodyPartList.Clear();
			this._bodyPartsList.Clear();
			SharedBodySystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedBodySystem>();
			IComponentFactory componentFactory = IoCManager.Resolve<IComponentFactory>();
			int num = 0;
			foreach (ValueTuple<EntityUid, BodyPartComponent> valueTuple in entitySystem.GetBodyChildren(this._currentEntity, null))
			{
				this._bodyPartsList[num++] = valueTuple.Item2.ParentSlot;
				this.BodyPartList.AddItem(Loc.GetString(componentFactory.GetComponentName(valueTuple.Item2.GetType())), null, true);
			}
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x00094270 File Offset: 0x00092470
		public void BodyPartOnItemSelected(ItemList.ItemListSelectedEventArgs args)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			this._currentBodyPart = EntityManagerExt.GetComponentOrNull<BodyPartComponent>(entityManager, this._bodyPartsList[args.ItemIndex].Child);
			BodyPartComponent currentBodyPart = this._currentBodyPart;
			if (currentBodyPart != null)
			{
				BodyPartSlot parentSlot = currentBodyPart.ParentSlot;
				if (parentSlot != null)
				{
					string id = parentSlot.Id;
					this.UpdateBodyPartBox(currentBodyPart, id);
				}
			}
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x000942C8 File Offset: 0x000924C8
		private void UpdateBodyPartBox(BodyPartComponent part, string slotName)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			this.BodyPartLabel.Text = Loc.GetString(slotName) + ": " + Loc.GetString(entityManager.GetComponent<MetaDataComponent>(part.Owner).EntityName);
			DamageableComponent damageableComponent;
			if (entityManager.TryGetComponent<DamageableComponent>(part.Owner, ref damageableComponent))
			{
				this.BodyPartHealth.Text = Loc.GetString("body-scanner-display-body-part-damage-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("damage", damageableComponent.TotalDamage)
				});
			}
			this.MechanismList.Clear();
			foreach (ValueTuple<EntityUid, OrganComponent> valueTuple in entityManager.System<SharedBodySystem>().GetPartOrgans(new EntityUid?(part.Owner), part))
			{
				string entityName = entityManager.GetComponent<MetaDataComponent>(valueTuple.Item1).EntityName;
				this.MechanismList.AddItem(entityName, null, true);
			}
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x000943CC File Offset: 0x000925CC
		public void MechanismOnItemSelected(ItemList.ItemListSelectedEventArgs args)
		{
			if (this._currentBodyPart == null)
			{
				this.UpdateMechanismBox(null);
				return;
			}
			ValueTuple<EntityUid, OrganComponent> valueTuple = IoCManager.Resolve<IEntityManager>().System<SharedBodySystem>().GetPartOrgans(new EntityUid?(this._currentBodyPart.Owner), this._currentBodyPart).ElementAt(args.ItemIndex);
			this.UpdateMechanismBox(new EntityUid?(valueTuple.Item1));
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x00094434 File Offset: 0x00092634
		private void UpdateMechanismBox(EntityUid? organ)
		{
			if (organ == null)
			{
				this.MechanismInfoLabel.SetMessage("");
				return;
			}
			string @string = Loc.GetString(IoCManager.Resolve<IEntityManager>().GetComponent<MetaDataComponent>(organ.Value).EntityName ?? "");
			this.MechanismInfoLabel.SetMessage(@string);
		}

		// Token: 0x04000D14 RID: 3348
		private EntityUid? _currentEntity;

		// Token: 0x04000D15 RID: 3349
		[Nullable(2)]
		private BodyPartComponent _currentBodyPart;

		// Token: 0x04000D16 RID: 3350
		private readonly Dictionary<int, BodyPartSlot> _bodyPartsList = new Dictionary<int, BodyPartSlot>();
	}
}
