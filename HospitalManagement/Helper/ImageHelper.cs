using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HospitalManagement.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string?> SaveImageAsync(IWebHostEnvironment env, IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0)
                return null;

            string uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        public static void DeleteImage(IWebHostEnvironment env, string? imageName)
        {
            if (string.IsNullOrEmpty(imageName) || imageName == "user.png")
                return;

            string filePath = Path.Combine(env.WebRootPath, "uploads", imageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
