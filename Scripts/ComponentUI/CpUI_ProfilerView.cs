using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace UIProfiler
{
    public class CpUI_ProfilerView : UIMonoBehaviour
    {
        private static CpUI_ProfilerView instance = null;
        public static CpUI_ProfilerView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_ProfilerView>("pf_ui_profiler_view");
                }

                return instance;
            }
        }

        public Text memoryUsageText; // UI 텍스트 컴포넌트에 연결하세요.

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.LAST, true);
        }

        public void On()
        {
            UIManager.Instance.Show(this);
            enabled = true;
        }

        public void FixedUpdate()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            // 메모리 정보 가져오기
            float totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f); // MB 단위
            float totalReservedMemory = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f); // MB 단위
            float totalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024f); // MB 단위
            float monoHeapSize = Profiler.GetMonoHeapSizeLong() / (1024f * 1024f); // MB 단위
            float monoUsedSize = Profiler.GetMonoUsedSizeLong() / (1024f * 1024f); // MB 단위

            // 디바이스 메모리 정보 (VRAM 및 시스템 메모리 등)
            int systemMemorySize = SystemInfo.systemMemorySize; // 시스템 메모리 (RAM) 크기 (MB)
            int graphicsMemorySize = SystemInfo.graphicsMemorySize; // 그래픽 카드 메모리 (VRAM) 크기 (MB)

            // 메모리 정보 문자열 생성
            string memoryInfo = "[Memory Usage]\n" +
                                $"<color=green>Total Allocated: {totalAllocatedMemory:F2} MB</color>\n" +
                                $"<color=yellow>Total Reserved: {totalReservedMemory:F2} MB</color>\n" +
                                $"<color=red>Total Unused Reserved: {totalUnusedReservedMemory:F2} MB</color>\n" +
                                $"<color=cyan>Mono Heap Size: {monoHeapSize:F2} MB</color>\n" +
                                $"<color=magenta>Mono Used Size: {monoUsedSize:F2} MB</color>\n" +
                                "[System Info]\n" +
                                $"System Memory (RAM): {systemMemorySize} MB\n" +
                                $"Graphics Memory (VRAM): {graphicsMemorySize} MB";

            // UI 텍스트에 정보 출력
            if (memoryUsageText != null)
            {
                memoryUsageText.text = memoryInfo;
            }
        }

        public override bool IsFixed()
        {
            return true;
        }
    }
}