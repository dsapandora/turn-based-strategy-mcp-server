using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Utilities
{
    /// <summary>
    /// Combines multiple meshes into a single mesh.
    /// </summary>
    public class CombineMeshes : MonoBehaviour
    {
        [Tooltip("Parent GameObject containing the meshes to combine.")]
        public GameObject parentObject;

        [Tooltip("Material to apply to the combined mesh.")]
        public Material combinedMaterial;

        [Tooltip("Should the original GameObjects be destroyed after combining?")]
        public bool deleteOriginals = true;

        public void Combine()
        {
            if (parentObject == null)
            {
                Debug.LogError("Parent Object is not set.");
                return;
            }

            MeshFilter[] meshFilters = parentObject.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0)
            {
                Debug.LogWarning("No MeshFilters found in children of the parent object.");
                return;
            }

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;

            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh == null)
                    continue;

                combine[i].mesh = mf.sharedMesh;
                combine[i].transform = mf.transform.localToWorldMatrix;
                i++;
            }

            GameObject combinedObject = new GameObject("CombinedMesh");
            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine, true, true);
            combinedMeshFilter.mesh = combinedMesh;
            combinedMeshRenderer.material = combinedMaterial;

            if (deleteOriginals)
            {
                foreach (var mf in meshFilters)
                {
                    DestroyImmediate(mf.gameObject);
                }
            }

            Debug.Log($"Combined {meshFilters.Length} meshes into {combinedObject.name}.");
        }
    }
}