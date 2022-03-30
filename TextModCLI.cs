namespace TextMod.CLI
{
    using System;
    using System.Text;
    using TextMod.Operations;


    internal class TextModCLI
    {

        #region Common variables and functions

        const string noInput_error = "Syntax-Error: insuffecient input.";
        const string operation_error = "Syntax-Error: invalid operation.";
        const string firstSelector_error = "Syntax-Error: invalid first selector.";
        const string secondSelector_error = "Syntax-Error: invalid second selector.";
        const string comboOperationWithSelectors_error = "Syntax-Error: cannot put selectors with combo operations.";
        const string firstSelectorWithoutOperation_error = "Syntax-Error: cannot place first selector before or without operation.";
        const string secondSelectorWithoutOperation_error = "Syntax-Error: cannot place second selector before or without operation.";
        const string secondSelectorWithoutFirst_error = "Syntax-Error: cannot place second selector before or without first selector.";
        const string logical_error = "Logical-Error: something went wrong.";
        private static bool caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi, sntcs;
        private static uint arg_index, pos_index;

        private static bool IsAllFalse(params bool[] booleans)
        {
            foreach (bool bln in booleans)
            {
                if (bln)
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsAnyTrue(params bool[] booleans)
        {
            foreach (bool bln in booleans)
            {
                if (bln)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region Flag validation functions

        private static FlagStatus ExtractOperationalFlag(string flag)
        {
            if (flag[0] != '-')
            {
                return FlagStatus.NotFound;
            }
            switch (flag)
            {
                case "-SENTENCECASE":
                case "-SENTENCE":
                case "-SEN":
                    sntcs = true;
                    break;
                case "-UPPERCASE":
                case "-UPPER":
                case "-U":
                case "-CAPITAL":
                case "-C":
                    caps = true;
                    break;
                case "-LOWERCASE":
                case "-LOWER":
                case "-L":
                case "-SMALL":
                case "-S":
                    ncaps = true;
                    break;
                case "-TOOGLECASE":
                case "-TOOGLE":
                case "-T":
                case "-REVERSECASE":
                case "-REVERSEUPPERLOWER":
                case "-RUL":
                case "-REVERSECAPITALSMALL":
                case "-RCS":
                    rcaps = true;
                    break;
                default:
                    return FlagStatus.Invalid;
            }
            return FlagStatus.Found;
        }
        private static FlagStatus ExtractFirstSelectetorFlag(string flag)
        {
            if (flag[0] != '-')
            {
                return FlagStatus.NotFound;
            }
            switch (flag)
            {
                case "-WS":
                case "-WORDPERSENTENCE":
                case "-WPS":
                    wrdpsnt = true;
                    break;
                case "-LS":
                case "-LETTERPERSENTENCE":
                case "-LPS":
                    ltrpsnt = true;
                    break;
                case "-LW":
                case "-LETTERPERWORD":
                case "-LPW":
                    ltrpwrd = true;
                    break;
                default:
                    return FlagStatus.Invalid;
            }
            return FlagStatus.Found;
        }
        private static FlagStatus ExtractSecondSelectorFlag(string flag)
        {
            if (flag[0] != '-')
            {
                return FlagStatus.NotFound;
            }
            switch (flag)
            {
                case "-END":
                case "-LAST":
                case "-L":
                    lasti = true;
                    break;
                case "-EVEN":
                case "-E":
                    eveni = true;
                    break;
                case "-ODD":
                case "-O":
                    oddi = true;
                    break;
                case "-START":
                case "-FIRST":
                case "-F":
                    cstmi = true;
                    pos_index = 1;
                    break;
                default:
                    try
                    {
                        pos_index = uint.Parse(flag[1..]);
                        if (pos_index >= 1)
                        {
                            cstmi = true;
                            break;
                        }
                        return FlagStatus.Invalid;
                    }
                    catch (Exception)
                    {
                        return FlagStatus.Invalid;
                    }
            }
            return FlagStatus.Found;
        }

        private static bool ExtractFlags(string[] args)
        {
            if (arg_index >= args.Length)
            {
                OutputToConsole(noInput_error);
                return false;
            }
            FlagStatus flagStatus = ExtractOperationalFlag(args[arg_index].ToUpper());
            if (flagStatus == FlagStatus.Found)
            {
                arg_index++;
            }
            if (flagStatus == FlagStatus.Invalid)
            {
                OutputToConsole(operation_error);
                return false;
            }
            if (arg_index >= args.Length)
            {
                OutputToConsole(noInput_error);
                return false;
            }
            flagStatus = ExtractFirstSelectetorFlag(args[arg_index].ToUpper());
            if (flagStatus == FlagStatus.Found)
            {
                if (sntcs)
                {
                    OutputToConsole(comboOperationWithSelectors_error);
                    return false;
                }
                if (IsAllFalse(caps, ncaps, rcaps))
                {
                    OutputToConsole(firstSelectorWithoutOperation_error);
                    return false;
                }
                arg_index++;
            }
            else if (flagStatus == FlagStatus.Invalid)
            {
                OutputToConsole(firstSelector_error);
                return false;
            }
            if (arg_index >= args.Length)
            {
                OutputToConsole(noInput_error);
                return false;
            }
            flagStatus = ExtractSecondSelectorFlag(args[arg_index].ToUpper());
            if (flagStatus == FlagStatus.Found)
            {
                if (sntcs)
                {
                    OutputToConsole(comboOperationWithSelectors_error);
                    return false;
                }
                if (IsAllFalse(caps, ncaps, rcaps))
                {
                    OutputToConsole(secondSelectorWithoutOperation_error);
                    return false;
                }
                if (IsAllFalse(wrdpsnt, ltrpsnt, ltrpwrd))
                {
                    OutputToConsole(secondSelectorWithoutFirst_error);
                    return false;
                }
                arg_index++;
            }
            else if (flagStatus == FlagStatus.Invalid)
            {
                OutputToConsole(secondSelector_error);
                return false;
            }
            if (arg_index >= args.Length)
            {
                OutputToConsole(noInput_error);
                return false;
            }
            return true;
        }

        #endregion


        #region IO operations

        private static string ExtractInput(string[] args)
        {
            StringBuilder text = new();
            for (uint i = arg_index; i < args.Length; i++)
            {
                text.Append(args[i]).Append(' ');
            }
            return text.ToString();
        }

        private static void OutputToConsole(string text)
        {
            Console.WriteLine(text);
        }

        private static string OperateInput(string inpText)
        {
            string modText;
            if (IsAllFalse(sntcs, caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                sntcs = true;
            }
            if (sntcs && IsAllFalse(caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToLowerCase_all(inpText);
                modText = modText = TextOperations.Operate_LetterPerSentence(modText, OperationType.Upper, PositionType.Custom, 1);
            }
            else if (caps && IsAllFalse(sntcs, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToUpperCase_all(inpText);
            }
            else if (ncaps && IsAllFalse(sntcs, caps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToLowerCase_all(inpText);
            }
            else if (rcaps && IsAllFalse(sntcs, caps, ncaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToReverseCase_all(inpText);
            }
            else if (!sntcs && IsAnyTrue(caps, ncaps, rcaps) && IsAnyTrue(wrdpsnt, ltrpsnt, ltrpwrd))
            {
                if (IsAllFalse(cstmi, lasti, eveni, oddi))
                {
                    cstmi = true;
                    pos_index = 1;
                }
                OperationType operation = caps ? OperationType.Upper : ncaps ? OperationType.Lower : OperationType.Reverse;
                PositionType position = cstmi ? PositionType.Custom : eveni ? PositionType.Even : oddi ? PositionType.Odd : PositionType.Last;

                if (wrdpsnt)
                {
                    modText = TextOperations.Operate_WordPerSentence(inpText, operation, position, pos_index);
                }
                else if (ltrpsnt)
                {
                    modText = TextOperations.Operate_LetterPerSentence(inpText, operation, position, pos_index);
                }
                else
                {
                    modText = TextOperations.Operate_LetterPerWord(inpText, operation, position, pos_index);
                }
            }
            else
            {
                return logical_error;
            }
            return modText;
        }

        #endregion


        private static void Main(string[] args)
        {
            if (!ExtractFlags(args))
            {
                return;
            }
            string inpText = ExtractInput(args);
            string modText = OperateInput(inpText);
            OutputToConsole(modText);
        }
    }
}