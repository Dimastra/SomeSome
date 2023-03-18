using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008E RID: 142
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VerbCategory
	{
		// Token: 0x060001B1 RID: 433 RVA: 0x00009534 File Offset: 0x00007734
		public VerbCategory(string text, [Nullable(2)] string icon, bool iconsOnly = false)
		{
			this.Text = Loc.GetString(text);
			this.Icon = ((icon == null) ? null : new SpriteSpecifier.Texture(new ResourcePath(icon, "/")));
			this.IconsOnly = iconsOnly;
		}

		// Token: 0x040001D2 RID: 466
		public readonly string Text;

		// Token: 0x040001D3 RID: 467
		[Nullable(2)]
		public readonly SpriteSpecifier Icon;

		// Token: 0x040001D4 RID: 468
		public int Columns = 1;

		// Token: 0x040001D5 RID: 469
		public readonly bool IconsOnly;

		// Token: 0x040001D6 RID: 470
		public static readonly VerbCategory Admin = new VerbCategory("verb-categories-admin", "/Textures/Interface/character.svg.192dpi.png", false);

		// Token: 0x040001D7 RID: 471
		public static readonly VerbCategory MeatyOre = new VerbCategory("MeatyOre", null, false);

		// Token: 0x040001D8 RID: 472
		public static readonly VerbCategory Antag = new VerbCategory("verb-categories-antag", "/Textures/Interface/VerbIcons/antag-e_sword-temp.192dpi.png", true)
		{
			Columns = 5
		};

		// Token: 0x040001D9 RID: 473
		public static readonly VerbCategory Examine = new VerbCategory("verb-categories-examine", "/Textures/Interface/VerbIcons/examine.svg.192dpi.png", false);

		// Token: 0x040001DA RID: 474
		public static readonly VerbCategory Debug = new VerbCategory("verb-categories-debug", "/Textures/Interface/VerbIcons/debug.svg.192dpi.png", false);

		// Token: 0x040001DB RID: 475
		public static readonly VerbCategory Eject = new VerbCategory("verb-categories-eject", "/Textures/Interface/VerbIcons/eject.svg.192dpi.png", false);

		// Token: 0x040001DC RID: 476
		public static readonly VerbCategory Insert = new VerbCategory("verb-categories-insert", "/Textures/Interface/VerbIcons/insert.svg.192dpi.png", false);

		// Token: 0x040001DD RID: 477
		public static readonly VerbCategory Buckle = new VerbCategory("verb-categories-buckle", "/Textures/Interface/VerbIcons/buckle.svg.192dpi.png", false);

		// Token: 0x040001DE RID: 478
		public static readonly VerbCategory Unbuckle = new VerbCategory("verb-categories-unbuckle", "/Textures/Interface/VerbIcons/unbuckle.svg.192dpi.png", false);

		// Token: 0x040001DF RID: 479
		public static readonly VerbCategory Rotate = new VerbCategory("verb-categories-rotate", "/Textures/Interface/VerbIcons/refresh.svg.192dpi.png", true)
		{
			Columns = 5
		};

		// Token: 0x040001E0 RID: 480
		public static readonly VerbCategory Smite = new VerbCategory("verb-categories-smite", "/Textures/Interface/VerbIcons/smite.svg.192dpi.png", true)
		{
			Columns = 6
		};

		// Token: 0x040001E1 RID: 481
		public static readonly VerbCategory Tricks = new VerbCategory("verb-categories-tricks", "/Textures/Interface/AdminActions/tricks.png", true)
		{
			Columns = 5
		};

		// Token: 0x040001E2 RID: 482
		public static readonly VerbCategory SetTransferAmount = new VerbCategory("verb-categories-transfer", "/Textures/Interface/VerbIcons/spill.svg.192dpi.png", false);

		// Token: 0x040001E3 RID: 483
		public static readonly VerbCategory Split = new VerbCategory("verb-categories-split", null, false);

		// Token: 0x040001E4 RID: 484
		public static readonly VerbCategory InstrumentStyle = new VerbCategory("verb-categories-instrument-style", null, false);

		// Token: 0x040001E5 RID: 485
		public static readonly VerbCategory ChannelSelect = new VerbCategory("verb-categories-channel-select", null, false);

		// Token: 0x040001E6 RID: 486
		public static readonly VerbCategory SetSensor = new VerbCategory("verb-categories-set-sensor", null, false);

		// Token: 0x040001E7 RID: 487
		public static readonly VerbCategory Lever = new VerbCategory("verb-categories-lever", null, false);

		// Token: 0x040001E8 RID: 488
		public static readonly VerbCategory SelectType = new VerbCategory("verb-categories-select-type", null, false);
	}
}
