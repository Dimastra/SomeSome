using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Instruments;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Instruments
{
	// Token: 0x0200044D RID: 1101
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SwappableInstrumentSystem : EntitySystem
	{
		// Token: 0x06001636 RID: 5686 RVA: 0x000754CC File Offset: 0x000736CC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SwappableInstrumentComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SwappableInstrumentComponent, GetVerbsEvent<AlternativeVerb>>(this.AddStyleVerb), null, null);
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x000754E8 File Offset: 0x000736E8
		private void AddStyleVerb(EntityUid uid, SwappableInstrumentComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess || component.InstrumentList.Count <= 1)
			{
				return;
			}
			SharedInstrumentComponent instrument;
			if (!base.TryComp<SharedInstrumentComponent>(uid, ref instrument))
			{
				return;
			}
			int priority = 0;
			using (Dictionary<string, ValueTuple<byte, byte>>.Enumerator enumerator = component.InstrumentList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ValueTuple<byte, byte>> entry = enumerator.Current;
					AlternativeVerb selection = new AlternativeVerb
					{
						Text = entry.Key,
						Category = VerbCategory.InstrumentStyle,
						Priority = priority,
						Act = delegate()
						{
							this._sharedInstrument.SetInstrumentProgram(instrument, entry.Value.Item1, entry.Value.Item2);
							this._popup.PopupEntity(Loc.GetString("swappable-instrument-component-style-set", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("style", entry.Key)
							}), args.User, args.User, PopupType.Small);
						}
					};
					priority--;
					args.Verbs.Add(selection);
				}
			}
		}

		// Token: 0x04000DEB RID: 3563
		[Dependency]
		private readonly SharedInstrumentSystem _sharedInstrument;

		// Token: 0x04000DEC RID: 3564
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
