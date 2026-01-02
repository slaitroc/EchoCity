using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class QuestController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIDocument hudDocument;
        [SerializeField] private QuestsManager questsManager;
        private IQuestsManager _questsManager => questsManager as IQuestsManager;

        [SerializeField] private float completedVisibleSeconds = 5.0f;
        [SerializeField] private float fadeOutSeconds = 2.0f;

        #region Visual Elements Management
        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _list;

        private struct RowCache
        {
            public int Index;
            public QuestsEnum questID;
            public int progression;
            public SOQuest Quest;
        }

        private readonly Dictionary<QuestsEnum, VisualElement> _rows = new();
        private readonly Dictionary<QuestsEnum, Coroutine> _fadeCoroutines = new();
        private readonly List<RowCache> _cache = new();
        private int _index = 0;
        #endregion



        private void OnEnable()
        {
            if (hudDocument == null) return;

            _root = hudDocument.rootVisualElement;
            StartCoroutine(InitNextFrame());
        }

        private IEnumerator InitNextFrame()
        {
            _panel = _root.Q<VisualElement>("QuestLoggerPanel");
            _list = _root.Q<VisualElement>("QuestLoggerList");

            yield return null;

            if (_list == null || _panel == null) yield break;

            RebuildFromCache();
            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility() => _panel.style.display = _rows.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        private void RebuildFromCache()
        {
            _list.Clear();
            _rows.Clear();

            _cache.Sort((a, b) => a.Index.CompareTo(b.Index));

            for (int i = 0; i < _cache.Count; i++)
            {
                var c = _cache[i];

                UpdateQuest(c.questID, c.progression);
            }

            UpdatePanelVisibility();
        }

        private void CacheRows(QuestsEnum id, int progression, SOQuest quest)
        {
            for (int i = 0; i < _cache.Count; i++)
                if (_cache[i].questID.Equals(id))
                {
                    _cache[i] = new RowCache { Index = _cache[i].Index, questID = id, progression = progression, Quest = quest };
                    return;
                }

            _cache.Add(new RowCache { Index = _index++, questID = id, progression = progression, Quest = quest });
            _cache.Sort((a, b) => a.Index.CompareTo(b.Index));
        }

        public void UpdateQuest(QuestsEnum questID, int progression)
        {
            SOQuest quest = _questsManager.ActiveQuests[(int)questID];
            CacheRows(questID, progression, quest);

            if (progression == (int)QuestStateEnum.Inactive)
            {
                RemoveRowImmediate(questID);
                _cache.RemoveAll(c => c.Quest == quest);
                UpdatePanelVisibility();
                return;
            }
            else if (progression == (int)QuestStateEnum.Completed)
            {
                StartFade(questID);
                return;
            }

            string counterText = "";

            if (quest.CountToComplete > 1) counterText = $" ({progression}/{quest.CountToComplete})";
            UpdateRow(questID, quest.Description + counterText, false);
            UpdatePanelVisibility();
        }

        private void UpdateRow(QuestsEnum questID, string text, bool completed)
        {
            if (_rows.TryGetValue(questID, out var row))
            {
                var label = row.Q<Label>(className: "quest-text");
                label.text = text;
            }
            else
            {
                var newRow = BuildRow(text, completed);
                _list.Add(newRow);
                _rows[questID] = newRow;
            }
        }

        private VisualElement BuildRow(string text, bool completed)
        {
            var row = new VisualElement();
            row.AddToClassList("quest-item");

            var check = new VisualElement();
            check.AddToClassList("quest-check");

            var label = new Label(text);
            label.AddToClassList("quest-text");

            row.Add(check);
            row.Add(label);

            if (completed)
                row.AddToClassList("completed");

            return row;
        }

        private void StartFade(QuestsEnum id)
        {
            if (_fadeCoroutines.ContainsKey(id)) return;
            if (!_rows.TryGetValue(id, out var row)) return;

            if (!row.ClassListContains("completed"))
                row.AddToClassList("completed");

            _fadeCoroutines[id] = StartCoroutine(FadeAndHide(id, row));
        }

        private IEnumerator FadeAndHide(QuestsEnum id, VisualElement row)
        {
            yield return new WaitForSeconds(completedVisibleSeconds);

            row.AddToClassList("fade-out");

            yield return new WaitForSeconds(fadeOutSeconds);

            row.RemoveFromHierarchy();
            _rows.Remove(id);

            if (_fadeCoroutines.TryGetValue(id, out var c) && c != null)
                _fadeCoroutines.Remove(id);

            UpdatePanelVisibility();
        }

        private void RemoveRowImmediate(QuestsEnum id)
        {
            if (_fadeCoroutines.TryGetValue(id, out var c) && c != null)
                StopCoroutine(c);

            _fadeCoroutines.Remove(id);

            if (_rows.TryGetValue(id, out var row))
            {
                row.RemoveFromHierarchy();
                _rows.Remove(id);
            }
        }
    }
}
