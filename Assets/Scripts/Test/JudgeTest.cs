using System.Collections.Generic;
using UnityEngine;  

namespace JudgeTest
{
    public class JudgeTest : MonoBehaviour
    {
        public List<MapSegment> judgeList = new List<MapSegment>();
        private TypingJudder typingJudder;

        private void Start()
        {
            typingJudder = new TypingJudder("きゅうきゅうしゃとかっぱ");
            judgeList = typingJudder.judgeList;

            foreach(var segment in judgeList)
            {
                Debug.Log(segment.ToString());
            }
        }
    }
}