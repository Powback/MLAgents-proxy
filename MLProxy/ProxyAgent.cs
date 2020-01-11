using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Net;
using MLProxy;

public class ProxyAgent : Agent
{
    public int id;
    public IPEndPoint endpoint;
    private ProxyAcademy academy;

    public override void InitializeAgent()
    {
        AgentReset();
    }
    public void SetAcademy(ProxyAcademy academy)
    {
        this.academy = academy;
    }
    public IEnumerator SetTransform(Vector3 pos, Quaternion rot)
    {
        this.gameObject.transform.position = pos;
        this.gameObject.transform.rotation= rot;
        yield return null;
    }
    public IEnumerator SetEnabled(bool value)
    {
        this.transform.gameObject.SetActive(value);
        yield return null;
    }
    public override void CollectObservations()
    {
        float[] data = academy.SendData(id, new GetObservationsCommand(), true);
        foreach(float value in data)
        {
            AddVectorObs(value);
        }
    }

    public override float[] Heuristic()
    {
        float[] data = academy.SendData(id, new GetHeuristicCommand(), true);
        return data;
    }
    public override void AgentAction(float[] vectorAction)
    {
        var command = new DoActionCommand();
        command.id = this.id;
        command.value = vectorAction;
        float reward = academy.SendData(id, command, true);
        AddReward(reward);
    }
    public override void AgentReset()
    {
    }
}
