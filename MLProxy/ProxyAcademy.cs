using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using MLProxy;

public class ProxyAcademy : Academy
{
    public int port = 1337;
    public GameObject agentPrefab;

    private UdpClient client;
    private Thread receiveThread;
    private List<GameObject> agentPool = new List<GameObject>();
    private List<ProxyAgent> agents = new List<ProxyAgent>();
    private List<GameObject> spawnedGameObjects = new List<GameObject>();
    private bool pauseReceive;
    private bool hasConnection;

    private void Start()
    {
        if(client == null)
        {
            client = new UdpClient(port);

            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            //receiveThread.IsBackground = true;
            receiveThread.Priority = System.Threading.ThreadPriority.Highest;
            receiveThread.Start();
        }

        for(int i = 0; i < 10; i++)
        {
            GameObject agent = Instantiate(this.agentPrefab);
            agent.GetComponent<ProxyAgent>().id = i;
            this.agents.Add(agent.GetComponent<ProxyAgent>());
            agent.gameObject.SetActive(false);
            this.agentPool.Add(agent);
        }
    }
    private void OnApplicationQuit()
    {
        this.client.Dispose();
        this.client = null;
    }
    private void ReceiveData()
    {
        while (true)
        {
            if(this.client == null)
            {
                this.client = new UdpClient(port);
            }
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP); // Try to receive data
                string text = Encoding.UTF8.GetString(data);
                Debug.Log("<< " + text);
                ResponseBase clientResponse = JsonUtility.FromJson<ResponseBase>(text);
                HandleResponse(clientResponse, text, anyIP);

            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
            }
        }
    }
    private dynamic HandleResponse(ResponseBase clientResponse, string responseRaw, IPEndPoint endPoint = null)
    {
        dynamic academyResponse = null;
        // There must be a better way to do this
        if (clientResponse.responseType == "command")
        {
            switch (clientResponse.type)
            {
                case "connect":
                    academyResponse = JsonUtility.FromJson<ConnectCommand>(responseRaw).Action(this);
                    hasConnection = true;
                    break;
                case "academyReset":
                    academyResponse = "ok";
                    AcademyReset();
                    break;
                case "spawnAgent":
                    academyResponse = JsonUtility.FromJson<SpawnAgentCommand>(responseRaw).Action(this, endPoint);
                    break;
                case "getHeuristic":
                    academyResponse = JsonUtility.FromJson<GetHeuristicCommand>(responseRaw).value;
                    break;
                case "doAction":
                    academyResponse = JsonUtility.FromJson<DoActionCommand>(responseRaw).value;
                    break;
                default:
                    break;
            }
        }
        Debug.Log(">> " + academyResponse);
        if(endPoint != null)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(academyResponse);
            var response = Encoding.UTF8.GetBytes(sendData.Length.ToString());
            client.Send(response, response.Length, endPoint); // Length of the next string
            client.Send(sendData, sendData.Length, endPoint); // The full thing
        }
        return academyResponse;
    }
    public dynamic SendData(int agentId, object data, bool waitForResponse = false)
    {
        if (this.client == null)
        {
            Debug.LogError("Not connected");
            return false;
        }
        if(this.agents[agentId] == null)
        {
            Debug.LogError("Attempted to send data to an agent that doesn't exist.");
            return false;
        }
        if(this.agents[agentId].endpoint == null)
        {
            Debug.LogError("No endpoint");
            return false;
        }
        var dataString = JsonUtility.ToJson(data, false);
        if (waitForResponse)
        {
            this.pauseReceive = true;
        }

        var request = Encoding.UTF8.GetBytes(dataString.Length.ToString());
        var result = this.client.Send(request, request.Length, this.agents[agentId].endpoint);
        if(result != 0)
        {
            Debug.LogError("Failed to send data. Error " + result);
            return false;
        }
        if(waitForResponse)
        {
            ResponseBase response = null;
            while(response == null)
            {
                byte[] responseData = this.client.Receive(ref this.agents[agentId].endpoint);
                string responseText = Encoding.UTF8.GetString(responseData);
                ResponseBase clientResponse = JsonUtility.FromJson<ResponseBase>(responseText);
                response = HandleResponse(clientResponse, responseText);
            }
            this.pauseReceive = false;
            return response;
        }
        return true;
    }

    public override void AcademyReset()
    {
        if(hasConnection && this.spawnedGameObjects.Count >= 1 && this.client != null)
        {
            SendData(0, new ResetSceneCommand());
        }
        foreach (GameObject go in this.spawnedGameObjects)
        {
            Destroy(go);
        }
        this.spawnedGameObjects.Clear();
    }
    public void RegisterAgent(ProxyAgent agent)
	{
        this.agents.Add(agent);
	}

    public GameObject SpawnAgent(int id, Vector3 pos, Quaternion rot, IPEndPoint endpoint)
    {
        var agent = this.agents[this.spawnedGameObjects.Count];
        var gameObject = this.agentPool[this.spawnedGameObjects.Count];

        UnityMainThreadDispatcher.Instance().Enqueue(agent.SetTransform(pos, rot));
        agent.endpoint = endpoint;
        agent.SetAcademy(this);
        UnityMainThreadDispatcher.Instance().Enqueue(agent.SetEnabled(true));
        this.spawnedGameObjects.Add(gameObject);
        return gameObject;
    }
}