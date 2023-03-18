using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Inventory;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Client.Clothing
{
	// Token: 0x020003B5 RID: 949
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientClothingSystem : ClothingSystem
	{
		// Token: 0x0600178D RID: 6029 RVA: 0x0008702C File Offset: 0x0008522C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClothingComponent, GetEquipmentVisualsEvent>(new ComponentEventHandler<ClothingComponent, GetEquipmentVisualsEvent>(this.OnGetVisuals), null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, VisualsChangedEvent>(new ComponentEventHandler<ClientInventoryComponent, VisualsChangedEvent>(this.OnVisualsChanged), null, null);
			base.SubscribeLocalEvent<SpriteComponent, DidUnequipEvent>(new ComponentEventHandler<SpriteComponent, DidUnequipEvent>(this.OnDidUnequip), null, null);
			base.SubscribeLocalEvent<ClientInventoryComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<ClientInventoryComponent, AppearanceChangeEvent>(this.OnAppearanceUpdate), null, null);
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x00087090 File Offset: 0x00085290
		private void OnAppearanceUpdate(EntityUid uid, ClientInventoryComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent spriteComponent;
			int num;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent) || !spriteComponent.LayerMapTryGet(HumanoidVisualLayers.StencilMask, ref num, false))
			{
				return;
			}
			HumanoidAppearanceComponent humanoidAppearanceComponent;
			EntityUid? entityUid;
			ClothingComponent clothingComponent;
			if (!base.TryComp<HumanoidAppearanceComponent>(uid, ref humanoidAppearanceComponent) || humanoidAppearanceComponent.Sex != Sex.Female || !this._inventorySystem.TryGetSlotEntity(uid, "jumpsuit", out entityUid, component, null) || !base.TryComp<ClothingComponent>(entityUid, ref clothingComponent))
			{
				spriteComponent.LayerSetVisible(num, false);
				return;
			}
			SpriteComponent spriteComponent2 = spriteComponent;
			int num2 = num;
			FemaleClothingMask femaleMask = clothingComponent.FemaleMask;
			string text;
			if (femaleMask != FemaleClothingMask.NoMask)
			{
				if (femaleMask != FemaleClothingMask.UniformTop)
				{
					text = "female_full";
				}
				else
				{
					text = "female_top";
				}
			}
			else
			{
				text = "female_none";
			}
			spriteComponent2.LayerSetState(num2, text);
			spriteComponent.LayerSetVisible(num, true);
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x00087148 File Offset: 0x00085348
		private void OnGetVisuals(EntityUid uid, ClothingComponent item, GetEquipmentVisualsEvent args)
		{
			ClientInventoryComponent clientInventoryComponent;
			if (!base.TryComp<ClientInventoryComponent>(args.Equipee, ref clientInventoryComponent))
			{
				return;
			}
			List<SharedSpriteComponent.PrototypeLayerData> list = null;
			if (clientInventoryComponent.SpeciesId != null)
			{
				item.ClothingVisuals.TryGetValue(args.Slot + "-" + clientInventoryComponent.SpeciesId, out list);
			}
			if (list == null && !item.ClothingVisuals.TryGetValue(args.Slot, out list) && !this.TryGetDefaultVisuals(uid, item, args.Slot, clientInventoryComponent.SpeciesId, out list))
			{
				return;
			}
			int num = 0;
			foreach (SharedSpriteComponent.PrototypeLayerData prototypeLayerData in list)
			{
				HashSet<string> mapKeys = prototypeLayerData.MapKeys;
				string text = (mapKeys != null) ? mapKeys.FirstOrDefault<string>() : null;
				if (text == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
					defaultInterpolatedStringHandler.AppendFormatted(args.Slot);
					defaultInterpolatedStringHandler.AppendLiteral("-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					text = defaultInterpolatedStringHandler.ToStringAndClear();
					num++;
				}
				args.Layers.Add(new ValueTuple<string, SharedSpriteComponent.PrototypeLayerData>(text, prototypeLayerData));
			}
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x00087268 File Offset: 0x00085468
		private bool TryGetDefaultVisuals(EntityUid uid, ClothingComponent clothing, string slot, [Nullable(2)] string speciesId, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out List<SharedSpriteComponent.PrototypeLayerData> layers)
		{
			layers = null;
			RSI rsi = null;
			SpriteComponent spriteComponent;
			if (clothing.RsiPath != null)
			{
				rsi = this._cache.GetResource<RSIResource>(SharedSpriteComponent.TextureRoot / clothing.RsiPath, true).RSI;
			}
			else if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				rsi = spriteComponent.BaseRSI;
			}
			if (rsi == null || rsi.Path == null)
			{
				return false;
			}
			string text = slot;
			ClientClothingSystem.TemporarySlotMap.TryGetValue(text, out text);
			string text2 = (clothing.EquippedPrefix == null) ? ("equipped-" + text) : (clothing.EquippedPrefix + "-equipped-" + text);
			RSI.State state;
			if (speciesId != null && rsi.TryGetState(text2 + "-" + speciesId, ref state))
			{
				text2 = text2 + "-" + speciesId;
			}
			else if (!rsi.TryGetState(text2, ref state))
			{
				return false;
			}
			SharedSpriteComponent.PrototypeLayerData prototypeLayerData = new SharedSpriteComponent.PrototypeLayerData();
			prototypeLayerData.RsiPath = rsi.Path.ToString();
			prototypeLayerData.State = text2;
			layers = new List<SharedSpriteComponent.PrototypeLayerData>
			{
				prototypeLayerData
			};
			return true;
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x00087374 File Offset: 0x00085574
		private void OnVisualsChanged(EntityUid uid, ClientInventoryComponent component, VisualsChangedEvent args)
		{
			ClothingComponent clothingComponent;
			if (!base.TryComp<ClothingComponent>(args.Item, ref clothingComponent) || clothingComponent.InSlot == null)
			{
				return;
			}
			this.RenderEquipment(uid, args.Item, clothingComponent.InSlot, component, null, clothingComponent);
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000873B0 File Offset: 0x000855B0
		private void OnDidUnequip(EntityUid uid, SpriteComponent component, DidUnequipEvent args)
		{
			ClientInventoryComponent clientInventoryComponent;
			SpriteComponent spriteComponent;
			if (!base.TryComp<ClientInventoryComponent>(uid, ref clientInventoryComponent) || !base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			HashSet<string> hashSet;
			if (!clientInventoryComponent.VisualLayerKeys.TryGetValue(args.Slot, out hashSet))
			{
				return;
			}
			foreach (string text in hashSet)
			{
				spriteComponent.RemoveLayer(text);
			}
			hashSet.Clear();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x00087434 File Offset: 0x00085634
		[NullableContext(2)]
		public void InitClothing(EntityUid uid, ClientInventoryComponent component = null, SpriteComponent sprite = null)
		{
			SlotDefinition[] array;
			if (!base.Resolve<SpriteComponent, ClientInventoryComponent>(uid, ref sprite, ref component, true) || !this._inventorySystem.TryGetSlots(uid, out array, component))
			{
				return;
			}
			foreach (SlotDefinition slotDefinition in array)
			{
				ContainerSlot containerSlot;
				SlotDefinition slotDefinition2;
				if (this._inventorySystem.TryGetSlotContainer(uid, slotDefinition.Name, out containerSlot, out slotDefinition2, component, null) && containerSlot.ContainedEntity != null)
				{
					this.RenderEquipment(uid, containerSlot.ContainedEntity.Value, slotDefinition.Name, component, sprite, null);
				}
			}
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x000874C1 File Offset: 0x000856C1
		protected override void OnGotEquipped(EntityUid uid, ClothingComponent component, GotEquippedEvent args)
		{
			base.OnGotEquipped(uid, component, args);
			this.RenderEquipment(args.Equipee, uid, args.Slot, null, null, component);
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x000874E4 File Offset: 0x000856E4
		[NullableContext(2)]
		private void RenderEquipment(EntityUid equipee, EntityUid equipment, [Nullable(1)] string slot, ClientInventoryComponent inventory = null, SpriteComponent sprite = null, ClothingComponent clothingComponent = null)
		{
			if (!base.Resolve<ClientInventoryComponent, SpriteComponent>(equipee, ref inventory, ref sprite, true) || !base.Resolve<ClothingComponent>(equipment, ref clothingComponent, false))
			{
				return;
			}
			int num;
			if (slot == "jumpsuit" && sprite.LayerMapTryGet(HumanoidVisualLayers.StencilMask, ref num, false))
			{
				HumanoidAppearanceComponent humanoidAppearanceComponent;
				if (base.TryComp<HumanoidAppearanceComponent>(equipee, ref humanoidAppearanceComponent) && humanoidAppearanceComponent.Sex == Sex.Female)
				{
					SpriteComponent spriteComponent = sprite;
					int num2 = num;
					FemaleClothingMask femaleMask = clothingComponent.FemaleMask;
					string text;
					if (femaleMask != FemaleClothingMask.NoMask)
					{
						if (femaleMask != FemaleClothingMask.UniformTop)
						{
							text = "female_full";
						}
						else
						{
							text = "female_top";
						}
					}
					else
					{
						text = "female_none";
					}
					spriteComponent.LayerSetState(num2, text);
					sprite.LayerSetVisible(num, true);
				}
				else
				{
					sprite.LayerSetVisible(num, false);
				}
			}
			SlotDefinition slotDefinition;
			if (!this._inventorySystem.TryGetSlot(equipee, slot, out slotDefinition, inventory))
			{
				return;
			}
			HashSet<string> hashSet;
			if (inventory.VisualLayerKeys.TryGetValue(slot, out hashSet))
			{
				foreach (string text2 in hashSet)
				{
					sprite.RemoveLayer(text2);
				}
				hashSet.Clear();
			}
			else
			{
				hashSet = new HashSet<string>();
				inventory.VisualLayerKeys[slot] = hashSet;
			}
			GetEquipmentVisualsEvent getEquipmentVisualsEvent = new GetEquipmentVisualsEvent(equipee, slot);
			base.RaiseLocalEvent<GetEquipmentVisualsEvent>(equipment, getEquipmentVisualsEvent, false);
			if (getEquipmentVisualsEvent.Layers.Count == 0)
			{
				base.RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, hashSet), true);
				return;
			}
			int num3;
			bool flag = sprite.LayerMapTryGet(slot, ref num3, false);
			foreach (ValueTuple<string, SharedSpriteComponent.PrototypeLayerData> valueTuple in getEquipmentVisualsEvent.Layers)
			{
				string item = valueTuple.Item1;
				SharedSpriteComponent.PrototypeLayerData item2 = valueTuple.Item2;
				if (!hashSet.Add(item))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(110, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Duplicate key for clothing visuals: ");
					defaultInterpolatedStringHandler.AppendFormatted(item);
					defaultInterpolatedStringHandler.AppendLiteral(". Are multiple components attempting to modify the same layer? Equipment: ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(equipment));
					Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					if (flag)
					{
						num3++;
						sprite.AddBlankLayer(new int?(num3));
						sprite.LayerMapSet(item, num3);
					}
					else
					{
						num3 = sprite.LayerMapReserveBlank(item);
					}
					SpriteComponent.Layer layer = sprite[num3] as SpriteComponent.Layer;
					if (layer == null)
					{
						return;
					}
					SpriteComponent spriteComponent2;
					if (item2.RsiPath == null && item2.TexturePath == null && layer.RSI == null && base.TryComp<SpriteComponent>(equipment, ref spriteComponent2))
					{
						layer.SetRsi(spriteComponent2.BaseRSI);
					}
					if (slot == "jumpsuit")
					{
						SharedSpriteComponent.PrototypeLayerData prototypeLayerData = item2;
						if (prototypeLayerData.Shader == null)
						{
							prototypeLayerData.Shader = "StencilDraw";
						}
					}
					sprite.LayerSetData(num3, item2);
					layer.Offset += slotDefinition.Offset;
				}
			}
			base.RaiseLocalEvent<EquipmentVisualsUpdatedEvent>(equipment, new EquipmentVisualsUpdatedEvent(equipee, slot, hashSet), true);
		}

		// Token: 0x04000C06 RID: 3078
		private static readonly Dictionary<string, string> TemporarySlotMap = new Dictionary<string, string>
		{
			{
				"head",
				"HELMET"
			},
			{
				"eyes",
				"EYES"
			},
			{
				"ears",
				"EARS"
			},
			{
				"mask",
				"MASK"
			},
			{
				"outerClothing",
				"OUTERCLOTHING"
			},
			{
				"jumpsuit",
				"INNERCLOTHING"
			},
			{
				"neck",
				"NECK"
			},
			{
				"back",
				"BACKPACK"
			},
			{
				"belt",
				"BELT"
			},
			{
				"gloves",
				"HAND"
			},
			{
				"shoes",
				"FEET"
			},
			{
				"id",
				"IDCARD"
			},
			{
				"pocket1",
				"POCKET1"
			},
			{
				"pocket2",
				"POCKET2"
			},
			{
				"suitstorage",
				"SUITSTORAGE"
			}
		};

		// Token: 0x04000C07 RID: 3079
		[Dependency]
		private readonly IResourceCache _cache;

		// Token: 0x04000C08 RID: 3080
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04000C09 RID: 3081
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
