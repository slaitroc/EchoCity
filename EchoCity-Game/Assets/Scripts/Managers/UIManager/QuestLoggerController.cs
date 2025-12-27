using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class QuestLoggerController : MonoBehaviour
    {
        [SerializeField] private UIDocument hudDocument;
        [SerializeField] private QuestContainer questContainer;

        [SerializeField] private float completedVisibleSeconds = 2.0f;
        [SerializeField] private float fadeOutSeconds = 0.35f;

        private VisualElement _root;
        private VisualElement _panel;
        private VisualElement _list;

        private readonly Dictionary<string, VisualElement> _rows = new();
        private readonly HashSet<string> _fadeInProgress = new();

        private void Awake()
        {
            if (questContainer == null) Log.E("Task Container not found");
        }

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
            RebuildFromContainer();
        }

        private VisualElement BuildRow(string taskId, string text, bool completed)
        {
            var row = new VisualElement();
            row.AddToClassList("quest-item");

            var check = new VisualElement();
            check.AddToClassList("quest-check");

            var labelText = new Label(text);
            labelText.AddToClassList("quest-text");

            row.Add(check);
            row.Add(labelText);

            if (completed) row.AddToClassList("completed");

            return row;
        }

        private void HandleTaskCompleted(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId)) return;
            if (_fadeInProgress.Contains(taskId)) return;

            _fadeInProgress.Add(taskId);

            if (_rows.TryGetValue(taskId, out var row))
            {
                if (!row.ClassListContains("completed"))
                    row.AddToClassList("completed");

                StartCoroutine(FadeAndRemove(taskId, row));
            }
            else
            {
                _fadeInProgress.Remove(taskId);
            }
        }

        private IEnumerator FadeAndRemove(string taskId, VisualElement row)
        {
            yield return new WaitForSeconds(completedVisibleSeconds);

            row.AddToClassList("fade-out");

            yield return new WaitForSeconds(fadeOutSeconds);

            row.RemoveFromHierarchy();
            _rows.Remove(taskId);
            _fadeInProgress.Remove(taskId);

            if (questContainer != null)
                questContainer.RemoveTask(taskId);
        }

        public void RebuildFromContainer()
        {
            if (_list == null || questContainer == null) return;

            var tasks = questContainer.Quests;
            var keep = new HashSet<string>();

            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                if (t == null || string.IsNullOrWhiteSpace(t.Id)) continue;

                keep.Add(t.Id);

                if (_rows.TryGetValue(t.Id, out var row))
                {
                    var label = row.Q<Label>(className: "quest-text");
                    if (label != null) label.text = t.Text;

                    if (t.Completed && !row.ClassListContains("completed"))
                        row.AddToClassList("completed");
                    HandleTaskCompleted(t.Id);
                }
                else
                {
                    var newRow = BuildRow(t.Id, t.Text, t.Completed);
                    _list.Add(newRow);
                    _rows[t.Id] = newRow;
                }
            }

            var toRemove = new List<string>();
            foreach (var kvp in _rows)
            {
                if (!keep.Contains(kvp.Key))
                    toRemove.Add(kvp.Key);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                var id = toRemove[i];
                if (_rows.TryGetValue(id, out var row))
                {
                    row.RemoveFromHierarchy();
                    _rows.Remove(id);
                }
                _fadeInProgress.Remove(id);
            }
        }

        public void AddTask(string taskId, string text)
        {
            if (questContainer == null) return;
            questContainer.AddOrUpdateTask(taskId, text);
        }

        public void CompleteTask(string taskId)
        {
            if (questContainer == null) return;
            questContainer.CompleteTask(taskId);
        }

        public void ClearAll()
        {
            if (questContainer == null) return;
            questContainer.Clear();
        }
    }
}
