using System.ComponentModel.DataAnnotations;

namespace Shell
{
    public sealed class WebWindowOptions
    {
        private double? _height = null;
        [Display(Description = "Set the height of the window. Default: 450")]
        public double? Height
        {
            get { return _height ?? DefaultHeight; }
            set { _height = value; }
        }

        [Display(Name = "icon", Description = $"Set the icon url for the window.")]
        public string? IconUrl { get; set; }

        private string? _title = null;
        [Display(Description = $"Set the title of the window. Default: {DefaultTitle}")]
        public string? Title
        {
            get { return _title ?? DefaultTitle; }
            set { _title = value; }
        }

        public string? _url;
        [Display(Description = $"Set the url for the web view. Default: {DefaultUrl}")]
        public string? Url
        {
            get { return _url ?? DefaultUrl; }
            set { _url = value; }
        }

        private double? _width = null;
        [Display(Description = $"Set the width of the window. Default: 800")]
        public double? Width
        {
            get { return _width ?? DefaultWidth; }
            set { _width = value; }
        }

        public const double DefaultHeight = 450;
        public const string DefaultTitle = "Compose Web Container";
        public const string DefaultUrl = "about:blank";
        public const double DefaultWidth = 800;
    }
}
