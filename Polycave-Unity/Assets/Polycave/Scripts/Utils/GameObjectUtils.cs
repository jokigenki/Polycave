using System;
using UnityEngine;

// Various utility methods for GameObjects
public static class GameObjectUtils
{

    public static void DestroyAllChildren (this GameObject go)
    {
        int count = go.transform.childCount;
        while (count > 0)
        {
            GameObject child = go.transform.GetChild (0).gameObject;
            if (child.tag != "DoNotDestroy")
            {
                child.transform.parent = null;
                GameObject.Destroy (child);
            }
            count--;
        }
    }

    public static void SetSpriteSortingOrder (this GameObject go, int layerID, int order, bool additiveSortOrder)
    {
        SpriteRenderer[] renderers = go.GetComponentsInChildren<SpriteRenderer> ();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (layerID != 0) renderer.sortingLayerID = layerID;
            if (additiveSortOrder) renderer.sortingOrder = order + renderer.sortingOrder;
            else renderer.sortingOrder = order;
        }
    }

    public static void SetSortOrder (GameObject target, int sortOrder)
    {
        if (target == null) return;
        SpriteRenderer sRen = target.GetComponent<SpriteRenderer> ();
        if (sRen != null)
        {
            sRen.sortingOrder = sortOrder;
            return;
        }
        LineRenderer lRen = target.GetComponent<LineRenderer> ();
        if (lRen != null)
        {
            lRen.sortingOrder = sortOrder;
            return;
        }
    }

    public static void SetSpriteSortingOrder (this GameObject go, string layerName, int order, bool additiveSortOrder)
    {
        SpriteRenderer[] renderers = go.GetComponentsInChildren<SpriteRenderer> ();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingLayerName = layerName;
            if (additiveSortOrder) renderer.sortingOrder = order + renderer.sortingOrder;
            else renderer.sortingOrder = order;
        }
    }

    public static Renderer GetRenderer (this GameObject go)
    {
        Renderer renderer = go.GetComponent<Renderer> ();
        if (renderer != null) return renderer;
        renderer = go.GetComponentInChildren<Renderer> ();
        return renderer;
    }

    public static GameObject InstantiateAtZero (GameObject prefab, GameObject parent, string name = "")
    {
        return InstantiateAtZero (prefab, parent.transform, name);
    }

    public static GameObject InstantiateAtZero (GameObject prefab, Transform parent, string name = "")
    {
        if (prefab == null) return null;
        GameObject go = UnityEngine.Object.Instantiate (prefab);
        if (name != "") go.name = name;
        if (parent != null)
        {
            go.transform.SetParent (parent, false);
        }
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler (Vector3.zero);
        go.transform.localScale = Vector3.one;
        return go;
    }

    public static void SetLayerRecursively (this GameObject go, int layerNumber)
    {
        Transform[] transforms = go.GetComponentsInChildren<Transform> (true);
        foreach (Transform trans in transforms)
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static Transform FindDescendent (this Transform aParent, string aName)
    {
        var result = aParent.Find (aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDescendent (aName);
            if (result != null)
                return result;
        }
        return null;
    }

    public static void FitColliderToChildren (this GameObject go)
    {
        BoxCollider collider = go.GetComponent<BoxCollider> ();
        if (collider == null) return;

        Vector3 targetScale = go.transform.localScale;
        targetScale.x = 1 / targetScale.x;
        targetScale.y = 1 / targetScale.y;
        targetScale.z = 1 / targetScale.z;
        bool hasBounds = false;
        Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);

        for (int i = 0; i < go.transform.childCount; ++i)
        {
            Renderer childRenderer = go.transform.GetChild (i).GetComponent<Renderer> ();
            if (childRenderer != null)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate (childRenderer.bounds);
                }
                else
                {
                    bounds = childRenderer.bounds;
                    hasBounds = true;
                }
            }
        }

        Vector3 size = bounds.size;
        size.Scale (targetScale);
        bounds.size = size;
        collider.center = bounds.center - go.transform.position;
        collider.size = bounds.size;
    }

    public static void SetButtonEnabled (this GameObject go, bool value)
    {
        Renderer renderer = go.GetComponent<Renderer> ();
        if (renderer != null)
        {
            renderer.enabled = value;
        }
        Renderer[] renderers = go.GetComponentsInChildren<Renderer> ();
        foreach (Renderer childRenderer in renderers)
        {
            childRenderer.enabled = value;
        }

        BoxCollider collider = go.GetComponent<BoxCollider> ();
        if (collider != null)
        {
            collider.enabled = value;
        }
    }

    public static void SetRenderersEnabledRecursively (this GameObject go, bool value)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer> ();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    public enum HitDirection { NONE, TOP, BOTTOM, FRONT, BACK, RIGHT, LEFT }
    public static HitDirection GetCollisionDirection (GameObject hitter, GameObject hittee)
    {
        RaycastHit myRayHit;
        Vector3 direction = (hitter.transform.position - hittee.transform.position).normalized;
        Ray myRay = new Ray (hittee.transform.position, direction);

        if (Physics.Raycast (myRay, out myRayHit))
        {
            if (myRayHit.collider != null)
            {

                Vector3 MyNormal = myRayHit.normal;
                MyNormal = myRayHit.transform.TransformDirection (MyNormal);

                if (MyNormal == myRayHit.transform.up) return HitDirection.TOP;
                if (MyNormal == -myRayHit.transform.up) return HitDirection.BOTTOM;
                if (MyNormal == myRayHit.transform.forward) return HitDirection.FRONT;
                if (MyNormal == -myRayHit.transform.forward) return HitDirection.BACK;
                if (MyNormal == myRayHit.transform.right) return HitDirection.RIGHT;
                if (MyNormal == -myRayHit.transform.right) return HitDirection.LEFT;
            }
        }

        return HitDirection.NONE;
    }

    public static void Colorise (this GameObject go, Color colour)
    {
        Renderer renderer = go.GetComponent<Renderer> ();
        renderer.material.color = colour;
    }

    public static void SetAlpha (this GameObject go, float alpha)
    {
        Renderer renderer = go.GetComponent<Renderer> ();
        if (renderer == null) return;
        Color colour = renderer.material.color;
        colour.a = alpha;
        renderer.material.color = colour;
    }

    public static bool IsVisible (this GameObject go, Camera cam, bool testChildren)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes (cam);
        if (testChildren)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer> ();
            foreach (Renderer ren in renderers)
            {
                if (GeometryUtility.TestPlanesAABB (planes, ren.bounds)) return true;
            }
            return false;
        }

        Renderer renderer = go.GetComponent<Renderer> ();
        if (renderer == null) return false;
        return GeometryUtility.TestPlanesAABB (planes, renderer.bounds);
    }

    public static void SetMaterialRecursively (this GameObject go, Material material)
    {
        Renderer[] renderers = go.GetComponents<Renderer> ();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }
}