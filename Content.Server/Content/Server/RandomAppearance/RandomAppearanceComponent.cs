using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RandomAppearance
{
	// Token: 0x02000256 RID: 598
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RandomAppearanceSystem)
	})]
	public sealed class RandomAppearanceComponent : Component, ISerializationHooks
	{
		// Token: 0x06000BDC RID: 3036 RVA: 0x0003E9B8 File Offset: 0x0003CBB8
		void ISerializationHooks.AfterDeserialization()
		{
			Enum @enum;
			if (IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(this.EnumKeyRaw, ref @enum, true))
			{
				this.EnumKey = @enum;
				return;
			}
			Logger.Error("RandomAppearance enum key " + this.EnumKeyRaw + " could not be parsed!");
		}

		// Token: 0x04000768 RID: 1896
		[Nullable(1)]
		[DataField("spriteStates", false, 1, false, false, null)]
		public string[] SpriteStates = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4"
		};

		// Token: 0x04000769 RID: 1897
		[Nullable(1)]
		[DataField("key", false, 1, true, false, null)]
		public string EnumKeyRaw;

		// Token: 0x0400076A RID: 1898
		[Nullable(2)]
		public Enum EnumKey;
	}
}
