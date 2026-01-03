using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace EchoCity
{
    // class to save data to a Google form from Unity
    // from https://www.youtube.com/watch?v=WM7f4yN4ZHA
    // also https://www.youtube.com/watch?v=2-tUwIQmBNE

    public struct GoogleFormData
    {
        private string _id;
        private string _data;

        public string Id => _id;
        public string Data => _data;

        public GoogleFormData(string id, string data)
        {
            _id = id;
            _data = data;
        }
    }

    public class GoogleFormSender : MonoBehaviour
    {
        [SerializeField] SOGoogleFormSettings settings;
        [Header("Observing Events")]
        [SerializeField] SOSubmitFeedbackEvent submitFeedbackEvent;

        private GoogleFormData[] formData;

#pragma warning disable CS0414
        private bool _completed = false;
        private bool _success = false;
#pragma warning restore CS0414

        void OnEnable()
        {
            if (submitFeedbackEvent)
                submitFeedbackEvent.OnEventRaised += FeedbackSubmit;
        }

        void OnDisable()
        {
            if (submitFeedbackEvent)
                submitFeedbackEvent.OnEventRaised -= FeedbackSubmit;
        }

        private void FeedbackSubmit(IEventSender sender, int rating, string suggestions)
        {
            formData = new GoogleFormData[]
            {
                new GoogleFormData(settings.FormFields[0].Id, rating.ToString()),
                new GoogleFormData(settings.FormFields[1].Id, suggestions)
            };
            SubmitForm();
        }

        public void SubmitForm()
        {
            StartCoroutine(Post(formData));
        }

        IEnumerator Post(GoogleFormData[] formData)
        {
            WWWForm form = new WWWForm();
            form.AddField(settings.FormFields[0].Id, formData[0].Data);
            form.AddField(settings.FormFields[1].Id, formData[1].Data);

            using (UnityWebRequest www = UnityWebRequest.Post(settings.Url, form))
            {
                print("Start submission");
                _completed = false;

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    _success = true;
                    Debug.Log("Form Submitted");
                }
                else
                {
                    _success = false;
                    Debug.Log("Form Submission Error: " + www.error);
                }
                print("End submission");

                _completed = true;
            }
        }
    }
}
