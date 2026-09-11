using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // بنحقن الـ IWebHostEnvironment عشان نوصل لمسار الـ wwwroot
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(IFormFile? file, string folderName, string? existingFileUrl = null)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // 🎯 التعديل الأول: تأمين الـ WebRootPath على Azure
            // ساعات الـ WebRootPath بيرجع null لو الـ wwwroot مش مرفع أو لسه م تكريتش
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrEmpty(wwwRootPath))
            {
                wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            // تحديد المسار الفعلي للحفظ على الهاردسك (مثلاً: wwwroot/images/categories)
            string uploadsFolder = Path.Combine(wwwRootPath, "images", folderName);

            // لو المجلد مش موجود، نكريته
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // بنعمل اسم فريد للصورة لمنع الـ Overwrite
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName); // 🎯 تأمين اسم الملف الأصلي من أي مسارات غريبة
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // بنرجع المسار النسبي اللي هيتخزن في الداتا بيز بيبدأ بسلاش
            return $"/images/{folderName}/{uniqueFileName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                // 🎯 التعديل الثاني: توحيد طريقة جلب الـ Root Path لتجنب الـ Broken Paths أثناء الحذف
                var wwwRootPath = _webHostEnvironment.WebRootPath;
                if (string.IsNullOrEmpty(wwwRootPath))
                {
                    wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var relativePath = fileUrl.TrimStart('/');
                var fullPath = Path.Combine(wwwRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // منع تعطل الأبليكشن لو حصلت مشكلة صلاحيات على السيرفر
            }

            await Task.CompletedTask;
        }
    }
}