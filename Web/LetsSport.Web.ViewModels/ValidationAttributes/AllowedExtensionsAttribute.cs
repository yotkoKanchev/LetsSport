namespace LetsSport.Web.ViewModels.ValidationAttributes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions =
        {
            ".ai", ".gif", ".webp", ".bmp", ".djvu", ".ps", ".ept", ".eps", ".eps3", ".fbx", ".flif", ".gif",
            ".gltf", ".heif", ".heic", ".ico", ".indd", ".jpg", ".jpe", ".jpeg", ".jp24", ".wdp", ".jxr",
            ".hdp", ".pdf", ".png", ".psd", ".arw", ".cr2", ".svg", ".tga", ".tif", ".tiff", ".webp",
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IEnumerable)
            {
                foreach (var obj in value as IEnumerable<IFormFile>)
                {
                    var file = obj as IFormFile;

                    if (file != null)
                    {
                        if (!this.extensions.Any(e => file.FileName.ToLower().EndsWith(e)))
                        {
                            return new ValidationResult("Not allowed file format uploaded!");
                        }
                    }
                }
            }
            else
            {
                var file = value as IFormFile;

                if (!(file == null))
                {
                    if (!this.extensions.Any(e => file.FileName.ToLower().EndsWith(e)))
                    {
                        return new ValidationResult("This image extension is not allowed!");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
