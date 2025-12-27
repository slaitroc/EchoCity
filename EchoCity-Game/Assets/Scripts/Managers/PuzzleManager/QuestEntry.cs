using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public class QuestEntry
    {
        public string Id;
        [TextArea] public string Text;
        public bool Completed;
        public bool PendingRemoval;
    }
}
