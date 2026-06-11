using UnityEngine;
using UnityEngine.UI;

namespace Ehaqui.UI
{
    public class ChainUI : MonoBehaviour
    {
        public static ChainUI Instance { get; private set; }

        [Header("Panels")]
        public GameObject ChainPanel;
        public Text ChainNameLabel;
        public Text StepLabel;
        public Text RiddleLabel;
        public InputField AnswerField;
        public Button SubmitButton;
        public GameObject CompletePanel;
        public Text CompleteLabel;

        [Header("Progress")]
        public Slider ProgressBar;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ChainPanel?.SetActive(false);
            CompletePanel?.SetActive(false);
            SubmitButton?.onClick.AddListener(OnSubmitAnswer);

            Gameplay.ChainManager.Instance.OnChainStarted += chain =>
            {
                ChainPanel?.SetActive(true);
                ChainNameLabel.text = chain.Name;
                UpdateClue(chain);
            };

            Gameplay.ChainManager.Instance.OnStepSolved += step =>
            {
                var chain = Gameplay.ChainManager.Instance.ActiveChain;
                UpdateClue(chain);
            };

            Gameplay.ChainManager.Instance.OnChainComplete += chain =>
            {
                ChainPanel?.SetActive(false);
                CompletePanel?.SetActive(true);
                CompleteLabel.text = $"Corrente completa! +{chain.Clues.Count * 50} XP";
            };
        }

        private void UpdateClue(Gameplay.ChainData chain)
        {
            var clue = Gameplay.ChainManager.Instance.GetCurrentClue();
            if (clue == null) return;

            StepLabel.text = $"Pista {clue.StepNumber} de {chain.Clues.Count}";
            RiddleLabel.text = clue.Riddle;
            AnswerField.text = "";
            ProgressBar.maxValue = chain.Clues.Count;
            ProgressBar.value = chain.CurrentStep;
        }

        private void OnSubmitAnswer()
        {
            var answer = AnswerField.text;
            var solved = Gameplay.ChainManager.Instance.TrySolveStep(answer);
            if (!solved)
            {
                Debug.LogWarning("Wrong answer!");
            }
        }
    }
}
