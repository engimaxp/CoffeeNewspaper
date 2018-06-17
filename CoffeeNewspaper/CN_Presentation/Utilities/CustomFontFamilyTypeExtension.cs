namespace CN_Presentation.Utilities
{
    public static class CustomFontFamilyTypeExtension
    {
        public static string ToFontFamilyNameText(this FontFamilyType type)
        {
            switch (type)
            {
                case FontFamilyType.FontAwesomeBrand:
                    return "Font Awesome 5 Brands";
                case FontFamilyType.FontAwesomeSolid:
                    return "Font Awesome 5 Free Solid";
                case FontFamilyType.FontAwesomeRegular:
                    return "Font Awesome 5 Free";
                case FontFamilyType.CNFont:
                    return "CN_Font";
                default: return "Font Awesome 5 Free Solid";
            }
        }
    }
}