using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class BlurScreenManager
    {
        RenderTexture blurScreenTexture;  //最后一次截屏的模糊图
        
        Material blurMaterial;
        RenderTextureFormat rtFormat = RenderTextureFormat.ARGB32;
        bool isCapturingBlur = false;  //同一帧触发两次截屏，会导致CaptureScreenshotAsTexture接口报错，所以加个标记
        Queue<Action> callBackQueue = new();
        Queue<RenderTexture> rtPool = new();
        Dictionary<RenderTexture,int> refCountMap = new();
        float lastBlurTime=0;
        private string lastBlurKey = string.Empty;        
        int autoId = 0;
        /// <summary>
        /// for editor debug
        /// </summary>
        public Dictionary<RenderTexture,int> RefCountMap
        {
            get { return refCountMap; }
        }
        private void CheckInit()
        {
            if (blurMaterial == null)
            {
                var shader = Shader.Find("Custom/Custom_TextureBlur");
                if (shader == null)
                {
                    Debug.LogError("shader not found");
                }
                blurMaterial= new Material(shader);
            }
        }


        bool isValidRT(RenderTexture rt)
        {
            if(rt==null)
            {
                return false;
            }

            return (refCountMap.ContainsKey(rt) == false || refCountMap[rt] <= 0);
        }
        
        RenderTexture GetTargetRT()
        {
            if(isValidRT(blurScreenTexture))  //如果上一次的模糊图没有被引用，就复用,rtPool可能还没包含，所以要判断一下
            {
                return blurScreenTexture;
            }

            while (rtPool.Count>0)
            {
                var poolRT = rtPool.Dequeue();
                if (isValidRT(poolRT))
                {
                    return poolRT;
                }
            }
           
            var ratio = 0.3f;
            var newRT = new RenderTexture((int)(Screen.width*ratio), (int)(Screen.height*ratio), 0,rtFormat);
            newRT.name = "RT_" + autoId++;
            return newRT;
        }
        
        IEnumerator CacheAndBlur()
        {
            yield return new WaitForEndOfFrame();
            var source = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();
            
            blurScreenTexture= GetTargetRT();
            
            CheckInit();
            RenderTexture rt1, rt2;
            //use quarter downsampling 
            rt1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4,0,rtFormat);
            rt2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4,0,rtFormat);
            Graphics.Blit(source, rt1, blurMaterial, 0);
            var iteration = 1;
            for (var i = 0; i < iteration; i++)
            {
                Graphics.Blit(rt1, rt2, blurMaterial, 1);
                Graphics.Blit(rt2, rt1, blurMaterial, 2);
            }
            
            Graphics.Blit(rt1, blurScreenTexture);
            UnityEngine.Object.Destroy(source);
            RenderTexture.ReleaseTemporary(rt1);
            RenderTexture.ReleaseTemporary(rt2);
            isCapturingBlur = false;
            
            while (callBackQueue.Count > 0)
            {
                var action = callBackQueue.Dequeue();
                action?.Invoke();
            }
        }

        public void DoCacheTexture(Action complete,string blurKey)
        {
            //防止短时间内重复打开同一个界面，导致模糊图的颜色值不断叠加，导致原来越亮
            if (blurScreenTexture!=null && Time.realtimeSinceStartup - lastBlurTime < 10f && lastBlurKey == blurKey)
            {
                complete?.Invoke();
                return;
            }
            lastBlurTime = Time.realtimeSinceStartup;
            lastBlurKey = blurKey;
            callBackQueue.Enqueue(complete);
            if (isCapturingBlur)
            {
                return;
            }
            isCapturingBlur = true;
            GameModule.Resource.StartCoroutine(CacheAndBlur());
        }
        
        public RenderTexture GetBlurTextureCopy()
        {
            if(blurScreenTexture == null)
            {
                return null;
            }

            if(refCountMap.ContainsKey(blurScreenTexture))
            {
                refCountMap[blurScreenTexture]++;
            }
            else
            {
                refCountMap.Add(blurScreenTexture,1);
            }
            return blurScreenTexture;
        }
        
        public void ReleaseRT(RenderTexture rt)
        {
            if(rt == null)
            {
                return;
            }
            if(refCountMap.ContainsKey(rt))
            {
                refCountMap[rt]--;
                if(refCountMap[rt] <= 0)
                {
                    refCountMap.Remove(rt);
                    rtPool.Enqueue(rt);
                }
            }
        }
    }
}
