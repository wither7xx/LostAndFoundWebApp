using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace LostAndFoundWebApp.Pages
{
    public class ClaimApplicationModel(IWebHostEnvironment environment) : PageModel
    {

        private readonly IWebHostEnvironment _environment = environment;

        [BindProperty]
        public LostAndFoundWebApp.Models.Claim Claim { get; set; } = null;

        public void OnGet()
        {
            // ��ʼ��Ĭ��ֵ
            Claim = new LostAndFoundWebApp.Models.Claim
            {
                CreateTime = DateTime.Now,
                Status = ClaimMetadata.Status.DefaultStatus
            };
        }


        public async Task<IActionResult> OnPostAsync(IFormFile proofFile)
        {
            // �ֶ���֤�ļ��ֶΣ��ƹ��Զ���֤��
            if (proofFile == null || proofFile.Length == 0)
            {
                ModelState.AddModelError("proofFile", "�����ϴ�֤���ļ�");
            }

            // ��ǰ�����ļ��ϴ�


            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var guid = Guid.NewGuid();
            var imageIdx = 0;
            var image = proofFile;
          
           
            string fileName = $"{guid}-{imageIdx}{Path.GetExtension(image.FileName)}";
            imageIdx++;
            string filePath = Path.Combine(uploadPath, fileName);

             using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

            Claim.ProofDocPath = $"/uploads/images/{fileName}";
            Console.WriteLine(Claim.ProofDocPath);

            // �����û�ID��ȷ������Ȩ��
            Claim.UserId = 1;

            // �ֶ���֤�����ֶ�
            if (Claim.ItemId <= 0)
            {
                ModelState.AddModelError("Claim.ItemId", "��ƷID��Ч");
                return Page();
            }

            if (DatabaseOperate.GetItem(Claim.ItemId) == null)
            {
                ModelState.AddModelError("Claim.ItemId", "��ƷID������");
                return Page();
            }
           

            // ���ݿ����
            try
            {
                var result = DatabaseOperate.CreateClaim(Claim);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"�ύʧ�ܣ�{ex.Message}");
                return Page();
            }
        }
    }
}
