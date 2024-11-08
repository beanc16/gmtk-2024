using UnityEngine;
using TMPro;

namespace Beanc16.Common.UI
{
    public class ScoreTextManager : TextManager
    {
        [SerializeField]
        private string scorePrefix = "Score: ";

        [SerializeField]
        private int score = 0;

        public int Score
        {
            get => score;
        }

        public void SetText(string text)
        {
            base.SetText($"{scorePrefix}{text}");
        }

        public void SetText(int number)
        {
            base.SetText($"{scorePrefix}{number}");
        }

        public void SetScore(int number)
        {
            score = number;
            SetText(score);
        }

        public void IncrementScore(int number)
        {
            score += number;
            SetText(score);
        }
    }
}
