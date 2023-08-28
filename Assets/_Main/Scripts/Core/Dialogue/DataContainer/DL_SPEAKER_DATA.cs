using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using DG.Tweening;
using System;

namespace DIALOGUE
{
    /// <summary>
    /// DL_SPEAKER_DATA 클래스는 대화창에 표시될 화자의 정보를 담고 있습니다.
    /// </summary>
    public class DL_SPEAKER_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        public string name, castName;

        /// <summary>
        /// 화자의 표시 이름을 반환합니다. 대화창에 표시될 이름입니다.
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
        /// DL_SPEAKER_DATA의 새 인스턴스를 초기화합니다.
        /// rawSpeaker 문자열을 분석하여 화자의 이름, 위치, 표현 등의 정보를 추출합니다.
        /// </summary>
        /// <param name="rawSpeaker">화자 정보가 담긴 원시 문자열</param>
        public DL_SPEAKER_DATA(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            // 기본 값을 설정하여 null 참조를 방지합니다.
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            // 일치하는 항목이 없으면 전체 줄이 화자의 이름입니다.
            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            // 그렇지 않으면, 화자 이름을 캐스팅 데이터에서 분리합니다.
            int index = matches[0].Index;

            name = rawSpeaker.Substring(0, index);

            // 각각의 일치하는 항목에 대해 적절한 처리를 수행합니다.
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;

                // 화자 이름이 지정된 경우 처리
                if (match.Value == NAMECAST_ID)
                {
                    startIndex = match.Index + NAMECAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                // 화자의 위치가 지정된 경우 처리
                else if (match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AXISDELIMITER_ID, System.StringSplitOptions.RemoveEmptyEntries);

                    // x, y 좌표를 파싱하여 위치를 설정합니다.
                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }
                // 화자의 표정이 지정된 경우 처리
                else if (match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    // 표정을 분할하고 각 표현의 레이어와 이름을 분리합니다.
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
