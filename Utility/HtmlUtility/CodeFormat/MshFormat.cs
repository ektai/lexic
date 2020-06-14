/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ektai Solutions LTD expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ektai Solutions LTD by email at sales@lexic.xyz
 *
 * The interactive user interfaces in modified source and object code versions of LEXIC must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original LEXIC logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by LEXIC" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


namespace ASC.Web.Studio.Utility.HtmlUtility.CodeFormat
{
    /// <summary>
    /// Generates color-coded HTML 4.01 from MSH (code name Monad) source code.
    /// </summary>
    internal class MshFormat : CodeFormat
    {
        /// <summary>
        /// Regular expression string to match single line comments (#).
        /// </summary>
        protected override string CommentRegEx
        {
            get { return @"#.*?(?=\r|\n)"; }
        }

        /// <summary>
        /// Regular expression string to match string and character literals. 
        /// </summary>
        protected override string StringRegEx
        {
            get { return @"@?""""|@?"".*?(?!\\).""|''|'.*?(?!\\).'"; }
        }

        /// <summary>
        /// The list of MSH keywords.
        /// </summary>
        protected override string Keywords
        {
            get
            {
                return "function filter global script local private if else"
                       + " elseif for foreach in while switch continue break"
                       + " return default param begin process end throw trap";
            }
        }

        /// <summary>
        /// Use preprocessors property to hilight operators.
        /// </summary>
        protected override string Preprocessors
        {
            get
            {
                return "-band -bor -match -notmatch -like -notlike -eq -ne"
                       + " -gt -ge -lt -le -is -imatch -inotmatch -ilike"
                       + " -inotlike -ieq -ine -igt -ige -ilt -ile";
            }
        }
    }
}