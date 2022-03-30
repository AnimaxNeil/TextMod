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
            uint count = 1;
            for (int i = 0; i < txt.Length; i++)
            {
                if (char.IsPunctuation(txt[i]))
                {
                    count = 1;
                }
                else if (char.IsSeparator(txt[i]))
                {
                    if (i > 0 && !char.IsSeparator(txt[i - 1]) && !char.IsPunctuation(txt[i - 1]))
                    {
                        count++;
                    }
                }
                else if ((position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(text[i]) ? char.ToLower(text[i]) : char.ToUpper(text[i]),
                        _ => default,
                    };
                }
            }
            return txt.ToString();
        }

        public static string Operate_LetterPerSentence(string text, OperationType operation, PositionType position, uint index)
        {
            StringBuilder txt = new(text);
            uint count = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                if (char.IsPunctuation(txt[i]))
                {
                    count = 0;
                }
                else if (!char.IsSeparator(txt[i]))
                {
                    count++;
                }
                if ((position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(text[i]) ? char.ToLower(text[i]) : char.ToUpper(text[i]),
                        _ => default,
                    };
                }
            }
            return txt.ToString();
        }

        public static string Operate_LetterPerWord(string text, OperationType operation, PositionType position, uint index)
        {
            StringBuilder txt = new(text);
            uint count = 0;
            for (int i = 0; i < txt.Length; i++)
            {
                if (char.IsPunctuation(txt[i]) || char.IsSeparator(txt[i]))
                {
                    count = 0;
                }
                else
                {
                    count++;
                }
                if ((position == PositionType.Custom && count == index) || (position == PositionType.Even && count % 2 == 0) || (position == PositionType.Odd && count % 2 != 0))
                {
                    txt[i] = operation switch
                    {
                        OperationType.Upper => char.ToUpper(txt[i]),
                        OperationType.Lower => char.ToLower(txt[i]),
                        OperationType.Reverse => char.IsUpper(text[i]) ? char.ToLower(text[i]) : char.ToUpper(text[i]),
                        _ => default,
                    };
                }
            }
            return txt.ToString();
        }

        #endregion

    }
}