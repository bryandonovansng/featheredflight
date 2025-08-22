using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class BirdController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float turnSpeed = 40f;
    public float verticalSpeed = 2f;

    private UdpClient client;
    private Thread receiveThread;

    private float armDiff = 0f;
    private float leftFlexion = 0f;
    private float rightFlexion = 0f;

    void Start()
    {
        client = new UdpClient(5051);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // Determine whether player is in "neutral" (flying straight) pose
        bool inNeutralRange =
            leftFlexion > 60f && leftFlexion < 140f &&
            rightFlexion > 60f && rightFlexion < 140f;

        // Turning: left arm up = turn right
        float turnInput = Mathf.Clamp(armDiff * 2f, -0.5f, 0.5f);
        float smoothTurn = Mathf.Lerp(0f, turnInput, 0.1f);
        transform.Rotate(Vector3.up, smoothTurn * turnSpeed * Time.deltaTime);

        // Forward movement: full speed if arms are neutral, otherwise half speed
        float moveSpeed = inNeutralRange ? forwardSpeed : forwardSpeed * 0.5f;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Ascend: both arms raised
        if (leftFlexion > 140f && rightFlexion > 140f)
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
        }
        // Descend: both arms lowered
        else if (leftFlexion < 40f && rightFlexion < 40f)
        {
            transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
        }
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5051);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                PoseData pose = JsonUtility.FromJson<PoseData>(message);

                armDiff = pose.arm_diff;
                leftFlexion = pose.left_flexion;
                rightFlexion = pose.right_flexion;
            }
            catch { }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        client?.Close();
    }

    [System.Serializable]
    public class PoseData
    {
        public float left_flexion;
        public float right_flexion;
        public float arm_diff;
    }
}
