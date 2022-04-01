namespace TextMod.Operations
{
    using System.Text;

    internal enum FlagStatus : byte
    {
        NotFound, Invalid, Found
    }
    internal enum OperationType : byte
    {
        Upper, Lower, Reverse
    }
    internal enum PositionType : byte
    {
        Custom, Even, Odd, Last
    }

    internal static class TextOperations
    {

        #region Common functions

        public static readonly char[] sentenceEndCharacters = new char[] { '.', '?', '!', '\n' };

        #endregion

        #region Simple operations

        public static string ToUpperCase_all(string text)
        {
            return text.ToUpper();
        }
        public static string ToLowerCase_all(string text)
        {
            return text.ToLower();
        }
        public static string ToReverseCase_all(string text)
        {
            StringBuilder txt = new(text);
            for (int i = 0; i < txt.Length; i++)
            {
                if (char.IsUpper(txt[i]))
                {
                    txt[i] = char.ToLower(txt[i]);
                }
                else if (char.IsLower(txt[i]))
                {
                    txt[i] = char.ToUpper(txt[i]);
                }
            }
            return txt.ToString();
        }
        #endregion

        #region Complex operations

        public static string Operate_WordPerSentence(string text, OperationType operation, PositionType position, uint index)
        {
            StringBuilder txt = new(text);
            bool lookingForNewWord = true;
            uint count = 0;
            int lastWordInSentencePos = -1;
            for (int i = 0; i < txt.Length; i++)
            {
                if (sentenceEndCharacters.Contains(txt[i]))
                {
                    count = 0;
                }
                else if (char.IsSeparator(txt[i]) || char.IsPunctuation(txt[i]))
                {
                    lookingForNewWord = true;
                }
                else if (lookingForNewWord)
                {
                    count++;
                    lastWordInSentencePos = i;
                    lookingForNewWord = false;
                }
                if (count > 0 && lookingForNewWord == false && ((position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0)))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(txt[i]) ? char.ToLower(txt[i]) : char.ToUpper(txt[i]),
                        _ => default,
                    };
                }
                else if (lookingForNewWord == false && position == PositionType.Last && count == 0 && lastWordInSentencePos != -1)
                {
                    int j = lastWordInSentencePos;
                    while (j < txt.Length && !(char.IsSeparator(txt[j]) || char.IsPunctuation(txt[j])))
                    {
                        txt[j] = operation switch
                        {
                            OperationType.Upper => char.ToUpper(txt[j]),
                            OperationType.Lower => char.ToLower(txt[j]),
                            OperationType.Reverse => char.IsUpper(txt[j]) ? char.ToLower(txt[j]) : char.ToUpper(txt[j]),
                            _ => default,
                        };
                        j++;
                    }
                    lastWordInSentencePos = -1;
                }
            }
            if (position == PositionType.Last && lastWordInSentencePos != -1)
            {
                int j = lastWordInSentencePos;
                while (j < txt.Length && !(char.IsSeparator(txt[j]) || char.IsPunctuation(txt[j])))
                {
                    txt[j] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[j]),
                        OperationType.Lower => char.ToLower(txt[j]),
                        OperationType.Reverse => char.IsUpper(txt[j]) ? char.ToLower(txt[j]) : char.ToUpper(txt[j]),
                        _ => default,
                    };
                    j++;
                }
                lastWordInSentencePos = -1;
            }
            return txt.ToString();
        }

        public static string Operate_LetterPerSentence(string text, OperationType operation, PositionType position, uint index)
        {
            StringBuilder txt = new(text);
            uint count = 0;
            int lastLetterInSentencePos = -1;
            for (int i = 0; i < txt.Length; i++)
            {
                if (sentenceEndCharacters.Contains(txt[i]))
                {
                    count = 0;
                }
                else if (!(char.IsSeparator(txt[i]) || char.IsPunctuation(txt[i])))
                {
                    count++;
                    lastLetterInSentencePos = i;
                }
                if (count > 0 && (position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(txt[i]) ? char.ToLower(txt[i]) : char.ToUpper(txt[i]),
                        _ => default,
                    };
                }
                else if (position == PositionType.Last && count == 0 && lastLetterInSentencePos != -1)
                {
                    txt[lastLetterInSentencePos] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[lastLetterInSentencePos]),
                        OperationType.Lower => char.ToLower(txt[lastLetterInSentencePos]),
                        OperationType.Reverse => char.IsUpper(txt[lastLetterInSentencePos]) ? char.ToLower(txt[lastLetterInSentencePos]) : char.ToUpper(txt[lastLetterInSentencePos]),
                        _ => default,
                    };
                    lastLetterInSentencePos = -1;
                }
            }
            return txt.ToString();
        }

        public static string Operate_LetterPerWord(string text, OperationType operation, PositionType position, uint index)
        {
            StringBuilder txt = new(text);
            uint count = 0;
            int lastLetterInWordPos = -1;
            for (int i = 0; i < txt.Length; i++)
            {
                if (char.IsPunctuation(txt[i]) || char.IsSeparator(txt[i]))
                {
                    count = 0;
                }
                else
                {
                    count++;
                    lastLetterInWordPos = i;
                }
                if ((count > 0 && position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(txt[i]) ? char.ToLower(txt[i]) : char.ToUpper(txt[i]),
                        _ => default,
                    };
                }
                else if (position == PositionType.Last && count == 0 && lastLetterInWordPos != -1)
                {
                    txt[lastLetterInWordPos] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[lastLetterInWordPos]),
                        OperationType.Lower => char.ToLower(txt[lastLetterInWordPos]),
                        OperationType.Reverse => char.IsUpper(txt[lastLetterInWordPos]) ? char.ToLower(txt[lastLetterInWordPos]) : char.ToUpper(txt[lastLetterInWordPos]),
                        _ => default,
                    };
                    lastLetterInWordPos = -1;
                }
            }
            return txt.ToString();
        }

        #endregion

    }
}