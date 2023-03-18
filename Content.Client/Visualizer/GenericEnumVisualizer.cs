using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Visualizer
{
	// Token: 0x02000053 RID: 83
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GenericEnumVisualizer : AppearanceVisualizer, ISerializationHooks
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600017E RID: 382 RVA: 0x0000BE49 File Offset: 0x0000A049
		// (set) Token: 0x0600017F RID: 383 RVA: 0x0000BE51 File Offset: 0x0000A051
		public Enum Key { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000BE5A File Offset: 0x0000A05A
		// (set) Token: 0x06000181 RID: 385 RVA: 0x0000BE62 File Offset: 0x0000A062
		public Dictionary<object, string> States { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000BE6B File Offset: 0x0000A06B
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000BE73 File Offset: 0x0000A073
		[DataField("layer", false, 1, false, false, null)]
		public int Layer { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000BE7C File Offset: 0x0000A07C
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000BE84 File Offset: 0x0000A084
		[DataField("states", true, 1, true, false, null)]
		private Dictionary<string, string> _statesRaw { get; set; }

		// Token: 0x06000186 RID: 390 RVA: 0x0000BE90 File Offset: 0x0000A090
		void ISerializationHooks.AfterDeserialization()
		{
			GenericEnumVisualizer.<>c__DisplayClass17_0 CS$<>8__locals1 = new GenericEnumVisualizer.<>c__DisplayClass17_0();
			CS$<>8__locals1.reflectionManager = IoCManager.Resolve<IReflectionManager>();
			this.Key = (Enum)CS$<>8__locals1.<Robust.Shared.Serialization.ISerializationHooks.AfterDeserialization>g__ResolveRef|0(this._keyRaw);
			this.States = this._statesRaw.ToDictionary((KeyValuePair<string, string> kvp) => base.<Robust.Shared.Serialization.ISerializationHooks.AfterDeserialization>g__ResolveRef|0(kvp.Key), (KeyValuePair<string, string> kvp) => kvp.Value);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000BF04 File Offset: 0x0000A104
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			object key;
			if (!component.TryGetData<object>(this.Key, ref key))
			{
				return;
			}
			string text;
			if (!this.States.TryGetValue(key, out text))
			{
				return;
			}
			spriteComponent.LayerSetState(this.Layer, text);
		}

		// Token: 0x040000FF RID: 255
		[DataField("key", true, 1, true, false, null)]
		private string _keyRaw;
	}
}
