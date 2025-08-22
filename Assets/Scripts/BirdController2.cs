using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class BirdController2 : MonoBehaviour
{
    public bool isUsingPython = false;

    public float forwardSpeed = 5f;
    public float turnSpeed = 40f;
    public float verticalSpeed = 2f;

    private UdpClient client;
    private Thread receiveThread;

    private float armDiff = 0f;
    private float leftAbduction = 0f;
    private float rightAbduction = 0f;
    private float leftExtension = 0f;
    private float rightExtension = 0f;

    void Start()
    {
        client = new UdpClient(5051);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        if(isUsingPython)
        {
            // Turning: left arm up = turn right
            float turnInput = Mathf.Clamp(armDiff * 2f, -0.5f, 0.5f);
            float smoothTurn = Mathf.Lerp(0f, turnInput, 0.1f);
            transform.Rotate(Vector3.up, smoothTurn * turnSpeed * Time.deltaTime);

            // Forward movement: slower when turning
            float moveSpeed = Mathf.Abs(turnInput) > 0.2f ? forwardSpeed * 0.5f : forwardSpeed;
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Ascend: both arms abducted out wide
            if (leftAbduction > 80f && rightAbduction > 80f)
            {
                transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
            }
            // Descend: both arms extended behind
            else if (leftExtension < -0.4f && rightExtension < -0.4f &&
                     leftAbduction < 70f && rightAbduction < 70f)
            {
                transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
            }
        }
        else
        {
            // C# different coordinates

            // Turning: left arm up = turn right
            float turnInput = Mathf.Clamp(armDiff * 2f, -0.5f, 0.5f);
            float smoothTurn = Mathf.Lerp(0f, turnInput, 0.1f);
            transform.Rotate(Vector3.up, smoothTurn * turnSpeed * Time.deltaTime);

            // Forward movement: slower when turning
            float moveSpeed = Mathf.Abs(turnInput) > 0.2f ? forwardSpeed * 0.5f : forwardSpeed;
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Ascend: both arms abducted out wide
            if (leftAbduction > 80f && rightAbduction > 80f)
            {
                transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
            }
            // Descend: both arms extended behind
            else if (leftExtension > 0.4f && rightExtension > 0.4f &&
                     leftAbduction < 70f && rightAbduction < 70f)
            {
                transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
            }
        }
        
    }

    void ReceiveData()
    {
        if (!isUsingPython) return;
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 5051);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                PoseData pose = JsonUtility.FromJson<PoseData>(message);

                armDiff = pose.arm_diff;
                leftAbduction = pose.left_abduction;
                rightAbduction = pose.right_abduction;
                leftExtension = pose.left_extension;
                rightExtension = pose.right_extension;

                Debug.Log($"Received Pose Data - Arm Diff: {armDiff}, Left Abduction: {leftAbduction}, Right Abduction: {rightAbduction}, Left Extension: {leftExtension}, Right Extension: {rightExtension}");
            }
            catch { }
        }
    }

    public void FeedData(PoseData pose)
    {
        armDiff = pose.arm_diff;
        leftAbduction = pose.left_abduction;
        rightAbduction = pose.right_abduction;
        leftExtension = pose.left_extension;
        rightExtension = pose.right_extension;
    }

    private void OnDestroy()
    {
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        client?.Close();
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        client?.Close();
    }

    [System.Serializable]
    public class PoseData
    {
        public float left_abduction;
        public float right_abduction;
        public float left_extension;
        public float right_extension;
        public float arm_diff;
    }
}
