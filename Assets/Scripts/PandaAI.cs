using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class PandaAI : RichPetAI {
    private float idleTime = -1f;
    private float IDLE_ANIM_DURATION = 3.8f;
    private RichPetAI baseAI;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RandomIdle());
        InvokeRepeating("Wander", 0f, 15f);
    }

    public override void GoToHooman()
    {
        base.GoToHooman();
        audioManager.PlayEvent("react");       
    }

    void OnToyThrown(GameObject toy)
    {
        goingToObject = true;
        GoToShinyObject(toy.transform.position);
        audioManager.PlayEvent("react");
    }

    public override IEnumerator DecideNextAction()
    {
        if (goingToHooman || goingToObject)
        {
            yield break;
        }

        if (hunger <= 25 && !goingToObject)
        {
            GoToShinyObject(foodBowl.transform.position);
            goingToObject = true;
            yield break;
        }

        int roll = Random.Range(0, 2);

        switch (roll)
        {
            case 0:
                GoToHooman();
                break;

            case 1:
                Wander();
                break;

            case 2:
                yield return new WaitForSeconds(5f);
                StartCoroutine(DecideNextAction());
                break;
        }

        yield break;
    }

    IEnumerator RandomIdle()
    {
        while (true) {
            if (lastPos == transform.position)
            {
                if (idleTime == -1f)
                {
                    idleTime = Time.time;
                }

                // Maybe play a random idle animation.
                else if (Time.time - idleTime >= 5f && Random.Range(0, 3) == 2)
                {
                    base.headAnimating = true;

                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            animator.SetTrigger("IdleB");
                            yield return new WaitForSeconds(2f);
                            break;

                        case 1:
                            animator.SetTrigger("IdleC");
                            yield return new WaitForSeconds(4f);
                            break;

                        case 2:
                            animator.SetTrigger("IdleD");
                            yield return new WaitForSeconds(4.3f);
                            break;

                        default: break;
                    }

                    idleTime = -1f;
                    animator.SetTrigger("backToIdleA");
                    base.headAnimating = false;
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
