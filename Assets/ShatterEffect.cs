using UnityEngine;
using System.Collections;

public class ShatterEffect : MonoBehaviour
{
    [Header("Audio settings")]
    [SerializeField] AudioClip glass;
    [SerializeField] AudioSource audioSource;


    [Header("Glass settings ")]
    [SerializeField] float TriangleSize;
    [SerializeField] float explosionForce;
    [SerializeField] private int numOfTriangles = 100;


    private bool isShattered = false;

    private void OnMouseDown()
    {
        ApplyShatter();
    }
    public void ApplyShatter()
    {
        if (!isShattered)
        {
            //Play shatter sound effect
            audioSource.clip = glass;
            audioSource.Play();

            //call the shatter method
            ShatterMesh(explosionForce, TriangleSize);
            isShattered = true;
        }
    }

    public void ShatterMesh(float explosionForce, float TriangleSize)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        Collider collider = GetComponent<Collider>();

        if (collider != null)
        {
            collider.enabled = false;
        }

        Mesh MeshObject = meshFilter.mesh;

        Renderer renderer = GetComponent<Renderer>();

        Material[] materials = renderer != null ? renderer.materials : new Material[0];


        //Creating three attributes for the new traingle mesh
        Vector3[] verts = MeshObject.vertices;
        Vector3[] normals = MeshObject.normals;
        Vector2[] uvs = MeshObject.uv;


        int createdTriangles = 0;

        for (int submesh = 0; submesh < MeshObject.subMeshCount; submesh++)
        {
            int[] indices = MeshObject.GetTriangles(submesh);

            // iterates over the triangles of the current submesh.
            for (int i = 0; i < indices.Length; i += 3)
            {
                if (createdTriangles >= numOfTriangles)
                {
                    // If the desired number of triangles is reached, break out of the loop
                    break;
                }

                // Create a new triangle
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];

                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];

                    Vector3 randomOffset = new (Random.Range(-0.1f, 0.2f), Random.Range(-0.1f, 0.2f), Random.Range(-0.1f, 0.2f));
                    newVerts[n] = (verts[index] + randomOffset) * TriangleSize;

                    newUvs[n] = uvs[index];

                    newNormals[n] = normals[index];
                }

                //A new Mesh object is then created and assigned these vertices,
                //normals, UVs, and a set of indices (triangles) to form a triangle.
                Mesh mesh = new()
                {
                    vertices = newVerts,
                    normals = newNormals,
                    uv = newUvs,
                    triangles = new int[] { 0, 1, 2, 2, 1, 0 }
                };

                GameObject _pieces = new("Piece " + (i / 3));

                _pieces.transform.SetParent(transform);
                _pieces.transform.SetPositionAndRotation(transform.position, transform.rotation);
                _pieces.AddComponent<MeshRenderer>().material = materials[submesh];
                _pieces.AddComponent<MeshFilter>().mesh = mesh;
                _pieces.AddComponent<BoxCollider>();


                // Applying explosion force
                Vector3 explosionPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), Random.Range(-0.5f, 0.5f));
                _pieces.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(300, 400), explosionPos, explosionForce );


                createdTriangles++;
            }
        }

        // Disable the renderer after splitting
        if (renderer != null)
        {
            renderer.enabled = false;
        }

     
    }
}