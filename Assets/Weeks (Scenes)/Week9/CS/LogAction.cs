using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LogAction", story: "Log A [Message]", category: "Action", id: "a7597009f18fd4568d48daa61bc858b5")]
public partial class LogAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Message;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

