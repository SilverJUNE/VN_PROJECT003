using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

namespace DIALOGUE
{
    /// <summary>
    /// DL_DIALOGUE_DATA Ŭ������ ��ȭ �����͸� �м��ϰ� �����ϴ� Ŭ�����Դϴ�.
    /// </summary>
    public class DL_DIALOGUE_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        /// <summary>
        /// ��ȭ�� �� ���׸�Ʈ�� �����ϴ� ����Ʈ�Դϴ�.
        /// </summary>
        public List<DIALOGUE_SEGMENT> segments;
        /// <summary>
        /// ���׸�Ʈ�� �ĺ��ϴ� ���Խ� �����Դϴ�.
        /// </summary>
        private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

        /// <summary>
        /// DL_DIALOGUE_DATA�� �������Դϴ�. rawDialogue ���ڿ��� �޾� ���׸�Ʈ�� �и��մϴ�.
        /// </summary>
        /// <param name="rawDialogue">�м��� ���� ��ȭ ������</param>
        public DL_DIALOGUE_DATA(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);
        }

        /// <summary>
        /// ���� ��ȭ �����͸� ���׸�Ʈ�� �����ϴ� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="rawDialogue">�м��� ���� ��ȭ ������</param>
        /// <returns>���ص� ��ȭ ���׸�Ʈ�� ����Ʈ</returns>
        private List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
        {
            List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

            int lastIndex = 0;
            // ������ ù ��° �Ǵ� ������ ���׸�Ʈ�� ã���ϴ�.
            DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
            segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
            segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
            segment.signalDelay = 0;
            segments.Add(segment);

            // ��ġ�� ���ٸ� ���� ���׸�Ʈ�� ��ȯ�մϴ�.
            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DIALOGUE_SEGMENT();

                // ���׸�Ʈ�� ���� ��ȣ�� �����ɴϴ�.
                string signalMatch = match.Value;//{A}
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

                // ��ȣ ������ �����ɴϴ�.
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);

                // ���׸�Ʈ�� ��ȭ�� �����ɴϴ�.
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }

            return segments;
        }

        /// <summary>
        /// DIALOGUE_SEGMENT ����ü�� ��ȭ�� ���� ���׸�Ʈ�� ��Ÿ���ϴ�.
        /// </summary>
        public struct DIALOGUE_SEGMENT
        {
            /// <summary>
            /// ��ȭ ���׸�Ʈ�� ���� �ؽ�Ʈ �������� ��Ÿ���ϴ�.
            /// </summary>
            public string dialogue;
            /// <summary>
            /// ���׸�Ʈ�� ������ ���� ��ȣ�� ��Ÿ���ϴ�.
            /// </summary>
            public StartSignal startSignal;
            /// <summary>
            /// ���׸�Ʈ�� ���� ��ȣ�� �����Ǵ� �ð��� ��Ÿ���ϴ�.
            /// </summary>
            public float signalDelay;

            /* �ñ׳� ����
             None   :           �ƹ��͵� ����
             C      : Clear     �����
             A      : Append    �����̱�
             WC     : Wait+C    �� �� �ڿ� �����
             WA     : Wait+A    �� �� �ڿ� �����̱�         
             */

            /// <summary>
            /// ��ȭ ���׸�Ʈ�� ������ ���� ��ȣ�� ��Ÿ���� �������Դϴ�.
            /// </summary>
            public enum StartSignal { NONE, C, A, WA, WC }

            /// <summary>
            /// ��ȭ �ؽ�Ʈ�� �� ���׸�Ʈ�� �߰��Ǿ�� �ϴ����� ��Ÿ���� �οﰪ�Դϴ�.
            /// </summary>
            public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
        }
    }

}

