using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Maps
{
	// Token: 0x02000339 RID: 825
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("tile", 1)]
	public sealed class ContentTileDefinition : IPrototype, IInheritingPrototype, ITileDefinition
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0002003D File Offset: 0x0001E23D
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x00020045 File Offset: 0x0001E245
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ContentTileDefinition>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] private set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x0002004E File Offset: 0x0001E24E
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x00020056 File Offset: 0x0001E256
		[NeverPushInheritance]
		[AbstractDataField(1)]
		public bool Abstract { get; private set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x0002005F File Offset: 0x0001E25F
		[IdDataField(1, null)]
		public string ID { get; } = string.Empty;

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x00020067 File Offset: 0x0001E267
		// (set) Token: 0x06000999 RID: 2457 RVA: 0x0002006F File Offset: 0x0001E26F
		public ushort TileId { get; private set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x00020078 File Offset: 0x0001E278
		// (set) Token: 0x0600099B RID: 2459 RVA: 0x00020080 File Offset: 0x0001E280
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x0600099C RID: 2460 RVA: 0x00020089 File Offset: 0x0001E289
		[Nullable(2)]
		[DataField("sprite", false, 1, false, false, null)]
		public ResourcePath Sprite { [NullableContext(2)] get; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x00020091 File Offset: 0x0001E291
		[DataField("cornerSprites", false, 1, false, false, null)]
		public List<ResourcePath> CornerSprites { get; } = new List<ResourcePath>();

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x0600099E RID: 2462 RVA: 0x00020099 File Offset: 0x0001E299
		[DataField("cardinalSprites", false, 1, false, false, null)]
		public List<ResourcePath> CardinalSprites { get; } = new List<ResourcePath>();

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x000200A1 File Offset: 0x0001E2A1
		// (set) Token: 0x060009A0 RID: 2464 RVA: 0x000200A9 File Offset: 0x0001E2A9
		[DataField("isSubfloor", false, 1, false, false, null)]
		public bool IsSubFloor { get; private set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x000200B2 File Offset: 0x0001E2B2
		[DataField("baseTurfs", false, 1, false, false, null)]
		public List<string> BaseTurfs { get; } = new List<string>();

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x060009A2 RID: 2466 RVA: 0x000200BA File Offset: 0x0001E2BA
		// (set) Token: 0x060009A3 RID: 2467 RVA: 0x000200C2 File Offset: 0x0001E2C2
		[DataField("canCrowbar", false, 1, false, false, null)]
		public bool CanCrowbar { get; private set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x060009A4 RID: 2468 RVA: 0x000200CB File Offset: 0x0001E2CB
		// (set) Token: 0x060009A5 RID: 2469 RVA: 0x000200D3 File Offset: 0x0001E2D3
		[DataField("canWirecutter", false, 1, false, false, null)]
		public bool CanWirecutter { get; private set; }

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x060009A6 RID: 2470 RVA: 0x000200DC File Offset: 0x0001E2DC
		[Nullable(2)]
		[DataField("footstepSounds", false, 1, false, false, null)]
		public SoundSpecifier FootstepSounds { [NullableContext(2)] get; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x000200E4 File Offset: 0x0001E2E4
		[Nullable(2)]
		[DataField("barestepSounds", false, 1, false, false, null)]
		public SoundSpecifier BarestepSounds { [NullableContext(2)] get; } = new SoundCollectionSpecifier("BarestepHard", null);

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x060009A8 RID: 2472 RVA: 0x000200EC File Offset: 0x0001E2EC
		// (set) Token: 0x060009A9 RID: 2473 RVA: 0x000200F4 File Offset: 0x0001E2F4
		[DataField("friction", false, 1, false, false, null)]
		public float Friction { get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060009AA RID: 2474 RVA: 0x000200FD File Offset: 0x0001E2FD
		// (set) Token: 0x060009AB RID: 2475 RVA: 0x00020105 File Offset: 0x0001E305
		[DataField("variants", false, 1, false, false, null)]
		public byte Variants { get; set; } = 1;

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060009AC RID: 2476 RVA: 0x0002010E File Offset: 0x0001E30E
		// (set) Token: 0x060009AD RID: 2477 RVA: 0x00020116 File Offset: 0x0001E316
		[DataField("placementVariants", false, 1, false, false, null)]
		public byte[] PlacementVariants { get; set; } = new byte[1];

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060009AE RID: 2478 RVA: 0x0002011F File Offset: 0x0001E31F
		// (set) Token: 0x060009AF RID: 2479 RVA: 0x00020127 File Offset: 0x0001E327
		[DataField("thermalConductivity", false, 1, false, false, null)]
		public float ThermalConductivity { get; set; } = 0.05f;

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x00020130 File Offset: 0x0001E330
		[DataField("itemDrop", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ItemDropPrototypeName { get; } = "FloorTileItemSteel";

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x00020138 File Offset: 0x0001E338
		// (set) Token: 0x060009B2 RID: 2482 RVA: 0x00020140 File Offset: 0x0001E340
		[DataField("isSpace", false, 1, false, false, null)]
		public bool IsSpace { get; private set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x00020149 File Offset: 0x0001E349
		// (set) Token: 0x060009B4 RID: 2484 RVA: 0x00020151 File Offset: 0x0001E351
		[DataField("sturdy", false, 1, false, false, null)]
		public bool Sturdy { get; private set; } = true;

		// Token: 0x060009B5 RID: 2485 RVA: 0x0002015A File Offset: 0x0001E35A
		public void AssignTileId(ushort id)
		{
			this.TileId = id;
		}

		// Token: 0x04000962 RID: 2402
		public const string SpaceID = "Space";

		// Token: 0x04000963 RID: 2403
		private string _name = string.Empty;

		// Token: 0x04000976 RID: 2422
		[DataField("heatCapacity", false, 1, false, false, null)]
		public float HeatCapacity = 0.0003f;

		// Token: 0x0400097A RID: 2426
		[DataField("weather", false, 1, false, false, null)]
		public bool Weather;
	}
}
