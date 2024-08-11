using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;



namespace Beanc16.Common.Async
{
    public class Timer
    {
        // Track if the timer is running (non-static version)
        private bool isRunning = false;

        // Allow stopping the timer
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken cancelToken;
        private static CancellationTokenSource cancelTokenSourceStatic = 
            new CancellationTokenSource();
        private static CancellationToken cancelTokenStatic;

        public Timer()
        {
            // Timer
            isRunning = false;

            // Allow the stopping of the timer
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;
        }

        

        public async Task Run(float durationInSeconds)
        {
            if (!isRunning)
            {
                isRunning = true;
                await RunTimer(durationInSeconds);
                isRunning = false;
            }

            else
            {
                Debug.LogWarning("INVALID: Tried to start a timer that " +
                                "was already running.");
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                cancelTokenSource.Cancel();
                isRunning = false;
            }

            else
            {
                Debug.LogWarning("INVALID: Tried to stop timer that was " +
                                "not running.");
            }
        }

        private async Task RunTimer(float durationInSeconds)
        {
            // Convert duration to milliseconds
            float milliseconds = durationInSeconds * 1000;
            int ms = Mathf.RoundToInt(milliseconds);

            // Allow the Stop() method to be called
            cancelToken.ThrowIfCancellationRequested();

            // Wait the duration
            await Task.Delay(ms, cancelToken);
        }

        public bool IsRunning()
        {
            return isRunning;
        }



        public static async Task Wait(float seconds)
        {
            /*
            * Enable the ability to stop static timers, if this is the 
            * first time Timer.Wait is being called
            */
            Timer.TryInitializeStaticToken();

            // Convert duration to milliseconds
            float milliseconds = seconds * 1000;
            int ms = Mathf.RoundToInt(seconds * 1000);

            // Allow the Timer.StopStaticWaits() method to be called
            cancelTokenStatic.ThrowIfCancellationRequested();

            // Wait the duration
            await Task.Delay(ms, cancelTokenStatic);
        }

        public static async Task WaitMillseconds(int ms)
        {
            /*
            * Enable the ability to stop static timers, if this is the 
            * first time Timer.WaitMillseconds is being called
            */
            Timer.TryInitializeStaticToken();

            // Allow the Timer.StopStaticWaits() method to be called
            cancelTokenStatic.ThrowIfCancellationRequested();

            // Wait the duration
            await Task.Delay(ms, cancelTokenStatic);
        }

        public static void StopStaticWaits()
        {
            /*
            * Enable the ability to stop static timers, if this is the 
            * first time Timer.StopStaticWaits is being called
            */
            Timer.TryInitializeStaticToken();

            // Stop the timer
            cancelTokenSourceStatic.Cancel();
        }

        private static void TryInitializeStaticToken()
        {
            /*
            * Enable the ability to stop static timers, if this is the 
            * first time Time.Wait, Timer.WaitMillseconds, or 
            * Timer.StopStaticWaits is being called
            */
            if (cancelTokenStatic == null)
            {
                cancelTokenStatic = Timer.cancelTokenSourceStatic.Token;
            }
        }
    }
}
