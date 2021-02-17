using UnityEngine.Assertions;
using UnityEngine;
// needed to update the score tracking
using TMPro;

public class BallPhysics : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_vTargetPos;
    [SerializeField]
    private Vector3 m_vInitialVel;
    [SerializeField]
    private bool m_bDebugKickBall = false;
    [SerializeField]
    private float m_fInputDeltaVal = 0.05f;

    private Rigidbody m_rb = null;
    private GameObject m_TargetDisplay = null;

    private bool m_bIsGrounded = true;

    private float m_fDistanceToTarget = 0f;

    private Vector3 vDebugHeading;

    public TMP_Text scoreValuetxt;

    // Not 0 so a goal won't state 0 when a goal is obtained.
    public static int score = 1;

    Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;
        //create a float the represent the distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;
        float Vxz = Sxz / time;
        float Vy = Sy / time + .5f * Mathf.Abs(Physics.gravity.y) * time;
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "Houston, we've got a problem here! No Rigidbody attached");

        CreateTargetDisplay();
        m_fDistanceToTarget = (m_TargetDisplay.transform.position - transform.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TargetDisplay != null && m_bIsGrounded)
        {
            m_TargetDisplay.transform.position = m_vTargetPos;
            vDebugHeading = m_vTargetPos - transform.position;
        }

        if (m_bDebugKickBall && m_bIsGrounded)
        {
            m_bDebugKickBall = false;
            //OnKickBall();
            Vector3 Vo = CalculateVelocity(m_vTargetPos, m_rb.position, 1f);
            transform.rotation = Quaternion.LookRotation(Vo);
            m_rb.velocity = Vo;
        }
        HandleUserInput();
    }
    private void HandleUserInput()
    {
        // Kicks the ball
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //m_projectile.
            m_bDebugKickBall = true;

        }
        // Moves the target for the ball to be kicked to
        if (Input.GetKey(KeyCode.W))
        {
            m_vTargetPos.z += m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_vTargetPos.x -= m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.S))
        {
            m_vTargetPos.z -= m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_vTargetPos.x += m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.R))
        {
            m_vTargetPos.y += m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.F))
        {
            m_vTargetPos.y -= m_fInputDeltaVal;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            //When Q is pushed, Goals reset to 0 to encourage the player to go for a high score of continuous goals
            transform.position = GameObject.Find("BallSpawn").transform.position;
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;
            scoreValuetxt.text = "Goals: ";
            score = 1;
        }
    }
    private void CreateTargetDisplay()
    {
        m_TargetDisplay = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_TargetDisplay.transform.position = new Vector3(1.0f, 1.0f, 5.0f);
        m_TargetDisplay.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        m_TargetDisplay.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // Target Position Set by Net
        m_vTargetPos.y = 0.3f;
        m_vTargetPos.z = 12;

        m_TargetDisplay.GetComponent<Renderer>().material.color = Color.red;
        m_TargetDisplay.GetComponent<Collider>().enabled = false;
    }

    public void OnKickBall()
    {
        // H = Vi^2 * sin^2(theta) / 2g
        // R = 2Vi^2 * cos(theta) * sin(theta) / g

        // Vi = sqrt(2gh) / sin(tan^-1(4h/r))
        // theta = tan^-1(4h/r)

        // Vy = V * sin(theta)
        // Vz = V * cos(theta)

        float fMaxHeight = m_TargetDisplay.transform.position.y;
        float fRange = (m_fDistanceToTarget * 2);
        float fTheta = Mathf.Atan((4 * fMaxHeight) / (fRange));

        float fInitVelMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

        m_vInitialVel.y = fInitVelMag * Mathf.Sin(fTheta);
        m_vInitialVel.z = fInitVelMag * Mathf.Cos(fTheta);

        m_rb.velocity = m_vInitialVel;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + vDebugHeading, transform.position);
    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goals")
        {
            Debug.Log("goal");
            scoreValuetxt.text = "Goals: "+score.ToString();
 
            score++;
            // Ensures Score increasing and Ball respawning upon goal
            transform.position = GameObject.Find("BallSpawn").transform.position;
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;

        }
    }
}
