using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.CharacterInfo
{
	// Token: 0x020003EB RID: 1003
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class CharacterInfoSystem : EntitySystem
	{
		// Token: 0x14000091 RID: 145
		// (add) Token: 0x0600189F RID: 6303 RVA: 0x0008DFD8 File Offset: 0x0008C1D8
		// (remove) Token: 0x060018A0 RID: 6304 RVA: 0x0008E010 File Offset: 0x0008C210
		public event Action<CharacterInfoSystem.CharacterData> OnCharacterUpdate;

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x060018A1 RID: 6305 RVA: 0x0008E048 File Offset: 0x0008C248
		// (remove) Token: 0x060018A2 RID: 6306 RVA: 0x0008E080 File Offset: 0x0008C280
		public event Action OnCharacterDetached;

		// Token: 0x060018A3 RID: 6307 RVA: 0x0008E0B5 File Offset: 0x0008C2B5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlayerAttachSysMessage>(new EntityEventHandler<PlayerAttachSysMessage>(this.OnPlayerAttached), null, null);
			base.SubscribeNetworkEvent<CharacterInfoEvent>(new EntitySessionEventHandler<CharacterInfoEvent>(this.OnCharacterInfoEvent), null, null);
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x0008E0E8 File Offset: 0x0008C2E8
		public void RequestCharacterInfo()
		{
			LocalPlayer localPlayer = this._players.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			base.RaiseNetworkEvent(new RequestCharacterInfoEvent(entityUid.Value));
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x0008E134 File Offset: 0x0008C334
		[NullableContext(1)]
		private void OnPlayerAttached(PlayerAttachSysMessage msg)
		{
			if (msg.AttachedEntity == default(EntityUid))
			{
				Action onCharacterDetached = this.OnCharacterDetached;
				if (onCharacterDetached == null)
				{
					return;
				}
				onCharacterDetached();
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x0008E168 File Offset: 0x0008C368
		[NullableContext(1)]
		private void OnCharacterInfoEvent(CharacterInfoEvent msg, EntitySessionEventArgs args)
		{
			SpriteComponent sprite = base.CompOrNull<SpriteComponent>(msg.EntityUid);
			CharacterInfoSystem.CharacterData obj = new CharacterInfoSystem.CharacterData(msg.JobTitle, msg.Objectives, msg.Briefing, sprite, base.Name(msg.EntityUid, null));
			Action<CharacterInfoSystem.CharacterData> onCharacterUpdate = this.OnCharacterUpdate;
			if (onCharacterUpdate == null)
			{
				return;
			}
			onCharacterUpdate(obj);
		}

		// Token: 0x04000C94 RID: 3220
		[Nullable(1)]
		[Dependency]
		private readonly IPlayerManager _players;

		// Token: 0x020003EC RID: 1004
		[NullableContext(1)]
		[Nullable(0)]
		public readonly struct CharacterData : IEquatable<CharacterInfoSystem.CharacterData>
		{
			// Token: 0x060018A8 RID: 6312 RVA: 0x0008E1BA File Offset: 0x0008C3BA
			public CharacterData(string Job, Dictionary<string, List<ConditionInfo>> Objectives, string Briefing, [Nullable(2)] SpriteComponent Sprite, string EntityName)
			{
				this.Job = Job;
				this.Objectives = Objectives;
				this.Briefing = Briefing;
				this.Sprite = Sprite;
				this.EntityName = EntityName;
			}

			// Token: 0x17000511 RID: 1297
			// (get) Token: 0x060018A9 RID: 6313 RVA: 0x0008E1E1 File Offset: 0x0008C3E1
			// (set) Token: 0x060018AA RID: 6314 RVA: 0x0008E1E9 File Offset: 0x0008C3E9
			public string Job { get; set; }

			// Token: 0x17000512 RID: 1298
			// (get) Token: 0x060018AB RID: 6315 RVA: 0x0008E1F2 File Offset: 0x0008C3F2
			// (set) Token: 0x060018AC RID: 6316 RVA: 0x0008E1FA File Offset: 0x0008C3FA
			public Dictionary<string, List<ConditionInfo>> Objectives { get; set; }

			// Token: 0x17000513 RID: 1299
			// (get) Token: 0x060018AD RID: 6317 RVA: 0x0008E203 File Offset: 0x0008C403
			// (set) Token: 0x060018AE RID: 6318 RVA: 0x0008E20B File Offset: 0x0008C40B
			public string Briefing { get; set; }

			// Token: 0x17000514 RID: 1300
			// (get) Token: 0x060018AF RID: 6319 RVA: 0x0008E214 File Offset: 0x0008C414
			// (set) Token: 0x060018B0 RID: 6320 RVA: 0x0008E21C File Offset: 0x0008C41C
			[Nullable(2)]
			public SpriteComponent Sprite { [NullableContext(2)] get; [NullableContext(2)] set; }

			// Token: 0x17000515 RID: 1301
			// (get) Token: 0x060018B1 RID: 6321 RVA: 0x0008E225 File Offset: 0x0008C425
			// (set) Token: 0x060018B2 RID: 6322 RVA: 0x0008E22D File Offset: 0x0008C42D
			public string EntityName { get; set; }

			// Token: 0x060018B3 RID: 6323 RVA: 0x0008E238 File Offset: 0x0008C438
			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CharacterData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060018B4 RID: 6324 RVA: 0x0008E284 File Offset: 0x0008C484
			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Job = ");
				builder.Append(this.Job);
				builder.Append(", Objectives = ");
				builder.Append(this.Objectives);
				builder.Append(", Briefing = ");
				builder.Append(this.Briefing);
				builder.Append(", Sprite = ");
				builder.Append(this.Sprite);
				builder.Append(", EntityName = ");
				builder.Append(this.EntityName);
				return true;
			}

			// Token: 0x060018B5 RID: 6325 RVA: 0x0008E30F File Offset: 0x0008C50F
			[CompilerGenerated]
			public static bool operator !=(CharacterInfoSystem.CharacterData left, CharacterInfoSystem.CharacterData right)
			{
				return !(left == right);
			}

			// Token: 0x060018B6 RID: 6326 RVA: 0x0008E31B File Offset: 0x0008C51B
			[CompilerGenerated]
			public static bool operator ==(CharacterInfoSystem.CharacterData left, CharacterInfoSystem.CharacterData right)
			{
				return left.Equals(right);
			}

			// Token: 0x060018B7 RID: 6327 RVA: 0x0008E328 File Offset: 0x0008C528
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<string>.Default.GetHashCode(this.<Job>k__BackingField) * -1521134295 + EqualityComparer<Dictionary<string, List<ConditionInfo>>>.Default.GetHashCode(this.<Objectives>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<Briefing>k__BackingField)) * -1521134295 + EqualityComparer<SpriteComponent>.Default.GetHashCode(this.<Sprite>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<EntityName>k__BackingField);
			}

			// Token: 0x060018B8 RID: 6328 RVA: 0x0008E3A1 File Offset: 0x0008C5A1
			[NullableContext(0)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is CharacterInfoSystem.CharacterData && this.Equals((CharacterInfoSystem.CharacterData)obj);
			}

			// Token: 0x060018B9 RID: 6329 RVA: 0x0008E3BC File Offset: 0x0008C5BC
			[CompilerGenerated]
			public bool Equals(CharacterInfoSystem.CharacterData other)
			{
				return EqualityComparer<string>.Default.Equals(this.<Job>k__BackingField, other.<Job>k__BackingField) && EqualityComparer<Dictionary<string, List<ConditionInfo>>>.Default.Equals(this.<Objectives>k__BackingField, other.<Objectives>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<Briefing>k__BackingField, other.<Briefing>k__BackingField) && EqualityComparer<SpriteComponent>.Default.Equals(this.<Sprite>k__BackingField, other.<Sprite>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<EntityName>k__BackingField, other.<EntityName>k__BackingField);
			}

			// Token: 0x060018BA RID: 6330 RVA: 0x0008E441 File Offset: 0x0008C641
			[CompilerGenerated]
			public void Deconstruct(out string Job, out Dictionary<string, List<ConditionInfo>> Objectives, out string Briefing, [Nullable(2)] out SpriteComponent Sprite, out string EntityName)
			{
				Job = this.Job;
				Objectives = this.Objectives;
				Briefing = this.Briefing;
				Sprite = this.Sprite;
				EntityName = this.EntityName;
			}
		}
	}
}
