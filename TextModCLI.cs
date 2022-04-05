namespace TextMod.CLI
{
    using System;
    using System.Text;
    using TextMod.Operations;


    internal class TextModCLI
    {

        #region Common variables and functions

        private const string noInput_error = "Syntax-Error: insuffecient input. Use -help to see a list of usable commands.";
        private const string operation_error = "Syntax-Error: invalid operation.";
        private const string firstSelector_error = "Syntax-Error: invalid first selector.";
        private const string secondSelector_error = "Syntax-Error: invalid second selector.";
        private const string comboOperationWithSelectors_error = "Syntax-Error: cannot put selectors with combo-operations and non-operations.";
        private const string firstSelectorWithoutOperation_error = "Syntax-Error: cannot place first selector before or without operation.";
        private const string secondSelectorWithoutOperation_error = "Syntax-Error: cannot place second selector before or without operation.";
        private const string secondSelectorWithoutFirst_error = "Syntax-Error: cannot place second selector before or without first selector.";
        private const string logical_error = "Logical-Error: something went wrong.";
        private const string help_text =
            "-------------------------- \n" +
            "TextMod v0.1 © 2022 AnimaxNeil \n\n" +
            "List of operations : \n" +
            "1. c, capital, u, upper, uppercase \n" +
            "   => Capatalize or convert to uppercase. \n" +
            "2. s, Small, l, lower, lowercase \n" +
            "   => Uncapitalize or convert to lover case. \n" +
            "3. rcs, reversecapitalsmall, rul, reverseupperlower, reversecase, t, toogle, tooglecase \n" +
            "   => Reverse the case or convert upper case to lower case and vice versa. \n" +
            "4. sen, sentence, sentencecase \n" +
            "   => Converts entire text to sentence case. \n\n" +
            "List of first selectors : \n" +
            "1. wps, wordpersentence, ws \n" +
            "   => Selection applies to words in each sentence. \n" +
            "2. lps, letterpersentence, ls \n" +
            "   => Selection applies to letters in each sentence. \n" +
            "3, lpw, letterperword, lw \n" +
            "   => Selection applies to letters in eaxh word. \n\n" +
            "List of second selectors : \n" +
            "1. f, first, start \n" +
            "   => First one or the one at the start. \n" +
            "2. l, last, end \n" +
            "   => Last one or the one at the end. \n" +
            "3. e, even \n" +
            "   => The ones at even positions. \n" +
            "4. o, odd \n" +
            "   => The ones at odd positions. \n" +
            "5. 1, 2, 3, 4, ... \n" +
            "   => The one at the given numeric position. \n\n" +
            "Valid command syntaxes : \n" +
            "[ The <arguments> are flags that should be written with prefix (-) and without the <> ] \n" +
            "1. txtmod <operation> <selector1> <selector2> <text> \n" +
            "   => Performs the operation on given text as per the first and second selections. \n" +
            "2. txtmod <operation> <selector1> <text> \n" +
            "   => Performs the operation on given text as per the first selector and the second selector is (1) by default. \n" +
            "3. txtmod <operation> <text> \n" +
            "   => Performs the operation on the entire given text. \n" +
            "4. txtmod <text> \n" +
            "   => Performs the default combined operation (sen) on the entire text. \n" +
            "Note: (sen) is a combo-operation and (help) is non-operation. These cannot be used with any selectors. \n\n" +
            "Some examples : \n" +
            "1. txtmod -c -lpw -e The Fox jumped over the moon. \n" +
            "   => THe FOx jUmPeD oVeR tHe mOoN. \n" +
            "2. txtmod -c -wps the Fox jumped over the moon. \n" +
            "   => THE Fox jumped over the moon. \n" +
            "3. txtmod -rcs THe FOx jUmPeD oVeR tHe mOoN. \n" +
            "   => thE foX JuMpEd OvEr ThE MoOn. \n" +
            "4. txtmod thE foX JuMpEd OvEr ThE MoOn. \n" +
            "   => The fox jumped over the moon. \n" +
             "-------------------------- \n";
        private static bool help, caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi, sntcs;
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
                case "-HELP":
                    help = true;
                    break;
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
                case "-START":
                case "-FIRST":
                case "-F":
                    cstmi = true;
                    pos_index = 1;
                    break;
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
            if (help)
            {
                OutputToConsole(help_text);
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
                if (help || sntcs)
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
                if (help || sntcs)
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
            if (IsAllFalse(help, sntcs, caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                sntcs = true;
            }
            if (help && IsAllFalse(sntcs, caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                return help_text;
            }
            else if (sntcs && IsAllFalse(help, caps, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToLowerCase_all(inpText);
                modText = modText = TextOperations.Operate_LetterPerSentence(modText, OperationType.Upper, PositionType.Custom, 1);
            }
            else if (caps && IsAllFalse(help, sntcs, ncaps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToUpperCase_all(inpText);
            }
            else if (ncaps && IsAllFalse(help, sntcs, caps, rcaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToLowerCase_all(inpText);
            }
            else if (rcaps && IsAllFalse(help, sntcs, caps, ncaps, wrdpsnt, ltrpsnt, ltrpwrd, cstmi, lasti, eveni, oddi))
            {
                modText = TextOperations.ToReverseCase_all(inpText);
            }
            else if (!help && !sntcs && IsAnyTrue(caps, ncaps, rcaps) && IsAnyTrue(wrdpsnt, ltrpsnt, ltrpwrd))
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