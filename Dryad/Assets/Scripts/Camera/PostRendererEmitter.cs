using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public interface PostRendererListenerInterface
{
    void OnPostRender();
}

public class PostRendererEmitter : MonoBehaviour
{
    private List<PostRendererListenerInterface> m_PostRenderListener = new List<PostRendererListenerInterface>();

    public void RegisterPostRendererListener(PostRendererListenerInterface listener)
    {
        m_PostRenderListener.Add(listener);
    }

    public void UnregisterPostRendererListener(PostRendererListenerInterface listener)
    {
        m_PostRenderListener.Remove(listener);
    }

    public IEnumerator OnPostRender()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < m_PostRenderListener.Count; ++i)
        {
            m_PostRenderListener[i].OnPostRender();
        }
    }
}
