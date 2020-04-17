using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCrater : MonoBehaviour
{
    public float minVelocity = 0f;
    public float radiusDeformate = 0.1f;
    public float multiply = 0.04f;

    Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    /*
    void Update()
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            { // if left button pressed...
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    CreateCrater(hit);
                }
            }
        }
    }

    void CreateCrater(Collision collision)
    {
        bool isDeformated = false;
        Vector3[] verticles = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            for (int j = 0; j < collision.contacts.Length; j++)
            {
                Vector3 point = transform.InverseTransformPoint(collision.contacts[j].point);
                Vector3 velocity = transform.InverseTransformVector(collision.relativeVelocity);
                float distance = Vector3.Distance(point, verticles[i]);
                if (distance < radiusDeformate)
                {
                    Vector3 deformate = velocity * (radiusDeformate - distance) * multiply;
                    verticles[i] += deformate;
                    isDeformated = true;
                }
            }
        }
        if (isDeformated)
        {
            mesh.vertices = verticles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

    }
    */
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > minVelocity)
        {
            bool isDeformated = false;
            Vector3[] verticles = mesh.vertices;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                for (int j = 0; j < collision.contacts.Length; j++)
                {
                    Vector3 point = transform.InverseTransformPoint(collision.contacts[j].point);
                    Vector3 velocity = transform.InverseTransformVector(collision.relativeVelocity);
                    float distance = Vector3.Distance(point, verticles[i]);
                    if (distance < radiusDeformate)
                    {
                        Vector3 deformate = velocity * (radiusDeformate - distance) * multiply;
                        verticles[i] += deformate;
                        isDeformated = true;
                    }
                }
            }
            if (isDeformated)
            {
                mesh.vertices = verticles;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                GetComponent<MeshCollider>().sharedMesh = mesh;
            }
        }
    }
}