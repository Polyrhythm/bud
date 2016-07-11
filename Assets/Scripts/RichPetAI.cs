using UnityEngine;
using Pathfinding;
using HoloToolkit.Unity;

public class RichPetAI : RichAI {
    public GameObject headBone;
    public int wanderThreshold = 30;
    private RecastGraph activeGraph;
    private GameObject hooman;
    private bool goingToHooman = false;
    private bool goingToObject = false;
    private float originalMaxSpeed = 0f;

    private bool isSlerping = false;
    private float slerpStart;
    private float slerpDuration = 0.5f;
    private Quaternion lookRotation;
    private Quaternion lastSlerpedRot;

    [HideInInspector]
    public Vector3 lastPos;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    // Whether we should be messing with where the animal looks or deferring to animation.
    public bool headAnimating;

    // Stateful attributes.
    [HideInInspector]
    public float affinity = 0;
    [HideInInspector]
    public float energy = 100;
    [HideInInspector]
    public float hunger = 100;

    private const string SPEED_PARAM = "speed";
    private float fieldOfView = 110f;

    [HideInInspector]
    public UAudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        activeGraph = AstarPath.active.astarData.recastGraph;
        hooman = Camera.main.gameObject;
        animator = GetComponentInChildren<Animator>();
        lastPos = transform.position;
        audioManager = GetComponent<UAudioManager>();
        GoToHooman();
    }

    protected override void Update()
    {
        base.Update();

        if (!headAnimating)
        {

            if (!isSlerping)
            {
                Debug.Log("not slerping");
                lookRotation = Quaternion.LookRotation(target - transform.position + Vector3.up * 2f);
                // I need to figure out what in thet actual fuck is making this work.
                lookRotation *= Quaternion.Euler(90, 270, 180);

                if (Quaternion.Angle(lookRotation, lastSlerpedRot) >= 10f && Vector3.Angle(target - transform.position, transform.forward.normalized) <= 60)
                {
                    Debug.Log("rotations not equal: " + Quaternion.Angle(lookRotation, lastSlerpedRot));
                    slerpStart = Time.time;
                    isSlerping = true;
                }
            }
        }

        if (lastPos == transform.position) {
            animator.SetFloat(SPEED_PARAM, 0);
            return;
        }

        Vector3 horPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastHorPos = new Vector3(lastPos.x, 0, lastPos.z);
        float speed = ((horPos - lastHorPos) / Time.deltaTime).magnitude;
        animator.SetFloat(SPEED_PARAM, speed);

        lastPos = transform.position;
    }

    void LateUpdate()
    {
        if (headAnimating) return;

        // TODO: Slerp back to where we were at.
        headBone.transform.rotation = lastSlerpedRot;

        if (!isSlerping) return;

        float timeSinceStarted = Time.time - slerpStart;
        float percentageComplete = timeSinceStarted / slerpDuration;

        headBone.transform.rotation = Quaternion.Slerp(headBone.transform.rotation, lookRotation, percentageComplete);

        if (percentageComplete >= 1.0f)
        {
            isSlerping = false;
            lastSlerpedRot = headBone.transform.rotation;
        }
    }

    protected override void OnTargetReached()
    {
        if (goingToHooman || goingToObject)
        {
            goingToObject = false;
            goingToHooman = false;
            maxSpeed = originalMaxSpeed;
        }
    }

    public void Wander()
    {
        if (goingToHooman || goingToObject) return;

        target = (Vector3)GetRandomNode(activeGraph).position;
    }

    public virtual void GoToHooman()
    {
        goingToHooman = true;
        originalMaxSpeed = maxSpeed;
        maxSpeed = maxSpeed * 2f;
        target = hooman.transform.position + hooman.transform.forward * 1;
    }

    public void GoToShinyObject(Vector3 pos)
    {
        goingToObject = true;
        originalMaxSpeed = maxSpeed;
        maxSpeed = maxSpeed * 1.5f;
        target = pos;
    }

    public GraphNode GetRandomNode(RecastGraph graph)
    {
        GraphNode resultNode = null;
        int i = 0;
        int randomInt = Random.Range(0, graph.CountNodes());

        graph.GetNodes(node =>
        {
            if (i != randomInt)
            {
                i += 1;
                return true;
            }

            resultNode = node;
            return false;
        });

        return resultNode;
    }
}
