using Lowscope.Saving;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Lowscope.Saving.Components
{
    /// <summary>
    /// Stores data for navMeshAgents
    /// </summary>

    [AddComponentMenu("Saving/Components/Save NavmeshAgent Data"), DisallowMultipleComponent]
    public class SaveNavMeshAgentData : MonoBehaviour, ISaveable
    {
        private Vector3 lastvelocity;
        private Vector3 lastDestination;
        private Vector3 lastPos;
        private Vector3 lastRotation;
        private Vector3 activeRotation;

        [System.Serializable]
        public struct SaveData
        {
            public Vector3 velocity;
            public Vector3 destination;
            public Vector3 position;
            public Vector3 rotation;
        }

        public void OnLoad(string data)
        {        
            StartCoroutine(delay(0.001f, data));//wait a teeny bit because if we don't, the navmesh might not have finished loading                        
        }
        
        IEnumerator delay(float time, string data)
        {
            yield return new WaitForSeconds(time);
            GetComponent<NavMeshAgent>().Warp(JsonUtility.FromJson<SaveData>(data).position);
            GetComponent<NavMeshAgent>().velocity = JsonUtility.FromJson<SaveData>(data).velocity;
            GetComponent<NavMeshAgent>().SetDestination(JsonUtility.FromJson<SaveData>(data).destination);
            transform.rotation = Quaternion.Euler(lastRotation);

            lastPos = transform.position;
            lastvelocity = GetComponent<NavMeshAgent>().velocity;
            lastDestination = GetComponent<NavMeshAgent>().destination;
        }

        public string OnSave()
        {
            lastRotation = activeRotation;
            lastvelocity = GetComponent<NavMeshAgent>().velocity;
            lastDestination = GetComponent<NavMeshAgent>().destination;
            lastPos = transform.position;
            return JsonUtility.ToJson(new SaveData() {velocity = lastvelocity, destination = lastDestination, position = lastPos, rotation = transform.rotation.eulerAngles});
        }

        public bool OnSaveCondition()
        {
            activeRotation = transform.rotation.eulerAngles;
            return ((lastvelocity != GetComponent<NavMeshAgent>().velocity) || (lastDestination != GetComponent<NavMeshAgent>().destination)||(lastPos != transform.position)||(lastRotation != activeRotation));
        }
    }
}
