using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Objectives;
using Content.Server.Objectives.Interfaces;
using Content.Server.Roles;
using Content.Shared.CharacterInfo;
using Content.Shared.Objectives;
using Robust.Shared.GameObjects;

namespace Content.Server.CharacterInfo
{
	// Token: 0x020006D8 RID: 1752
	public sealed class CharacterInfoSystem : EntitySystem
	{
		// Token: 0x06002491 RID: 9361 RVA: 0x000BE54A File Offset: 0x000BC74A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<RequestCharacterInfoEvent>(new EntitySessionEventHandler<RequestCharacterInfoEvent>(this.OnRequestCharacterInfoEvent), null, null);
		}

		// Token: 0x06002492 RID: 9362 RVA: 0x000BE568 File Offset: 0x000BC768
		[NullableContext(1)]
		private void OnRequestCharacterInfoEvent(RequestCharacterInfoEvent msg, EntitySessionEventArgs args)
		{
			if (args.SenderSession.AttachedEntity != null)
			{
				EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
				EntityUid entityUid = msg.EntityUid;
				if (attachedEntity != null && (attachedEntity == null || !(attachedEntity.GetValueOrDefault() != entityUid)))
				{
					EntityUid entity = args.SenderSession.AttachedEntity.Value;
					Dictionary<string, List<ConditionInfo>> conditions = new Dictionary<string, List<ConditionInfo>>();
					string jobTitle = "No Profession";
					string briefing = "!!ERROR: No Briefing!!";
					MindComponent mindComponent;
					if (this.EntityManager.TryGetComponent<MindComponent>(entity, ref mindComponent) && mindComponent.Mind != null)
					{
						Mind mind = mindComponent.Mind;
						foreach (Objective objective in mind.AllObjectives)
						{
							if (!conditions.ContainsKey(objective.Prototype.Issuer))
							{
								conditions[objective.Prototype.Issuer] = new List<ConditionInfo>();
							}
							foreach (IObjectiveCondition condition in objective.Conditions)
							{
								conditions[objective.Prototype.Issuer].Add(new ConditionInfo(condition.Title, condition.Description, condition.Icon, condition.Progress));
							}
						}
						foreach (Role role in mind.AllRoles)
						{
							if (!(role.GetType() != typeof(Job)))
							{
								jobTitle = role.Name;
								break;
							}
						}
						briefing = mind.Briefing;
					}
					base.RaiseNetworkEvent(new CharacterInfoEvent(entity, jobTitle, conditions, briefing), args.SenderSession);
					return;
				}
			}
		}
	}
}
