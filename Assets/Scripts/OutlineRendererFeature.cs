using UnityEngine;
using UnityEngine.Rendering.Universal;

// RenderPass 생성 및 Renderer에 등록
public class OutlineRendererFeature : ScriptableRendererFeature
{
    // shader graph 방식에선,
    // pass에 선언되어있는 material을 여기 feature에서 inspector 에서 받아줘야함.
    public Material outlineMaterial;
    // Render Pass 담을 변수
    OutlineRenderPass outlinePass;

    // Pass 생성
    public override void Create()
    {
        outlinePass = new OutlineRenderPass();
        // 인스펙터에서 받아온 material pass에 적용시켜주기. (shader graph 방식)
        outlinePass.outlineMaterial = outlineMaterial;   
    }

    // Renderer에 등록. 의미 : 이제 이 Renderer는 Outline Pass 를 포함한다.

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }

}
