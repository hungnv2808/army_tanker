using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : MonoBehaviour
{
    [SerializeField] private MovementNode[] m_relativeNodes;
    private Transform m_transform;
    private void Start() {
        m_transform = this.transform;
    }
    private void OnDrawGizmos() {
        if (m_relativeNodes != null) {
            for (int i = 0; i < m_relativeNodes.Length; i++)
            {
                Debug.DrawLine(this.transform.position, m_relativeNodes[i].transform.position, Color.white);
            }
        }
    }
    public Vector3 Position {
        get {
            return m_transform.position;
        }
    }
    public MovementNode[] NextNodes {
        get {
            return m_relativeNodes;
        }
    }
}
