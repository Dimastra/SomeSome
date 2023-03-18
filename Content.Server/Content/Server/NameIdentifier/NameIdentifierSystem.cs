using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.GameTicking;
using Content.Shared.NameIdentifier;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.NameIdentifier
{
	// Token: 0x0200038B RID: 907
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NameIdentifierSystem : EntitySystem
	{
		// Token: 0x0600129B RID: 4763 RVA: 0x00060714 File Offset: 0x0005E914
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NameIdentifierComponent, ComponentInit>(new ComponentEventHandler<NameIdentifierComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.CleanupIds), null, null);
			this.InitialSetupPrototypes();
			this._prototypeManager.PrototypesReloaded += this.OnReloadPrototypes;
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0006076C File Offset: 0x0005E96C
		public override void Shutdown()
		{
			base.Shutdown();
			this._prototypeManager.PrototypesReloaded -= this.OnReloadPrototypes;
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x0006078C File Offset: 0x0005E98C
		public string GenerateUniqueName(EntityUid uid, NameIdentifierGroupPrototype proto)
		{
			string entityName = base.Name(uid, null);
			HashSet<int> set;
			if (!this.CurrentIds.TryGetValue(proto, out set))
			{
				return entityName;
			}
			if (set.Count == proto.MaxValue - proto.MinValue + 1)
			{
				return entityName;
			}
			int randomVal = this._robustRandom.Next(proto.MinValue, proto.MaxValue);
			while (set.Contains(randomVal))
			{
				randomVal = this._robustRandom.Next(proto.MinValue, proto.MaxValue);
			}
			set.Add(randomVal);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (proto.Prefix == null)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<int>(randomVal);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(proto.Prefix);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(randomVal);
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00060864 File Offset: 0x0005EA64
		private void OnComponentInit(EntityUid uid, NameIdentifierComponent component, ComponentInit args)
		{
			NameIdentifierGroupPrototype group;
			if (!this._prototypeManager.TryIndex<NameIdentifierGroupPrototype>(component.Group, ref group))
			{
				return;
			}
			MetaDataComponent meta = base.MetaData(uid);
			string uniqueName = this.GenerateUniqueName(uid, group);
			meta.EntityName = (group.FullName ? uniqueName : (meta.EntityName + " (" + uniqueName + ")"));
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x000608C0 File Offset: 0x0005EAC0
		private void InitialSetupPrototypes()
		{
			foreach (NameIdentifierGroupPrototype proto in this._prototypeManager.EnumeratePrototypes<NameIdentifierGroupPrototype>())
			{
				this.CurrentIds.Add(proto, new HashSet<int>());
			}
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x0006091C File Offset: 0x0005EB1C
		private void OnReloadPrototypes(PrototypesReloadedEventArgs ev)
		{
			PrototypesReloadedEventArgs.PrototypeChangeSet set;
			if (!ev.ByType.TryGetValue(typeof(NameIdentifierGroupPrototype), out set))
			{
				return;
			}
			foreach (KeyValuePair<string, IPrototype> keyValuePair in set.Modified)
			{
				string text;
				IPrototype prototype;
				keyValuePair.Deconstruct(out text, out prototype);
				NameIdentifierGroupPrototype group = prototype as NameIdentifierGroupPrototype;
				if (group != null && !this.CurrentIds.ContainsKey(group))
				{
					this.CurrentIds.Add(group, new HashSet<int>());
				}
			}
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000609B8 File Offset: 0x0005EBB8
		private void CleanupIds(RoundRestartCleanupEvent ev)
		{
			foreach (KeyValuePair<NameIdentifierGroupPrototype, HashSet<int>> keyValuePair in this.CurrentIds)
			{
				NameIdentifierGroupPrototype nameIdentifierGroupPrototype;
				HashSet<int> hashSet;
				keyValuePair.Deconstruct(out nameIdentifierGroupPrototype, out hashSet);
				hashSet.Clear();
			}
		}

		// Token: 0x04000B6D RID: 2925
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000B6E RID: 2926
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000B6F RID: 2927
		[ViewVariables]
		public Dictionary<NameIdentifierGroupPrototype, HashSet<int>> CurrentIds = new Dictionary<NameIdentifierGroupPrototype, HashSet<int>>();
	}
}
