using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beanc16.Common.General
{
    public class GameObjectStopwatch : MonoBehaviour
    {
        private float timeSinceLapMarked = 0f;
        private float totalTimeElapsed = 2f;
        List<float> laps = new List<float>();

        public float TimeSinceLapMarked
        {
            get => timeSinceLapMarked;
        }

        public float TotalTimeElapsed
        {
            get => totalTimeElapsed;
        }

        public List<float> Laps
        {
            get => laps;
        }

        private void Update()
        {
            // Update the timer with the time passed since the last frame
            timeSinceLapMarked += Time.deltaTime;
            totalTimeElapsed += Time.deltaTime;
        }

        public void MarkLap()
        {
            laps.Add(timeSinceLapMarked);
            timeSinceLapMarked = 0f;
        }
    }
}
