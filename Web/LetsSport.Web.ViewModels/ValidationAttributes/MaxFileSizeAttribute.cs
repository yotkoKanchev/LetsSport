namespace LetsSport.Web.ViewModels.ValidationAttributes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Common;
    using Microsoft.AspNetCore.Http;

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly long maxFileSize = GlobalConstants.ImageMaxSizeMB * 1024 * 1024;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IEnumerable)
            {
                foreach (var obj in value as IEnumerable<IFormFile>)
                {
                    var file = obj as IFormFile;

                    if (file != null)
                    {
                        if (file.Length > this.maxFileSize)
                        {
                            return new ValidationResult($"Maximum allowed file size is {GlobalConstants.ImageMaxSizeMB}MB.");
                        }
                    }
                    else
                    {
                        return new ValidationResult("File not selected!");
                    }
                }
            }
            else
            {
                var file = value as IFormFile;

                if (file != null)
                {
                    if (file.Length > this.maxFileSize)
                    {
                        return new ValidationResult($"Maximum allowed file size is {GlobalConstants.ImageMaxSizeMB}MB.");
                    }
                }
                else
                {
                    return new ValidationResult("File not selected!");
                }
            }

            return ValidationResult.Success;
        }
    }
}
