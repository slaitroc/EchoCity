using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoCity
{
    public class QuestContainer : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOEventVoid questListChangedEvent;
        [SerializeField] private SOStringEvent questCompletedEvent;

        [Header("Tasks")]
        [SerializeField] private List<QuestEntry> quests = new();


        public IReadOnlyList<QuestEntry> Quests => quests;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => false;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };

        public void Clear()
        {
            quests.Clear();
            questListChangedEvent?.RaiseEvent(this);
        }

        public void AddOrUpdateTask(string id, string text)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var t = FindTask(id);
            if (t == null)
            {
                quests.Add(new QuestEntry { Id = id, Text = text, Completed = false, PendingRemoval = false });
            }
            else
            {
                t.Text = text;
            }

            questListChangedEvent?.RaiseEvent(this);
        }

        public void CompleteTask(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var t = FindTask(id);
            if (t == null) return;

            if (!t.Completed)
            {
                t.Completed = true;
                t.PendingRemoval = true;
                questCompletedEvent?.RaiseEvent(this, id);
                questListChangedEvent?.RaiseEvent(this);
            }
        }

        public void RemoveTask(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            quests.RemoveAll(x => x.Id == id);
            questListChangedEvent?.RaiseEvent(this);
        }

        public QuestEntry FindTask(string id)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (quests[i] != null && quests[i].Id == id)
                    return quests[i];
            }
            return null;
        }

        public bool IsCompleted(string id)
        {
            var t = FindTask(id);
            return t != null && t.Completed;
        }
    }
}
