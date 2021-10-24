using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
public class MyGameEventSystem : MonoBehaviourSingleton<MyGameEventSystem>
{
    public Borderline player1BorderlineInstance;
    public Borderline player2BorderlineInstance;

    public class BallEvents
    {
        public class OwnerChanged
        {
            public static event System.Action<GameObject> performed;
            public static void Perform(GameObject newOwner) //TODO: box arguments
            {
                performed?.Invoke(newOwner);
            }
        }
        public class BallIn
        {
            public static Borderline player1Borderline;
            public static Borderline player2Borderline;
            public static void Perform(IOwnerShip ball, GameObject hittedborderLine) //TODO: box arguments
            {

                if (hittedborderLine == player1Borderline.gameObject)
                {
                    if (player1Borderline.owner != ball.owner)
                        Player2Goaled.Perform();
                }
                else if (hittedborderLine == player2Borderline.gameObject)
                {
                    if (player2Borderline.owner != ball.owner)
                    {
                        Player1Goaled.Perform();
                    }
                }
            }
        }
        public class Player1Goaled
        {
            public static event System.Action performed;
            public static void Perform() //TODO: box arguments
            {
                performed?.Invoke();
            }
        }
        public class Player2Goaled
        {
            public static event System.Action performed;
            public static void Perform() //TODO: box arguments
            {
                performed?.Invoke();
            }
        }

    }
    public class UsePortalItemEvent
    {
        public static event System.Action<GameObject> performed;
        public static event System.Action canceled;

        public static bool isPortalEnabled { private set; get; }
        public static Coroutine delayResumeCoroutine;

        public static void Perform(GameObject user, float duration)
        {
            performed?.Invoke(user);
            if (delayResumeCoroutine != null) Instance.TerminateCoroutine(delayResumeCoroutine);
            delayResumeCoroutine = Instance.ExecuteCoroutine(DelayResume(duration));
        }
        public static IEnumerator DelayResume(float duration)
        {
            isPortalEnabled = true;
            yield return new WaitForSeconds(duration);
            isPortalEnabled = false;
            canceled?.Invoke();
        }
    }
    public class UseBigBallItemEvent
    {
        public static event System.Action<GameObject, float> performed;
        public static void Perform(GameObject user, float amplifier) //TODO: box arguments
        {
            performed?.Invoke(user, amplifier);
        }
    }
    public class UseExtentedPlaneItemEvent
    {
        public static event System.Action<GameObject> performed;
        public static void Perform(GameObject user)
        {
            performed?.Invoke(user);
        }
    }
    public class AcumulatedBallEvent
    {
        public static event System.Action<int, GameObject> performed;
        public static void Perform(int count, GameObject user)
        {
            performed?.Invoke(count, user);
        }
    }
    public class UseBallIncreasedItemEvent
    {
        public static event System.Action<int, GameObject> performed;
        public static void Perform(int count, GameObject user)
        {
            performed?.Invoke(count, user);
        }
    }
    public class UseGravityChangedItemEvent
    {
        public static event System.Action<Vector4, float> performed;
        public static event System.Action canceled;

        public static bool isCustomGravityEnabled = true;
        public static Vector4 finalDirection;

        public static Coroutine delayResumeCoroutine;
        public static void Perform(Vector4 direction, float duration)
        {
            
            finalDirection += direction;
            //Debug.Log(finalDirection);
            performed?.Invoke(finalDirection, duration);
            if (delayResumeCoroutine != null) Instance.TerminateCoroutine(delayResumeCoroutine);
            delayResumeCoroutine = Instance.ExecuteCoroutine(DelayResume(duration));
        }
        public static IEnumerator DelayResume(float duration)
        {
            isCustomGravityEnabled = true;
            yield return new WaitForSeconds(duration);
            isCustomGravityEnabled = false;
            finalDirection = Vector4.zero;
            canceled?.Invoke();
        }
    }
    public class UsePushLevelItemEvent
    {
        public static event System.Action<float> performed;
        public static void Perform(float distance)
        {
            performed?.Invoke(distance);
        }

    }
    public Coroutine ExecuteCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void TerminateCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    private void Start()
    {
        BallEvents.BallIn.player1Borderline = player1BorderlineInstance;
        BallEvents.BallIn.player2Borderline = player2BorderlineInstance;

    }

}