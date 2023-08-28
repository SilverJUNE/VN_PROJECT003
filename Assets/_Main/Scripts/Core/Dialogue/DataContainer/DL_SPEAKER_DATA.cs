using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using DG.Tweening;
using System;

namespace DIALOGUE
{
    /// <summary>
    /// DL_SPEAKER_DATA Ŭ������ ��ȭâ�� ǥ�õ� ȭ���� ������ ��� �ֽ��ϴ�.
    /// </summary>
    public class DL_SPEAKER_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        public string name, castName;

        /// <summary>
        /// ȭ���� ǥ�� �̸��� ��ȯ�մϴ�. ��ȭâ�� ǥ�õ� �̸��Դϴ�.
        /// </summary>


        public string displayName => isCastingName ? castName : name;

        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }
        public bool isCastingName => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;

        public bool makeCharacterEnter = false;

        private const string NAMECAST_ID               = " as ";
        private const string POSITIONCAST_ID           = " at ";
        private const string EXPRESSIONCAST_ID         = " [";
        private const char   AXISDELIMITER_ID          = ':';
        private const char   EXPRESSIONLAYER_JOINER    = ',';
        private const char   EXPRESSIONLAYER_DELIMITER = ':';

        private const string ENTER_KEYWORD = "enter ";

        private string ProcessKeywords(string rawSpeaker)
        {
            if(rawSpeaker.StartsWith(ENTER_KEYWORD))
            {
                rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharacterEnter = true;
            }
            return rawSpeaker;
        }
        

        /// <summary>
        /// DL_SPEAKER_DATA�� �� �ν��Ͻ��� �ʱ�ȭ�մϴ�.
        /// rawSpeaker ���ڿ��� �м��Ͽ� ȭ���� �̸�, ��ġ, ǥ�� ���� ������ �����մϴ�.
        /// </summary>
        /// <param name="rawSpeaker">ȭ�� ������ ��� ���� ���ڿ�</param>
        public DL_SPEAKER_DATA(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            // �⺻ ���� �����Ͽ� null ������ �����մϴ�.
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            // ��ġ�ϴ� �׸��� ������ ��ü ���� ȭ���� �̸��Դϴ�.
            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            // �׷��� ������, ȭ�� �̸��� ĳ���� �����Ϳ��� �и��մϴ�.
            int index = matches[0].Index;

            name = rawSpeaker.Substring(0, index);

            // ������ ��ġ�ϴ� �׸� ���� ������ ó���� �����մϴ�.
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;

                // ȭ�� �̸��� ������ ��� ó��
                if (match.Value == NAMECAST_ID)
                {
                    startIndex = match.Index + NAMECAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                // ȭ���� ��ġ�� ������ ��� ó��
                else if (match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);

                    // x, y ��ǥ�� �Ľ��Ͽ� ��ġ�� �����մϴ�.
                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }
                // ȭ���� ǥ���� ������ ��� ó��
                else if (match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    // ǥ���� �����ϰ� �� ǥ���� ���̾�� �̸��� �и��մϴ�.
                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER)
                        .Select(x =>
                        {
                            var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);
                            if (parts.Length == 2)
                                return (int.Parse(parts[0]), parts[1]);
                            else
                                return (0, parts[0]);
                        }).ToList();
                }
            }
        }

    }

}
