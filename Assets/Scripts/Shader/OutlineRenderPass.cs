using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// Shader Graph로 작성된 Shader를 사용할때와
// HLSL 코드로 작성된 Shader를 사용할 때, Shader를 추가하는 방식의 차이가 존재함.
public class OutlineRenderPass : ScriptableRenderPass
{
    // 무엇을 그릴지
    FilteringSettings filteringSettings;
    // shader graph 방식. material을 가져옴. Renderer Pipe Line Asset_Renderer에서 추가해줘야함 (inspector)
    Material outlineMaterial;
    // 어떤 셰이더를 쓸지 ( HLSL 방식 )
    //ShaderTagId shaderTagId;
    // 위와같이 셰이더를 태그로 찾아서 설정해도되고,
    // public material 이런식으로 특정 shader로 만들어진 material을 쓰겠다 해놓고 inspector에서 추가해도됨.
    public OutlineRenderPass(Material material)
    {
        // Pass 생성자에서 실행 시점 지정
        renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;
        // 생성자에서 어떤걸 그릴지 생성. outline pass이므로 outline layer의 오브젝트만 그리겠다.
        filteringSettings = new FilteringSettings(
                            RenderQueueRange.all,
                            LayerMask.GetMask("Outline")
                            );
        // 해당 pass는 Outline 태그를 가진 shader를 사용할것이다.(HLSL 방식)
        //shaderTagId = new ShaderTagId("Outline"); (HLSL 방식)
        outlineMaterial = material;
    }

    // Render Pass의 본체
    public override void Execute(
        ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        
        // Command Buffer 생성.
        CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

        
        // Shader Graph 방식
        DrawingSettings drawingSettings =
            CreateDrawingSettings(
            new ShaderTagId("UniversalForward"),
            ref renderingData,
            SortingCriteria.CommonOpaque
        );
        drawingSettings.overrideMaterial = outlineMaterial;

        // (HLSL 방식)
        // DrawingSettings drawingSettings =
        //     CreateDrawingSettings(
        //         shaderTagId,
        //         ref renderingData,
        //         SortingCriteria.CommonOpaque
        //         );

        // 실제 Draw call 생성
        context.DrawRenderers(
            renderingData.cullResults,
            ref drawingSettings,
            ref filteringSettings
            );

        // Command buffer 제출
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
