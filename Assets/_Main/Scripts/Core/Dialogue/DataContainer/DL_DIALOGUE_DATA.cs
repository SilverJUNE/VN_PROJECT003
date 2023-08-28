using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace DIALOGUE
{
    /// <summary>
    /// DL_DIALOGUE_DATA 클래스는 대화 데이터를 분석하고 저장하는 클래스입니다.
    /// </summary>
    public class DL_DIALOGUE_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        /// <summary>
        /// 대화의 각 세그먼트를 저장하는 리스트입니다.
        /// </summary>
        public List<DIALOGUE_SEGMENT> segments;
        /// <summary>
        /// 세그먼트를 식별하는 정규식 패턴입니다.
        /// </summary>
        private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

        /// <summary>
        /// DL_DIALOGUE_DATA의 생성자입니다. rawDialogue 문자열을 받아 세그먼트로 분리합니다.
        /// </summary>
        /// <param name="rawDialogue">분석할 원시 대화 데이터</param>
        public DL_DIALOGUE_DATA(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);
        }

        /// <summary>
        /// 원시 대화 데이터를 세그먼트로 분해하는 메소드입니다.
        /// </summary>
        /// <param name="rawDialogue">분석할 원시 대화 데이터</param>
        /// <returns>분해된 대화 세그먼트의 리스트</returns>
        private List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
        {
            List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

            int lastIndex = 0;
            // 파일의 첫 번째 또는 유일한 세그먼트를 찾습니다.
            DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
            segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
            segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
            segment.signalDelay = 0;
            segments.Add(segment);

            // 매치가 없다면 현재 세그먼트를 반환합니다.
            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DIALOGUE_SEGMENT();

                // 세그먼트의 시작 신호를 가져옵니다.
                string signalMatch = match.Value;//{A}
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

                // 신호 지연을 가져옵니다.
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);

                // 세그먼트의 대화를 가져옵니다.
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }

            return segments;
        }

        /// <summary>
        /// DIALOGUE_SEGMENT 구조체는 대화의 개별 세그먼트를 나타냅니다.
        /// </summary>
        public struct DIALOGUE_SEGMENT
        {
            /// <summary>
            /// 대화 세그먼트의 실제 텍스트 콘텐츠를 나타냅니다.
            /// </summary>
            public string dialogue;
            /// <summary>
            /// 세그먼트가 시작할 때의 신호를 나타냅니다.
            /// </summary>
            public StartSignal startSignal;
            /// <summary>
            /// 세그먼트의 시작 신호가 지연되는 시간을 나타냅니다.
            /// </summary>
            public float signalDelay;

            /* 시그널 설명
             None   :           아무것도 없음
             C      : Clear     지우기
             A      : Append    덧붙이기
             WC     : Wait+C    몇 초 뒤에 지우기
             WA     : Wait+A    몇 초 뒤에 덧붙이기         
             */

            /// <summary>
            /// 대화 세그먼트가 시작할 때의 신호를 나타내는 열거형입니다.
            /// </summary>
            public enum StartSignal { NONE, C, A, WA, WC }

            /// <summary>
            /// 대화 텍스트가 이 세그먼트에 추가되어야 하는지를 나타내는 부울값입니다.
            /// </summary>
            public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
        }
    }

}

