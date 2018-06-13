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
                case IconType.Play:
                    return "\uf04b";
                case IconType.Check:
                    return "\uf00c";
                case IconType.Times:
                    return "\uf00d";
                case IconType.Pause:
                    return "\uf04c";

                // If none found, return null
                default:
                    return null;
            }
        }
    }
}