using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    public class VerifyLoginModel : PageModel
    {
        public IActionResult OnGet(string token)
        {
            // ��֤ token��ʵ����Ŀ�п��Լ򵥴洢 token ��ֱ��ͨ�� URL ��֤��
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("��¼������Ч");
            }

            // ģ���¼�ɹ�
            return Content("��¼�ɹ���");
        }
    }
}
