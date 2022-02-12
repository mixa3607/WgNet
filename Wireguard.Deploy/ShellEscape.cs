using System;
using System.Text;

namespace ArkProjects.Wireguard.Deploy
{
    /// <summary>
    /// Quotes a path in a way to be suitable to be used with a shell-based server.
    /// </summary>
    /// From SSH.NET
    internal class ShellEscape
    {
        /// <summary>
        /// Quotes a path in a way to be suitable to be used with a shell-based server.
        /// </summary>
        /// <param name="path">The path to transform.</param>
        /// <returns>A quoted path.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <c>null</c>.</exception>
        /// <remarks>
        /// <para>
        /// If <paramref name="path" /> contains a single-quote, that character is embedded
        /// in quotation marks (eg. "'"). Sequences of single-quotes are grouped in a single
        /// pair of quotation marks.
        /// </para>
        /// <para>
        /// An exclamation mark in <paramref name="path" /> is escaped with a backslash. This is
        /// necessary because C Shell interprets it as a meta-character for history substitution
        /// even when enclosed in single quotes or quotation marks.
        /// </para>
        /// <para>
        /// All other characters are enclosed in single quotes. Sequences of such characters are grouped
        /// in a single pair of single quotes.
        /// </para>
        /// <para>
        /// References:
        /// <list type="bullet">
        ///   <item>
        ///     <description><a href="http://pubs.opengroup.org/onlinepubs/7908799/xcu/chap2.html">Shell Command Language</a></description>
        ///   </item>
        ///   <item>
        ///     <description><a href="https://earthsci.stanford.edu/computing/unix/shell/specialchars.php">Unix C-Shell special characters and their uses</a></description>
        ///   </item>
        ///   <item>
        ///     <description><a href="https://docstore.mik.ua/orelly/unix3/upt/ch27_13.htm">Differences Between Bourne and C Shell Quoting</a></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <list type="table">
        ///   <listheader>
        ///     <term>Original</term>
        ///     <term>Transformed</term>
        ///   </listheader>
        ///   <item>
        ///     <term>/var/log/auth.log</term>
        ///     <term>'/var/log/auth.log'</term>
        ///   </item>
        ///   <item>
        ///     <term>/var/mp3/Guns N' Roses</term>
        ///     <term>'/var/mp3/Guns N'"'"' Roses'</term>
        ///   </item>
        ///   <item>
        ///     <term>/var/garbage!/temp</term>
        ///     <term>'/var/garbage'\!'/temp'</term>
        ///   </item>
        ///   <item>
        ///     <term>/var/would be 'kewl'!, not?</term>
        ///     <term>'/var/would be '"'"'kewl'"'"\!', not?'</term>
        ///   </item>
        ///   <item>
        ///     <term></term>
        ///     <term>''</term>
        ///   </item>
        ///   <item>
        ///     <term>Hello "World"</term>
        ///     <term>'Hello "World"'</term>
        ///   </item>
        /// </list>
        /// </example>
        public static string Transform(string path)
        {
            StringBuilder stringBuilder = path != null ? new StringBuilder(path.Length + 2) : throw new ArgumentNullException(nameof(path));
            ShellQuoteState shellQuoteState = ShellQuoteState.Unquoted;
            foreach (char ch in path)
            {
                switch (ch)
                {
                    case '!':
                        switch (shellQuoteState)
                        {
                            case ShellQuoteState.Unquoted:
                                stringBuilder.Append('\\');
                                break;
                            case ShellQuoteState.SingleQuoted:
                                stringBuilder.Append('\'');
                                stringBuilder.Append('\\');
                                break;
                            case ShellQuoteState.Quoted:
                                stringBuilder.Append('"');
                                stringBuilder.Append('\\');
                                break;
                        }
                        shellQuoteState = ShellQuoteState.Unquoted;
                        break;
                    case '\'':
                        switch (shellQuoteState)
                        {
                            case ShellQuoteState.Unquoted:
                                stringBuilder.Append('"');
                                break;
                            case ShellQuoteState.SingleQuoted:
                                stringBuilder.Append('\'');
                                stringBuilder.Append('"');
                                break;
                        }
                        shellQuoteState = ShellQuoteState.Quoted;
                        break;
                    default:
                        switch (shellQuoteState)
                        {
                            case ShellQuoteState.Unquoted:
                                stringBuilder.Append('\'');
                                break;
                            case ShellQuoteState.Quoted:
                                stringBuilder.Append('"');
                                stringBuilder.Append('\'');
                                break;
                        }
                        shellQuoteState = ShellQuoteState.SingleQuoted;
                        break;
                }
                stringBuilder.Append(ch);
            }
            switch (shellQuoteState)
            {
                case ShellQuoteState.SingleQuoted:
                    stringBuilder.Append('\'');
                    break;
                case ShellQuoteState.Quoted:
                    stringBuilder.Append('"');
                    break;
            }
            if (stringBuilder.Length == 0)
                stringBuilder.Append("''");
            return stringBuilder.ToString();
        }

        private enum ShellQuoteState
        {
            Unquoted = 1,
            SingleQuoted = 2,
            Quoted = 3,
        }
    }
}