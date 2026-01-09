using UnityEngine;



namespace EchoCity
{
    [System.Serializable]
    public struct GoogleFormField
    {
        [SerializeField] private string _name;
        [SerializeField] private string _id;

        public string Name => _name;
        public string Id => _id;

        public GoogleFormField(string name, string id)
        {
            this._name = name;
            this._id = id;
        }
    }
    [CreateAssetMenu(fileName = "New Google Form Settings", menuName = "ECHO CITY/Others/Google Form Settings")]
    public class SOGoogleFormSettings : ScriptableObject
    {
        [SerializeField] private string url;
        [SerializeField] private GoogleFormField[] formFields;

        public string Url => url;
        public GoogleFormField[] FormFields => formFields;


#if UNITY_EDITOR
        [Multiline]
        [Tooltip("Event description (only available when using the Unity Editor).")]
        public string Description = "";
#endif
    }
}