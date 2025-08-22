using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PoseReceiver : MonoBehaviour
{
    public Transform phoenix;
    public float forwardSpeed = 5f;
    public float rotationSpeed = 100f;
    public float verticalSpeed = 5f;

    UdpClient client;
    Thread receiveThread;
    float leftFlexion = 0f;
    float rightFlexion = 0f;
    float armDiff = 0f;

    void Start()
    {
        client = new UdpClient(5051);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5051);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(data);
                PoseData pose = JsonUtility.FromJson<PoseData>(message);
                leftFlexion = pose.left_flexion;
                rightFlexion = pose.right_flexion;
                armDiff = pose.arm_diff;
            }
            catch { }
        }
    }

    void Update()
    {
        // Constant forward motion
        phoenix.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Vertical movement: negative armDiff when arms are raised (because y decreases up)
        // So we invert it to make up mean up!
        float verticalInput = Mathf.Clamp(-armDiff * 5f, -1f, 1f); // Tune multiplier as needed
        phoenix.Translate(Vector3.up * verticalInput * verticalSpeed * Time.deltaTime);

        // Horizontal rotation based on arm height difference
        phoenix.Rotate(Vector3.up, verticalInput * rotationSpeed * Time.deltaTime); // Optional: use a different value here if needed
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        client.Close();
    }

    [System.Serializable]
    public class PoseData
    {
        public float left_flexion;
        public float right_flexion;
        public float arm_diff;
    }
}
