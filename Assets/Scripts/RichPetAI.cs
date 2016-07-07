using UnityEngine;
using Pathfinding;
using HoloToolkit.Unity;

public class RichPetAI : RichAI {
    public int wanderThreshold = 30;
    private RecastGraph activeGraph;
    private GameObject hooman;
    private bool goingToHooman = false;
    private float originalMaxSpeed = 0f;
    private Vector3 lastPos;

    private Animator animator;
    private const string SPEED_PARAM = "speed";

    public UAudioManager audioManager;

    protected override void Start()
    {
        base.Start();
        activeGraph = AstarPath.active.astarData.recastGraph;
        hooman = GameObject.Find("Main Camera");
        InvokeRepeating("Wander", 0, wanderThreshold);
        animator = GetComponentInChildren<Animator>();
        lastPos = transform.position;
        audioManager = GetComponent<UAudioManager>();
    }

    protected override void Update()
    {
        base.Update();
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

    protected override void OnTargetReached()
    {
        if (goingToHooman)
        {
            goingToHooman = false;
            maxSpeed = originalMaxSpeed;
        }
        Wander();
    }

    public void Wander()
    {
        if (goingToHooman) return;

        target = (Vector3)GetRandomNode(activeGraph).position;
    }

    public virtual void GoToHooman()
    {
        goingToHooman = true;
        originalMaxSpeed = maxSpeed;
        maxSpeed = maxSpeed * 1.5f;
        target = UtilManager.Instance.GetGroundPosition(hooman);
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
