using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class FSMNodeBase : Node
{
    [Input(backingValue = ShowBackingValue.Never)] public FSMNodeBase Entry;

    protected IEnumerable<T> GetAllOnPort<T>(string fieldName) where T : FSMNodeBase
    {
        NodePort port = GetOutputPort(fieldName);
        for (var portIndex = 0; portIndex < port.ConnectionCount; portIndex++) {
            yield return port.GetConnection(portIndex).node as T;
        }
    }

    protected T GetFirst<T>(string fieldName) where T : FSMNodeBase
    {
        NodePort port = GetOutputPort(fieldName);
        if (port.ConnectionCount > 0)
            return port.GetConnection(0).node as T;
        return null;
    }
}
