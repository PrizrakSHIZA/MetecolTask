using DecalSystem;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;


public class MeshDeformer : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] meshvertices;
    GameObject decal;

    [Header("Deforming settings")]
    [Tooltip("Radius of mesh deformation")]
    public float radius = 1f;
    [Tooltip("Multuply deplacement value")]
    public float multiply = 1f;
    [Tooltip("Set the object tag that can make effect to collision. If lenght of this var = 0, all collisions will count")]
    public string[] tags;
    [Tooltip("Updating collider mesh to deplaced mesh. Mesh collider required")]
    public bool UpdateCollider = false;

    [Header("Decal system")]
    [Tooltip("Select layermask that will affect on the script. All selected layers will affect on sprite of decal system.")]
    public LayerMask layermask;
    [Tooltip("Material that must contain needed sprite.")]
    public Material material;
    [Tooltip("Sprite that will be used placed on mesh")]
    public Sprite sprite;
    [Tooltip("Additional sprite size")]
    public float AdditionalSize = 0f;

    private bool collided = false;
    private Vector3 bulletDirection;

    Vector3 point, point2, deformate, avgPoint = new Vector3(0, 0, 0), direction = new Vector3();
    Rigidbody rigitbody;
    float distance, temp;

    private void Start()
    {
        rigitbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(!collided)
            bulletDirection = rigitbody.velocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collided = true;
        //calculation average coordinates of all contact points
        CalculateAvrgPoint(collision);

        //Checking for another colliders near
        Collider[] colliders = Physics.OverlapSphere(avgPoint, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                deformingMesh = collider.gameObject.GetComponent<MeshFilter>().mesh;
                meshvertices = deformingMesh.vertices;

                if (tags.Length == 0 || Array.IndexOf(tags, collider.gameObject.tag) != -1)
                {
                    //CalculateAvrgPoint(collision);
                    Debug.Log(collider.gameObject.name);
                    for (int i = 0; i < meshvertices.Length; i++)
                    {
                        CalculatePoint(collider.gameObject, i);
                    }
                    deformingMesh.vertices = meshvertices;
                    deformingMesh.RecalculateNormals();

                    //Updating mesh collider
                    if (UpdateCollider)
                        collider.gameObject.GetComponent<MeshCollider>().sharedMesh = deformingMesh;

                }
            }
        }
        //adding decal
        PlaceDecal(collision);
        //destroy bullet
        Destroy(gameObject);

    }

    private void CalculateAvrgPoint(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            avgPoint += collision.contacts[i].point;
        }
        avgPoint = avgPoint / collision.contacts.Length;
    }

    private void CalculatePoint(GameObject someobject, int i)
    {
        point = avgPoint;
        point2 = someobject.transform.TransformPoint(meshvertices[i]);
        direction = bulletDirection;
        distance = (point - someobject.transform.TransformPoint(meshvertices[i])).sqrMagnitude;
        if(distance < radius * radius)
        {
            temp = (radius * radius - distance) / (radius * radius);
            deformate = point2 + direction * temp * multiply;
            meshvertices[i] = someobject.transform.InverseTransformPoint(deformate);
        }
    }

    private void PlaceDecal(Collision collision)
    {
        decal = new GameObject("Decal");
        decal.transform.parent = collision.gameObject.transform;
        decal.transform.position = collision.contacts[0].point;
        decal.transform.localScale = decal.transform.localScale * (radius + AdditionalSize);
        decal.transform.LookAt(collision.gameObject.transform);
        Decal decalscript = decal.AddComponent<Decal>();
        decalscript.Material = material;
        decalscript.Sprite = sprite;
        decalscript.MaxAngle = 180f;
        decalscript.Offset = 0.005f;
        decalscript.LayerMask = layermask;
        decalscript.BuildAndSetDirty();
    }
}
