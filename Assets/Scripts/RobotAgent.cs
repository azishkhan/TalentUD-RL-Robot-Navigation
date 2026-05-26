using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RobotAgent : Agent
{
    [Header("Walk Speed")]
    [Range(0.1f, 5f)]
    [SerializeField]
    private float m_TargetWalkingSpeed = 3f;
    const float m_MaxWalkingSpeed = 5f;
    public bool randomizeWalkSpeedEachEpisode = false;

    [Header("References")]
    public Transform target;
    public float rotateSpeed = 150f;

    [Header("Visual Feedback")]
    public Renderer floorRenderer;

    private Color defaultColor = Color.white;
    private Color successColor = new Color(0f, 0.8f, 0f);
    private Color failColor = new Color(0.8f, 0f, 0f);

    private Rigidbody rb;
    private Animator animator;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private int episodeCount = 0;
    private float episodeReward = 0f;
    private float episodeTimer = 0f;
    private bool episodeEnding = false;

    // -------------------------------------------------------
    // Initialization
    // -------------------------------------------------------
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    // -------------------------------------------------------
    // Floor Color
    // -------------------------------------------------------
    private void SetFloorColor(Color color)
    {
        if (floorRenderer != null)
        {
            floorRenderer.material.SetColor("_BaseColor", color);
            floorRenderer.material.SetColor("_Color", color);
        }
        
    }

    // -------------------------------------------------------
    // Episode Begin
    // -------------------------------------------------------
    public override void OnEpisodeBegin()
    {
        episodeCount++;
        episodeReward = 0f;
        episodeTimer = 0f;
        episodeEnding = false;

        SetFloorColor(defaultColor);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;

        if (randomizeWalkSpeedEachEpisode)
            m_TargetWalkingSpeed = Random.Range(0.5f, m_MaxWalkingSpeed);

        target.localPosition = new Vector3(
            Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    // -------------------------------------------------------
    // Observations (12 total)
    // -------------------------------------------------------
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;

        sensor.AddObservation(dirToTarget);                                       // 3
        sensor.AddObservation(Vector3.Distance(transform.position,
                                               target.position));                 // 1
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));  // 3
        sensor.AddObservation(transform.forward);                                 // 3
        sensor.AddObservation(rb.velocity.magnitude / m_MaxWalkingSpeed);         // 1
        sensor.AddObservation(m_TargetWalkingSpeed / m_MaxWalkingSpeed);          // 1
        // Total: 12
    }

    // -------------------------------------------------------
    // Actions
    // -------------------------------------------------------
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (episodeEnding) return;

        episodeTimer += Time.deltaTime;

        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        rb.MovePosition(transform.position +
            transform.forward * move * m_TargetWalkingSpeed * Time.deltaTime);
        transform.Rotate(0f, rotate * rotateSpeed * Time.deltaTime, 0f);

        if (animator != null)
            animator.SetFloat("speed", Mathf.Abs(move) * m_TargetWalkingSpeed);

        float distToTarget = Vector3.Distance(transform.position, target.position);

        // Time penalty
        AddReward(-0.001f);
        episodeReward -= 0.001f;

        // Small orientation reward
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        dirToTarget.y = 0;
        float lookAtReward = Vector3.Dot(transform.forward, dirToTarget);
        AddReward(lookAtReward * 0.0005f);
        episodeReward += lookAtReward * 0.0005f;
        if (distToTarget < 1.5f)
        {
            episodeEnding = true;
            SetFloorColor(successColor);
            AddReward(20.0f);
            episodeReward += 20.0f;
            EndEpisode();
        }
    }

    // -------------------------------------------------------
    // Collision
    // -------------------------------------------------------
    private void OnCollisionEnter(Collision col)
    {
        if (episodeEnding) return;

        if (col.gameObject.CompareTag("Wall") ||
            col.gameObject.CompareTag("Obstacle"))
        {
            episodeEnding = true;
            SetFloorColor(failColor);
            AddReward(-0.5f);
            episodeReward -= 0.5f;
            EndEpisode();
        }
    }

    // -------------------------------------------------------
    // Manual Control
    // -------------------------------------------------------
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Vertical");
        ca[1] = Input.GetAxis("Horizontal");

        
    }

    // -------------------------------------------------------
    // Velocity Matching (from WalkerAgent)
    // -------------------------------------------------------
    public float GetMatchingVelocityReward(Vector3 velocityGoal,
                                           Vector3 actualVelocity)
    {
        float velDeltaMagnitude = Mathf.Clamp(
            Vector3.Distance(actualVelocity, velocityGoal), 0, m_MaxWalkingSpeed);

        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / m_MaxWalkingSpeed, 2), 2);
    }
}