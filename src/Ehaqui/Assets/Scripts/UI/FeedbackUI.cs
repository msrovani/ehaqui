using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ehaqui.UI
{
    public class FeedbackUI : MonoBehaviour
    {
        public static FeedbackUI Instance { get; private set; }

        [Header("Panels")]
        public GameObject FeedbackPanel;
        public Text TreasureNameLabel;
        public Button SubmitButton;
        public Button SkipButton;

        [Header("Rating")]
        public Button FunButton;
        public Button AmazingButton;
        public Button ChallengingButton;
        public Button ConfusingButton;
        public Button BoringButton;

        [Header("Comment")]
        public InputField CommentField;

        private string _treasureId;
        private string _selectedRating;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FeedbackPanel?.SetActive(false);
            SubmitButton?.onClick.AddListener(Submit);
            SkipButton?.onClick.AddListener(Skip);
        }

        public void Show(string treasureId, string treasureName)
        {
            _treasureId = treasureId;
            TreasureNameLabel.text = treasureName;
            _selectedRating = null;
            FeedbackPanel?.SetActive(true);
        }

        public void SelectRating(string rating)
        {
            _selectedRating = rating;
        }

        private void Submit()
        {
            if (string.IsNullOrEmpty(_selectedRating))
            {
                Debug.LogWarning("Select a rating first");
                return;
            }

            var feedback = new FeedbackData
            {
                TreasureId = _treasureId,
                Rating = _selectedRating,
                Comment = CommentField?.text ?? "",
                PlayerId = Core.GameState.Instance.PlayerId,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            Offline.SyncQueue.Instance.Enqueue("feedback", JsonUtility.ToJson(feedback));
            FeedbackPanel?.SetActive(false);
        }

        private void Skip()
        {
            FeedbackPanel?.SetActive(false);
        }

        [Serializable]
        private class FeedbackData
        {
            public string TreasureId;
            public string Rating;
            public string Comment;
            public string PlayerId;
            public long Timestamp;
        }
    }
}
