using UnityEngine;

public class CarController : MonoBehaviour
{

    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    public float maxSteerAngle;
    public float motorForce;

    public LayerMask raycastMask; 
    private float[] input = new float[4]; 
    public float probingDistance; 


    public Rigidbody rb;
    public BoxCollider boxCollider;
    private NeuralNetwork myNN;

    private bool carStarted = false;
    private int fitness;
    public bool hitWall = false;
    private int lastFitness;

    #region Getters/Setters
    public int GetFitness()
    {
        return fitness;
    }

    public void SetNeuralNetwork(NeuralNetwork nn)
    {
        myNN = nn;
        StartCar();
    }
    #endregion

    #region Game Loop functions
    void Awake()
    {
        raycastMask = LayerMask.GetMask("Barrier");
        fitness = 0;
        lastFitness = fitness;
    }

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        IgnoreContactWithOtherCars();
    }


    private void FixedUpdate()
    {
        if (myNN != null && carStarted && hitWall == false)
        {
            GetInput();
            Steer();
            Accelerate();
            UpdateWheelPoses();
        }
        else
        {

            rb.velocity = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region Functions that manage car movement

    private void GetInput()
    {
        GetInputFromProximitySensors();

        m_horizontalInput = 0;
        m_verticalInput = 0;

        float output = myNN.FeedForward(input);

        if (output == 0)
        {
            m_verticalInput = 1;
        }
        else if (output == 1)
        {
            m_horizontalInput = -1;
        }
        else if (output == 2)
        {
            m_horizontalInput = 1;
        }
        else
        {
            m_verticalInput = -1;
        }
    }

   
    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

 
    private void Accelerate()
    {
        frontDriverW.motorTorque = m_verticalInput * motorForce;
        frontPassengerW.motorTorque = m_verticalInput * motorForce;
    }

   
    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

   
    private void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;

        collider.GetWorldPose(out pos, out quat);

        transform.position = pos;
        transform.rotation = quat;
    }
    #endregion

    #region Functions that manage collisions
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.gameObject.tag == "Barrier")
        {
            hitWall = true;
        }
    }

    void OnTriggerEnter(Collider collision)
    {

        int nextCheckpoint = (fitness % 118 ) + 1;
        if (collision.gameObject.name == ("CheckPoint (" + nextCheckpoint + ")") && hitWall == false)
        {
            fitness += 1;
        }

        if (collision.gameObject.name == "CheckPoint (118)")
        {
            hitWall = true;
        }
    }

    void GetInputFromProximitySensors()
    {
        Vector3 forwardSensor = transform.forward;
        Vector3 northeastSensor = transform.forward + transform.right;
        Vector3 northwestSensor = transform.forward - transform.right;
        Vector3[] proximitySensors = new Vector3[] { forwardSensor, northeastSensor, northwestSensor };

        Vector3 offset = new Vector3(0, 1, 0);
        float distance;
        for (int i = 0; i < proximitySensors.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + offset, proximitySensors[i], out hit, probingDistance, raycastMask))
            {
                distance = probingDistance - hit.distance;
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.red);
            }
            else
            {
                distance = 0;
                Debug.DrawLine(transform.position + offset, transform.position + (proximitySensors[i] * probingDistance), Color.green);
            }

            input[i] = distance;
        }

     
        input[3] = rb.velocity.magnitude;
    }

    void IgnoreContactWithOtherCars()
    {
        Object[] allWheels = GameObject.FindObjectsOfType(typeof(WheelCollider));
        Object[] allBoxColliders = GameObject.FindObjectsOfType(typeof(BoxCollider));

        for (int i = 0; i < allWheels.Length; i++)
        {
         
            for (int j = 0; j < allWheels.Length; j++)
            {
                Physics.IgnoreCollision(allWheels[i] as WheelCollider, allWheels[j] as WheelCollider);
            }

            
            for (int j = 0; j < allBoxColliders.Length; j++)
            {
                Physics.IgnoreCollision(allWheels[i] as WheelCollider, allBoxColliders[j] as BoxCollider);
            }
        }

        for (int i = 0; i < allBoxColliders.Length; i++)
        {
            for (int j = 0; j < allWheels.Length; j++)
            {
                Physics.IgnoreCollision(allBoxColliders[i] as BoxCollider, allWheels[j] as WheelCollider);
            }

 
            for (int j = 0; j < allBoxColliders.Length; j++)
            {
                BoxCollider myBox = allBoxColliders[i] as BoxCollider;
                BoxCollider otherBox = allBoxColliders[j] as BoxCollider;
                if (myBox.gameObject.tag == "CheckPoint" || otherBox.gameObject.tag == "CheckPoint")
                {
                    continue;
                }

                Physics.IgnoreCollision(allBoxColliders[i] as BoxCollider, allBoxColliders[j] as BoxCollider);
            }
        }
    }
    #endregion

    #region Manage car state
    public void StartCar()
    {
        carStarted = true;
       
        InvokeRepeating("CheckProgression", 10.0f, 10.0f);
    }

    public void StopCar()
    {
        carStarted = false;
    }

    public void CheckProgression()
    {

        if (lastFitness >= fitness)
        {
            hitWall = true;
        }
        else
        {
            lastFitness = fitness;
        }
    }
    #endregion
}
