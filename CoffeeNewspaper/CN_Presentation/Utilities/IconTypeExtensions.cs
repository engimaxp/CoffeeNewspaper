namespace CN_Presentation.Utilities
{
    /// <summary>
    ///     Helper functions for <see cref="IconType" />
    /// </summary>
    public static class IconTypeExtensions
    {
        /// <summary>
        ///     Converts <see cref="IconType" /> to a FontAwesome string
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns></returns>
        public static string ToFontAwesomeText(this IconType type)
        {
            // Return a FontAwesome string based on the icon type
            switch (type)
            {
                case IconType.File:
                    return "\uf0f6";

                case IconType.Picture:
                    return "\uf1c5";

                case IconType.BreifCase:
                    return "\uf0b1";
                case IconType.Tasks:
                    return "\uf0ae";
                case IconType.Notes:
                    return "\uf249";
                case IconType.ChartArea:
                    return "\uf1fe";
                case IconType.GraduationCap:
                    return "\uf19d";
                case IconType.Cog:
                    return "\uf013";
                // If none found, return null
                default:
                    return null;
            }
        }
        /// <summary>
        ///     Converts <see cref="IconType" /> to a FontAwesome string
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns></returns>
        public static string ToFontAwesomeButton(this IconType type)
        {
            // Return a FontAwesome string based on the icon type
            switch (type)
            {
                case IconType.File:
                    return "&#xf0f6";

                case IconType.Picture:
                    return "&#xf1c5";

                case IconType.BreifCase:
                    return "&#xf0b1";
                case IconType.Tasks:
                    return "&#xf0ae";
                case IconType.Notes:
                    return "&#xf249";
                case IconType.ChartArea:
                    return "&#xf1fe";
                case IconType.GraduationCap:
                    return "&#xf19d";
                case IconType.Cog:
                    return "&#xf013";
                // If none found, return null
                default:
                    return null;
            }
        }
    }
}