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

        protected void Start()
        {
            base.Start();
            SetScore(score);
        }

        public void SetText(string text)
        {
            SetText(scorePrefix + text);
        }

        public void SetText(int number)
        {
            SetText(scorePrefix + number.ToString());
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
